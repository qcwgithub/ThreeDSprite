using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using Data;
using Script;
using System;

public class BMServer : ClientServer
{
    public ResEnterBattle resEnterBattle { get; set; }
    public override bool isConnected => throw new System.NotImplementedException();

    public BMNetworkStatus status { get; protected set; } = BMNetworkStatus.Init;
    public string statusMsg { get; protected set; }
    public event Action<BMNetworkStatus, string> OnStatusChange;

    protected void setStatus(BMNetworkStatus status, string statusMsg = null)
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

    public override void start()
    {
        this.loginProcedure();
    }

    private TcpClientScriptC protoBM = null;
    public BMResPlayerLogin resBM { get; private set; }
    public event Action<bool> OnBMConnectionChange;
    private int loginSucCount = 0;

    
    private LoginBMFailAdvice getLoginBMFailAdvice(MyResponse r, int bmConnectFailCount, bool isReconnect)
    {
        var ret = new LoginBMFailAdvice
        {
            retryBM = true,
            retryBMMs = 3000,
            logout = false,
        };

        // 这一段不需要。如果其他客户端已经登上，此时错误会是 InvalidToken
        // 除非：AAAPlayerLogin 使用相同的 token。因此这一段还是留着吧
        // if (isReconnect && r.err == ECode.OldSocket && r.res != null && Bootstrap.Instance != null)
        // {
        //     var old = (r.res as ResMisc).oldSocketTimestamp;
        //     if (old > 0 && old != BMServer.processTimestamp)
        //     {
        //         ret.retryBM = false;
        //         ret.retryAAA = false;
        //         ret.logout = true;
        //         return ret;
        //     }
        // }

        // switch (r.err)
        // {
        //     case ECode.InvalidParam:
        //     case ECode.InvalidToken:
        //     case ECode.PlayerNotExist:
        //     case ECode.ShouldLoginAAA:
        //     case ECode.OldPlayer:
        //         ret.retryBM = false;
        //         ret.retryAAA = true;
        //         ret.retryAAAMs = 0; // 迅速重连
        //         break;
        //     case ECode.ConnectFailed:
        //         if (bmConnectFailCount >= 6)
        //         {
        //             ret.retryBM = false;
        //             ret.retryAAA = true;
        //         }
        //         break;
        //     default:
        //         Debug.LogWarning("unhandled bm login error: " + r.err);
        //         break;
        // }
        return ret;
    }

    BMMsgPlayerLogin getMsgBM(bool isReconnect)
    {
        var msg = new BMMsgPlayerLogin();
        msg.battleId = this.resEnterBattle.battleId;
        msg.playerId = 0;
        msg.token = "";
        return msg;
    }

    private async Task<MyResponse> loginBMOnce(TcpClientScriptC proto, bool isReconnect)
    {
        MyResponse rBM = null;

        this.setStatus(BMNetworkStatus.ConnectToBattle);

        proto.onConnect = (bool success, string message) =>
        {
            if (!success)
            {
                rBM = new MyResponse(ECode.ConnectFailed, null);
                this.setStatus(BMNetworkStatus.ConnectToBattleFailed, message);
            }
            else
            {
                this.setStatus(BMNetworkStatus.LoginToBattle);
                var msgBM = this.getMsgBM(isReconnect);

                proto.send(MsgType.BMPlayerLogin, msgBM, (ECode err, object res) =>
                {
                    var r = rBM = new MyResponse(err, res);
                    if (r.err != ECode.Success)
                    {
                        // Debug.Log(">LoginToBM - error: " + r.err);
                        // _this.onFail(NetworkError.BMLoginError, r.err);
                        this.setStatus(BMNetworkStatus.LoginToBattleFailed, r.err.ToString());
                    }
                    else
                    {
                        //Debug.Log(">LoginToBM - success, r.res: " + JSON.stringify(r.res));
                        // _this.onSuccess(r.res as ResLoginBM);
                        // Debug.Log(">LoginToBattle - success, resBM.id: " + (r.res as ResBMPlayerLogin).id);
                    }
                }, 10000);
            }
        };

        proto.onClose = (string message) =>
        {
            rBM = new MyResponse(ECode.ConnectFailed, null);
            this.setStatus(BMNetworkStatus.LoginToBattleFailed, message);
        };

        proto.open();
        while (rBM == null)
        {
            await Task.Delay(10);
        }
        return rBM;
    }

    async void loginProcedure()
    {
        Debug.Log("login procedure start");
        int bmConnectFailCount = 0;

        while (true)
        {
            if (this.destroyed)
            {
                Debug.Log("login procedure: give up because this.destroyed");
                break;
            }

            if (this.protoBM != null)
            {
                if (this.protoBM.isConnected())
                {
                    // this.resendTimeoutRequests();

                    // Debug.Log("wait 1000");
                    await Task.Delay(1000);
                    continue;
                }

                this.protoBM.cleanup();
                this.protoBM = null;

                if (this.OnBMConnectionChange != null)
                {
                    this.OnBMConnectionChange(false);
                }
            }

            this.protoBM = new TcpClientScriptC(this.resEnterBattle.bmIp, this.resEnterBattle.bmPort);
            bool isReconnect = this.loginSucCount > 0;
            MyResponse rBM = await this.loginBMOnce(this.protoBM, isReconnect);
            if (rBM.err == ECode.Success)
            {
                bmConnectFailCount = 0;
                this.resBM = rBM.res as BMResPlayerLogin;

                // TODO 放这好像不对
                //if (TimeMgr.Instance != null)
                //{
                //    TimeMgr.Instance.SetServerTime(this.resBM.timeMs, this.resBM.timezoneOffset);
                //}

                // Debug.Log("fire logintobmsucceeded!");
                this.setStatus(BMNetworkStatus.LoginToBattleSucceeded);

                if (this.loginSucCount == 0)
                {
                    this.onFirstLoginBMSuccess();
                }

                this.loginSucCount++;
                this.protoBM.onClose = null; // reset to null

                if (this.OnBMConnectionChange != null)
                {
                    this.OnBMConnectionChange(true);
                }
            }
            else
            {
                this.protoBM.cleanup();
                this.protoBM = null;
                if (rBM.err == ECode.ConnectFailed)
                {
                    bmConnectFailCount++;
                }
                LoginBMFailAdvice advice = this.getLoginBMFailAdvice(rBM, bmConnectFailCount, isReconnect);
                if (advice.logout)
                {
                    Bootstrap.Instance.logout();
                    return;
                }
                if (advice.retryBM)
                {
                    Debug.Log("wait " + advice.retryBMMs + " to login BM");
                    await Task.Delay(advice.retryBMMs);
                    continue;
                }
            }
        }
    }

    void onFirstLoginBMSuccess()
    {

    }
}
