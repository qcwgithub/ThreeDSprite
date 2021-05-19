// import PMUploadProfile from "./PMUploadProfile";

public class PMRegister : BaseRegister {
    public override void register(Server server) {
        base.register(server);

        server.dispatcher.addHandler(new PMStart());
        server.dispatcher.addHandler(new PMOnDisconnect());
        server.dispatcher.addHandler(new PMKeepAliveToAAA());
        server.dispatcher.addHandler(new PMPlayerLogin());
        server.dispatcher.addHandler(new PMChangeChannel());
        server.dispatcher.addHandler(new PMPreparePlayerLogin());
        server.dispatcher.addHandler(new PMDestroyPlayer());
        server.dispatcher.addHandler(new PMPlayTurnTable());
        server.dispatcher.addHandler(new PMGetTurnTableReward());
        // server.dispatcher.addHandler(new PMUploadProfile());
        server.dispatcher.addHandler(new PMSyncProfile());
        server.dispatcher.addHandler(new PMPlayerSave());
        server.dispatcher.addHandler(new PMChangeName());
        server.dispatcher.addHandler(new PMChangePortrait());
        server.dispatcher.addHandler(new PMDailySignin());
        server.dispatcher.addHandler(new PMADDiamond());
        server.dispatcher.addHandler(new PMLoginReward());
        server.dispatcher.addHandler(new PMAuthReward());
        server.dispatcher.addHandler(new PMGetDevReward());
        server.dispatcher.addHandler(new PMGetNpcMoney());
        server.dispatcher.addHandler(new PMGetNpcDiamond());
        server.dispatcher.addHandler(new PMRandomShipReward());
        server.dispatcher.addHandler(new PMGetShipReward());
        server.dispatcher.addHandler(new PMCollectOfflineBonus());
        server.dispatcher.addHandler(new PMShoppingMall());
        server.dispatcher.addHandler(new PMGetVipDailyReward());
        server.dispatcher.addHandler(new PMUpgradeHouse());
        server.dispatcher.addHandler(new PMRefreshDailyTask());
        server.dispatcher.addHandler(new PMRefreshAllDailyTask());
        server.dispatcher.addHandler(new PMRefreshPiggyShop());
        server.dispatcher.addHandler(new PMPiggyBuyItem());
        server.dispatcher.addHandler(new PMBuyFreeBox());
        server.dispatcher.addHandler(new PMBuyMoney());
        server.dispatcher.addHandler(new PMCompleteDailyTask());
        server.dispatcher.addHandler(new PMCompleteTownTask());
        server.dispatcher.addHandler(new PMInviteNpc());
        server.dispatcher.addHandler(new PMCollectLetterBonus());
        server.dispatcher.addHandler(new PMCollectRepairDiamond());
        server.dispatcher.addHandler(new PMGetPartyMoney());
        server.dispatcher.addHandler(new PMNpcGenReward());
        server.dispatcher.addHandler(new PMNpcLeave());
        server.dispatcher.addHandler(new PMCompleteStoryTask());
        server.dispatcher.addHandler(new PMActiveTown());
        server.dispatcher.addHandler(new PMFinishDoctorKTask());
        server.dispatcher.addHandler(new PMCompleteDoctorKTask());
        server.dispatcher.addHandler(new PMReduceDoctorKTaskTime());
        server.dispatcher.addHandler(new PMRefreshDoctorKTask());
        server.dispatcher.addHandler(new PMChangeTownNodeSprite());
        server.dispatcher.addHandler(new PMBuyTownNodeSprite());
        server.dispatcher.addHandler(new PMPartyLevelUp());
        server.dispatcher.addHandler(new PMBuyKSSkin());
        server.dispatcher.addHandler(new PMApplyKSSkin());

        server.dispatcher.addHandler(new PMRandomActAwards());
        server.dispatcher.addHandler(new PMOpenActAward());
        server.dispatcher.addHandler(new PMOpenAllActAward());
        server.dispatcher.addHandler(new PMActAddKey());

        server.dispatcher.addHandler(new PMBuyHouse());

        server.dispatcher.addHandler(new PMPayiOS());
        server.dispatcher.addHandler(new PMPayTest());

        server.dispatcher.addHandler(new PMGetSummary());

        server.dispatcher.addHandler(new PMActivityStart());
        server.dispatcher.addHandler(new PMActivityEnd());

        server.dispatcher.addHandler(new PMPayLtStart());
        server.dispatcher.addHandler(new PMPayLt());

        server.dispatcher.addHandler(new PMPayIvyStart());
        server.dispatcher.addHandler(new PMPayIvy());

        server.dispatcher.addHandler(new PMAction());
    }
}