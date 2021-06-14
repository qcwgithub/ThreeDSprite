
// using System;
// using System.Collections.Generic;
// public class GooglePay : IPayInterface {
//     public string getName() { return "GooglePay"; }
//     private readonly string className = "org/cocos2dx/javascript/IAP/IAPBridge";
//     private bool inited = false;
//     public void init() {
//         if (this.inited) return;
//         this.inited = true;
//         jsb.reflection.callStaticMethod(this.className, "initWithKey", "()V");
//         this.emit(SDKEvent.Inited, this);
//     }
//     public bool isInited {
//         get { return this.inited; }
//     }

//     public void onEnterGame() {
//         this._iapConfig = sc.iapConfig;
//     }
//     public void onLogoutGame() {

//     }

//     private IapConfig _iapConfig = null;
//     public IapConfig iapConfig { get { return this._iapConfig; } }

//     public int finishTransaction(string transactionId) {
//         return 1;
//     }
//     public void refreshProducts() {

//     }

//     //// 订阅
//     // getProductionPriceS(): string {
//     //     return jsb.reflection.callStaticMethod(this.className, "getProductionPrice", "()Ljava/lang/String;");
//     // }
//     // getProductionPrice(): number {
//     //     return Utils.purePrice(this.getProductionPriceS());
//     // }
//     public void subscribe() {
//         return jsb.reflection.callStaticMethod(this.className, "purchaseSubscription", "()V");
//     }
//     public void restore() {
//         return jsb.reflection.callStaticMethod(this.className, "restoreSubscription", "()V");
//     }

//     public RProductInfo getProductById(int id) {
//         return null;
//     }
//     public RProductInfo getProductByProductId(string productId) {
//         return null;
//     }
//     private List<RProductInfo> empty = new List<RProductInfo>();
//     public List<RProductInfo> getProducts() {
//         return this.empty;
//     }
//     public RProductInfo getMonthlyCardProduct() {
//         return this.getProductById(this.iapConfig.monthlyCard.id);
//     }

//     //// 买钻石
//     public bool isReady {
//         get { return true; }
//     }

//     public void buyDiamond(int id) {
//     }

//     //// 买礼品券
//     public void buyGift(int id) {

//     }

//     //// 买金币
//     public void buyBeginnerMoney(int id) {

//     }
// }