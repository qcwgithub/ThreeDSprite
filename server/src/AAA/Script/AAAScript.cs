
using System.Collections;
using System.Collections.Generic;

public class AAAScript : IScript {
    public Server server { get; set; }
    public AAAData data { get { return this.server.aaaData; } }
    public BaseScript baseScript { get { return this.server.baseScript; } }

    public bool acceptClient() {
        var baseData = this.server.baseData;
        var aaaData = this.server.aaaData;
        return baseData.state == ServerState.Started && aaaData.active && aaaData.playerIdReady && aaaData.pmReady;
    }

    public IEnumerator verifyAccount(string channel, string channelUserId, Dictionary<string, object> verifyData, MyResponse r) {
        if (channel == HermesChannels.uuid) {
            // if (msg.channelUserId == null) {
            //     msg.channelUserId = v4();
            // }

            yield return this.server.channelUuid.verifyAccount(channelUserId, verifyData, r);
            if (r.err != ECode.Success) {
                yield break;
            }
        }
        else if (channel == HermesChannels.debug) {
            yield return this.server.channelDebug.verifyAccount(channelUserId, verifyData, r);
            if (r.err != ECode.Success) {
                yield break;
            }
        }
        else if (channel == HermesChannels.apple) {
            yield return this.server.channelApple.verifyAccount(channelUserId, verifyData, r);
            if (r.err != ECode.Success) {
                yield break;
            }
        }
        else if (channel == HermesChannels.leiting) {
            yield return this.server.channelLeiting.verifyAccount(channelUserId, verifyData, r);
            if (r.err != ECode.Success) {
                yield break;
            }
        }
        else if (channel == HermesChannels.ivy) {
            yield return this.server.channelIvy.verifyAccount(channelUserId, verifyData, r);
            if (r.err != ECode.Success) {
                yield break;
            }
        }
        else {
            r.err = ECode.InvalidChannel;
        }
    }
    public AAAUserInfo getUserInfo(string channel, string channelUserId, Dictionary<string, object> verifyData) {
        var ret = new AAAUserInfo {
            userName = null,
            detail = null,
        };
        if (channel == HermesChannels.uuid) {

        }
        else if (channel == HermesChannels.debug) {

        }
        else if (channel == HermesChannels.apple) {

        }
        else if (channel == HermesChannels.leiting) {
            // detail=LeitingLogin.LoginData
            if (verifyData != null && verifyData.ContainsKey("detail")) {
                ret.detail = verifyData["detail"] as Dictionary<string, object>;
                if (ret.detail != null && ret.detail.ContainsKey("userName"))
                {
                    ret.userName = ret.detail["userName"] as string; // nickName
                }
            }
        }
        else if (channel == HermesChannels.ivy) {
            // verifyData=IvyLogin.IvyUserInfo
            if (verifyData != null) {
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