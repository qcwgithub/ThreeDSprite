// using System;
// using System.Collections.Generic;
// public class LeiTingAdLog : LeitingBase, IAdLogInterface
// {
//     public string getName() { return "LeitingAdLog"; }
//     private readonly string key = "#Leiting#";

//     // override
//     protected override void initNativeCallbacks()
//     {
//         super.initNativeCallbacks();

//         // var _cc: any = cc;
//         // _cc.LeitingADLog = _cc.LeitingADLog || {};
//         // _cc.LeitingADLog.invoke = (a, b, c) => this.invoke(a, b, c);
//     }

//     // override
//     protected string className
//     {
//         get { return "LeiTingAdLog"; }
//     }

//     private string channel
//     {
//         get { return this.getChannelId(); }
//     }

//     private string url
//     {
//         get
//         {
//             return CC_DEBUG
//           ? "http://testlogmonitor.leiting.com/ad_log/report"
//           : "http://logmonitor.leiting.com/ad_log/report";
//         }
//     }

//     // hacking
//     // 通过channel这个参数来判断是否需要向leiting发送日志
//     private bool enable { get { return this.channel != undefined; } }
//     private string imei { get { return this.getIMEI(); } }
//     private string mac { get { return this.getMac(); } }
//     private string sid;
//     private string userName { get { return this.profile.userName; } }
//     private string account;
//     private string signKey;

//     private string _media;
//     private string media
//     {
//         get
//         {
//             if (this._media == "csj")
//                 return "pangolin";

//             return this._media;
//         }
//     }

//     private Profile profile;
//     public void initWithGame()
//     {
//         if (!this.enable)
//         {
//             // hacking
//             // 通过channel这个参数来判断是否需要向leiting发送日志
//             return;
//         }

//         this.profile = sc.profile;
//         this.sid = this.profile.userID.substring(0, 10);
//         this.account = $"{this.channel}{this.sid}";
//         this.signKey = this.sign();
//         this._media = HermesAdBridge.getAdMedia();
//     }

//     private void register()
//     {
//         if (!this.enable)
//         {
//             return;
//         }

//         var data = new Dictionary<string, string> {
//             {"accountName", this.account},
//             {"extend", this.getExtend()},
//         };

//         //cc.log($"report leiting register event{JSON.stringify(data)}");
//         this.eventReport("2", JSON.stringify(data));
//     }

//     private void createCharacter()
//     {
//         if (!this.enable)
//         {
//             return;
//         }

//         var data = new Dictionary<string, string> {
//             { "accountName", this.account },
//             { "roleName", this.profile.userName },
//             { "roleId", this.sid },
//             { "serverName", "none" },
//             { "serverId", "none" },
//             { "extend", this.getExtend() },
//         };

//         //cc.log(`report leiting create character event[${JSON.stringify(data)}]`);
//         this.eventReport("3", JSON.stringify(data));
//     }

//     public void login()
//     {
//         this.doLoginLogout(1);
//         if (this.enable && this.profile != null && !sc.clientProfile.registed)
//         {
//             sc.clientProfile.registed = true;
//             sc.clientProfile.saveregisted();

//             this.register();
//             this.createCharacter();
//         }
//     }

//     public void logout()
//     {
//         this.doLoginLogout(2);
//     }

//     // type: "vip" | "diamond" | "giftVoucher" | "money"
//     public void purchase(string type, string tid, string price)
//     {
//         if (!this.enable)
//         {
//             return;
//         }

//         Action moneyType = () =>
//         {
//             switch (type)
//             {
//                 case "diamond":
//                     return 1;
//                 case "money":
//                     return 2;
//                 case "vip":
//                     return 4;
//                 case "giftVoucher":
//                     return 5;
//                 default:
//                     cc.error("unknow purchase type!");
//             }
//         };

//         var data = new Dictionary<string, object> {
//             {"accountName", this.account},
//             {"type", 1},
//             {"moneyType", moneyType()},
//             {"orderId", tid},
//             {"roleName", this.userName},
//             {"roleId", this.sid},
//             {"roleLevel", 1},
//             {"serverName", "none"},
//             {"serverId", "none"},
//             {"price", price},
//             {"extend", this.getExtend()},
//         };

//         this.eventReport("5", JSON.stringify(data));
//     }

//     private doLoginLogout(int type)
//     {
//         if (!this.enable)
//         {
//             return;
//         }

//         var data = new Dictionary<string, object> {
//             { accountName, this.account },
//             { loginMode, type },// 1 -> login; 2 -> logout
//             { loginType, 1 },
//             { newAccount, 2 },
//             { creditAccount, 2 },
//             { roleName, this.userName },
//             { roleId, this.sid },
//             { roleLevel, 1 },
//             { serverName, "none" },
//             { serverId, "none" },
//             { extend, this.getExtend() },
//         };

//         this.eventReport("4", JSON.stringify(data));
// }

// public void adClick(string placeId, string type)
// {
//     this.onEvent(placeId, 1);
// }

// public void adShow(string placeId)
// {
//     this.onEvent(placeId, 2);
// }

// public void adView(string placeId, string type)
// {
//     this.onEvent(placeId, 3);
// }

// public void adGiveAward(string placeId)
// {
//     this.onEvent(placeId, 4);
//     this.onEvent(placeId, 5);
// }

// public void adError(string placeId)
// {
//     this.onEvent(placeId, 6);
// }

// public void adGoto(string placeId)
// {
//     this.onEvent(placeId, 7);
// }

// private void onEvent(string placement, object type)
// {
//     if (!this.enable)
//     {
//         return;
//     }

//     string data = $"gameCode={LeitingBase.gameCode}&" +
//             $"sid={this.sid}&" +
//             $"account={this.account}&" +
//             $"channel={this.channel}&" +
//             $"status={type}&" +
//             $"sign={this.signKey}&" +
//             $"place={placement}&" +
//             $"media={this.media}&" +
//             $"imei={this.imei}&" +
//             $"mac={this.mac}&" +
//             $"extend={JSON.stringify(this.getExtend())}";


//         HttpNetwork.postForm(this.url, data);
// }

// public string sign()
// {
//     // md5(gameCode+sid +account+ channel + key) 
//     return md5(LeitingBase.gameCode + this.sid + this.account + this.channel + this.key);
// }

// private object getExtend()
// {
//     if (cc.sys.platform == cc.sys.ANDROID)
//     {
//         return new Dictionary<string, string> { { "oaid", this.getOAID() } };
//     }
//     else
//     {
//         return "";
//     }
// }

// private void eventReport(string key, string value)
// {
//     switch (cc.sys.platform)
//     {
//         case cc.sys.ANDROID:
//             return jsb.reflection.callStaticMethod("com/YezhStudio/LeitingBridge", "eventReport", "(Ljava/lang/String;Ljava/lang/String;)V", key, value);
//         case cc.sys.IPHONE:
//         case cc.sys.IPAD:
//             return jsb.reflection.callStaticMethod("LeitingBridge", "eventReport:withValue:", key, value);
//         default:
//             break;
//     }
// }

// // 从 java oc 调过来
// // result:
// // 0 show failed
// // 1 show success
// // 2 close
// private void invoke(string placement, string type, int result)
// {
//     Debug.Log("LeitingADLog.invoke " + placement + " " + type + " " + result);
//     if (result == 0)
//     {
//         this.adError(placement);
//     }
//     else if (result == 1)
//     {
//         this.adShow(placement);
//     }
//     else if (result == 2)
//     {
//         KochavaBridge.Instance.adView(placement, type);
//         this.adView(placement, type);
//     }
// }
// }