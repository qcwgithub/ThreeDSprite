using System;
using System.Collections.Generic;
using UnityEngine;
using Data;

public static class ServerEvent
{
    public static readonly string ChannelChanged = "ChannelChanged";
}

// client server
public abstract class ClientServer
{
    // show, reason, timeS, delayS
    private Action<bool, string, int, int> loadingFun = null;

    public void showRequestLoading(bool show, int timeS)
    {
        if (this.loadingFun != null)
        {
            this.loadingFun(show, "net-request", timeS, 1);
        }
        else
        {
            // this.loadingShowCache["net-request"] = new List<int> { show ? timeS : 0, 1 };
        }
    }

    public event Action<string> OnError = null;

    protected bool destroyed = false;
    public virtual void onDestroy()
    {
        this.destroyed = true;
    }
    
    public abstract bool isConnected { get; }
    public abstract void start();

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
        if (this.needPromptError(e) && this.OnError != null)
        {
            this.OnError("E." + e);
        }
        cb(new MyResponse(e, res));
    }

    protected void reply2(Action<MyResponse> cb, MyResponse r)
    {
        if (this.needPromptError(r.err))
        {
            if (this.OnError != null)
            {
                this.OnError("E." + r.err);
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
}