
// 客户端完成支付后，会发送此消息来领奖励
public class PMPayLt : PMHandler {
    public override MsgType msgType { get { return MsgType.PMPayLt; } }

    *handle(object socket, MsgPay msg) {
        var PMPlayerInfo player = this.server.netProto.getPlayer(socket);
        if (player == null) {
            r.err = ECode.PlayerNotExist;
            yield break;
        }
        this.logger.info("%s playerId: %d", this.msgName, player.id);

        var vipScript = this.server.vipScript;
        var logger = this.logger;
        if (player.ltPaying) {
            logger.warn("playerId %d ltPaying", player.id);
            r.err = ECode.LastPayNotEnd;
            yield break;
        }

        var res = new ResPay();
        var e = vipScript.payCheck(player, msg, res);
        if (e != ECode.Success) {
            return e;
        }

        this.logger.info("PMPayLtGetReward playerId %d", player.id);
        player.ltPaying = true;

        yield return this.server.payLtSqlUtils.queryPayLt_p_s_g(player.id, PayLtState.Succeeded, 0, r);
        if (r.err != ECode.Success) {
            return r.err;
        }

        SqlTablePayLt infos[] = r.res;
        if (infos == null || infos.length == 0) {
            return new MyResponse(ECode.Success, res);
        }

        var string orderIds[] = [];
        // var iapConfig = this.server.pmData.iapConfig;
        for (int i = 0; i < infos.length; i++) {
            SqlTablePayLt info = infos[i];
            IapItemBase iapItem = this.server.vipScript.getItemById(info.id);
            if (iapItem == null) {
                this.baseScript.error("iapItem==null, id %d", info.id);
                continue;
            }

            PurchasedItem purItem = {
                duplicated: false,
                productId: info.productId,
                transactionId: info.orderId,
                addDiamond: 0,
                addGiftVoucher: 0,
            };

            if (iapItem.productType == ProductType.MonthlyCard) {
                res.monthlyCardUpdated = true;
                var now = this.server.gameScript.getTime();
                var oneMonth = 30 * 24 * 3600 * 1000;
                if (player.profile.subscribe.expireTime > now) {
                    res.expiresDateMs = player.profile.subscribe.expireTime + oneMonth;
                }
                else {
                    res.purchaseDateMs = now;
                    res.expiresDateMs = res.purchaseDateMs + oneMonth;
                }
            }
            else if (iapItem.productType == ProductType.Diamond) {
                purItem.addDiamond = (iapItem as IapDiamondItem).diamond;
            }
            else if (iapItem.productType == ProductType.GiftVoucher) {
                var gvItem = iapItem as IapGiftVoucherItem;
                purItem.addGiftVoucher = gvItem.voucher;

                // 记录购买次数
                if (gvItem.oncePerDay) {
                    res.numberUpdates.push(gvItem.counterNumberIndex);
                    var today = this.server.gameScript.getTodayTime(0, 0, 0, 0);
                    res.numberUpdates.push(today);
                }
            }
            else if (iapItem.productType == ProductType.BeginnerMoney) {
                // var moneyItem = iapItem as IapBeginnerMoneyItem;
                // 记录购买索引
                var array = this.pmData.iapConfig.beginnerMoney;
                for (var index = 0; index < array.length; index++) {
                    if (iapItem.id == array[index].id) {
                        res.numberUpdates.push(ProfileNumberIndex.BeginnerMoneyIndex);
                        res.numberUpdates.push(index + 1); // 设置为下一个
                        break;
                    }
                }
            }
            else {
                this.baseScript.error("unknown productType %d", iapItem.productType);
                continue;
            }

            // 放最后
            res.items.push(purItem);
            orderIds.push(info.orderId);
        }

        r = yield return this.server.payLtSqlUtils.updatePayLtGotMany(orderIds, 1);
        if (r.err != ECode.Success) {
            return r.err;
        }

        //// ok
        vipScript.payExecute(player, msg, res);

        this.pmScript.postHandlePayResult(player, res);
        return new MyResponse(ECode.Success, res);
    }

    // 
    postHandle(object socket, msg: MsgPay) {
        var PMPlayerInfo player = this.server.netProto.getPlayer(socket);
        if (player != null) {
            this.logger.info("PMPayLtGetReward postHandle playerId: %d", player.id);
            player.ltPaying = false;
        }
        else {
            this.logger.info("PMPayLtGetReward postHandle playerId: null");
        }
    }
}