using System;
public interface ILoginInterface : ISDKInterface
{
    string channel { get; }

    //// login
    bool useNativeButton { get; }
    void showNativeButton(int x, int y, int w, int h);
    void hideNativeButton();
    void login(Action<bool> cb);
    bool isLogged { get; }
    string channelUserId { get; }
    string originalChannelUserId { get; }
    object verifyData { get; }
    void clearCache();
    bool hasAccountInfo { get; }
    void viewAccountInfo();
    void logout();
}

// public class DefaultLogin : EventEmitter implements ILoginInterface {
//     get channelType() {
//         return 'default';
//     }

//     //// init
//     init() {
//         this.emit(SDKEvent.Inited, this);
//     }
//     public bool isInited {
//         return true;
//     }

//     //// login
//     private _logged = false;
//     login() {
//         this._logged = true;
//         this.emit(SDKEvent.StatusChanged, this);
//     }
//     get busy() {
//         return false;
//     }
//     get logged(): boolean {
//         return this._logged;
//     }

//     logout() {
//         this._logged = false;
//         this.emit(SDKEvent.StatusChanged, this);
//     }
// }