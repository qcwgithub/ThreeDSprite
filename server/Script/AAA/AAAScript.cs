
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Data;

namespace Script
{
    public class AAAScript : IServerScript<AAAServer>
    {
        public AAAServer server { get; set; }
        public AAAData aaaData { get { return this.server.aaaData; } }

        public async Task<MyResponse> verifyAccount(string channel, string channelUserId, Dictionary<string, object> verifyData)
        {
            MyResponse r = null;
            if (channel == MyChannels.pc)
            {
                // if (msg.channelUserId == null) {
                //     msg.channelUserId = v4();
                // }

                r = await this.server.channelPc.verifyAccount(channelUserId, verifyData);
                if (r.err != ECode.Success)
                {
                    return r;
                }
            }
            else if (channel == MyChannels.uuid)
            {
                // if (msg.channelUserId == null) {
                //     msg.channelUserId = v4();
                // }

                r = await this.server.channelUuid.verifyAccount(channelUserId, verifyData);
                if (r.err != ECode.Success)
                {
                    return r;
                }
            }
            else if (channel == MyChannels.debug)
            {
                r = await this.server.channelDebug.verifyAccount(channelUserId, verifyData);
                if (r.err != ECode.Success)
                {
                    return r;
                }
            }
            else if (channel == MyChannels.apple)
            {
                r = await this.server.channelApple.verifyAccount(channelUserId, verifyData);
                if (r.err != ECode.Success)
                {
                    return r;
                }
            }
            else if (channel == MyChannels.leiting)
            {
                // var r = await this.server.channelLeiting.verifyAccount(channelUserId, verifyData);
                // if (r.err != ECode.Success) {
                //     return r;
                // }
            }
            else if (channel == MyChannels.ivy)
            {
                r = await this.server.channelIvy.verifyAccount(channelUserId, verifyData);
                if (r.err != ECode.Success)
                {
                    return r;
                }
            }
            else
            {
                return ECode.InvalidChannel;
            }
            return r;
        }
        public AAAUserInfo getUserInfo(string channel, string channelUserId, Dictionary<string, object> verifyData)
        {
            var ret = new AAAUserInfo
            {
                userName = null,
                detail = null,
            };
            if (channel == MyChannels.uuid)
            {

            }
            else if (channel == MyChannels.debug)
            {

            }
            else if (channel == MyChannels.apple)
            {

            }
            else if (channel == MyChannels.leiting)
            {
                // detail=LeitingLogin.LoginData
                if (verifyData != null && verifyData.ContainsKey("detail"))
                {
                    ret.detail = verifyData["detail"] as Dictionary<string, object>;
                    if (ret.detail != null && ret.detail.ContainsKey("userName"))
                    {
                        ret.userName = ret.detail["userName"] as string; // nickName
                    }
                }
            }
            else if (channel == MyChannels.ivy)
            {
                // verifyData=IvyLogin.IvyUserInfo
                if (verifyData != null)
                {
                    ret.detail = verifyData;
                    if (ret.detail.ContainsKey("name"))
                    {
                        ret.userName = ret.detail["name"] as string;
                    }
                }
            }

            return ret;
        }

        //////////// account table ////////////

        // clearPlayerSocket(AAAPlayerInfo player) {
        //     if (player.socket != null) {
        //         this.server.netProto.removeAllListeners(player.socket);
        //         this.server.netProto.closeSocket(player.socket);
        //         player.socket = null;
        //     }
        // }


    }
}