using System;
using System.Collections.Generic;
using UnityEngine;

public class SDKConfig
{
    public List<string> logins;
    public string auth;
    public string pay;
    public List<string> others;
}

public class SDKManager
{
    private static SDKManager instance = null;
    public static SDKManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new SDKManager();
            }
            return instance;
        }
    }

    // SDKManager不关心当前哪个渠道已登录或下线，只负责提供接口
    private IDeviceInterface deviceInterface = null;
    private List<ILoginInterface> loginInterfaces = new List<ILoginInterface>();
    // private IAuthInterface authInterface = null;
    // private IAdLogInterface adLogInterface = null;
    // private IPayInterface payInterface = null;

    private List<ISDKInterface> allInterfaces = new List<ISDKInterface>();

    public bool loginEnabled
    {
        get { return this.loginInterfaces.Count > 0; }
    }

    public IDeviceInterface device => this.deviceInterface;
    // public IAuthInterface auth => this.authInterface;
    // public IAdLogInterface adLog => this.adLogInterface;
    // public IPayInterface pay => this.payInterface;

    // public LeitingBase anyLt = null;
    // public string getLeitingChannelId(string defaultValue)
    // {
    //     return this.anyLt != null ? this.anyLt.getChannelId() : defaultValue;
    // }

    private bool _inited = false;
    public void init()
    {
        if (this._inited)
        {
            Debug.Log("SDKManager ignore init");
            return;
        }
        this._inited = true;

        // 赋默认值，loginInterfaces 默认为空数组
        this.deviceInterface = null;
        // this.authInterface = new DefaultAuth();
        // this.adLogInterface = new DefaultAdLog();
        // this.payInterface = new DefaultPay();

        SDKConfig config = new SDKConfig(); // TODO (cc as any).preDefined.sdkConfig || { };
        if (config.logins == null) 
            config.logins = new List<string>();
        if (config.auth == null)
            config.auth = string.Empty;            
        if (config.pay == null)
            config.pay = string.Empty;
        if (config.others == null)
            config.others = new List<string>();

        var _logins = this.loginInterfaces;

        //// mobile
        //if (PlatformUtils.isMobile())
        //{
            // LeitingLogin ltLogin = null;
            // if (config.logins.Contains("leiting"))
            // {
            //     ltLogin = new LeitingLogin();
            //     _logins.Add(ltLogin);

            //     if (this.deviceInterface == null)
            //         this.deviceInterface = ltLogin;
            //     if (this.anyLt == null)
            //         this.anyLt = ltLogin;
            // }

            // LeitingAuth ltAuth = null;
            // if (config.auth == "leiting")
            // {
            //     ltAuth = new LeitingAuth();
            //     if (ltLogin != null)
            //     {
            //         // ltLogin 会影响 ltAuth
            //         ltAuth.setLeitingLogin(ltLogin);
            //     }
            //     this.authInterface = ltAuth;

            //     if (this.deviceInterface == null)
            //         this.deviceInterface = ltAuth;
            //     if (this.anyLt == null)
            //         this.anyLt = ltAuth;
            // }

            // if (ltLogin != null || ltAuth != null)
            // {
            //     // 启动 ltLogin 或 ltAuth 则同时开 LeitingAdLog
            //     this.adLogInterface = new LeiTingAdLog();
            // }
        //}

        //// iOS
        // if (PlatformUtils.isiOS())
        // {
        //     if (AppleLogin.isSupported() && config.logins.Contains("apple"))
        //     {
        //         _logins.Add(new AppleLogin());
        //     }
        //     if (config.pay == "apple")
        //     {
        //         this.payInterface = new ApplePay();
        //     }
        // }

        //// android
        //if (PlatformUtils.isAndroid())
        //{
            // if (config.logins.Contains("ivy"))
            // {
            //     var ivyLogin = new IvyLogin();
            //     this.deviceInterface = this.deviceInterface || ivyLogin;
            //     _logins.Add(ivyLogin);
            // }

            // if (config.pay == "leiting")
            // {
            //     Debug.Log("pay: leiting pay");
    
            //     this.payInterface = new LeitingPay();
            // }
            // else if (config.pay == "google")
            // {
            //     this.payInterface = new GooglePay();
            // }
            // else if (config.pay == "ivy")
            // {
            //     var ivyPay = new IvyPay();
            //     this.deviceInterface = this.deviceInterface || ivyPay;
            //     this.payInterface = ivyPay;
            // }

            // if (config.others.Contains("ivyTrack"))
            // {
            //     var ivyTrack = new IvyTrack();
            //     this.deviceInterface = this.deviceInterface || ivyTrack;
            //     this.allInterfaces.Add(ivyTrack);
            // }
        //}

        // if (config.logins.Contains("debug"))
        // {
        //     _logins.Add(new DebugLogin());
        // }

        _logins.Add(new UuidLogin());

        // if (this.deviceInterface == null)
        // {
        //     this.deviceInterface = new DefaultDevice();
        // }

        this.allInterfaces.AddRange(_logins);
        // this.allInterfaces.AddRange(new ISDKInterface[] { this.deviceInterface, this.authInterface, this.adLogInterface, this.payInterface });

        var buffer = new List<string>();
        for (var i = 0; i < this.allInterfaces.Count; i++)
        {
            buffer.Add(this.allInterfaces[i].getName());
        }

        Debug.Log(">SDKManager allInterfaces: " + string.Join("", buffer.ToArray()));

        // register events
        for (var i = 0; i < this.allInterfaces.Count; i++)
        {
            this.allInterfaces[i].on(SDKEvent.Inited, this.onInterfaceInited);
        }

        // call init
        for (var i = 0; i < this.allInterfaces.Count; i++)
        {
            this.allInterfaces[i].init();
        }
    }

    private bool _allSdkInited = false;
    public bool sdkMgrInited => this._allSdkInited;
    public event Action OnSDKMgrInited;
    private void onInterfaceInited(object who)
    {
        if (this._allSdkInited)
        {
            return;
        }
        var finish = true;
        for (var i = 0; i < this.allInterfaces.Count; i++)
        {
            if (!this.allInterfaces[i].isInited)
            {
                finish = false;
                break;
            }
        }
        if (finish)
        {
            this._allSdkInited = true;
            if (OnSDKMgrInited != null)
            {
                OnSDKMgrInited();
            }
        }
    }

    public ILoginInterface getLoginInterface(string channel)
    {
        for (var i = 0; i < this.loginInterfaces.Count; i++)
        {
            if (this.loginInterfaces[i].channel == channel)
            {
                return this.loginInterfaces[i];
            }
        }
        return null;
    }

    public void onEnterGame()
    {
        for (var i = 0; i < this.allInterfaces.Count; i++)
        {
            this.allInterfaces[i].onEnterGame();
        }
    }

    public void onLogoutGame()
    {
        for (var i = 0; i < this.allInterfaces.Count; i++)
        {
            this.allInterfaces[i].onLogoutGame();
        }
    }
}