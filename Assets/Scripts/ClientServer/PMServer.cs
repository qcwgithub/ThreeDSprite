using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using System.Threading.Tasks;

using Data;
using Script;

class CachedRequest
{
    public MsgType type;
    public object msg;
    public bool block;
    public Action<MyResponse> cb;
    public int timeoutMs;
}

public class PMServer : ClientServer
{
    public string aaaIp { get; set; }
    public int aaaPort { get; set; }

    public override bool isConnected
    {
        get
        {
            return this.protoPM != null && this.protoPM.isConnected();
        }
    }

    public PMNetworkStatus status { get; protected set; } = PMNetworkStatus.Init;
    public string statusMsg { get; protected set; }
    public event Action<PMNetworkStatus, string> OnStatusChange;

    protected void setStatus(PMNetworkStatus status, string statusMsg = null)
    {
        if (this.status != status)
        {
            this.status = status;
            this.statusMsg = statusMsg;
            Debug.Log("status: " + status + ", message: " + statusMsg);

            if (this.OnStatusChange != null)
            {
                this.OnStatusChange(status, statusMsg);
            }
        }
    }

    #region init
    public override void start()
    {
        if (this.aaaIp == null)
        {
            Debug.LogError("aaaIp is null");
            return;
        }

        // load last channel
        this.initChannel();

        var interface_ = SDKManager.Instance.getLoginInterface(this.channel);
        if (interface_ == null)
        {
            Debug.Log("channel " + this.channel + " not exist, fall back to uuid");
            this.setChannel(MyChannels.uuid);
        }

        // start login process
        this.loginProcedure();
    }
    //#endregion

    //#region channel
    public string channel { get; private set; }
    public void initChannel()
    {
        if (PlatformUtils.getPlatformString() == MyChannels.pc)
        {
            this.channel = MyChannels.pc;
        }
        else
        {
            this.channel = LSUtils.GetString(LSKeys.CHANNEL, null);
            if (!MyChannels.isValidChannel(this.channel))
            {
                this.channel = MyChannels.uuid;

                LSUtils.SetString(LSKeys.CHANNEL, this.channel);
                LSUtils.Save();
            }
        }
        Debug.Log(">RealServer.initChannel: " + this.channel);
    }
    public event Action<string> OnChannelChanged;
    public void setChannel(string c)
    {
        if (this.channel != c)
        {
            this.channel = c;
            LSUtils.SetString(LSKeys.CHANNEL, c);
            if (OnChannelChanged != null)
            {
                OnChannelChanged(c);
            }
        }
    }
    //#endregion

    //#region login to sdk
    private async Task loginToSdk()
    {
        Debug.Log(">RealServer: login to sdk...");

        var end = false;
        // login to SDK
        var interface_ = SDKManager.Instance.getLoginInterface(this.channel);
        interface_.login(suc =>
        {
            if (!suc)
            {
                Debug.Log("channel " + this.channel + " login failed, fall back to uuid");
                this.setChannel(MyChannels.uuid);
                interface_ = SDKManager.Instance.getLoginInterface(this.channel);
                interface_.login(_ => end = true);
            }
            else
            {
                end = true;
            }
        });

        while (!end)
        {
            await Task.Delay(300);
        }
    }
    //#endregion

    //#region login to AAA
    private ResLoginAAA resAAA = null;
    public int playerId
    {
        get
        {
            if (this.resAAA != null)
            {
                return this.resAAA.playerId;
            }
            return 0;
        }
    }

    private MsgLoginAAA getMsgAAA()
    {
        var interface_ = SDKManager.Instance.getLoginInterface(this.channel);
        MsgLoginAAA msgAAA = new MsgLoginAAA
        {
            platform = PlatformUtils.getPlatformString(),
            version = PlatformUtils.getAppVersion(),

            // sdk
            channel = this.channel,
            channelUserId = interface_.channelUserId,
            verifyData = null,//interface_.verifyData,
            oaid = null,//SDKManager.Instance.device.getOAID(), // maybe null
            imei = null,//SDKManager.Instance.device.getIMEI(), // maybe null
        };
        return msgAAA;
    }
    public LoginAAAFailAdvice getLoginAAAFailAdvice(MyResponse r, bool isReconnect)
    {
        LoginAAAFailAdvice ret = new LoginAAAFailAdvice
        {
            retry = true,
            retryMs = 5000,
            fallbackToUuid = false,
            clearInterfaceCache = false,
            logout = false,
        };

        // 1 是否停止登录
        switch (r.err)
        {
            case ECode.AccountBan:
            case ECode.LowVersion:
                ret.retry = false;
                return ret;
            // break;
            default:
                break;
        }

        if (isReconnect && r.err == ECode.OldSocket && r.res != null && Bootstrap.Instance != null)
        {
            var old = (r.res as ResMisc).oldSocketTimestamp;
            if (old > 0 && old != PMServer.processTimestamp)
            {
                ret.retry = false;
                ret.logout = true;
                return ret;
            }
        }

        // 2 是否改用 MyChannels.uuid
        if (!isReconnect && this.channel != MyChannels.uuid)
        {
            switch (r.err)
            {
                case ECode.InvalidParam:
                case ECode.InvalidChannel:
                    ret.retryMs = 1000;
                    ret.fallbackToUuid = true;
                    return ret;
                    // break;
            }
        }

        // 3 是否等一会再重试
        switch (r.err)
        {
            case ECode.InvalidParam:
            case ECode.InvalidChannel:
                ret.retryMs = 5000;
                break;

            case ECode.ServerNotReady:
            case ECode.OldSocket:
            case ECode.NoAvailablePlayerManager:
            case ECode.VerifyAccountErrorResponse:
            case ECode.VerifyAccountErrorStatusCode:
            case ECode.Timeout:
            case ECode.ConnectFailed:
                ret.retryMs = 3000;
                break;

            case ECode.AccountNotExist2:
                Debug.Log("ECode.AccountNotExist2");
                ret.clearInterfaceCache = true;
                ret.retryMs = 1000;
                break;

            default:
                Debug.LogWarning("getLoginAAAFailAdvice what to do with: " + r.err);
                break;
        }

        return ret;
    }
    private async Task<MyResponse> loginAAAOnce()
    {
        this.setStatus(PMNetworkStatus.ConnectToAccount);

        var proto = new TcpClientScriptC(this.aaaIp, this.aaaPort);
        MyResponse rAAA = null;

        proto.onConnect = (bool success, string message) =>
        {
            if (!success)
            {
                rAAA = new MyResponse(ECode.ConnectFailed, null);
                this.setStatus(PMNetworkStatus.ConnectToAFailed, message);
            }
            else
            {
                var msgAAA = this.getMsgAAA();
                this.setStatus(PMNetworkStatus.LoginToAccount, msgAAA.channel);
                proto.send(MsgType.AAAPlayerLogin, msgAAA, (ECode err, object res) =>
                {
                    var r = rAAA = new MyResponse(err, res);
                    if (r.err != ECode.Success)
                    {
                        //Debug.Log(">LoginToAAA - login failed, e: " + r.err);
                        this.setStatus(PMNetworkStatus.LoginToAFailed, r.err.ToString());
                    }
                    else
                    {
                        Debug.Log(">aaa login success, playerId: " +
                            (r.res as ResLoginAAA).playerId + ", pmId: " +
                            (r.res as ResLoginAAA).pmId);
                    }
                }, 10000);
            }
        };

        proto.onClose = (string message) =>
        {
            rAAA = new MyResponse(ECode.ConnectFailed, null);
            this.setStatus(PMNetworkStatus.ConnectToAFailed, message);
        };

        proto.open();
        while (rAAA == null)
        {
            await Task.Delay(10);
        }

        proto.cleanup();
        return rAAA;
    }
    //#endregion

    //#region login to PM
    private TcpClientScriptC protoPM = null;
    public ResLoginPM resPM { get; private set; }
    private int loginSucCount = 0;
    public string payNotifyUri()
    {
        return this.resPM.payNotifyUri;
    }

    // 用处：用于标识此客户端
    // 登录时如果服务器说玩家已经登录，此时通过对比时间戳来判断是不是自己，如果是自己不需要回到loading场景；如果不是则需要回
    private static int _processTimestamp = 0;
    private static int processTimestamp
    {
        get
        {
            if (_processTimestamp == 0)
            {
                _processTimestamp = (int)DateTime.Now.Subtract(new DateTime(1970, 1, 1)).TotalMilliseconds;
            }
            return _processTimestamp;
        }
    }
    private MsgLoginPM getMsgPM(ResLoginAAA resAAA, bool isReconnect)
    {
        var msgPM = new MsgLoginPM
        {
            playerId = this.resAAA.playerId,
            token = this.resAAA.pmToken,
            isReconnect = isReconnect,
            //profile = null,
            timestamp = PMServer.processTimestamp,
        };

        if (isReconnect)
        {
            return msgPM;
        }

        // var uuidLogin = SDKManager.Instance.getLoginInterface(MyChannels.uuid) as UuidLogin;

        ////
        // if (LSUtils.GetItem(LSKeys.INIT_CLIENT_PROFILE_FROM_MASTER_S) != "1")
        // {
        //     LSUtils.SetItem(LSKeys.INIT_CLIENT_PROFILE_FROM_MASTER_S, "1");
        //     var p = uuidLogin.loadUserProfile();
        //     if (p != null)
        //     {
        //         Debug.Log(">transfer client only data S...");
        //         ClientProfile.s_moveClientOnlyData(resAAA.playerId.toString(), p);
        //     }
        // }

        ////
        // if (resAAA.needUploadProfile && LSUtils.GetItem(LSKeys.PROFILE_UPLOADED) != "1")
        // {
        //     var cp = uuidLogin.loadUserProfile();
        //     Debug.Log(">upload profile ? yes, profile exist ? " + (cp != null ? "yes" : "no"));
        //     msgPM.profile = cp;
        // }
        // else
        // {
        //     Debug.Log(">upload profile ? no");
        // }
        return msgPM;
    }
    private LoginPMFailAdvice getLoginPMFailAdvice(MyResponse r, int pmConnectFailCount, bool isReconnect)
    {
        var ret = new LoginPMFailAdvice
        {
            retryPM = true,
            retryPMMs = 3000,
            retryAAA = false,
            retryAAAMs = 3000,
            logout = false,
        };

        // 这一段不需要。如果其他客户端已经登上，此时错误会是 InvalidToken
        // 除非：AAAPlayerLogin 使用相同的 token。因此这一段还是留着吧
        if (isReconnect && r.err == ECode.OldSocket && r.res != null && Bootstrap.Instance != null)
        {
            var old = (r.res as ResMisc).oldSocketTimestamp;
            if (old > 0 && old != PMServer.processTimestamp)
            {
                ret.retryPM = false;
                ret.retryAAA = false;
                ret.logout = true;
                return ret;
            }
        }

        switch (r.err)
        {
            case ECode.InvalidParam:
            case ECode.InvalidToken:
            case ECode.PlayerNotExist:
            case ECode.ShouldLoginAAA:
            case ECode.OldPlayer:
                ret.retryPM = false;
                ret.retryAAA = true;
                ret.retryAAAMs = 0; // 迅速重连
                break;
            case ECode.ConnectFailed:
                if (pmConnectFailCount >= 6)
                {
                    ret.retryPM = false;
                    ret.retryAAA = true;
                }
                break;
            default:
                Debug.LogWarning("unhandled pm login error: " + r.err);
                break;
        }
        return ret;
    }
    private async Task<MyResponse> loginPMOnce(TcpClientScriptC proto, ResLoginAAA resAAA, bool isReconnect)
    {
        MyResponse rPM = null;

        this.setStatus(PMNetworkStatus.ConnectToGame);

        proto.onConnect = (bool success, string message) =>
        {
            if (!success)
            {
                rPM = new MyResponse(ECode.ConnectFailed, null);
                this.setStatus(PMNetworkStatus.ConnectToGFailed, message);
            }
            else
            {
                this.setStatus(PMNetworkStatus.LoginToG);
                var msgPM = this.getMsgPM(resAAA, isReconnect);

                proto.send(MsgType.PMPlayerLogin, msgPM, (ECode err, object res) =>
                {
                    var r = rPM = new MyResponse(err, res);
                    if (r.err != ECode.Success)
                    {
                        // Debug.Log(">LoginToPM - error: " + r.err);
                        // _this.onFail(NetworkError.PMLoginError, r.err);
                        this.setStatus(PMNetworkStatus.LoginToGFailed, r.err.ToString());
                    }
                    else
                    {
                        //Debug.Log(">LoginToPM - success, r.res: " + JSON.stringify(r.res));
                        // _this.onSuccess(r.res as ResLoginPM);
                        Debug.Log(">LoginToPM - success, resPM.id: " + (r.res as ResLoginPM).id);
                    }
                }, 10000);
            }
        };

        proto.onClose = (string message) =>
        {
            rPM = new MyResponse(ECode.ConnectFailed, null);
            this.setStatus(PMNetworkStatus.ConnectToGFailed, message);
        };

        proto.open();
        while (rPM == null)
        {
            await Task.Delay(10);
        }
        return rPM;
    }

    private void onFirstLoginPMSuccess()
    {
        // 登录成功之后才能标记
        //LSUtils.SetItem(LSKeys.PROFILE_UPLOADED, "1");

        var res = this.resPM;

        //// keeySync
        //SyncProfileComponent.keepSync = res.keepSyncProfile;

        //// master profile
        //var masterKey = LSKeys.s_formatNewMasterKey(res.id);
        //var newProfile = ProfileStorage.s_loadFromFile(masterKey);

        //if (newProfile != null)
        //{
        //    Debug.Log(">try override client profile to server profile ? yes");
        /* TODO */ // ProfileOverrider.override (res.profile, newProfile);
                   //}
                   //else
                   //{
                   //    Debug.Log(">try override client profile to server profile ? no");
                   //}

        //ProfileStorageMaster.Instance.setKey(masterKey);
        // 这里是为了把字符串转换成 bigInt
        //ProfileStorageMaster.Instance.setProfile(CProfile.ensure(res.profile, res.profile.userName));

        //// activity profile
        // var activityKey = LSKeys.s_formatNewActivityKey(res.id);
        // if (LSUtils.getItem(LSKeys.ACTIVITY_PROFILE_TO_NEW_KEY) != "1") {
        //     LSUtils.setItem(LSKeys.ACTIVITY_PROFILE_TO_NEW_KEY, "1");
        //     var aps = LSUtils.getItem(LSKeys.ACTIVITY_PROFILE_KEY, null);
        //     if (aps != null) {
        //         LSUtils.setItem(activityKey, aps);
        //     }
        // }
        // ProfileStorageActivity.Instance.setKey(activityKey);

        //// bugly userId
        // Bugly.setUserId("player" + res.id.toString());

        //// client profile userId
        // ClientProfile.Instance.setUserID(res.id.toString());
    }
    //#endregion

    //#region login process

    public event Action<bool, ResLoginPM> OnPMConnectionChange;

    private async void loginProcedure()
    {
        Debug.Log("login procedure start");
        bool toSdk = true;
        bool toAAA = true;
        int pmConnectFailCount = 0;

        while (true)
        {
            if (this.destroyed)
            {
                Debug.Log("login procedure: give up because this.destroyed");
                break;
            }
            if (toSdk)
            {
                toSdk = false;
                await this.loginToSdk();
            }
            if (toAAA)
            {
                MyResponse rAAA = await this.loginAAAOnce();
                if (rAAA.err == ECode.Success)
                {
                    this.resAAA = rAAA.res as ResLoginAAA;
                    this.setChannel(this.resAAA.channel);
                    SDKManager.Instance.getLoginInterface(this.channel);
                    this.setStatus(PMNetworkStatus.LoginToASucceeded);

                    toAAA = false;
                }
                else
                {
                    LoginAAAFailAdvice advice = this.getLoginAAAFailAdvice(rAAA, this.loginSucCount > 0);
                    if (advice.logout)
                    {
                        Bootstrap.Instance.logout();
                        return;
                    }
                    if (advice.fallbackToUuid)
                    {
                        Debug.Log("fall back to uuid");
                        this.setChannel(MyChannels.uuid);
                    }
                    if (advice.clearInterfaceCache)
                    {
                        var interface_ = SDKManager.Instance.getLoginInterface(this.channel);
                        interface_.clearCache();
                        toSdk = true;
                    }
                    if (advice.retry)
                    {
                        Debug.Log("wait " + advice.retryMs + " to login AAA");
                        await Task.Delay(advice.retryMs);
                    }
                    else
                    {
                        Debug.LogWarning("give up login");
                        return;
                    }
                    continue;
                }
            }

            if (this.protoPM != null)
            {
                if (this.protoPM.isConnected())
                {
                    this.resendTimeoutRequests();
                    // Debug.Log("wait 1000");
                    await Task.Delay(1000);
                    continue;
                }

                this.protoPM.cleanup();
                this.protoPM = null;

                if (this.OnPMConnectionChange != null)
                {
                    this.OnPMConnectionChange(false, null);
                }
            }

            this.protoPM = new TcpClientScriptC(resAAA.pmIp, resAAA.pmPort);
            bool isReconnect = this.loginSucCount > 0;
            MyResponse rPM = await this.loginPMOnce(this.protoPM, this.resAAA, isReconnect);
            if (rPM.err == ECode.Success)
            {
                pmConnectFailCount = 0;
                this.resPM = rPM.res as ResLoginPM;

                // TODO 放这好像不对
                if (TimeMgr.Instance != null)
                {
                    TimeMgr.Instance.SetServerTime(this.resPM.timeMs, this.resPM.timezoneOffset);
                }

                // TODO 放这好像不对
                sc.profile = Profile.Ensure(this.resPM.profile);

                // Debug.Log("fire logintopmsucceeded!");
                this.setStatus(PMNetworkStatus.LoginToGSucceeded);

                if (this.loginSucCount == 0)
                {
                    this.onFirstLoginPMSuccess();
                }

                this.loginSucCount++;
                this.protoPM.onClose = null; // reset to null

                if (this.OnPMConnectionChange != null)
                {
                    this.OnPMConnectionChange(true, this.resPM);
                }
            }
            else
            {
                this.protoPM.cleanup();
                this.protoPM = null;
                if (rPM.err == ECode.ConnectFailed)
                {
                    pmConnectFailCount++;
                }
                LoginPMFailAdvice advice = this.getLoginPMFailAdvice(rPM, pmConnectFailCount, isReconnect);
                if (advice.logout)
                {
                    Bootstrap.Instance.logout();
                    return;
                }
                if (advice.retryPM)
                {
                    Debug.Log("wait " + advice.retryPMMs + " to login PM");
                    await Task.Delay(advice.retryPMMs);
                    continue;
                }
                if (advice.retryAAA)
                {
                    pmConnectFailCount = 0;
                    Debug.Log("wait " + advice.retryAAAMs + " to login AAA");
                    await Task.Delay(advice.retryAAAMs);
                    toAAA = true;
                    continue;
                }
            }
        }
    }
    #endregion

    public override void OnDestroy()
    {
        this.cachedRequests.Clear();

        if (this.protoPM != null)
        {
            this.protoPM.cleanup();
            this.protoPM = null;
        }
        base.OnDestroy();
    }

    private List<CachedRequest> cachedRequests = new List<CachedRequest>();
    private void resendTimeoutRequests()
    {
        if (this.cachedRequests.Count > 0)
        {
            var arr = this.cachedRequests;
            this.cachedRequests = new List<CachedRequest>();

            for (var i = 0; i < arr.Count; i++)
            {
                var req = arr[i];
                Debug.Log("just connected, resend request ONCE for MsgType." + req.type);
                this.request(req.type, req.msg, req.block, req.cb, req.timeoutMs, false);
            }
            arr.Clear();
        }
    }

    //#region request

    public override void request(MsgType type, object _msg, bool block, Action<MyResponse> cb, int timeoutMs = 10000, bool retryOnReconnect = true)
    {
        if (this.destroyed)
        {
            return;
        }
        Debug.Log("request " + type + " " + type);

        if (block)
        {
            this.showRequestLoading(true, timeoutMs / 1000);
        }
        this.protoPM.send(type, _msg, (ECode err, object res) =>
        {
            Debug.Log(string.Format("response {0},{1}", err, JsonUtils.stringify(res)));

            var r = new MyResponse(err, res);
            if (block)
            {
                this.showRequestLoading(false, 0);
            }

            if (retryOnReconnect && (r.err == ECode.NotConnected || r.err == ECode.Timeout))
            {
                Debug.Log("ECode." + r.err + "! Cache request MsgType." + type);
                var cachedReq = new CachedRequest { type = type, msg = _msg, block = block, cb = cb, timeoutMs = timeoutMs };
                this.cachedRequests.Add(cachedReq);
            }
            else
            {
                this.reply2(cb, r);
                this.checkTimeJump(r);
            }
        }, timeoutMs);
    }

    // 检测客户端调时间
    private int TIMEGAP = 10000;
    private void checkTimeJump(object msg)
    {
        // if (!TimeMgr.Instance.hasServerTime())
        // {
        //     return;
        // }
        // var stime = msg[MagicValue.MSG_SERVERTIME];
        // if (!(stime > 0))
        // {
        //     return;
        // }
        // var ctime = TimeMgr.Instance.getTime();
        // var delta = stime - ctime;

        // if (CC_DEBUG)
        // {
        //     Debug.Log("stime " + stime + " ctime " + ctime + " delta " + delta);
        // }
        // if (delta > -this.TIMEGAP && delta < this.TIMEGAP)
        // {
        //     return;
        // }
        // Bugly.error("server", "stime " + stime + " ctime " + ctime + " delta " + delta);
        // setTimeout(() => Bootstrap.Instance.logout(), 10);
    }
    //#endregion
}
