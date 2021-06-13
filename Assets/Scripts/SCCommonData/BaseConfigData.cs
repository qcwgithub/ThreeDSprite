using System;

public partial class BaseConfigData
{
    public BattlefieldConfig battlefieldConfig;
    public VitalityConfig vitalityConfig;
    public OfflineConfig offlineConfig;
    public MapEventConfig mapEventConfig;
    public LotteryConfig lotteryConfig;
}

[Serializable]
public class VitalityConfig
{
    public int checkpointBattleExpand;
    public int maxVitality;
    public int vitalityDurationS;
}

[Serializable]
public class OfflineConfig
{
    public int maxOfflineTimeH;
    public int offlineDurationS;
    public int moneyPulsH;                  // 每小时产出个数等于岛屿个数的平方*这个数
    public int reinforcedMatePulsH;         // 每小时产出个数等于岛屿个数的平方*这个数
    public int advancedMatePerIslandD;    // 每天产出个数等于岛屿个数*这个值
}

[Serializable]
public class MapEventConfig
{
    public int robDurationH;
    public int maxCanNotRobIsland;
    public int robSuccessRate;
}

[Serializable]
public class BattlefieldConfig
{
    public int rotationDestroyThreshold;
    public int battleDurationS;
}

[Serializable]
public class LotteryConfig
{
    public int onceLotteryPrice;
    public int tenLotteryPrice;
}