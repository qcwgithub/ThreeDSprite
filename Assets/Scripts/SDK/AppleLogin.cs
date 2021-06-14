// using System;
// using System.Collections.Generic;
// using UnityEngine;
// using Data;

// public class AppleVerifyData
// {
//     public string token;
//     public string code;
// }

// public class AppleLogin : ILoginInterface
// {
//     public string getName() { return "AppleLogin"; }
//     public string channel => MyChannels.apple;

//     // 提供给原生回调
//     private void initNativeCallbacks()
//     {
//         //-------TODO
//         // var _cc: any = cc;
//         // _cc.AppleIdBridge = _cc.AppleIdBridge || {};
//         // _cc.AppleIdBridge.onLoginClick = () => this.onLoginClick();
//         // _cc.AppleIdBridge.onLoginResult = (a, b, c, d) => this.onLoginResult(a, b, c, d);
//     }

//     //// init
//     private bool _inited = false;
//     public void init()
//     {
//         this._inited = true;
//         this.initNativeCallbacks();
//         this.emit(SDKEvent.Inited, this);
//     }
//     public bool isInited
//     {
//         get { return this._inited; }
//     }
//     public void onEnterGame()
//     {

//     }
//     public void onLogoutGame()
//     {

//     }

//     //// login
//     public bool useNativeButton
//     {
//         // 目前还不能改成 true，改成 true 后，调用 login 会无效，必须配合 showNativeButton
//         get
//         {
//             return false;
//         }
//     }

//     private bool logged;
//     private Action<bool> cbLogin = null;
//     public void login(Action<bool> cb)
//     {
//         // if (!this.logged)
//         // {
//         //     var id = LSUtils.getItem(LSKeys.APPLE_CHANNEL_USER_ID, null);
//         //     if (id != null)
//         //     {
//         //         Debug.Log("~auto login~");
//         //         this._channelUserId = EncryptUtils.decrypt(id);
//         //         this.logged = true;
//         //     }
//         // }
//         // if (this.logged)
//         // {
//         //     this.cbLogin = null;
//         //     if (cb)
//         //     {
//         //         cb(true);
//         //     }
//         // }
//         // else
//         // {
//         //     if (Bootstrap.Instance)
//         //     {
//         //         sc.loading.show("login", 0, 10000);
//         //     }
//         //     this.cbLogin = cb;
//         //     if (!this.useNativeButton)
//         //     {
//         //         jsb.reflection.callStaticMethod("AppleIdBridge", "login");
//         //     }
//         // }
//     }
//     public bool isLogged
//     {
//         get
//         {
//             return this.logged;
//         }
//     }

//     private string _channelUserId = null;
//     public string channelUserId
//     {
//         get { return this._channelUserId; }
//     }
//     public string originalChannelUserId
//     {
//         get { return this.channelUserId; }
//     }

//     private AppleVerifyData _verifyData = new AppleVerifyData
//     {
//         token = "",
//         code = ""
//     };
//     public object verifyData
//     {
//         get { return this._verifyData; }
//     }
//     public void clearCache()
//     {
//         this.logged = false;
//         LSUtils.RemoveItem(LSKeys.APPLE_CHANNEL_USER_ID);
//     }
//     public bool hasAccountInfo
//     {
//         get { return false; }
//     }
//     public void viewAccountInfo()
//     {

//     }

//     public void logout()
//     {
//         this.clearCache();
//         this.logged = false;
//     }

//     public static bool isSupported()
//     {
//         return false; //return jsb.reflection.callStaticMethod("AppleIdBridge", "isSupported") == 1;
//     }

//     public void showNativeButton(int x, int y, int w, int h)
//     {
//         if (this.useNativeButton)
//         {
//             // return jsb.reflection.callStaticMethod("AppleIdBridge", "show:y:w:h:", x, y, w, h);
//         }
//     }
//     public void hideNativeButton()
//     {
//         if (this.useNativeButton)
//         {
//             // return jsb.reflection.callStaticMethod("AppleIdBridge", "hide");
//         }
//     }

//     //// 原生回调
//     public void onLoginClick()
//     {
//         Debug.Log("AppleLogin.onLoginClick");
//         this.emit(SDKEvent.NativeButtonClick, this);
//     }
//     public void onLoginResult(bool success, string userId, string token, string code)
//     {
//         // if (sc.loading)
//         // {
//         //     sc.loading.hide("login");
//         // }

//         Debug.Log(string.Format("AppleLogin.onLoginResult success={0},userId={1},token={2},code={3}", success, userId, token, code));

//         this.logged = success;
//         if (success)
//         {
//             this._channelUserId = userId;
//             LSUtils.SetItem(LSKeys.APPLE_CHANNEL_USER_ID, EncryptUtils.encrypt(userId));
//             this._verifyData = new AppleVerifyData { token = token, code = code };
//         }

//         if (this.cbLogin != null)
//         {
//             this.cbLogin(this.logged);
//             this.cbLogin = null;
//         }
//     }
// }