using System;
using System.Collections.Generic;
using Data;

namespace Script
{
    public class PMScript : IServerScript<PMServer>
    {
        public PMServer server { get; set; }
        public PMData pmData { get { return this.server.pmData; } }

        public log4net.ILog logger { get { return this.server.logger; } }        

        public T toJsonOrNull<T>(string str) where T : class
        {
            if (str == null)
            {
                return null;
            }
            T obj = null;
            try
            {
                obj = JsonUtils.parse<T>(str);
            }
            catch (Exception ex)
            {
                this.server.logger.Error("toJsonOrNull " + ex);
                obj = null;
            }
            return obj;
        }

        public void setDestroyTimer(PMPlayer player, string place)
        {
            var SEC = this.pmData.playerDestroyTimeoutS;
            this.logger.InfoFormat("setDestroyTimer playerId({1}), seconds({2}), reason({3})", place, player.id, SEC, place);
            this.clearDestroyTimer(player, false);

            player.destroyTimer = this.server.setTimer(
                SEC, MsgType.PMSendDestroyPlayer, 
                new MsgSendDestroyPlayer { playerId = player.id });
        }

        public void clearDestroyTimer(PMPlayer player, bool log = true)
        {
            if (log)
            {
                this.logger.InfoFormat("clearDestroyTimer playerId({0})", player.id);
            }
            if (player.destroyTimer > 0)
            {
                this.server.clearTimer(player.destroyTimer);
                player.destroyTimer = 0;
            }
        }

        public void setSaveTimer(PMPlayer player)
        {
            var SEC = this.pmData.playerSCSaveIntervalS;
            this.clearSaveTimer(player);

            MsgPlayerSCSave msg = new MsgPlayerSCSave { playerId = player.id, place = "timer" };
            this.server.setTimer(SEC, MsgType.PMPlayerSave, msg);
        }

        public void clearSaveTimer(PMPlayer player)
        {
            if (player.saveTimer > 0)
            {
                this.server.clearTimer(player.saveTimer);
                player.saveTimer = 0;
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

        public void postHandlePayResult(PMPlayer player, ResPay res)
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