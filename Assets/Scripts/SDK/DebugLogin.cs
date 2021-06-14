
// using System;

// // 测试用，点击后就登录，再点后就登出
// public class DebugLogin : ILoginInterface {
//     public string getName() { return "DebugLogin"; }
//     public string channel {
//         get { return MyChannels.debug; }
//     }

//     //// init
//     public void sinit() {
//         this.emit(SDKEvent.Inited, this);
//     }
//     public bool isInited {
//         get { return true; }
//     }
//     public void onEnterGame() {

//     }
//     public void onLogoutGame() {

//     }
//     public bool useNativeButton {
//         get { return false; }
//     }

//     public void showNativeButton(int x, int y, int w, int h) {

//     }
//     public void hideNativeButton() {

//     }

//     //// login
//     private bool logged = false;
//     public void login(Action<bool> cb) {
//         this.logged = true;
//         if (cb != null) {
//             cb(this.logged);
//         }
//     }
//     public bool isLogged {
//         get { return this.logged; }
//     }
//     public string channelUserId {
//         get { return "debugChannelUserId006"; }
//     }
//     public string originalChannelUserId {
//         get { return this.channelUserId; }
//     }
//     public object verifyData {
//         get { return new object(); }
//     }
//     public void clearCache() {

//     }

//     public bool hasAccountInfo {
//         get { return false; }
//     }
//     public void viewAccountInfo() {

//     }
//     public void logout() {
//         this.logged = false;
//     }
// }