
public class PMPayTest : PMHandler {
    public override MsgType msgType { get { return MsgType.PMPayiOSTest; } }
    * handle(object socket, object msg) {
        var baseScript = this.baseScript;
        var vipScript = this.server.vipScript;
        var sqlU = this.pmSqlUtils;
        var logger = this.logger;

        var res = new ResPay();
        var receipt = fs.readFileSync("./iOSIAPData/receipt4.txt", "utf-8");

        // 请求苹果服务器
        var waiter = new WaitCallBack();
        var fun = () => {
            iap.config({
                appleExcludeOldTransactions: true,
                applePassword: "e9cf1727e1e147c1a8daa3fe197c9674",
                verbose: true,
            });
            iap.setup().then(() => {
                iap.validate(receipt)
                    .then((validatedData: iap.ValidationResponse) => {
                        waiter.finish(new MyResponse(ECode.Success, validatedData));
                    })
                    .catch(error => {
                        baseScript.error("pay test error: %s", error);
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
            logger.warn("pay test purchasedData is empty");
            return new MyResponse(ECode.PurchaseDataIsEmpty, null);
        }

        if (purchasedData.length > 1) {
            logger.warn("purchasedData.length = %d > 1", purchasedData.length);
        }

        // 有服务器的情况下，res.items 需要另行计算
        // 默认为空，即没有新变化
        res.items = [];
        // 默认传回原值，若有新值，则覆盖
        res.monthlyCardUpdated = false;

        // 一个一个处理，通常是1
        for (int i = 0; i < purchasedData.length; i++) {
            iap item.PurchasedItem = purchasedData[i];
            var int purchaseDateMs = (item as any).purchaseDateMs;
            var int expiresDateMs = (item as any).expiresDateMs;

            if (!baseScript.checkArgs("SSSSI", item.transactionId, item.productId, item.bundleId, item.productId, purchaseDateMs)) {
                baseScript.error("pay test invalid iap.PurchasedItem %s", JSON.stringify(item));
                continue;
            }

            var originalTransactionId = item.originalTransactionId || null;
            IapItemBase configItem = vipScript.getItemByProductId(item.productId);

            if (configItem.productType == ProductType.MonthlyCard) {
                if (!baseScript.checkArgs("I", expiresDateMs)) {
                    baseScript.error("playerId %d invalid iap.PurchasedItemA %s", 0, JSON.stringify(item));
                    continue;
                }
            }

            if (!(expiresDateMs > 0)) {
                expiresDateMs = purchaseDateMs;
            }
            var rInsert = yield sqlU.insertPayiOSYield(0, "ios", configItem.id, item.productId, item.bundleId, item.quantity, item.transactionId, originalTransactionId, purchaseDateMs, expiresDateMs);
            if (rInsert.err != ECode.Success) {
                // playerId + transactionId 是唯一的，如果重复就会插入报错
               var resMysqlError = new ResMysqlError rInsert.res;
                const ER_DUP_ENTRY = 1062;
                if (resMysqlError.errno == ER_DUP_ENTRY) { // 已经存在的，要把此交易 ID 返回给客户端，让他 finishTransaction
                    res.items.push({
                        duplicated: true,
                        productId: item.productId,
                        transactionId: item.transactionId,
                        addDiamond: 0,
                        addGiftVoucher: 0
                    });
                }

                baseScript.error("pay test insertPay failed, %s, code %s, errno %d", ECode[rInsert.err], resMysqlError.code, resMysqlError.errno);
                continue;
            }
        }

        return new MyResponse(ECode.Success, res);
    }
}