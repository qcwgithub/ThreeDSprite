// using System;
// using System.Collections.Generic;

// public class ApplePay : IPayInterface {
//     public string getName() { return "ApplePay"; }
//     private readonly string className = "IAPBridge";
//     private bool _inited = false;

//     // 提供给原生回调
//     private void initNativeCallbacks() {
//         //var _cc: any = cc;
//         //_cc.IAP = _cc.IAP || {};
//         //_cc.IAP.didPayiOS = (data) => this.didPayiOS(data);
//     }

//     public void init() {
//         if (this._inited) return;
//         this._inited = true;
//         jsb.reflection.callStaticMethod(this.className, "init");

//         this.initNativeCallbacks();
//         this.emit(SDKEvent.Inited, this);
//     }
//     public bool isInited {
//         get { return this._inited; }
//     }
//     public void onEnterGame() {
//         this.isReady();

//         if (Bootstrap.Instance && this.cache.length > 0) {
//             var arr = this.cache;
//             this.cache = new List<int[]>();
//             for (int i = 0; i < arr.length; i++) {
//                 this.didPayiOS(arr[i]);
//             }
//         }
//     }
//     public void onLogoutGame() {

//     }

//     public IapConfig iapConfig {
//         get { return Bootstrap.Instance.iapConfig; }
//     }
//     private List<RProductInfo> products = new List<RProductInfo>();
//     private bool productInited = false;
//     private void initProducts() {
//         if (!this._isReady) {
//             return;
//         }
//         if (this.productInited) {
//             return;
//         }

//         try {
//             string str = jsb.reflection.callStaticMethod(this.className, "getProducts");
//             List<RProductInfo> array = JSON.parse<List<RProductInfo>>(str);
//             for (int i = 0; i < array.length; i++) {
//                 var ele = array[i];
//                 var item = sc.game.vipScript.getItemByProductId(ele.productId);
//                 if (item == null) {
//                     // 如果苹果后台有其他多余的项....
//                     Debug.Log("item == null, ignore productId: " + array[i].productId);
//                     continue;
//                 }

//                 RProductInfo info = new RProductInfo {
//                     id = item.id,
//                     productId = ele.productId,
//                     price = Utils.purePrice(ele.priceS),
//                     priceS = ele.priceS,
//                     config = item,
//                 };
//                 this.products.push(info);
//             }
//             this.productInited = true;
//         }
//         catch (Exception ex) {
//             Bugly.error("iap", $"isReady - getProducts error:{error}", false);
//         }
//     }

//     private bool _isReady = false;
//     public bool isReady() {
//         if (this._isReady) {
//             return true;
//         }
//         this._isReady = jsb.reflection.callStaticMethod(this.className, "isReady") == 1;
//         if (this._isReady) {
//             this.initProducts();
//         }

//         return this._isReady;
//     }

//     public int finishTransaction(string transactionId) {
//         return jsb.reflection.callStaticMethod(this.className, "finishTransaction:", transactionId);
//     }
//     public void refreshProducts() {
//         Debug.Log("ApplePay.refreshProducts()");
//         jsb.reflection.callStaticMethod(this.className, "refreshProducts");
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
//     public List<RProductInfo> getProducts() {
//         this.isReady();
//         return this.products;
//     }
//     public RProductInfo getMonthlyCardProduct() {
//         return this.getProductById(this.iapConfig.monthlyCard.id);
//     }

//     // 0初始 1玩家点了买 2失败
//     private int buyClicked = 0;
//     private void buy(int id) {
//         var product = this.getProductById(id);
//         if (product == null) {
//             Bugly.error("iap", "2465468465321321," + id);
//             return;
//         }
//         this.buyClicked = 1;
//         sc.loading.show("pay", 0, 180);
//         jsb.reflection.callStaticMethod(this.className, "buy:", product.productId);
//     }

//     //// 订阅
//     public void subscribe() {
//         this.buy(this.iapConfig.monthlyCard.id);
//     }
//     public void restore() {
//         jsb.reflection.callStaticMethod(this.className, "restore");
//     }

//     //// 买钻石
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

//     private string fetchReceipt() {
//         return jsb.reflection.callStaticMethod(this.className, "fetchReceipt");
//     }
//     // 仅当有服务器时使用
//     // 1) 假设这个消息发送至服务器失败了，下次启动游戏会从 didPayiOS 再调过来
//     // 2) 假设这个消息发送至服务器成功了，但客户端没有收到回复，也没事，下次启动游戏会从 didPayiOS 再调过来
//     private bool paying = false;
//     private requestPayiOS(bool block) {
//         if (this.paying) {
//             return;
//         }
//         this.paying = true;

//         string receipt = this.fetchReceipt();
//         Debug.Log("receipt.length = " + receipt.length);
//         var msg = new MsgPay { receipt = receipt, };

//         sc.loading.cancelHideForgroundOnce(); // 立刻显示进度条
//         var timeoutMs = 60000; // 这个请求需要等待的时间长一些
//         ClientServer.Instance.request(MsgType.PMPayiOS, msg, block, r => {
//             this.paying = false;

//             if (r.err == ECode.Success) {
//                 var res = (ResPay)r.res;
//                 sc.game.vipScript.payExecute(sc.game, msg, res);

//                 Bootstrap.Instance.handleResPay(res);
//             }
//         }, timeoutMs);
//     }

//     private List<int[]> cache = new List<int[]>();
//     private int counter = 0;
//     private didPayiOS(int[] data) {
//         if (Bootstrap.Instance == null) {
//             Debug.Log("didPayiOS cached!" + JSON.stringify(data));
//             this.cache.Add(data);
//             return;
//         }
//         Debug.Log("didPayiOS " + JSON.stringify(data));

//         var purchasing = data[0];
//         var defered = data[1];
//         var failed = data[2];
//         var purchased = data[3];
//         var restored = data[4];

//         if (purchasing == 0 && defered == 0 && failed > 0 && purchased == 0 && restored == 0) {
//             if (this.buyClicked == 1) {
//                 Debug.Log("this.buyClicked == 1, show buy failed, and call restore!");
//                 Bootstrap.Instance.showBuyResultGui().initWithDiamond(false, 0);
//                 this.restore();
//             }
//         }


//         if (defered > 0 || failed > 0 || purchased > 0 || restored > 0) {
//             // 已结束
//             sc.loading.hide("pay");
//         }

//         // if (purchased == 0 && restored > 0 && this.vip.isVip) {
//         //     // 已是VIP的情况下不处理恢复购买
//         //     return;
//         // }

//         // 如果只是 restore，不显示 loading
//         if (purchased > 0 || restored > 0) {
//             this.requestPayiOS(purchased > 0 && this.counter > 0);
//         }

//         this.buyClicked = 0;
//         this.counter++;
//     }
// }