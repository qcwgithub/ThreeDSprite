using Data;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class UuidLogin : ILoginInterface
{
    public string getName() { return "UuidLogin"; }
    public string channel
    {
        get { return MyChannels.uuid; }
    }

    // private bool inited1 = false;
    // private CProfile clientUserProfile = null;
    // public CProfile loadUserProfile() {
    //     if (this.inited1) {
    //         return this.clientUserProfile;
    //     }
    //     this.inited1 = true;
    //     this.clientUserProfile = ProfileStorage.s_loadFromFile(LSKeys.MASTER_PROFILE_KEY);
    //     return this.clientUserProfile;
    // }

    protected Dictionary<string, List<Action<ISDKInterface>>> dictEvents =
        new Dictionary<string, List<Action<ISDKInterface>>>();
    public void on(string eventName, Action<ISDKInterface> cb)
    {
        List<Action<ISDKInterface>> list;
        if (!this.dictEvents.TryGetValue(eventName, out list))
        {
            list = new List<Action<ISDKInterface>>();
            this.dictEvents.Add(eventName, list);
        }
        list.Add(cb);
    }
    public void off(string eventName, Action<ISDKInterface> cb)
    {
        List<Action<ISDKInterface>> list;
        if (this.dictEvents.TryGetValue(eventName, out list))
        {
            list.Remove(cb);
            if (list.Count == 0)
            {
                this.dictEvents.Remove(eventName);
            }
        }
    }
    protected void emit(string eventName)
    {
        List<Action<ISDKInterface>> list;
        if (this.dictEvents.TryGetValue(eventName, out list))
        {
            var array = list.ToArray();
            foreach (var action in array)
            {
                action(this);
            }
        }
    }

    //// init
    public Task init()
    {
        this._channelUserId = LSUtils.GetString(LSKeys.UUID_CHANNEL_USER_ID, null);
        if (string.IsNullOrEmpty(this._channelUserId))
        {
            this._channelUserId = Guid.NewGuid().ToString();
            LSUtils.SetString(LSKeys.UUID_CHANNEL_USER_ID, this._channelUserId);
        }
        Debug.Log("this._channelUserId = " + this._channelUserId);
        this.emit(SDKEvent.Inited);
        return null;
    }
    public bool isInited
    {
        get { return true; }
    }
    public void onEnterGame()
    {

    }
    public void onLogoutGame()
    {

    }
    //// login
    public bool useNativeButton
    {
        get { return false; }
    }
    public void showNativeButton(int x, int y, int w, int h)
    {

    }
    public void hideNativeButton()
    {

    }

    public void login(Action<bool> cb)
    {
        if (cb != null)
        {
            cb(true);
        }
    }
    public bool isLogged
    {
        get { return true; }
    }
    private string _channelUserId = null;
    public string channelUserId
    {
        get { return this._channelUserId; }
    }
    public string originalChannelUserId
    {
        get { return this.channelUserId; }
    }
    public object verifyData
    {
        get { return new object(); }
    }


    public void clearCache()
    {

    }

    public bool hasAccountInfo
    {
        get { return false; }
    }
    public void viewAccountInfo()
    {

    }
    public void logout()
    {
        throw new Exception("98098088");
    }
}