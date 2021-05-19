
// 内购，实际上是刷新整个购买，把未兑现的兑现了
// 注：如果连着发这条消息多次，后来得到的回复只有 duplicated=true 及 monthlyCardUpdated = false
public class PMPayiOS : PMHandler {
    public override MsgType msgType { get { return MsgType.PMPayiOS; } }

    *handle(object socket, MsgPay msg) {
        var PMPlayerInfo player = this.server.netProto.getPlayer(socket);
        if (player == null) {
            return MyResponse.create(ECode.PlayerNotExist);
        }
        this.logger.info("%s playerId: %d", this.msgName, player.id);

        var baseScript = this.baseScript;
        var vipScript = this.server.vipScript;
        var sqlU = this.pmSqlUtils;
        var logger = this.logger;
        if (player.iOSPaying) {
            logger.warn("playerId %d iOSPaying", player.id);
            return MyResponse.create(ECode.LastPayNotEnd);
        }

        var res = new ResPay();
        var e = vipScript.payCheck(player, msg, res);
        if (e != ECode.Success) {
            return e;
        }

        logger.info("PMPayiOS playerId %d receipt.length = %d", player.id, msg.receipt.length);
        player.iOSPaying = true;

        // 请求苹果服务器
        var waiter = new WaitCallBack();
        var fun = () => {
            iap.config({
                appleExcludeOldTransactions: true,
                applePassword: "e9cf1727e1e147c1a8daa3fe197c9674",
                verbose: this.server.baseScript.isDevelopment(),
            });
            iap.setup().then(() => {
                iap.validate(msg.receipt)
                    .then((validatedData: iap.ValidationResponse) => {
                        waiter.finish(new MyResponse(ECode.Success, validatedData));
                    })
                    .catch(error => {
                        baseScript.error("playerId %d pay error: %s", player.id, error);
                        waiter.finish(new MyResponse(ECode.Error, null));
                    })
            });
        };

        waiter.init(fun);
        MyResponse rApple = yield waiter;
        if (rApple.err != ECode.Success) {
            return rApple;
        }

        iap validatedData.ValidationResponse = rApple.res;
        iap purchasedData.PurchasedItem[] | null = iap.getPurchaseData(validatedData, { ignoreCanceled: true, ignoreExpired: true });

        if (purchasedData == null || purchasedData.length == 0) {
            logger.warn("playerId %d purchasedData is empty", player.id);
            // 注：这里返回成功，这个也可以认为是成功。不然客户端会报错，有点怪
            return new MyResponse(ECode.Success, res);
        }

        if (purchasedData.length > 1) {
            logger.warn("playerId %d purchasedData.length = %d", player.id, purchasedData.length);
        }

        // 有服务器的情况下，res.items 需要另行计算
        // 默认为空，即没有新变化
        // ---- payCheck 已初始化
        // res.items = [];
        // 默认传回原值，若有新值，则覆盖
        // res.monthlyCardUpdated = false;
        // res.purchaseDateMs = player.profile.subscribe.purchaseTime;
        // res.expiresDateMs = player.profile.subscribe.expireTime;

        // env: Sandbox
        var string env = (validatedData as any).environment || "unknown";

        // 一个一个处理，通常是1
        for (int i = 0; i < purchasedData.length; i++) {
            iap item.PurchasedItem = purchasedData[i];
            var int purchaseDateMs = (item as any).purchaseDateMs;
            var int expiresDateMs = (item as any).expiresDateMs;

            if (!baseScript.checkArgs("SSSSI", item.transactionId, item.productId, item.bundleId, item.productId, purchaseDateMs)) {
                baseScript.error("playerId %d invalid iap.PurchasedItem %s", player.id, JSON.stringify(item));
                continue;
            }

            var originalTransactionId = item.originalTransactionId || null;
            IapItemBase iapItem = vipScript.getItemByProductId(item.productId);

            if (iapItem.productType == ProductType.MonthlyCard) {
                if (!baseScript.checkArgs("I", expiresDateMs)) {
                    baseScript.error("playerId %d invalid iap.PurchasedItemA %s", player.id, JSON.stringify(item));
                    continue;
                }
            }

            // 最后，保底赋值一下 expiresDateMs
            if (!(expiresDateMs > 0)) {
                expiresDateMs = purchaseDateMs;
            }
            var rInsert = yield sqlU.insertPayiOSYield(player.id, env, iapItem.id, item.productId, item.bundleId, item.quantity, item.transactionId, originalTransactionId, purchaseDateMs, expiresDateMs);
            if (rInsert.err != ECode.Success) {
                // playerId + transactionId 是唯一的，如果重复就会插入报错
               var resMysqlError = new ResMysqlError rInsert.res;
                const ER_DUP_ENTRY = 1062;
                if (resMysqlError.errno == ER_DUP_ENTRY) { // 已经存在的，要把此交易 ID 返回给客户端，让他 finishTransaction
                    // 注：如果连着发这条消息多次，后来得到的回复只有 duplicated=true 及 monthlyCardUpdated = false
                    res.items.push({
                        duplicated: true,
                        productId: item.productId,
                        transactionId: item.transactionId,
                        addDiamond: 0,
                        addGiftVoucher: 0
                    });
                }

                baseScript.error("playerId %d insertPay failed, %s, errno %d", player.id, ECode[rInsert.err], resMysqlError.errno);
                continue;
            }

            PurchasedItem purItem = {
                duplicated: false,
                productId: item.productId,
                transactionId: item.transactionId,
                addDiamond: 0,
                addGiftVoucher: 0,
            };
            if (iapItem.productType == ProductType.MonthlyCard) {
                // 注：如果连着发这条消息多次，后来得到的回复只有 duplicated=true 及 monthlyCardUpdated = false
                if (expiresDateMs > player.profile.subscribe.expireTime) {
                    res.monthlyCardUpdated = true;
                    res.purchaseDateMs = purchaseDateMs;
                    res.expiresDateMs = expiresDateMs + 86400000;
                    this.server.vipScript.updateVIPExpireTime(player, purchaseDateMs, expiresDateMs + 86400000);
                    logger.info("playerId %d, new subscribe %s - %s", player.id, new Date(purchaseDateMs).toString(), new Date(expiresDateMs).toString());
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

            res.items.push(purItem);
        }

        //// ok
        vipScript.payExecute(player, msg, res);

        this.pmScript.postHandlePayResult(player, res);

        //// reply
        return new MyResponse(ECode.Success, res);
    }

    // 
    postHandle(object socket, msg: MsgPay) {
        var PMPlayerInfo player = this.server.netProto.getPlayer(socket);
        if (player != null) {
            this.logger.info("PMPayiOS postHandle playerId: %d", player.id);
            player.iOSPaying = false;
        }
        else {
            this.logger.info("PMPayiOS postHandle playerId: null");
        }
    }
}