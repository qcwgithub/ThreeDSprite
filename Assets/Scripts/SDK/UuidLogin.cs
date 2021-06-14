using Data;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public class UuidLogin : ILoginInterface {
    public string getName() { return "UuidLogin"; }
    public string channel {
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
    public Task init() {
        // init UuidLogin

        // uuid.userChannelId 过程
        // 1-单机过度时从 localStorage.userProfile 读取（新玩家无此步骤）
        // 2-如果没有1，则新建一个
        // 3-首次初始化后，需要存起来
        // 4-在绑定账号后，清除；如果是切换为渠道账号，则不会清除
        // string uuid = LSUtils.GetItem(LSKeys.UUID_CHANNEL_USER_ID, null);
        // if (uuid == null) {
        //     var p = this.loadUserProfile();
        //     if (p != null) {
        //         uuid = p.userID;
        //     }
        //     else {
        //         uuid = v4();
        //     }
        //     LSUtils.SetItem(LSKeys.UUID_CHANNEL_USER_ID, uuid);
        // }
        // this._channelUserId = uuid;
        this.emit(SDKEvent.Inited);
        return null;
    }
    public bool isInited {
        get { return true; }
    }
    public void onEnterGame() {

    }
    public void onLogoutGame() {

    }
    //// login
    public bool useNativeButton {
        get { return false; }
    }
    public void showNativeButton(int x, int y, int w, int h) {

    }
    public void hideNativeButton() {

    }

    public void login(Action<bool> cb) {
        if (cb != null) {
            cb(true);
        }
    }
    public bool isLogged {
        get { return true; }
    }
    private string _channelUserId = null;
    public string channelUserId {
        get { return this._channelUserId; }
    }
    public string originalChannelUserId {
        get { return this.channelUserId; }
    }
    public object verifyData {
        get { return new object(); }
    }


    public void clearCache() {

    }

    public bool hasAccountInfo {
        get { return false; }
    }
    public void viewAccountInfo() {

    }
    public void logout() {
        throw new Exception("98098088");
    }
}