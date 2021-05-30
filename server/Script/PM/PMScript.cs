using System;
using System.Collections.Generic;
using Data;

namespace Script
{
    public class PMScript : IServerScript<PMServer>
    {
        public PMServer server { get; set; }
        public PMData pmData { get { return this.server.pmData; }}

        public log4net.ILog logger { get { return this.server.logger; } }

        public bool acceptClient()
        {
            return pmData.state == ServerState.Started && pmData.aaaReady && pmData.allowClientConnect;
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
                this.server.logger.Error("toJsonOrNull " + ex);
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
            this.logger.InfoFormat("[{0}] set destroy timer for playerId: {1}, seconds: {2}", place, player.id, SEC);
            this.clearDestroyTimer(player, false);

            player.destroyTimer = this.server.timerScript.setTimer(() =>
            {
                player.destroyTimer = -1;
                this.logger.Info("send destroy playerId: " + player.id);
                MsgDestroyPlayer msgDestroy = new MsgDestroyPlayer { playerId = player.id, place = "pmDestroyTimer" };
                this.server.tcpClientScript.send(this.pmData.aaaSocket, MsgType.AAADestroyPlayer, msgDestroy, null);
            }, SEC * 1000);
        }

        public void clearDestroyTimer(PMPlayerInfo player, bool log = true)
        {
            if (log)
            {
                this.logger.Info("clear destroy timer for playerId: " + player.id);
            }
            if (player.destroyTimer != -1)
            {
                this.server.timerScript.clearTimer(player.destroyTimer);
                player.destroyTimer = -1;
            }
        }

        public void setSaveTimer(PMPlayerInfo player)
        {
            var SEC = this.pmData.playerSCSaveIntervalS;
            this.clearSaveTimer(player);

            MsgPlayerSCSave msg = new MsgPlayerSCSave { playerId = player.id, place = "timer" };
            player.saveTimer = this.server.timerScript.setInterval(() =>
            {
                this.server.baseScript.sendToSelf(MsgType.PMPlayerSave, msg);
            }, SEC * 1000);
        }

        public void clearSaveTimer(PMPlayerInfo player)
        {
            if (player.saveTimer != -1)
            {
                this.server.timerScript.clearInterval(player.saveTimer);
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

        public void playerOperInfo(Handler<PMServer> handler, int playerId)
        {
            this.server.logger.InfoFormat("{0} playerId: {1}", handler.msgName, playerId);
        }

        public ECode playerOperError(Handler<PMServer> handler, int playerId, ECode e)
        {
            this.server.logger.ErrorFormat("{0} playerId: {1}, ECode.{2}", handler.msgName, playerId, e);
            return e;
        }
    }
}