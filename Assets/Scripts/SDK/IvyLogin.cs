// using System;
// using System.Collections.Generic;

// class IvyUserInfo {
//     string id;
//     string name;
//     string picture;
// }

// public class IvyLogin : IvyBase implements ILoginInterface {
//     getName() { return "IvyLogin"; }
//     get channel() {
//         return MyChannels.ivy;
//     }
//     loginData: IvyUserInfo = null;
//     friends: IvyUserInfo[] = null;

//     // override
//     protected override void initNativeCallbacks() {
//         super.initNativeCallbacks();

//         // var _cc: any = cc;
//         // _cc.IvyBridge = _cc.IvyBridge || {};
//         // _cc.IvyBridge.onReceiveLoginResult = d => this.onReceiveLoginResult(d);
//         // _cc.IvyBridge.onReceiveFriends = d => this.onReceiveFriends(d);
//         // _cc.IvyBridge.onReceiveInviteResult = d => {

//         // };
//         // _cc.IvyBridge.onReceiveChallengeResult = d => {

//         // };
//         // _cc.IvyBridge.onReceiveLikeResult = d => {

//         // };
//     }
//     // override
//     protected override string className {
//         get { return "IvyLogin"; }
//     }

//     public bool useNativeButton {
//         get { return false; }
//     }

//     public void showNativeButton(int x, int y, int w, int h) {

//     }
//     public void hideNativeButton() {

//     }

//     private Action<bool> cbLogin = null;
//     public void login(Action<bool> cb) {
//         this.cbLogin = null;

//         if (this.logged) {
//             if (cb) {
//                 cb(true);
//             }
//         }
//         else {
//             if (Bootstrap.Instance) {
//                 sc.loading.show("login", 0, 10000);
//             }
//             this.cbLogin = cb;
//             switch (cc.sys.platform) {
//                 case cc.sys.ANDROID:
//                     return jsb.reflection.callStaticMethod(this.androidClassName, "login", "()V");
//                     break;
//                 case cc.sys.IPHONE:
//                 case cc.sys.IPAD:
//                     return jsb.reflection.callStaticMethod(this.iosClassName, "login");
//                     break;
//             }
//         }
//     }

//     // private _busy: boolean = false;
//     // get busy(): boolean {
//     //     return this._busy;
//     // }

//     private bool logged = false;
//     public bool isLogged {
//         get { return this.logged; }
//     }
//     private void setLogged(bool b) {
//         if (b != this.logged) {
//             this.logged = b;
//             this.emit(SDKEvent.LoginStateChanged, this);
//         }
//     }

//     public string channelUserId {
//         return "ivy_" + this.loginData.id;
//     }
//     public string originalChannelUserId {
//         return "ivy_" + this.loginData.id;
//     }

    
//     public object verifyData {
//         return this.loginData;
//     }
//     public void clearCache() {

//     }

//     public void logout() {
//         if (this.isLogged) {
//             switch (cc.sys.platform) {
//                 case cc.sys.ANDROID:
//                     return jsb.reflection.callStaticMethod(this.androidClassName, "logout", "()V");
//                     break;
//                 case cc.sys.IPHONE:
//                 case cc.sys.IPAD:
//                     return jsb.reflection.callStaticMethod(this.iosClassName, "logout");
//                     break;
//             }
//         }
//     }

//     bool hasAccountInfo {
//         get { return false; }
//     }
//     public void viewAccountInfo() {
//     }

//     public void helper() {
        
//     }

//     public void onReceiveLoginResult(json: IvyUserInfo) {
//         if (sc.loading) {
//             sc.loading.hide("login");
//         }

//         if (json == null || json.id == null || typeof(json.id) !== "string" || json.id.length == 0) {
//             this.setLogged(false);
//         }
//         else {
//             this.loginData = json;
//             this.setLogged(true);
//         }
//         if (this.cbLogin) {
//             this.cbLogin(this.logged);
//             this.cbLogin = null;
//         }
//     }

//     public void onReceiveFriends(json: IvyUserInfo[]) {
//         this.friends = json;
//     }
// }