
// 购物狂欢（加速）
public class PMShoppingMall : PMHandler {
    public override MsgType msgType { get { return MsgType.PMShoppingMall; } }

    handle(object socket, MsgShoppoingMall msg) {
        var PMPlayerInfo player = this.server.netProto.getPlayer(socket);
        if (player == null) {
            return MyResponse.create(ECode.PlayerNotExist);
        }
        this.logger.info("%s playerId: %d", this.msgName, player.id);

        if (player.profile.shoppingMallInfo == null) {
            return MyResponse.create(ECode.ShoppingMallNotInited);
        }

        var script = this.server.shoppingMallScript;
        var res = new ResShoppingMall();
        var e = script.turnCheck(player, msg, res);
        if (e != ECode.Success) {
            this.pmScript.playerOperError(this, player.id, e);
            return e;
        }

        //// ok
        script.turnExecute(player, msg, res);

        //// db
        switch (msg.priceType) {
            case PriceType.Tutorial:
                player.profileChanged(PMProfileType.tutorial);
                break;

            case PriceType.AD: // 看广告或 vip
                break;

            case PriceType.Diamond:
                player.profileChanged(PMProfileType.diamond);
                this.sqlLog.player_diamond(player, DiamondPlace.ShoppingMall, -res.subDiamond);
                break;
        }
        player.profileChanged(PMProfileType.shoppingMallInfo);

        //// reply
        return new MyResponse(ECode.Success, res);
    }
}