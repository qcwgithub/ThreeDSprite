
// using System;
// using System.Collections.Generic;

// class PriceInfo {
//     string id; // com.ivy.xxx.xxx
//     string type; // inapp | subs
//     string price; // HK$15.00
//     int price_amount;
//     string currency; // HKD
//     string title;
//     string desc;
// }

// public class IvyPay : IvyBase. IPayInterface {
//     getName() { return "IvyPay"; }
//     // override
//     protected void initNativeCallbacks() {
//         super.initNativeCallbacks();

//         // var _cc: any = cc;
//         // _cc.IvyBridge = _cc.IvyBridge || {};
//         // _cc.IvyBridge.onPaymentSuccess = d => this.onPaymentSuccess(d);
//         // _cc.IvyBridge.onPaymentFail = d => this.onPaymentFail(d);
//         // _cc.IvyBridge.onPaymentCanceled = d => this.onPaymentCanceled(d);
//         // _cc.IvyBridge.onPaymentSystemValid = () => this.onPaymentSystemValid();
//         // _cc.IvyBridge.onPrices = d => this.onPrices(d);
//     }

//     // override
//     protected override string className {
//         get { return "IvyPay"; }
//     }

//     private bool _isReady = false;
//     private List<RProductInfo> products = new List<RProductInfo>();
//     private productInited = false;
//     private initProducts() {
//         if (!this._isReady) {
//             return;
//         }
//         if (this.productInited) {
//             return;
//         }

//         try {
//             var array: { int id, info: PriceInfo }[] = [];
//             this.prices.forEach((info, key, m) => {
//                 array.push({ id: key, info: info });
//             });
//             array.sort((a, b) => a.id - b.id);
//             for (int i = 0; i < array.length; i++) {
//                 var ele = array[i];
//                 var item = sc.game.vipScript.getItemById(ele.id);
//                 if (item == null) {
//                     // 如果后台有其他多余的项....
//                     Debug.Log("item == null, ignore item id: " + ele.id);
//                     continue;
//                 }

//                 var ivyInfo = sc.game.vipScript.getIvyItemById(ele.id);

//                 var info: RProductInfo = {
//                     id: item.id,
//                     productId: ivyInfo.productId,
//                     price: ele.info.price_amount,
//                     priceS: ele.info.price,
//                     config: item,
//                 };
//                 this.products.push(info);
//             }
//             this.productInited = true;
//         }
//         catch (error) {
//             Bugly.error("iap", `isReady - getProducts error:${error}`, false);
//         }
//     }

//     public bool isReady() {
//         if (this._isReady) {
//             return true;
//         }
//         if (this.prices.size == 0) {
//             this.getPrices();
//             return false;
//         }
//         this._isReady = true;
//         this.initProducts();
//         return true;
//     }
//     private int getPrices() {
//         switch (cc.sys.platform) {
//             case cc.sys.ANDROID:
//                 return jsb.reflection.callStaticMethod(this.androidClassName, "getPrices", "()V");
//                 break;
//             case cc.sys.IPHONE:
//             case cc.sys.IPAD:
//                 return jsb.reflection.callStaticMethod(this.iosClassName, "getPrices");
//                 break;
//         }
//     }

//     public RProductInfo getProductById(int id) {
//         this.isReady();
//         for (int i = 0; i < this.products.length; i++) {
//             if (this.products[i].id == id) {
//                 return this.products[i];
//             }
//         }

//         return null;
//     }
//     public RProductInfo getProductByProductId(string productId) {
//         this.isReady();
//         for (int i = 0; i < this.products.length; i++) {
//             if (this.products[i].productId == productId) {
//                 return this.products[i];
//             }
//         }

//         return null;
//     }
//     public RProductInfo getMonthlyCardProduct() {
//         return this.getProductById(this.iapConfig.monthlyCard.id);
//     }
//     public List<RProductInfo> getProducts() {
//         return this.products;
//     }

//     // override
//     public override void onEnterGame() {
//         super.onEnterGame();
//         this.isReady();

//         var clientProfile = sc.clientProfile;
//         if (clientProfile.ivyPay == 1) {
//             Debug.Log("clientProfile.ivyPay == 1, setInterval");
//             this.setInterval();
//         }
//     }

//     public override void onLogoutGame() {
//         super.onLogoutGame();
//         this.clearInterval();
//     }

//     private bool pending = false;
//     private void onInterval() {
//         if (this.pending) {
//             Debug.Log("IvyPay.onInterval pending");
//             return;
//         }

//         if (ClientServer.Instance == null || !ClientServer.Instance.isConnected()) {
//             Debug.Log("IvyPay.onInterval not connected");
//             return;
//         }

//         if (this.purchased.length == 0) {
//             Debug.Log("IvyPay.onInterval this.purchasedIds.length == 0");
//             this.clearInterval();
//             return;
//         }

//         Debug.Log("IvyPay.onInterval");

//         this.pending = true;
//         // 随便传一个 receipt，反正用不上
//         var msg = new MsgPay = { receipt: JSON.stringify(this.purchased) };
//         ClientServer.Instance.request(MsgType.PMPayIvy, msg, true, r => {
//             this.pending = false;
//             if (r.err == ECode.Success) {
//                 this.purchased = [];

//                 var res: ResPay = r.res;
//                 sc.game.vipScript.payExecute(sc.game, msg, res);
//                 Bootstrap.Instance.handleResPay(res);
//                 this.clearInterval();

//                 // 清除标记
//                 var clientProfile = sc.clientProfile;
//                 clientProfile.ivyPay = 0;
//                 clientProfile.saveivyPay();
//             }
//         });
//     }

//     get iapConfig() {
//         return Bootstrap.Instance.iapConfig;
//     }

//     public int finishTransaction(string transactionId) {
//         return 1;
//     }
//     public void refreshProducts() {
//         console.error("TODO");
//     }

//     public void subscribe() {
//         this.buy(this.iapConfig.monthlyCard.id);
//     }
//     public void restore() {
//         this.onInterval();
//     }

//     private void buy(int id) {
//         this.sendRequest(id);
//     }

//     //// 买钻石

//     private int getPrice(int id) {
//         return 0;
//     }

//     public int getDiamondPrice(int id) {
//         return this.getPrice(id);
//     }

//     private id2orderId = new Map<number, string>();
//     private void sendRequest(int id) {
//         Debug.Log("send request, id: " + id);

//         var msg: MsgPayIvyStart = { id: id };
//         ClientServer.Instance.request(MsgType.PMPayIvyStart, msg, true, r => {
//             var res: ResPayIvyStart = r.res;
//             if (r.err == ECode.Success) {
//                 Debug.Log("orderId: " + res.orderId);
//                 this.id2orderId.set(id, res.orderId);

//                 // Bootstrap.Instance.loading.show("pay", 0, 180);
//                 jsb.reflection.callStaticMethod(this.androidClassName, "pay", "(ILjava/lang/String;)V", id, res.orderId);
//             }
//         });
//     }

//     public void buyDiamond(int id) {
//         this.buy(id);
//     }

//     //// 买礼品券
//     public void buyGift(int id) {
//         this.buy(id);
//     }

//     //// 买金币
//     public void buyBeginnerMoney(int id) {
//         this.buy(id);
//     }

//     private reqTimer: NodeJS.Timeout = null;
//     private void setInterval() {
//         this.onInterval();
//         if (this.reqTimer == null) {
//             Debug.Log("IvyPay.setInterval");
//             this.reqTimer = setInterval(() => {
//                 this.onInterval();
//             }, 2000);
//         }
//     }
//     private void clearInterval() {
//         if (this.reqTimer != null) {
//             Debug.Log("IvyPay.clearInterval");
//             clearInterval(this.reqTimer);
//             this.reqTimer = null;
//         }
//     }

//     private purchased: { int id, string orderId }[] = [];
//     //支付成功
//     private void onPaymentSuccess(int billId) {
//         Debug.Log("onPaymentSuccess " + billId);
//         // Bootstrap.Instance.loading.hide("pay");

//         this.purchased.push({ id: billId, orderId: this.id2orderId.get(billId) });

//         // 设置标记
//         var clientProfile = sc.clientProfile;
//         clientProfile.ivyPay = 1;
//         clientProfile.saveivyPay();

//         this.setInterval();
//     }

//     //支付失败
//     private void onPaymentFail(int billId) {
//         Debug.Log("onPaymentFail " + billId);
//         // Bootstrap.Instance.loading.hide("pay");
//     }

//     public void onPaymentCanceled(int bill) {
//         Debug.Log("onPaymentCanceled " + bill);
//         // Bootstrap.Instance.loading.hide("pay");
//     }

//     //手机，平板等支持支付功能,支付环境有效的回调
//     private void onPaymentSystemValid() {
//         Debug.Log("onPaymentSystemValid");
//     }

//     private prices = new Map<number, PriceInfo>();
//     private void onPrices(json: any) {
//         try {
//             for (var k in json) {
//                 var id = parseInt(k);
//                 var info = json[k];
//                 this.prices.set(id, info);
//             }
//         } catch (ex) {
//             Bugly.error("ivy", "parse prices error, " + ex, false);
//         }
//     }
// }