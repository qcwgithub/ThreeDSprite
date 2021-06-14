using System;
using System.Collections.Generic;

public enum SubscribeType
{
    Week,
    Month,
}

public class RProductInfo
{
    public int id;
    public string productId;
    public int price;  // 元
    public string priceS; // 元
    // public IapItemBase config;
}

// public interface IPayInterface : ISDKInterface
// {
//     IapConfig iapConfig { get; }
//     bool isReady { get; }
//     int finishTransaction(string transactionId);
//     void refreshProducts();

//     //// products access
//     RProductInfo getProductById(int id);
//     RProductInfo getProductByProductId(string productId);
//     List<RProductInfo> getProducts();
//     RProductInfo getMonthlyCardProduct();

//     //// 订阅
//     void subscribe();
//     void restore();

//     //// 买钻石
//     void buyDiamond(int id);

//     //// 买礼品券
//     void buyGift(int id);

//     //// 买金币
//     void buyBeginnerMoney(int id);
// }