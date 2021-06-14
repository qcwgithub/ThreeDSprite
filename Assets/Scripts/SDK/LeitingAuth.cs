// using System;
// using System.Collections.Generic;
// using System.Threading.Tasks;
// using UnityEngine;

// public class AuthData
// {
//     string birthday;
//     string status;
//     string age;
// }

// public class LeitingAuth : LeitingBase, IAuthInterface
// {
//     public override string getName() { return "LeitingAuth"; }
//     private bool _authed = false;
//     public bool isAuthed
//     {
//         get { return this._authed; }
//     }
//     private void setAuthed(bool b)
//     {
//         // Debug.Log("set authed " + b);
//         if (b != this._authed)
//         {
//             this._authed = b;
//             this.emit(SDKEvent.AuthStateChanged);
//         }
//     }

//     private AuthData authData = null;

//     // override
//     protected override void initNativeCallbacks()
//     {
//         base.initNativeCallbacks();

//         // var _cc: any = cc;
//         // _cc.LeitingBridge = _cc.LeitingBridge || {};
//         // _cc.LeitingBridge.onRealNameAuthCallback = d => this.onRealNameAuthCallback(d);
//     }
//     // override
//     protected override string className
//     {
//         get { return "LeitingAuth"; }
//     }

//     public override async Task init()
//     {
//         await base.init();
//         this.loadData();
//     }
//     // -1: auth1 < auth2
//     //  1: auth1 > auth2
//     //  0: auth1 == auth2
//     // private int compareAuth(ProfileAuth auth1, ProfileAuth auth2)
//     // {
//     //     if (auth1.authed != auth2.authed)
//     //     {
//     //         return !auth1.authed ? -1 : 1;
//     //     }
//     //     if (!auth1.authed)
//     //     {
//     //         return 0;
//     //     }
//     //     if (auth1.age != auth2.age)
//     //     {
//     //         return auth1.age < auth2.age ? -1 : 1;
//     //     }
//     //     return 0;
//     // }
//     public override void onEnterGame()
//     {
//         base.onEnterGame();

//         // if (this.leitingLogin)
//         // {
//         //     this.onLtLoginStateChanged();
//         // }

//         // if (!this.isAuthed && LoadingComponent.serverList.popAuth)
//         // {
//         //     this.auth();
//         // }
//     }
//     private void checkCopy()
//     {
//         // var profile = sc.profile;
//         // if (profile == null)
//         // {
//         //     Debug.Log("checkCopy profile==null(not init end)");
//         //     return;
//         // }

//         // var thisAuth = new ProfileAuth
//         // {
//         //     authed = this.isAuthed,
//         //     age = this.age,
//         // };
//         // var cmp = this.compareAuth(profile.auth, thisAuth);
//         // if (cmp == -1)
//         // {
//         //     Debug.Log("copy auth to game");
//         //     // copy game <-- this
//         //     profile.auth.authed = this.isAuthed;
//         //     profile.auth.age = this.age;
//         //     sc.game.profileChanged(ProfileType.auth);
//         // }
//         // else if (cmp == 1)
//         // {
//         //     Debug.Log("copy auth from game");
//         //     // copy game --> this
//         //     this._authed = profile.auth.authed;
//         //     this.age = profile.auth.age;
//         //     this.saveAuthed(this._authed);
//         //     this.saveAge(this.age);
//         // }
//     }

//     // 加载之前存储的数据
//     private void loadData()
//     {
//         this._authed = this.loadAuthed();
//         if (this._authed)
//         {
//             this.age = this.loadAge();
//             if (!this.isAdult())
//             {
//                 var newAge = this.calcAge(this.loadBirthday());
//                 if (newAge > this.age)
//                 {
//                     Debug.Log("LeitingBridge: new age");
//                     this.age = newAge;
//                     this.saveAge(this.age);
//                 }
//             }
//             this.refreshLimit(this.age);
//         }

//         Debug.Log("LeitingAuth.loadData authed ? " + this._authed);
//     }

//     private void saveAuthed(bool b)
//     {
//         // cc.sys.localStorage.setItem("LF8799XC", b ? "1" : "0");
//     }
//     private bool loadAuthed()
//     {
//         // return cc.sys.localStorage.getItem("LF8799XC") == "1";
//         return true;
//     }

//     private void saveAge(int age)
//     {
//         // var s = EncryptUtils.encrypt(age.toString());
//         // cc.sys.localStorage.setItem("AYUI8XP1", s);
//     }

//     private int loadAge()
//     {
//         // int age = 0;
//         // try
//         // {
//         //     var s = cc.sys.localStorage.getItem("AYUI8XP1");
//         //     if (s)
//         //     {
//         //         s = EncryptUtils.decrypt(s);
//         //         age = parseInt(s);
//         //     }
//         // }
//         // catch
//         // {

//         // }
//         // return age || 0;
//         return 0;
//     }

//     private void saveBirthday(object birthday)
//     {
//         // if (birthday)
//         // {
//         //     string s = EncryptUtils.encrypt(birthday.toString());
//         //     cc.sys.localStorage.setItem("B5NV9ER3", s);
//         // }
//     }
//     private string loadBirthday()
//     {
//         // var s = cc.sys.localStorage.getItem("B5NV9ER3");
//         // if (s)
//         // {
//         //     s = EncryptUtils.decrypt(s);
//         // }
//         // return s;
//         return null;
//     }

//     private int age = 0;
//     public AgeLevel ageLevel { get { return AgeLevel.L8; } }

//     public int oneChargeLimit = 0;
//     public int monthlyChargeLimit = 0;
//     public int dayPlayTimeS = 0;

//     public bool isAdult()
//     {
//         return this.age >= 18;
//     }

//     private int calcAge(string s)
//     {
//         // if (s == null || s.Length != 8)
//         // {
//         //     return -1;
//         // }

//         // int birthYear = 0;
//         // int birthMonth = 0;
//         // int birthDay = 0;
//         // try
//         // {
//         //     birthYear = int.Parse(s.Substring(0, 4));
//         //     birthMonth = int.Parse(s.Substring(4, 2));
//         //     birthDay = int.Parse(s.Substring(6, 2));
//         // }
//         // catch
//         // {
//         //     return -1;
//         // }

//         // int d = TimeMgr.Instance.clientFrameTime;
//         // int year = d.getFullYear();
//         // if (birthYear >= year)
//         // {
//         //     return 0;
//         // }

//         // int age = year - birthYear;
//         // int month = d.getMonth() + 1;
//         // if (birthMonth != month)
//         // {
//         //     return birthMonth < month ? age : age - 1;
//         // }
//         // int day = d.getDate();
//         // return birthDay <= day ? age : age - 1;
//         return 0;
//     }

//     public PlayLimitType isPlayLimit(User user)
//     {
//         // if (this.isAdult())
//         // {
//         //     return PlayLimitType.None;
//         // }

//         // var h = TimeMgr.Instance.clientFrameTime.getHours();
//         // if (h >= 22 || h < 8)
//         // {
//         //     return PlayLimitType.TooLate;
//         // }

//         // int todayPlayTimeS = user.getTodayPlayTimeS();
//         // if (todayPlayTimeS > this.dayPlayTimeS)
//         // {
//         //     return PlayLimitType.DayLimit;
//         // }

//         return PlayLimitType.None;
//     }

//     private void refreshLimit(int age)
//     {
//         // if (age < 8)
//         // {
//         //     this.ageLevel = AgeLevel.L8;
//         //     this.oneChargeLimit = 0;
//         //     this.monthlyChargeLimit = 0;
//         //     this.dayPlayTimeS = 5400;
//         //     return;
//         // }
//         // if (age < 16)
//         // {
//         //     this.ageLevel = AgeLevel.L16;
//         //     this.oneChargeLimit = 50;
//         //     this.monthlyChargeLimit = 200;
//         //     this.dayPlayTimeS = 5400;
//         //     return;
//         // }
//         // if (age < 18)
//         // {
//         //     this.ageLevel = AgeLevel.L18;
//         //     this.oneChargeLimit = 100;
//         //     this.monthlyChargeLimit = 400;
//         //     this.dayPlayTimeS = 5400;
//         //     return;
//         // }
//         // this.ageLevel = AgeLevel.Adult;
//         // this.oneChargeLimit = 9999;
//         // this.monthlyChargeLimit = 99999999;
//         // this.dayPlayTimeS = 9999999;
//     }

//     public ChargeLimitType isChargeLimit(int currCharge, User user)
//     {
//         // if (currCharge > this.oneChargeLimit)
//         // {
//         //     return ChargeLimitType.OneLimit;
//         // }

//         // if (user.getMonthlyCharge() + currCharge > this.monthlyChargeLimit)
//         // {
//         //     return ChargeLimitType.MonthlyLimit;
//         // }

//         return ChargeLimitType.None;
//     }

//     public bool silentlyPlayFreeTime()
//     {
//         return this.isLeitingOfficial();
//     }

//     public int isAuthLiteralAuthOrLogin()
//     {
//         // 1-auth 2-login
//         return 1;
//         // if (this.isLeitingOfficial()) {
//         //     return 1;
//         // }
//         // else {
//         //     return 2;
//         // }
//     }

//     public void auth()
//     {
//         if (this.isAuthed)
//         {
//             return;
//         }
//         // if (this.isByLogin) {
//         //     this.leitingLogin.login(suc => {
//         //         if (suc) {
//         //             var loginData = this.leitingLogin.loginData;
//         //             if (loginData.auth == "1") {
//         //                 this.age = 18;
//         //             }
//         //             else {
//         //                 this.age = parseInt(loginData.age);
//         //             }
//         //             this.refreshLimit(this.age);
//         //             this.saveAuthed(true);
//         //             this.saveAge(this.age);

//         //             if (cb) {
//         //                 cb(true);
//         //             }
//         //         }
//         //         else {
//         //             if (cb) {
//         //                 cb(false);
//         //             }
//         //         }
//         //     });
//         // }
//         // else {
//         // switch (cc.sys.platform)
//         // {
//         //     case cc.sys.ANDROID:
//         //         return jsb.reflection.callStaticMethod("com/YezhStudio/LeitingBridge", "realNameAuth", "()V");
//         //         break;
//         //     case cc.sys.IPHONE:
//         //     case cc.sys.IPAD:
//         //         return jsb.reflection.callStaticMethod("LeitingBridge", "realNameAuth");
//         //         break;
//         // }
//         // }
//     }

//     private LeitingLogin leitingLogin = null;
//     public void setLeitingLogin(LeitingLogin login)
//     {
//         this.leitingLogin = login;
//         login.on(SDKEvent.LoginStateChanged, this.onLtLoginStateChanged);
//         this.onLtLoginStateChanged(null);
//     }
//     private void onLtLoginStateChanged(ISDKInterface _)
//     {
//         if (this.leitingLogin.isLogged)
//         {
//             var loginData = this.leitingLogin.loginData;
//             // 防沉迷 0：已实名认证未成年 1：已实名认证成年人 2：未实名认证游客账号返回2
//             if (loginData.auth == "2")
//             {
//                 return;
//             }
//             try
//             {
//                 this.age = int.Parse(loginData.age);
//             }
//             catch
//             {
//                 return;
//             }
//             this.setAuthed(true);
//             this.refreshLimit(this.age);
//             this.saveAuthed(true);
//             this.saveAge(this.age);

//             this.checkCopy();
//         }
//     }

//     private void onRealNameAuthCallback(object data)
//     {
//         // Debug.Log("onRealNameAuthCallback: " + JSON.stringify(data));
//         // bool suc = false;
//         // if (data && data.status == "1")
//         // {
//         //     suc = true;
//         //     this.setAuthed(true);
//         //     this.authData = data;
//         //     this.age = parseInt(this.authData.age);
//         //     this.refreshLimit(this.age);

//         //     this.saveAuthed(true);
//         //     this.saveAge(this.age);
//         //     this.saveBirthday(this.authData.birthday);

//         //     this.checkCopy();
//         // }
//     }
// }