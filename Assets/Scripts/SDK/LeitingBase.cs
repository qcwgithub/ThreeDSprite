// using System;
// using System.Collections.Generic;
// using System.Threading.Tasks;
// using UnityEngine;

// public class QuitData
// {
//     string status; // 1退出成功
// }

// public abstract class LeitingBase : IDeviceInterface
// {
//     public virtual string getName()
//     {
//         return "LeitingBase";
//     }
//     private bool _isInited = false;
//     public bool isInited { get { return this._isInited; } }
//     private QuitData quitData = null;

//     // virtual
//     protected virtual void initNativeCallbacks()
//     {
//         // var _cc: any = cc;
//         // _cc.LeitingBridge = _cc.LeitingBridge || {};
//         // _cc.LeitingBridge.onQuitCallback = d => this.onQuitCallback(d);
//     }

//     protected Dictionary<string, List<Action<ISDKInterface>>> dictEvents =
//         new Dictionary<string, List<Action<ISDKInterface>>>();
//     public void on(string eventName, Action<ISDKInterface> cb)
//     {
//         List<Action<ISDKInterface>> list;
//         if (!this.dictEvents.TryGetValue(eventName, out list))
//         {
//             list = new List<Action<ISDKInterface>>();
//             this.dictEvents.Add(eventName, list);
//         }
//         list.Add(cb);
//     }
//     public void off(string eventName, Action<ISDKInterface> cb)
//     {
//         List<Action<ISDKInterface>> list;
//         if (this.dictEvents.TryGetValue(eventName, out list))
//         {
//             list.Remove(cb);
//             if (list.Count == 0)
//             {
//                 this.dictEvents.Remove(eventName);
//             }
//         }
//     }
//     protected void emit(string eventName)
//     {
//         List<Action<ISDKInterface>> list;
//         if (this.dictEvents.TryGetValue(eventName, out list))
//         {
//             var array = list.ToArray();
//             foreach (var action in array)
//             {
//                 action(this);
//             }
//         }
//     }

//     // virtual
//     protected virtual string className
//     {
//         get { return "LeitingBase"; }
//     }

//     // virtual
//     public virtual async Task init()
//     {
//         this.initNativeCallbacks();

//         // while (true) {
//         //     var r = 0;
//         //     switch (cc.sys.platform) {
//         //         case cc.sys.ANDROID:
//         //             r = jsb.reflection.callStaticMethod("com/YezhStudio/LeitingBridge", "isInited", "()I");
//         //             break;
//         //         case cc.sys.IPHONE:
//         //         case cc.sys.IPAD:
//         //             r = 1; // iOS 是同步完成的
//         //             break;
//         //         default:
//         //             r = 1;
//         //             break;
//         //     }
//         //     if (r == 1) {
//         //         break;
//         //     }
//         //     else {
//         //         await Utils.PromiseTimeout(500);
//         //         Debug.Log("wait leiting init");
//         //     }
//         // }

//         Debug.Log(">" + this.className + ".init finish");
//         this._isInited = true;
//         this.emit(SDKEvent.Inited);
//     }
//     public virtual void onEnterGame()
//     {

//     }
//     public virtual void onLogoutGame()
//     {

//     }
//     static readonly int gameCode = 10002;
//     public bool isLeitingOfficial()
//     {
//         return true;
//         // switch (cc.sys.platform) {
//         //     case cc.sys.ANDROID:
//         //         return this.getChannelId() == "110001";
//         //     //break;
//         //     case cc.sys.IPHONE:
//         //     case cc.sys.IPAD:
//         //         return true;
//         //     //break;
//         //     default:
//         //         return true;
//         //         break;
//         // }
//     }

//     private string channelId = null;
//     public string getChannelId()
//     {
//         if (this.channelId != null && this.channelId.Length > 0)
//         {
//             return this.channelId;
//         }

//         // switch (cc.sys.platform) {
//         //     case cc.sys.ANDROID:
//         //         this.channelId = jsb.reflection.callStaticMethod("com/YezhStudio/LeitingBridge", "getChannelId", "()Ljava/lang/String;");
//         //         break;
//         //     case cc.sys.IPHONE:
//         //     case cc.sys.IPAD:
//         //         this.channelId = jsb.reflection.callStaticMethod("LeitingBridge", "getChannelId");
//         //         break;
//         //     default:
//         //         this.channelId = "110001";
//         //         break;
//         // }

//         return this.channelId;
//     }

//     public string getOAID()
//     { // Just android
//         // switch (cc.sys.platform) {
//         //     case cc.sys.ANDROID:
//         //         return jsb.reflection.callStaticMethod("com/YezhStudio/LeitingBridge", "getOAID", "()Ljava/lang/String;");
//         //     case cc.sys.IPHONE:
//         //     case cc.sys.IPAD:
//         //         new Error("unsupport method on IOS");
//         //     default:
//         //         break;
//         // }
//         return null;
//     }

//     public string getMac()
//     {
//         // switch (cc.sys.platform) {
//         //     case cc.sys.ANDROID:
//         //         return jsb.reflection.callStaticMethod("com/YezhStudio/LeitingBridge", "getMac", "()Ljava/lang/String;");
//         //     case cc.sys.IPHONE:
//         //     case cc.sys.IPAD:
//         //         return jsb.reflection.callStaticMethod("LeitingBridge", "getMac");
//         //     default:
//         //         break;
//         // }
//         return null;
//     }

//     public string getIMEI()
//     {
//         // switch (cc.sys.platform) {
//         //     case cc.sys.ANDROID:
//         //         return jsb.reflection.callStaticMethod("com/YezhStudio/LeitingBridge", "getIMEI", "()Ljava/lang/String;");
//         //     case cc.sys.IPHONE:
//         //     case cc.sys.IPAD:
//         //         return jsb.reflection.callStaticMethod("LeitingBridge", "getIMEI");
//         //     default:
//         //         break;
//         // }
//         return null;
//     }

//     public void quit()
//     {
//         // switch (cc.sys.platform) {
//         //     case cc.sys.ANDROID:
//         //         jsb.reflection.callStaticMethod("com/YezhStudio/LeitingBridge", "quit", "()V");
//         //         break;
//         //     case cc.sys.IPHONE:
//         //     case cc.sys.IPAD:
//         //         break;
//         //     default:
//         //         break;
//         // }
//     }

//     public void onQuitCallback(QuitData data)
//     {
//         // Debug.Log("onQuitCallback: " + JSON.stringify(data));
//         // this.quitData = data;
//         // if (this.quitData && this.quitData.status == "1") {
//         //     cc.game.end();
//         //     // this.logged = false;
//         //     // this.saveLogged(false);
//         //     // this.fireLogStateChanged();
//         // }
//     }

//     public void showPrivacy()
//     {
//         // switch (cc.sys.platform) {
//         //     case cc.sys.ANDROID:
//         //         return jsb.reflection.callStaticMethod("com/YezhStudio/LeitingBridge", "start", "(Ljava/lang/String;)V", "Privacy");
//         //     case cc.sys.IPHONE:
//         //     case cc.sys.IPAD:
//         //         return jsb.reflection.callStaticMethod("LeitingBridge", "start:", "Privacy");
//         //     default:
//         //         break;
//         // }
//     }
// }