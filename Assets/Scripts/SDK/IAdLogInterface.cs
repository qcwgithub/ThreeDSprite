// public interface IAdLogInterface : ISDKInterface
// {
//     void initWithGame();
//     void login();
//     void logout();
//     void purchase(string type//: 'vip' | 'diamond' | 'giftVoucher' | 'money'
//     , string tid, string price);
//     void adClick(string placeId, string type);
//     void adShow(string placeId);
//     void adView(string placeId, string type);
//     void adGiveAward(string placeId);
//     void adError(string placeId);
//     void adGoto(string placeId);
// }

// public class DefaultAdLog : IAdLogInterface
// {
//     public string getName() { return "DefaultAdLog"; }
//     public void init()
//     {
//         this.emit(SDKEvent.Inited, this);
//     }
//     public bool isInited
//     {
//         get
//         {
//             return true;
//         }
//     }
//     public void onEnterGame()
//     {

//     }
//     public void onLogoutGame()
//     {

//     }
//     public void initWithGame() { }

//     public void login() { }
//     public void logout() { }
//     public void purchase(string type//: "vip" | "diamond" | "giftVoucher' | 'money'
//     , string tid, string price)
//     { }
//     public void adClick(string placeId, string type) { }
//     public void adShow(string placeId) { }
//     public void adView(string placeId, string type) { }
//     public void adGiveAward(string placeId) { }
//     public void adError(string placeId) { }
//     public void adGoto(string placeId) { }
// }