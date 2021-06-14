using System;
using System.Threading.Tasks;

public interface ISDKInterface
{
    string getName();
    //// init
    Task init();
    bool isInited { get; }

    void onEnterGame();
    void onLogoutGame();

    void on(string eventName, Action<ISDKInterface> cb);
}