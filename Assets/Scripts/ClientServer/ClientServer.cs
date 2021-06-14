using System;
using System.Collections.Generic;
using UnityEngine;
using Data;

public static class ServerEvent
{
    public static readonly string ChannelChanged = "ChannelChanged";
}

// client server
public class ClientServer
{
    // 默认初始化为单机模式
    public static ClientServer Instance = new ClientServer();

    private Dictionary<string, List<int>> loadingShowCache = new Dictionary<string, List<int>>();
    // show, reason, timeS, delayS
    private Action<bool, string, int, int> loadingFun = null;
    public void setLoadingFun(Action<bool, string, int, int> fun)
    {
        this.loadingFun = fun;

        if (fun != null)
        {
            foreach (var kv in this.loadingShowCache)
            {
                if (kv.Value.Count > 0)
                {
                    fun(true, kv.Key, kv.Value[0], kv.Value[1]);
                }
            }
        }
        this.loadingShowCache.Clear();
    }

    // connected, resPM
    private Action<bool, ResLoginPM> connectionFun = null;
    // 重连回调
    public void setConnectionFun(Action<bool, ResLoginPM> fun)
    {
        this.connectionFun = fun;
    }

    public void showRequestLoading(bool show, int timeS)
    {
        if (this.loadingFun != null)
        {
            this.loadingFun(show, "net-request", timeS, 1);
        }
        else
        {
            this.loadingShowCache["net-request"] = new List<int> { show ? timeS : 0, 1 };
        }
    }
    public void onPMConnectionChange(bool connected, ResLoginPM resPM)
    {
        bool show = !connected;
        if (this.loadingFun != null)
        {
            this.loadingFun(show, "net-reconnect", 36000, 0);
        }
        else
        {
            this.loadingShowCache["net-reconnect"] = new List<int> { show ? 36000 : 0, 0 };
        }

        if (this.connectionFun != null)
        {
            this.connectionFun(connected, resPM);
        }
    }

    protected Action<string> errorFun = null;
    public void setErrorFun(Action<string> fun)
    {
        this.errorFun = fun;
    }

    protected bool destroyed = false;
    public virtual void onDestroy()
    {
        this.destroyed = true;
        this.setLoadingFun(null);
        this.setErrorFun(null);
        this.removeAllListeners();
    }

    protected virtual void removeAllListeners()
    {
        // TODO
    }

    // virtual
    public virtual bool isConnected { get { return true; } }



    // public virtual void login(Action<ECode, NetworkStatus> cb)
    // {
    //     Debug.LogError("不应该走到这里");
    //     cb(ECode.Success, NetworkStatus.Init, null, null);
    // }

    private bool needPromptError(ECode e)
    {
        if (e == ECode.Success ||
            // 有一些错误并不需要提示
            e == ECode.ShouldLoginAAA)
        {
            return false;
        }
        return true;
    }

    protected void reply1(Action<MyResponse> cb, ECode e, object res = null)
    {
        if (this.needPromptError(e) && this.errorFun != null)
        {
            this.errorFun("E." + e);
        }
        cb(new MyResponse(e, res));
    }

    protected void reply2(Action<MyResponse> cb, MyResponse r)
    {
        if (this.needPromptError(r.err))
        {
            if (this.errorFun != null)
            {
                this.errorFun("E." + r.err);
            }
            else
            {
                Debug.LogError("E." + r.err);
            }
        }
        cb(r);
    }

    public virtual void request(MsgType type, object _msg, bool block, Action<MyResponse> cb, int timeoutMs = 10000, bool retryOnReconnect = true)
    {
        // var game = sc.game;

        // // 打印一下看看消息有多少
        // Debug.Log("request " + type + " " + type);

        // switch (type)
        // {

        //     case MsgType.PMChangeName:
        //         {
        //             var msg = (MsgChangeName)_msg;
        //             var res = new ResChangeName();
        //             var e = game.gameScript.changeNameCheck(game, msg, res);
        //             if (e != ECode.Success)
        //             {
        //                 return this.reply1(cb, e);
        //             }
        //             this.reply1(cb, ECode.Success, res);
        //         }
        //         break;

        //     case MsgType.PMChangeChannel:
        //         {
        //             var msg = (MsgChangeChannel)_msg;
        //             var res = new ResChangeChannel();
        //             var e = game.gameScript.changeChannelCheck(game, msg, res);
        //             if (e != ECode.Success)
        //             {
        //                 return this.reply1(cb, e);
        //             }
        //             this.reply1(cb, ECode.Success, res);
        //         }
        //         break;

        //     default:
        //         {
        //             this.reply1(cb, ECode.Error, null);
        //             return;
        //         }
        // }
    }

    // virtual
    // 一定要发送给服务器，如果 hasServer == 0，那么什么事也不会发生
    public virtual void request2(MsgType type, object _msg, bool block, Action<MyResponse> cb, int timeoutMs, bool retryOnReconnect = true)
    {
    }
}