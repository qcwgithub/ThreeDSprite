using System.Collections.Generic;

public enum ProfileNumberIndex {
    Version = 0,
    BeginnerGiftVoucherBuyTime = 1,
    BeginnerMoneyIndex = 2,
    AuthRewardIndex = 3,    // 认证奖励
    Count,
}

public class ProfileNumbers {
    public List<int> array;

    static ProfileNumbers ensure(ProfileNumbers obj) {
        if (obj == null) {
            obj = new ProfileNumbers();
        }
        if (obj.array == null) {
            obj.array = new List<int>();
        }
        while (obj.array.Count < (int)ProfileNumberIndex.Count) {
            obj.array.Add(0);
        }

        // 也许以后需要做字段变更
        // version = obj.array[ProfileNumberIndex.Version]

        return obj;
    }

    static bool isValidNumberIndex(int i) {
        return (i > (int)ProfileNumberIndex.Version && i < (int)ProfileNumberIndex.Count);
    }
}