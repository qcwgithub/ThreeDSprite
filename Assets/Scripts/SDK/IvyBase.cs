// using System;
// using System.Threading.Tasks;

// public abstract class IvyBase : IDeviceInterface
// {
//     protected string androidClassName = "com/YezhStudio/IvyBridge";
//     protected string iosClassName = "IvyBridge";
//     public string getName() {
//         return "IvyBase";
//     }
//     private bool _isInited = false;
//     public bool isInited { get { return this._isInited; } }

//     // virtual
//     protected void initNativeCallbacks() {
        
//     }

//     // virtual
//     protected string className {
//         get { return "IvyBase"; }
//     }

//     // virtual
//     public async Task init() {
//         this.initNativeCallbacks();

//         while (true) {
//             int r = 0;
//             switch (cc.sys.platform) {
//                 case cc.sys.ANDROID:
//                     r = jsb.reflection.callStaticMethod(this.androidClassName, "isInited", "()I");
//                     break;
//                 case cc.sys.IPHONE:
//                 case cc.sys.IPAD:
//                     r = 1; // iOS 是同步完成的
//                     break;
//                 default:
//                     r = 1;
//                     break;
//             }
//             if (r == 1) {
//                 break;
//             }
//             else {
//                 await Utils.PromiseTimeout(500);
//                 Debug.Log("wait ivy init");
//             }
//         }

//         Debug.Log(">" + this.className + ".init finish");
//         this._isInited = true;
//         this.emit(SDKEvent.Inited, this);
//     }
//     public void onEnterGame() {

//     }
//     public void onLogoutGame() {

//     }
    
//     public string getOAID() { return null; }
//     public string getMac() { return null; }
//     public string getIMEI() { return null; }
//     public void quit() {
//         switch (cc.sys.platform) {
//             case cc.sys.ANDROID:
//                 jsb.reflection.callStaticMethod(this.androidClassName, "quit", "()V");
//                 break;
//             case cc.sys.IPHONE:
//             case cc.sys.IPAD:
//                 // 苹果不需要
//                 break;
//             default:
//                 break;
//         }
//     }
// }