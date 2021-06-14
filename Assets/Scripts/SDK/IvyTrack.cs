// using System;
// using System.Collections.Generic;

// public class IvyTrack : IvyBase, ISDKInterface {
//     getName() { return 'IvyTrack'; }

//     // override
//     protected override string className {
//         return 'IvyTrack';
//     }

//     private static instance: IvyTrack = null;
//     static get Instance(): IvyTrack {
//         return this.instance;
//     }

//     private tutorialMap: { [string keys]: string };
//     private keyToIndex: { [string keys]: number };
//     public async Task init() {
//         super.init();

//         IvyTrack.instance = this;
//         // 注册监视网络状况的事件
//         this.tutorialMap = {};
//         this.tutorialMap["entree"] = "guide_1_storyline_1_GO";
//         this.tutorialMap["buy"] = "guide_2_free_1_tap";
//         this.tutorialMap["secondBuy"] = "guide_2_free_2_tap";
//         this.tutorialMap["merge"] = "guide_3_merge_lv1_mall";
//         this.tutorialMap["npc"] = "guide_4_storyline_2_close";
//         this.tutorialMap["buy3"] = "guide_5_free_1_tap";
//         this.tutorialMap["buy4"] = "guide_5_free_2_tap";
//         this.tutorialMap["secondMerge"] = "guide_6_merge_lv1_mall";
//         this.tutorialMap["party"] = "guide_10_cityevent_participate_2";
//         this.tutorialMap["kingStreet"] = "guide10_storyline_3_close";
//         this.tutorialMap["inviteNpc"] = "guide_11_hotel_invite_2";
//         this.tutorialMap["completeTownTask"] = "guide_11_hammer_tap_1";
//         this.tutorialMap["collectDiamond"] = "guide_11_hammer_tap_2";
//         this.tutorialMap["clickShip"] = "guide_12_airship";
//         this.tutorialMap["openBox"] = "guide_12_airship_box";
//         this.tutorialMap["useShip"] = "guide_12_airship_free";
//         this.tutorialMap["landmarkOpen"] = "guide_13_shoppingmall_tap";
//         this.tutorialMap["landmarkMove"] = "guide_13_shoppingmall_plan";
//         this.tutorialMap["design"] = "guide_13_shoppingmall_placed";
//         this.tutorialMap["landmarkUse1"] = "guide_13_shoppingmall_free";

//         this.keyToIndex = {};
//         this.keyToIndex["guide_11_hotel_tap2enter"] = 1;
//         this.keyToIndex["guide_11_decorate_tap_tick"] = 2;
//         this.keyToIndex["guide_11_hotel_invite_1"] = 3;
//         this.keyToIndex["guide_11_hotel_invite_3"] = 4;
//         this.keyToIndex["guide_7_free_1_tap"] = 5;
//         this.keyToIndex["guide_7_free_2_tap"] = 6;
//         this.keyToIndex["guide_7_free_3_tap"] = 7;
//         this.keyToIndex["guide_7_free_4_tap"] = 8;
//         this.keyToIndex["video_cityevent_extra_reward"] = 9;
//         this.keyToIndex["video_airship_free"] = 10;
//         this.keyToIndex["video_shop_free"] = 11;
//         this.keyToIndex["video_mall_unlock"] = 12;
//         this.keyToIndex["video_mall_upgrade"] = 13;
//         this.keyToIndex["video_signin_earningsx2"] = 14;
//         this.keyToIndex["video_theatre_preview"] = 15;
//         this.keyToIndex["video_spinstar_free"] = 16;
//         this.keyToIndex["video_shoppingmall_free"] = 17;
//         this.keyToIndex["video_offline_revenuex2"] = 18;
//         this.keyToIndex["video_20_malls_free"] = 19;
//     }

//     public override void onEnterGame() {
//         super.onEnterGame();
//         var game = sc.game;

//         // lGame.buildMgr.on(BuildEvent.kHighestHouseLevelUp1, this.onHighestHouseLevelUp, this);
//         sc.myTown.node.on(GameEvents.kTaskFinished, this.onTownTaskCompleted, this);
//         sc.guiMgr.node.on(CGUIMgr.kShowNewFunctionEndEvent, this.unlockTownClose, this)
//         // lGame.partyMgr.on(PartyMgrEvents.kEndServer, this.onPartyEndServer, this);
//         game.onEvent(IvyTrackEvent.ITapShopFreeBox, this.tapShopFreeBox, this);
//         game.onEvent(IvyTrackEvent.ITutorialComplete, this.onTutorialComplete, this);
//         // game.onEvent(IvyTrackEvent.IEnterTown, this.enterTown, this);
//         game.onEvent(IvyTrackEvent.ITownInviteNPCStoryClose, this.storyClose, this);
//         game.onEvent(IvyTrackEvent.ITownShopSubmit, this.townShopSubmit, this);
//         game.onEvent(IvyTrackEvent.IClickInvite, this.clickInvite, this);
//         game.onEvent(IvyTrackEvent.IUnlockLv2TapClose, this.unlockLv2TapClose, this);
//         game.onEvent(IvyTrackEvent.IUnlockLv3, this.unlockLv3, this);
//         game.onEvent(IvyTrackEvent.IUnlockLv4, this.unlockLv4, this);
//         game.onEvent(IvyTrackEvent.IFirstPartyTapClose, this.firstPartyTapClose, this);
//         game.onEvent(IvyTrackEvent.IShowOpenParyGUI, this.showOpenParyGUI, this);
//         game.onEvent(IvyTrackEvent.IinviteGUIClose, this.inviteGUIClose, this);
//         game.onEvent(IvyTrackEvent.IClickExtraReward, this.clickExtraReward, this);
//         game.onEvent(IvyTrackEvent.IClickShipFree, this.clickShipFree, this);
//         game.onEvent(IvyTrackEvent.IClickShopFree, this.clickShopFree, this);
//         game.onEvent(IvyTrackEvent.IClickShopADHouse, this.clickShopADHouse, this)
//         game.onEvent(IvyTrackEvent.IClickHouseUpgrade, this.clickHouseUpgrade, this)
//         game.onEvent(IvyTrackEvent.IClickSigninEarningsx2, this.clickSigninEarningsx2, this)
//         game.onEvent(IvyTrackEvent.IClickTheatrePreview, this.clickTheatrePreview, this)
//         game.onEvent(IvyTrackEvent.IClickCasinoFree, this.clickCasinoFree, this)
//         game.onEvent(IvyTrackEvent.IClickShoppingmallFree, this.clickShoppingmallFree, this)
//         game.onEvent(IvyTrackEvent.IClickOfflineRevenuex2, this.clickOfflineRevenuex2, this)
//         game.onEvent(IvyTrackEvent.IEnterFloor, this.enterFloor, this)
//         game.onEvent(IvyTrackEvent.IADFillFreeHouse, this.adFillFreeHouse, this)

//         this.lastConsumeDiamond = game.profile.statistics.diamond.consumed;
//         if (this.lastConsumeDiamond < this.diamondMax) {
//             game.onEvent(ProfileEvent.kDiamondRefresh, this.onDiamondEvent, this);
//         }

//         var m = game.profile.statistics.money.consumed;
//         if (m.lesser(this.moneyMax)) {
//             this.lastConsumeMoney = m.toJSNumber();
//             game.onEvent(ProfileEvent.kMoneyRefresh, this.onMoneyEvent, this);
//         }
//     }

//     public override void onLogoutGame() {
//         var game = sc.game;
        
//         // lGame.buildMgr.off(BuildEvent.kHighestHouseLevelUp1, this.onHighestHouseLevelUp, this);
//         sc.myTown.node.off(GameEvents.kTaskFinished, this.onTownTaskCompleted, this);
//         // game.partyMgr.off(PartyMgrEvents.kEndServer, this.onPartyEndServer, this);
//         game.offEvent(ProfileEvent.kDiamondRefresh, this.onDiamondEvent, this);
//         game.offEvent(ProfileEvent.kMoneyRefresh, this.onMoneyEvent, this);
//         sc.guiMgr.node.off(CGUIMgr.kShowNewFunctionEndEvent, this.unlockTownClose, this)
//         game.offEvent(IvyTrackEvent.ITapShopFreeBox, this.tapShopFreeBox, this);
//         game.offEvent(IvyTrackEvent.ITutorialComplete, this.onTutorialComplete, this);
//         // game.offEvent(IvyTrackEvent.IEnterTown, this.enterTown, this);
//         game.offEvent(IvyTrackEvent.ITownInviteNPCStoryClose, this.storyClose, this);
//         game.offEvent(IvyTrackEvent.ITownShopSubmit, this.townShopSubmit, this);
//         game.offEvent(IvyTrackEvent.IClickInvite, this.clickInvite, this);
//         game.offEvent(IvyTrackEvent.IUnlockLv2TapClose, this.unlockLv2TapClose, this);
//         game.offEvent(IvyTrackEvent.IUnlockLv3, this.unlockLv3, this);
//         game.offEvent(IvyTrackEvent.IUnlockLv4, this.unlockLv4, this);
//         game.offEvent(IvyTrackEvent.IFirstPartyTapClose, this.firstPartyTapClose, this);
//         game.offEvent(IvyTrackEvent.IShowOpenParyGUI, this.showOpenParyGUI, this);
//         game.offEvent(IvyTrackEvent.IinviteGUIClose, this.inviteGUIClose, this);
//         game.offEvent(IvyTrackEvent.IClickExtraReward, this.clickExtraReward, this);
//         game.offEvent(IvyTrackEvent.IClickShipFree, this.clickShipFree, this);
//         game.offEvent(IvyTrackEvent.IClickShopFree, this.clickShopFree, this);
//         game.offEvent(IvyTrackEvent.IClickShopADHouse, this.clickShopADHouse, this)
//         game.offEvent(IvyTrackEvent.IClickHouseUpgrade, this.clickHouseUpgrade, this)
//         game.offEvent(IvyTrackEvent.IClickSigninEarningsx2, this.clickSigninEarningsx2, this)
//         game.offEvent(IvyTrackEvent.IClickTheatrePreview, this.clickTheatrePreview, this)
//         game.offEvent(IvyTrackEvent.IClickCasinoFree, this.clickCasinoFree, this)
//         game.offEvent(IvyTrackEvent.IClickShoppingmallFree, this.clickShoppingmallFree, this)
//         game.offEvent(IvyTrackEvent.IClickOfflineRevenuex2, this.clickOfflineRevenuex2, this)
//         game.offEvent(IvyTrackEvent.IEnterFloor, this.enterFloor, this)
//         game.offEvent(IvyTrackEvent.IADFillFreeHouse, this.adFillFreeHouse, this)

//         super.onLogoutGame();
//     }

//     // private onHighestHouseLevelUp() {
//     //     var lGame = Bootstrap.Instance.masterGame;
//     //     var lv = lGame.profile.highestHouseLevel + 1;
//     //     this.track2('level_up', 'levelup,level_' + lv);

//     //     if (lv <= 20) {
//     //         this.track1('level_' + lv);
//     //     }
//     // }

//     // private lastCount = 0;
//     private void onTownTaskCompleted() {
//         this.track1('decorate_hammer_all_over_today');

//         // 第 2 个事件只需要前 10 个
//         // if (this.lastCount > 10) {
//         //     return;
//         // }

//         // if (t.concreteCfg.floorId != 1) {
//         //     return;
//         // }
//         cc.log('TODO');
//         // var lGame = Bootstrap.Instance.masterGame;
//         // var count = lGame.townTaskScript.getFloorCTasks(lGame, t.concreteCfg.floorId).length;
//         // this.lastCount = count;
//         // if (count < 0 || count > 10) {
//         //     return;
//         // }
//         // this.track1('decorate_hammer_' + count);
//     }

//     private void onPartyEndServer() {
//         this.track1('city_tournament_all_over_today');
//     }

//     ////
//     private lastConsumeDiamond = 0;
//     private int diamondArray[] = [5, 10, 20, 50, 100, 500];
//     private get diamondMax(): number {
//         return this.diamondArray[this.diamondArray.length - 1];
//     }
//     private void onDiamondEvent() {
//         var pre = this.lastConsumeDiamond;
//         var int curr = sc.game.profile.statistics.diamond.consumed;
//         if (curr >= this.diamondMax) {
//             sc.game.offEvent(ProfileEvent.kDiamondRefresh, this.onDiamondEvent, this);
//         }
//         for (int i = 0; i < this.diamondArray.length; i++) {
//             var v = this.diamondArray[i];
//             if (pre < v && curr >= v) {
//                 this.track1('gem_' + v);
//             }
//         }
//         this.lastConsumeDiamond = curr;
//     }

//     ////
//     private int lastConsumeMoney = 0;
//     private int moneyArray[] = [100, 1000, 10000, 50000, 100000, 500000, 1000000];
//     private get moneyMax(): number {
//         return this.moneyArray[this.moneyArray.length - 1];
//     }
//     private void onMoneyEvent() {
//         var pre = this.lastConsumeMoney;
//         var m = sc.game.profile.statistics.money.consumed;
//         var int curr = 0;
//         if (m.greaterOrEquals(this.moneyMax)) {
//             curr = this.moneyMax;
//             sc.game.offEvent(ProfileEvent.kMoneyRefresh, this.onMoneyEvent, this);
//         }
//         else {
//             curr = m.toJSNumber();
//         }

//         for (int i = 0; i < this.moneyArray.length; i++) {
//             var v = this.moneyArray[i];
//             if (pre < v && curr >= v) {
//                 this.track1('coin_' + v);
//             }
//         }
//         this.lastConsumeMoney = curr;
//     }


//     private void storyClose() {
//         this.track1("guide_11_storyline_4_close");
//     }
//     private void unlockLv2TapClose() {
//         this.track1("guide_3_lv2_mall_page_close");
//     }
//     private void unlockLv3() {
//         this.track1("guide_8_merge_lv2_mall");
//     }
//     private void unlockLv4() {
//         this.track1("guide_9_merge_lv3_mall");
//     }
//     private void firstPartyTapClose() {
//         this.track1("guide_10_cityevent_participate_3");
//     }
//     private void showOpenParyGUI() {
//         this.track1("guide_10_cityevent_participate_1");
//     }
//     private void enterFloor(int floorID) {
//         if (floorID > 0 && floorID < 6) {
//             this.track1("enter_" + floorID.toString() + "F");
//         }
//     }

//     // private enterTown() {
//     //     if (!this.isTrackSent("guide_11_hotel_tap2enter")) {
//     //         this.sendOnceTrack("guide_11_hotel_tap2enter");
//     //         this.enterFloor(1); // 这个时候也是进入1层的时候
//     //     }
//     // }
//     private void townShopSubmit() {
//         this.sendOnceTrack("guide_11_decorate_tap_tick");
//     }
//     private void clickInvite() {
//         this.sendOnceTrack("guide_11_hotel_invite_1");
//     }
//     private void inviteGUIClose() {
//         this.sendOnceTrack("guide_11_hotel_invite_3");
//     }
//     private void clickExtraReward() {
//         this.sendOnceTrack("video_cityevent_extra_reward");
//     }
//     private void clickShipFree() {
//         this.sendOnceTrack("video_airship_free");
//     }
//     private void clickShopFree() {
//         this.sendOnceTrack("video_shop_free");
//     }
//     private void clickShopADHouse() {
//         this.sendOnceTrack("video_mall_unlock");
//     }
//     private void clickHouseUpgrade() {
//         this.sendOnceTrack("video_mall_upgrade");
//     }
//     private void clickSigninEarningsx2() {
//         this.sendOnceTrack("video_signin_earningsx2");
//     }
//     private void clickTheatrePreview() {
//         this.sendOnceTrack("video_theatre_preview");
//     }
//     private void clickCasinoFree() {
//         this.sendOnceTrack("video_spinstar_free");
//     }
//     private void clickShoppingmallFree() {
//         this.sendOnceTrack("video_shoppingmall_free");
//     }
//     private void clickOfflineRevenuex2() {
//         this.sendOnceTrack("video_offline_revenuex2");
//     }

//     private void tapShopFreeBox() {
//         if (!this.isTrackSent("guide_7_free_1_tap")) {
//             this.sendOnceTrack("guide_7_free_1_tap");
//         } else if (!this.isTrackSent("guide_7_free_2_tap")) {
//             this.sendOnceTrack("guide_7_free_2_tap");
//         } else if (!this.isTrackSent("guide_7_free_3_tap")) {
//             this.sendOnceTrack("guide_7_free_3_tap");
//         } else if (!this.isTrackSent("guide_7_free_4_tap")) {
//             this.sendOnceTrack("guide_7_free_4_tap");
//         }
//     }

//     private void unlockTownClose(string key) {
//         if (key == 'town') {
//             this.track1("guide_11_hotel_unlock");
//         }
//     }

//     private void onTutorialComplete(string tutorialKey) {
//         var key = this.tutorialMap[tutorialKey] || "";
//         if (key.length <= 0) {
//             return;
//         }
//         this.track1(key);
//     }

//     private void adFillFreeHouse() {
//         this.track1("video_20_malls_free");
//     }

//     private void sendOnceTrack(string key) {
//         if (!this.isTrackSent(key)) {
//             this.track1(key);
//             this.trackSent(key);
//         }
//     }

//     private bool isTrackSent(string key) {
//         var rightShift = this.keyToIndex[key] || 0;
//         if (!(rightShift > 0)) { return false; }
//         return ((sc.clientProfile.ivyTrackRecord >> rightShift) & 1) == 1;
//     }
//     private void trackSent(string key) {
//         var leftShift = this.keyToIndex[key] || 0;
//         if (!(leftShift > 0)) { return; }
//         sc.clientProfile.ivyTrackRecord |= (1 << leftShift);
//         sc.clientProfile.saveIvyTrackRecord();
//     }

//     private void track1(string s1) {
//         Debug.Log('ivyTrack1 ' + s1);
//         switch (cc.sys.platform) {
//             case cc.sys.ANDROID:
//                 return jsb.reflection.callStaticMethod(this.androidClassName, "track1", "(Ljava/lang/String;)V", s1);
//                 break;
//             case cc.sys.IPHONE:
//             case cc.sys.IPAD:
//                 return jsb.reflection.callStaticMethod(this.iosClassName, "track1:", s1);
//                 break;
//         }
//     }

//     private void track2(string s1, string s2) {
//         Debug.Log('ivyTrack2 ' + s1 + ',' + s2);
//         switch (cc.sys.platform) {
//             case cc.sys.ANDROID:
//                 return jsb.reflection.callStaticMethod(this.androidClassName, "track2", "(Ljava/lang/String;Ljava/lang/String;)V", s1, s2);
//                 break;
//             case cc.sys.IPHONE:
//             case cc.sys.IPAD:
//                 return jsb.reflection.callStaticMethod(this.iosClassName, "track2:s2:", s1, s2);
//                 break;
//         }
//     }

//     // private track4(string s1, string s2, string s3, int i4) {
//     //     Debug.Log('ivyTrack4 ' + s1 + ',' + s2 + ',' + s3 + ',' + i4);
//     //     switch (cc.sys.platform) {
//     //         case cc.sys.ANDROID:
//     //             return jsb.reflection.callStaticMethod(this.androidClassName, "track4", "(Ljava/lang/String;Ljava/lang/String;Ljava/lang/String;I)V", s1, s2, s3, i4);
//     //             break;
//     //         case cc.sys.IPHONE:
//     //         case cc.sys.IPAD:
//     //             return jsb.reflection.callStaticMethod(this.iosClassName, "track4:s2:s3:i4:", s1, s2, s3, i4);
//     //             break;
//     //     }
//     // }

// }