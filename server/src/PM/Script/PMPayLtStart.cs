
// 创建订单。购买前先请求，获得一个订单号
public class PMPayLtStart : PMHandler {
    public override MsgType msgType { get { return MsgType.PMPayLtStart; } }

    *handle(object socket, MsgPayLtStart msg) {
        this.logger.info("PMPayLtStart %s,%s", msg.productId, msg.fen);
        var PMPlayerInfo player = this.server.netProto.getPlayer(socket);
        if (player == null) {
            return MyResponse.create(ECode.PlayerNotExist);
        }
        this.logger.info("%s playerId: %d", this.msgName, player.id);

        if (!this.server.scUtils.checkArgs("SS", msg.productId, msg.fen)) {
            return MyResponse.create(ECode.InvalidParam);
        }

        IapLeitingInfo ltItem = this.server.vipScript.getLeitingItemByProductId(msg.productId);
        if (ltItem == null) {
            return MyResponse.create(ECode.InvalidProductId);
        }

        IapItemBase iapItem = this.server.vipScript.getItemById(ltItem.id);
        if (iapItem == null) {
            this.baseScript.error("iapItem == null, ltItem.id=" + ltItem.id);
            return MyResponse.create(ECode.InvalidProductId);
        }

       var res = new ResPayLtStart {
            orderId = "",
            ltProducts = null,
        };

        var fen = (ltItem.rmb * 100).toString();
        if (msg.fen !== fen) {
            res.ltProducts = this.server.pmData.iapConfig.platformInfo.leiting;
            return new MyResponse(ECode.ProductPriceRefreshed, res);
        }

        // 检查购买数量限制
        if (iapItem.productType == ProductType.GiftVoucher) {
            IapGiftVoucherItem giftVoucher = iapItem as IapGiftVoucherItem;
            if (giftVoucher.oncePerDay) {
                var time = this.server.gameScript.getNumber(player, giftVoucher.counterNumberIndex);
                var today = this.server.gameScript.getTodayTime(0, 0, 0, 0);
                if (time == today) {
                    return MyResponse.create(ECode.MaxCount);
                }
            }
        }

        res.orderId = v4();
        this.logger.info("PMPayLtStart playerId: %d, orderId: %s", player.id, res.orderId);

        MyResponse r = yield this.server.payLtSqlUtils.insertPayLtYield(player.id, ltItem.id, msg.productId, 1, fen, res.orderId);
        if (r.err != ECode.Success) {
            return r.err;
        }

        //// reply
        return new MyResponse(ECode.Success, res);
    }
}