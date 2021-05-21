using System;
using System.Collections.Generic;

public class PMScript : IScript
{
    public Server server { get; set; }
    public FakeLogger logger { get { return this.server.logger; } }
    public PMData pmData { get { return this.server.pmData; } }

    public bool acceptClient()
    {
        var baseData = this.server.baseData;
        var pmData = this.server.pmData;
        return baseData.state == ServerState.Started && pmData.aaaReady && pmData.allowClientConnect;
    }

    public T toJsonOrNull<T>(string str) where T : class
    {
        if (str == null)
        {
            return null;
        }
        T obj = null;
        try
        {
            obj = this.server.JSON.parse<T>(str);
        }
        catch (Exception ex)
        {
            this.server.baseScript.error("toJsonOrNull " + ex);
            obj = null;
        }
        return obj;
    }

    public PMPlayerInfo decodePlayer(SqlTablePlayer sqlData)
    {
        var player = new PMPlayerInfo();

        // server only data
        player.id = sqlData.id;
        var p = player.profile = new CProfile();
        CProfile.ensure(p, p.userName);
        return player;
    }

    public void setDestroyTimer(PMPlayerInfo player, string place)
    {
        var SEC = this.pmData.playerDestroyTimeoutS;
        this.logger.info("[%s] set destroy timer for playerId: %d, seconds: %d", place, player.id, SEC);
        this.clearDestroyTimer(player, false);

        player.destroyTimer = this.server.baseScript.setTimer(() =>
        {
            player.destroyTimer = -1;
            this.logger.info("send destroy playerId: " + player.id);
            MsgDestroyPlayer msgDestroy = new MsgDestroyPlayer { playerId = player.id, place = "pmDestroyTimer" };
            this.server.netProto.send(this.pmData.aaaSocket, MsgType.AAADestroyPlayer, msgDestroy, null);
        }, SEC * 1000);
    }

    public void clearDestroyTimer(PMPlayerInfo player, bool log = true)
    {
        if (log)
        {
            this.logger.info("clear destroy timer for playerId: " + player.id);
        }
        if (player.destroyTimer != -1)
        {
            this.server.baseScript.clearTimer(player.destroyTimer);
            player.destroyTimer = -1;
        }
    }

    public void setSaveTimer(PMPlayerInfo player)
    {
        var SEC = this.pmData.playerSCSaveIntervalS;
        this.clearSaveTimer(player);

        MsgPlayerSCSave msg = new MsgPlayerSCSave { playerId = player.id, place = "timer" };
        player.saveTimer = this.server.baseScript.setInterval(() =>
        {
            this.server.baseScript.sendToSelf(MsgType.PMPlayerSave, msg);
        }, SEC * 1000);
    }

    public void clearSaveTimer(PMPlayerInfo player)
    {
        if (player.saveTimer != -1)
        {
            this.server.baseScript.clearInterval(player.saveTimer);
            player.saveTimer = -1;
        }
    }

    // addDiamond_save_log(PMPlayerInfo player, reason: AddDiamondReason, int delta) {
    //     // execute
    //     player.diamond += delta;

    //     // save
    //     this.server.pmSqlUtils.saveField(player.id, "diamond", player.diamond);

    //     // log
    //     this.server.logSqlUtils.playerDiamond(player.id, reason, delta, player.diamond);
    // }

    public void postHandlePayResult(PMPlayerInfo player, ResPay res)
    {
        // log...一次性记
        List<string> productIdsDiamond = new List<string>();
        List<string> productIdsGiftVoucher = new List<string>();
        var totalAddDiamond = 0;
        var totalAddGiftVoucher = 0;
        for (int i = 0; i < res.items.Count; i++)
        {
            var item = res.items[i];
            if (!item.duplicated)
            {
                if (item.addDiamond > 0)
                {
                    productIdsDiamond.Add(item.productId);
                    totalAddDiamond += item.addDiamond;
                }

                if (item.addGiftVoucher > 0)
                {
                    productIdsGiftVoucher.Add(item.productId);
                    totalAddGiftVoucher += item.addGiftVoucher;
                }
            }
        }
    }

    public void playerOperInfo(Handler handler, int playerId)
    {
        this.server.logger.info("%s playerId: %d", handler.msgName, playerId);
    }

    public ECode playerOperError(Handler handler, int playerId, ECode e)
    {
        this.server.baseScript.error("%s playerId: %d, ECode.%s", handler.msgName, playerId, e);
        return e;
    }
}