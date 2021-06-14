// using System;
// using System.Collections.Generic;
// using UnityEngine;
// using Data;

// // Y：必传
// // N：非必传

// class LeitingPayParam
// {
//     public string userId;     // Y 账号唯一标识
//     public string zoneId;     // Y 区组ID
//     public string money;      // Y 充值金额，单位：分
//     public string notifyUri;  // Y 服务器端通知地址
//     public string orderId;    // Y 游戏订单号（需保证唯一性）
//     public string roleId;     // N 角色ID
//     public string roleName;   // N 角色名
//     public string extInfo;    // N 透传字段，充值成功时原样回传给游戏服务端通知接口
//     public string productName; // Y 商品名称
//     public string productId;  // Y 商品ID
//     public string showGameArea;// N 是否在充值页面显示区组信息 0不显示 1显示 默认1
//     public string ratio;      // N 人民币跟游戏币比例，整数，例如100表示1元人民币兑换100游戏币
// }
// class LeitingPayResult
// {
//     public string status;     // Y 1：成功，其他状态为失败
//     public string leitingNo;  // N 雷霆订单号
//     public string money;      // N 金额，单位：分
//     public string resultMsg;  // N 提示信息
// }

// public class LeitingPay : LeitingBase, IPayInterface
// {
//     public override string getName() { return "LeitingPay"; }
//     // override
//     protected override void initNativeCallbacks()
//     {
//         base.initNativeCallbacks();

//         // var _cc: any = cc;
//         // _cc.LeitingBridge = _cc.LeitingBridge || {};
//         // _cc.LeitingBridge.onPayCallback = d => this.onPayCallback(d);
//     }

//     // override
//     protected override string className
//     {
//         get { return "LeitingPay"; }
//     }

//     private bool _isReady = false;
//     private List<RProductInfo> products = new List<RProductInfo>();
//     private void reloadProducts()
//     {
//         this.products.Clear();

//         var iapConfig = this.iapConfig;
//         for (int i = 0; i < iapConfig.platformInfo.leiting.length; i++)
//         {
//             var ltProduct = iapConfig.platformInfo.leiting[i];
//             // var iapItem = Bootstrap.Instance.masterGame.vipScript.getItemById(ltProduct.id);
//             var info = new RProductInfo
//             {
//                 id = ltProduct.id,
//                 productId = ltProduct.productId,
//                 price = ltProduct.rmb,
//                 priceS = "¥" + ltProduct.rmb.toString(),
//                 config = sc.game.vipScript.getItemById(ltProduct.id)
//             };
//             if (info.config == null)
//             {
//                 Debug.LogError("iapitembase is null, id: " + ltProduct.id);
//             }
//             this.products.Add(info);
//         }
//     }

//     public bool isReady
//     {
//         get
//         {
//             if (this._isReady)
//             {
//                 return true;
//             }
//             this._isReady = this.iapConfig != null;
//             if (this._isReady)
//             {
//                 this.reloadProducts();
//             }
//             return this._isReady;
//         }
//     }

//     public RProductInfo getProductById(int id)
//     {
//         bool b = this.isReady;
//         for (int i = 0; i < this.products.Count; i++)
//         {
//             if (this.products[i].id == id)
//             {
//                 return this.products[i];
//             }
//         }

//         return null;
//     }
//     public RProductInfo getProductByProductId(string productId)
//     {
//         bool b = this.isReady;
//         for (int i = 0; i < this.products.Count; i++)
//         {
//             if (this.products[i].productId == productId)
//             {
//                 return this.products[i];
//             }
//         }

//         return null;
//     }
//     public RProductInfo getMonthlyCardProduct()
//     {
//         return this.getProductById(this.iapConfig.monthlyCard.id);
//     }
//     public List<RProductInfo> getProducts()
//     {
//         return this.products;
//     }

//     // override
//     public override void onEnterGame()
//     {
//         base.onEnterGame();
//         var b = this.isReady;

//         var clientProfile = sc.clientProfile;
//         if (clientProfile.ltPay == 1)
//         {
//             Debug.Log("clientProfile.ltPay == 1, setInterval");
//             this.setInterval();
//         }
//     }

//     public override void onLogoutGame()
//     {
//         base.onLogoutGame();
//         this.clearInterval();
//     }

//     private bool pending = false;
//     private void onInterval()
//     {
//         if (this.pending)
//         {
//             Debug.Log("LeitingPay.onInterval pending");
//             return;
//         }

//         if (ClientServer.Instance == null || !ClientServer.Instance.isConnected())
//         {
//             Debug.Log("LeitingPay.onInterval not connected");
//             return;
//         }
//         Debug.Log("LeitingPay.onInterval");

//         this.pending = true;
//         // 随便传一个 receipt，反正用不上
//         var msg = new MsgPay { receipt = "no-use-receipt" };
//         ClientServer.Instance.request(MsgType.PMPayLt, msg, true, r =>
//         {
//             this.pending = false;
//             if (r.err == ECode.Success)
//             {
//                 var res = (ResPay)r.res;
//                 sc.game.vipScript.payExecute(sc.game, msg, res);
//                 Bootstrap.Instance.handleResPay(res);
//                 this.clearInterval();

//                 // 清除标记
//                 var clientProfile = sc.clientProfile;
//                 clientProfile.ltPay = 0;
//                 clientProfile.saveltPay();
//             }
//         });
//     }

//     public IapConfig iapConfig
//     {
//         get { return Bootstrap.Instance.iapConfig; }
//     }

//     public int finishTransaction(string transactionId)
//     {
//         return 1;
//     }
//     public void refreshProducts()
//     {
//         Debug.LogError("TODO");
//     }

//     //// 订阅
//     private bool checkLogged()
//     {
//         var realServer = ClientServer.Instance as RealServer;
//         var interface_ = SDKManager.Instance.getLoginInterface(realServer.channel);
//         if (!interface_.isLogged)
//         {
//             interface_.login(null);
//             return false;
//         }
//         return true;
//     }

//     public void subscribe()
//     {
//         this.buy(this.iapConfig.monthlyCard.id);
//     }
//     public void restore()
//     {
//         this.onInterval();
//     }

//     private void buy(int id)
//     {
//         if (!this.checkLogged())
//         {
//             return;
//         }

//         var product = this.getProductById(id);
//         if (product == null)
//         {
//             Bugly.error("iap", "4689765631," + id);
//             return;
//         }
//         string title = "";
//         switch (product.config.productType)
//         {
//             case ProductType.MonthlyCard:
//                 title = "月卡";
//                 break;
//             case ProductType.Diamond:
//                 title = "钻石x" + (product.config as IapDiamondItem).diamond;
//                 break;
//             case ProductType.GiftVoucher:
//                 title = "礼品券x" + (product.config as IapGiftVoucherItem).voucher;
//                 break;
//             case ProductType.BeginnerMoney:
//                 title = "新手礼包" + (id % 100).toString();
//                 break;
//             default:
//                 title = "title missing";
//                 console.error("title missing!!!");
//                 break;
//         }

//         this.sendRequest(product.productId, title, product.price);
//     }

//     //// 买钻石

//     private int getPrice(int id)
//     {
//         var ltProduct = sc.game.vipScript.getLeitingItemById(id);
//         if (ltProduct == null)
//         {
//             Bugly.error("iap", "497520983092," + id);
//             return 0;
//         }
//         return ltProduct.rmb;
//     }

//     public int getDiamondPrice(int id)
//     {
//         return this.getPrice(id);
//     }

//     private void sendRequest(string productId, string productName, int rmb)
//     {
//         Debug.Log("send request, productId: " + productId);
//         var fen = (rmb * 100).ToString();
//         var msg = new MsgPayLtStart { productId = productId, fen = fen };
//         ClientServer.Instance.request(MsgType.PMPayLtStart, msg, true, r =>
//         {
//             var res = (ResPayLtStart)r.res;
//             if (r.err == ECode.Success)
//             {
//                 Debug.Log("orderId: " + res.orderId);

//                 sc.loading.show("pay", 0, 180);
//                 var realServer = ClientServer.Instance as RealServer;
//                 var json = new LeitingPayParam
//                 {
//                     userId = SDKManager.Instance.getLoginInterface(realServer.channel).originalChannelUserId,
//                     zoneId = "cn",
//                     money = fen,
//                     notifyUri = realServer.payNotifyUri,
//                     orderId = res.orderId,
//                     roleId = realServer.playerId.toString(),
//                     roleName = sc.game.profile.userName,
//                     extInfo = "",
//                     productName = productName,
//                     productId = productId,
//                     showGameArea = "0",
//                     ratio = "",
//                 };
//                 jsb.reflection.callStaticMethod("com/YezhStudio/LeitingBridge", "pay", "(Ljava/lang/String;)V", JSON.stringify(json));
//             }
//             else if (r.err == ECode.ProductPriceRefreshed)
//             { // 与服务器价格不一致时，需要刷新界面，让玩家重新点按钮
//                 if (res)
//                 {
//                     Bootstrap.Instance.overrideLeitingProducts(res.ltProducts);
//                     this.reloadProducts();
//                     BankGemGUI.Instance && BankGemGUI.Instance.refreshAllBuy();
//                 }
//             }
//         });
//     }

//     public void buyDiamond(int id)
//     {
//         this.buy(id);
//     }

//     //// 买礼品券
//     public void buyGift(int id)
//     {
//         this.buy(id);
//     }

//     //// 买金币
//     public void buyBeginnerMoney(int id)
//     {
//         this.buy(id);
//     }

//     private reqTimer: NodeJS.Timeout = null;
//     private void setInterval()
//     {
//         if (this.reqTimer == null)
//         {
//             Debug.Log("LeitingPay.setInterval");
//             this.reqTimer = setInterval(() =>
//             {
//                 this.onInterval();
//             }, 2000);
//         }
//     }
//     private void clearInterval()
//     {
//         if (this.reqTimer != null)
//         {
//             Debug.Log("LeitingPay.clearInterval");
//             clearInterval(this.reqTimer);
//             this.reqTimer = null;
//         }
//     }

//     private void onPayCallback(LeitingPayResult result)
//     {
//         sc.loading.hide("pay");
//         if (result.status !== "1")
//         {
//             Debug.Log("pay failed");
//             return;
//         }

//         // 设置标记
//         var clientProfile = sc.clientProfile;
//         clientProfile.ltPay = 1;
//         clientProfile.saveltPay();

//         this.setInterval();
//     }
// }