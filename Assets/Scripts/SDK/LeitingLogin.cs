
// using System;
// using System.Collections.Generic;
// using Data;

// class LoginData {
//     public string status; // 状态码 1：成功；2：失败(eg:1) 9：需要激活(官方渠道)
//     public string bind;
//     public string game;
//     public string userId; // 用户SID,一般该字段为用户的唯一标识
//     public string channelNo;
//     public string token;
//     public string userName;
//     public string memo;
//     public string userKey;
//     public string statusCode;
//     public string nickName;
//     public string type;
//     public string auth; // 防沉迷 0：已实名认证未成年 1：已实名认证成年人 2：未实名认证游客账号返回2
//     public string age;
//     public string isGuest; // 是否是游客账号 1：游客账号 0：不是游客账号 
// }

// class LogoutData {
//     public string status; // 状态码 1：退出成功
//     public string memo;
//     public string game;
//     public string channelNo;
//     public string bind;
//     public string userId; // 用户SID,一般该字段为用户的唯一标识
//     public string token;
//     public string userName;
//     public string userKey;
//     public string statusCode;
//     public string nickName;
//     public string type;
// }

// class _ltVerifyData
// {
//     public string token;
//     public string game;
//     public string channelNo;
//     public LoginData detail;
// }

// public class LeitingLogin : LeitingBase, ILoginInterface {
//     public override string getName() { return "LeitingLogin"; }
//     public string channel {
//         get { return MyChannels.leiting; }
//     }
//     public LoginData loginData = null;
//     private LogoutData logoutData = null;

//     // override
//     protected override void initNativeCallbacks() {
//         base.initNativeCallbacks();

//         // var _cc: any = cc;
//         // _cc.LeitingBridge = _cc.LeitingBridge || {};
//         // _cc.LeitingBridge.onLoginCallback = d => this.onLoginCallback(d);
//         // _cc.LeitingBridge.onLogoutCallback = d => this.onLogoutCallback(d);
//     }
//     // override
//     protected override string className {
//         get { return "LeitingLogin"; }
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
//             if (cb != null) {
//                 cb(true);
//             }
//         }
//         else {
//             // if (Bootstrap.Instance) {
//             //     sc.loading.show("login", 0, 10000);
//             // }
//             // this.cbLogin = cb;
//             // switch (cc.sys.platform) {
//             //     case cc.sys.ANDROID:
//             //         return jsb.reflection.callStaticMethod("com/YezhStudio/LeitingBridge", "login", "()V");
//             //         break;
//             //     case cc.sys.IPHONE:
//             //     case cc.sys.IPAD:
//             //         return jsb.reflection.callStaticMethod("LeitingBridge", "login");
//             //         break;
//             // }
//         }
//     }

//     // private bool _busy = false;
//     // bool busy {
//     //     get { return this._busy; }
//     // }

//     private bool logged = false;
//     public bool isLogged {
//         get { return this.logged; }
//     }
//     private void setLogged(bool b) {
//         if (b != this.logged) {
//             this.logged = b;
//             this.emit(SDKEvent.LoginStateChanged);
//         }
//     }

//     public string channelUserId {
//         // 雷霆是聚合，userId 是渠道返回的，不同渠道可能重复，因此需要加上渠道id
//         get { return this.loginData.channelNo + "_" + this.loginData.userId; }
//     }
//     public string originalChannelUserId {
//         get { return this.loginData.userId; }
//     }

//     private _ltVerifyData _verifyData = new _ltVerifyData {
//         token = "",
//         game = "",
//         channelNo = "",
//         detail = null,
//     };
//     public object verifyData {
//         get { return this._verifyData; }
//     }
//     public void clearCache() {

//     }

//     public void logout() {
//         // if (this.isLogged) {
//         //     switch (cc.sys.platform) {
//         //         case cc.sys.ANDROID:
//         //             return jsb.reflection.callStaticMethod("com/YezhStudio/LeitingBridge", "logout", "()V");
//         //             break;
//         //         case cc.sys.IPHONE:
//         //         case cc.sys.IPAD:
//         //             return jsb.reflection.callStaticMethod("LeitingBridge", "logout");
//         //             break;
//         //     }
//         // }
//     }

//     public bool hasAccountInfo {
//         get { return true; }
//     }
//     public void viewAccountInfo() {
//         // switch (cc.sys.platform) {
//         //     case cc.sys.ANDROID:
//         //         return jsb.reflection.callStaticMethod("com/YezhStudio/LeitingBridge", "accountCenter", "()V");
//         //         break;
//         //     case cc.sys.IPHONE:
//         //     case cc.sys.IPAD:
//         //         return jsb.reflection.callStaticMethod("LeitingBridge", "accountCenter");
//         //         break;
//         // }
//     }

//     public void helper() {
//         // switch (cc.sys.platform) {
//         //     case cc.sys.ANDROID:
//         //         return jsb.reflection.callStaticMethod("com/YezhStudio/LeitingBridge", "helper", "()V");
//         //         break;
//         //     case cc.sys.IPHONE:
//         //     case cc.sys.IPAD:
//         //         return jsb.reflection.callStaticMethod("LeitingBridge", "helper");
//         //         break;
//         // }
//     }

//     private void onLoginCallback(LoginData data) {
//         // Debug.Log("onLoginCallback: " + JSON.stringify(data));
//         // if (sc.loading) {
//         //     sc.loading.hide("login");
//         // }

//         // this.loginData = data;
//         // if (data && data.status == "1"
//         //     //&& data.isGuest != "1"
//         // ) {
//         //     this._verifyData = new _ltVerifyData {
//         //         token = data.token,
//         //         game = data.game,
//         //         channelNo = data.channelNo,
//         //         detail = data,
//         //     };

//         //     this.setLogged(true);
//         // }
//         // else {
//         //     this.setLogged(false);
//         // }

//         // if (this.cbLogin) {
//         //     this.cbLogin(this.logged);
//         //     this.cbLogin = null;
//         // }
//     }

//     private void onLogoutCallback(LogoutData data) {
//         // Debug.Log("onLogoutCallback: " + JSON.stringify(data));

//         // this.logoutData = data;
//         // if (this.logoutData && this.logoutData.status == "1") {
//         //     this.setLogged(false);
//         // }
//         // else {

//         // }
//     }
// }