
// 创建订单。购买前先请求，获得一个订单号
public class PMPayIvyStart : PMHandler {
    public override MsgType msgType { get { return MsgType.PMPayIvyStart; } }

    *handle(object socket, msg: MsgPayIvyStart) {
        var PMPlayerInfo player = this.server.netProto.getPlayer(socket);
        if (player == null) {
            return MyResponse.create(ECode.PlayerNotExist);
        }
        this.logger.info("%s %d", this.msgName, msg.id);

        if (!this.server.scUtils.checkArgs("I", msg.id)) {
            return MyResponse.create(ECode.InvalidParam);
        }

        IapIvyInfo ivyItem = this.server.vipScript.getIvyItemById(msg.id);
        if (ivyItem == null) {
            return MyResponse.create(ECode.InvalidProductId);
        }

        IapItemBase iapItem = this.server.vipScript.getItemById(msg.id);
        if (iapItem == null) {
            this.baseScript.error("iapItem == null, ivyItem.id=" + msg.id);
            return MyResponse.create(ECode.InvalidProductId);
        }

       var res = new ResPayIvyStart {
            orderId: "",
        };

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
        this.logger.info("%s playerId: %d, orderId: %s", this.msgName, player.id, res.orderId);

        MyResponse r = yield this.server.payIvySqlUtils.insertPayIvyYield(player.id, iapItem.id, ivyItem.productId, 1, "0", res.orderId);
        if (r.err != ECode.Success) {
            return r.err;
        }

        //// reply
        return new MyResponse(ECode.Success, res);
    }
}