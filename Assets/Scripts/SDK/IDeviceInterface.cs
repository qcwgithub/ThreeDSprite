using System;
using System.Threading.Tasks;

public interface IDeviceInterface : ISDKInterface
{
    string getOAID();
    string getMac();
    string getIMEI();
    void quit();
}

// public class DefaultDevice : IDeviceInterface
// {
//     public string getName() { return "DefaultDevice"; }
//     //// init
//     public Task init()
//     {
//         this.emit(SDKEvent.Inited);
//         return null;
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
//     public string getOAID()
//     {
//         return null;
//     }

//     public string getMac()
//     {
//         return null;
//     }

//     public string getIMEI()
//     {
//         return null;
//     }

//     public void quit()
//     {

//     }
// }