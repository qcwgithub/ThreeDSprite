using System.Collections.Generic;

public enum ProductType
{
    MonthlyCard = 1,    // 订阅
    Diamond = 2,        // 钻石
    GiftVoucher = 3,    // 礼品券
    BeginnerMoney = 4,
}

public class IapItemBase
{
    public int id;
    // 运行时
    public ProductType productType;
}

public class IapMonthlyCardItem : IapItemBase
{
    public int dailyDiamond;
}

public class IapDiamondItem : IapItemBase
{
    public int diamond;
    public string icon;
    public bool hot;
    public int discount; //0 没有 >0有
}

public class IapGiftVoucherItem : IapItemBase
{
    public int voucher;
    public bool oncePerDay;    // 是不是一天只能买一次
    public int counterNumberIndex; // 在 CProfile.ints 的索引，当 dayOnce = true 时必须配置正确，当 dayOnce = false 时，配置为-1。
}

public class IapBeginnerMoneyItem : IapItemBase
{
    public int timeH;
}

public class IapAppleInfo
{
    public int id;
    public string[] productIds;
}

public class IapLeitingInfo
{
    public int id;
    public string productId;
    public int rmb;
}
public class IapIvyInfo
{
    public int id;
    public string productId;
}

public class _PlatformInfo
{
    public IapAppleInfo[] apple;
    public IapLeitingInfo[] leiting; // 会从服务器下载
    public IapIvyInfo[] ivy;
}

// 说明：
// 1) id 为游戏商品 id，productId 是平台 id
// 2) 不同平台的 productId 也不能重复
public class IapConfig
{
    public IapMonthlyCardItem monthlyCard;
    public IapDiamondItem[] diamond;
    public IapGiftVoucherItem[] giftVoucher;
    public IapBeginnerMoneyItem[] beginnerMoney;
    public _PlatformInfo platformInfo;

    // 运行时
    public Dictionary<int, IapItemBase> mapId;
    public Dictionary<string, IapItemBase> mapP;

    public Dictionary<int, IapAppleInfo> mapIdApple;
    public Dictionary<string, IapAppleInfo> mapPApple;

    public Dictionary<int, IapLeitingInfo> mapIdLeiting;
    public Dictionary<string, IapLeitingInfo> mapPLeiting;

    public Dictionary<int, IapIvyInfo> mapIdIvy;
    public Dictionary<string, IapIvyInfo> mapPIvy;
}