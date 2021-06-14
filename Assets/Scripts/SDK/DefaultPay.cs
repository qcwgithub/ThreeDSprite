// using System;
// using System.Collections.Generic;
// using Data;

// public class DefaultPay : IPayInterface
// {
//     public string getName() { return "DefaultPay"; }
//     private List<RProductInfo> products = new List<RProductInfo>();
//     public void init()
//     {
//         this.emit(SDKEvent.Inited, this);
//     }
//     public bool isInited
//     {
//         get { return true; }
//     }
//     public void onEnterGame()
//     {
//         this._iapConfig = Bootstrap.Instance.iapConfig;
//         var vipScript = sc.game.vipScript;

//         if (this.products.length == 0 && !PlatformUtils.hasServer())
//         {
//             this.products.Add(new RProductInfo
//             {
//                 id = 201,
//                 productId = "diamond01",
//                 priceS = "¥ 6",
//                 price = 6,
//                 config = vipScript.getItemById(201),
//             });
//             this.products.Add(new RProductInfo
//             {
//                 id = 202,
//                 productId = "diamond02",
//                 priceS = "¥ 6",
//                 price = 6,
//                 config = vipScript.getItemById(202),
//             });
//             this.products.Add(new RProductInfo
//             {
//                 id = 101,
//                 productId = "com.jys.hermes.ios.goldpass02",
//                 priceS = "¥ 30",
//                 price = 30,
//                 config = vipScript.getItemById(101),
//             });
//             this.products.Add(new RProductInfo
//             {
//                 id = 301,
//                 productId = "com.jys.hermes.ios.gift01",
//                 priceS = "¥ 1",
//                 price = 1,
//                 config = vipScript.getItemById(301),
//             });
//             this.products.Add(new RProductInfo
//             {
//                 id = 302,
//                 productId = "com.jys.hermes.ios.gift02",
//                 priceS = "¥ 6",
//                 price = 6,
//                 config = vipScript.getItemById(302),
//             });

//             this.products.Add(new RProductInfo
//             {
//                 id = 401,
//                 productId = "com.jys.hermes.ios.beginnermoney01",
//                 priceS = "¥ 1",
//                 price = 1,
//                 config = vipScript.getItemById(401),
//             });
//             this.products.Add(new RProductInfo
//             {
//                 id = 402,
//                 productId = "com.jys.hermes.ios.beginnermoney02",
//                 priceS = "¥ 1",
//                 price = 1,
//                 config = vipScript.getItemById(402),
//             });
//             this.products.Add(new RProductInfo
//             {
//                 id = 403,
//                 productId = "com.jys.hermes.ios.beginnermoney03",
//                 priceS = "¥ 1",
//                 price = 1,
//                 config = vipScript.getItemById(403),
//             });
//             this.products.Add(new RProductInfo
//             {
//                 id = 404,
//                 productId = "com.jys.hermes.ios.beginnermoney04",
//                 priceS = "¥ 3",
//                 price = 3,
//                 config = vipScript.getItemById(404),
//             });
//             this.products.Add(new RProductInfo
//             {
//                 id = 405,
//                 productId = "com.jys.hermes.ios.beginnermoney05",
//                 priceS = "¥ 6",
//                 price = 6,
//                 config = vipScript.getItemById(405),
//             });
//             this.products.Add(new RProductInfo
//             {
//                 id = 406,
//                 productId = "com.jys.hermes.ios.beginnermoney06",
//                 priceS = "¥ 12",
//                 price = 12,
//                 config = vipScript.getItemById(406),
//             });
//         }
//     }
//     public void onLogoutGame()
//     {

//     }
//     private IapConfig _iapConfig = null;
//     public IapConfig iapConfig { get { return this._iapConfig; } }

//     public int finishTransaction(string transactionId)
//     {
//         return 1;
//     }
//     public void refreshProducts()
//     {

//     }

//     public RProductInfo getProductById(int id)
//     {
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
//         for (int i = 0; i < this.products.Count; i++)
//         {
//             if (this.products[i].productId == productId)
//             {
//                 return this.products[i];
//             }
//         }

//         return null;
//     }
//     public List<RProductInfo> getProducts()
//     {
//         return this.products;
//     }
//     public RProductInfo getMonthlyCardProduct()
//     {
//         return this.getProductById(this.iapConfig.monthlyCard.id);
//     }

//     //// 订阅
//     public void subscribe()
//     {
//         if (PlatformUtils.hasServer())
//         {
//             return;
//         }
//         var t = TimeMgr.Instance.getTime() - 1000;
//         var res = new ResPay
//         {
//             items = new List<PurchasedItem>(),
//             monthlyCardUpdated = true,
//             purchaseDateMs = t,
//             expiresDateMs = t + 1000 * 60 * 10,
//             numberUpdates = new List<int>(),
//         };

//         sc.game.vipScript.payExecute(sc.game, null, res);
//         Bootstrap.Instance.handleResPay(res);
//     }
//     public void restore()
//     {

//     }

//     //// 买钻石
//     public bool isReady()
//     {
//         return true;
//     }

//     public void buyDiamond(int id)
//     {
//         if (PlatformUtils.hasServer())
//         {
//             return;
//         }
//         var product = this.getProductById(id);
//         var res = new ResPay
//         {
//             items = new List<PurchasedItem>{
//                 new PurchasedItem {
//                     duplicated = false,
//                     productId = product.productId,
//                     transactionId = v4(),
//                     addDiamond = (product.config as IapDiamondItem).diamond,
//                     addGiftVoucher = 0,
//                 }
//             },
//             monthlyCardUpdated = false,
//             purchaseDateMs = 0,
//             expiresDateMs = 0,
//             numberUpdates = []
//         };

//         sc.game.vipScript.payExecute(sc.game, null, res);
//         Bootstrap.Instance.handleResPay(res);
//     }

//     //// 买礼品券
//     public void buyGift(int id)
//     {
//         if (PlatformUtils.hasServer())
//         {
//             return;
//         }
//         var product = this.getProductById(id);
//         var giftItem = product.config as IapGiftVoucherItem;
//         List<int> numberUpdates = new List<int>();
//         if (giftItem.oncePerDay)
//         {
//             if (ProfileNumbers.isValidNumberIndex(giftItem.counterNumberIndex))
//             {
//                 numberUpdates.Add(giftItem.counterNumberIndex);
//                 numberUpdates.Add(TimeMgr.Instance.getTodayTime(0, 0, 0, 0));
//             }
//         }

//         var res = new ResPay
//         {
//             items = new List<PurchasedItem> { new PurchasedItem {
//                 duplicated = false,
//                 productId = product.productId,
//                 transactionId = v4(),
//                 addDiamond = 0,
//                 addGiftVoucher = giftItem.voucher,
//             }},
//             monthlyCardUpdated = false,
//             purchaseDateMs = 0,
//             expiresDateMs = 0,
//             numberUpdates = new List<int>(),
//         };

//         sc.game.vipScript.payExecute(sc.game, null, res);

//         Bootstrap.Instance.handleResPay(res);
//     }
//     //// 买金币
//     public void buyBeginnerMoney(int id)
//     {
//         if (PlatformUtils.hasServer())
//         {
//             return;
//         }

//         List<int> numberUpdates = new List<int>();
//         var array = this.iapConfig.beginnerMoney;
//         for (int i = 0; i < array.length; i++)
//         {
//             if (id == array[i].id)
//             {
//                 numberUpdates.Add(ProfileNumberIndex.BeginnerMoneyIndex);
//                 numberUpdates.Add(i + 1); // 设置为下一个
//                 break;
//             }
//         }

//         var product = this.getProductById(id);
//         var res = new ResPay
//         {
//             items = new List<PurchasedItem> { new PurchasedItem {
//                 duplicated = false,
//                 productId = product.productId,
//                 transactionId = v4(),
//                 addDiamond = 0,
//                 addGiftVoucher = 0,
//             }},
//             monthlyCardUpdated = false,
//             purchaseDateMs = 0,
//             expiresDateMs = 0,
//             numberUpdates = new List<int>(),
//         };

//         sc.game.vipScript.payExecute(sc.game, null, res);
//         Bootstrap.Instance.handleResPay(res);
//     }
// }