// public enum PlayLimitType
// {
//     None,
//     DayLimit, // 每天只能玩 1.5 小时
//     TooLate, // 22点后不能玩
// }

// public enum ChargeLimitType
// {
//     None,
//     OneLimit,
//     MonthlyLimit,
// }

// public enum AgeLevel
// {
//     L8,
//     L16,
//     L18,
//     Adult,
// }

// public interface IAuthInterface : ISDKInterface
// {
//     AgeLevel ageLevel { get; }
//     bool isAuthed { get; }
//     void auth();
//     int isAuthLiteralAuthOrLogin(); // 1-auth 2-login
//     ChargeLimitType isChargeLimit(int currCharge, User user);
//     PlayLimitType isPlayLimit(User user);
//     bool silentlyPlayFreeTime();
// }

// public class DefaultAuth : IAuthInterface
// {
//     public string getName() { return "DefaultAuth"; }
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

//     }
//     public void onLogoutGame()
//     {

//     }
//     public int age
//     {
//         get { return 18; }
//     }
//     public AgeLevel ageLevel
//     {
//         get { return AgeLevel.Adult; }
//     }

//     public bool isAuthed
//     {
//         get { return true; }
//     }

//     public void auth()
//     {
//     }

//     public int isAuthLiteralAuthOrLogin()
//     { // 1-auth 2-login
//         return 1;
//     }

//     public ChargeLimitType isChargeLimit(int currCharge, User user)
//     {
//         return ChargeLimitType.None;
//     }

//     public PlayLimitType isPlayLimit(User user)
//     {
//         return PlayLimitType.None;
//     }

//     public bool silentlyPlayFreeTime()
//     {
//         return true;
//     }
// }