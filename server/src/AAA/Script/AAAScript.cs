
public class AAAScript : IScript {
    AAAServer server = null;
    public TODO data { get { return this.server.aaaData; } }
    public TODO baseScript { get { return this.server.baseScript; } }

    public bool acceptClient() {
        var baseData = this.server.baseData;
        var aaaData = this.server.aaaData;
        return baseData.state == ServerState.Started && aaaData.active && aaaData.playerIdReady && aaaData.pmReady;
    }

    *verifyAccount(string channel, string channelUserId, object verifyData) {
        MyResponse r = null;
        if (channel == HermesChannels.uuid) {
            // if (msg.channelUserId == null) {
            //     msg.channelUserId = v4();
            // }

            r = yield this.server.channelUuid.verifyAccount(channelUserId, verifyData);
            if (r.err != ECode.Success) {
                return r.err;
            }
        }
        else if (channel == HermesChannels.debug) {
            r = yield this.server.channelDebug.verifyAccount(channelUserId, verifyData);
            if (r.err != ECode.Success) {
                return r.err;
            }
        }
        else if (channel == HermesChannels.apple) {
            r = yield this.server.channelApple.verifyAccount(channelUserId, verifyData);
            if (r.err != ECode.Success) {
                return r.err;
            }
        }
        else if (channel == HermesChannels.leiting) {
            r = yield this.server.channelLeiting.verifyAccount(channelUserId, verifyData);
            if (r.err != ECode.Success) {
                return r.err;
            }
        }
        else if (channel == HermesChannels.ivy) {
            r = yield this.server.channelIvy.verifyAccount(channelUserId, verifyData);
            if (r.err != ECode.Success) {
                return r.err;
            }
        }
        else {
            return MyResponse.create(ECode.InvalidChannel);
        }
        return r;
    }
    getUserInfo(string channel, string channelUserId, object verifyData): AAAUserInfo {
        AAAUserInfo ret = {
            userName: null,
            detail: null,
        };
        if (channel == HermesChannels.uuid) {

        }
        else if (channel == HermesChannels.debug) {

        }
        else if (channel == HermesChannels.apple) {

        }
        else if (channel == HermesChannels.leiting) {
            // detail=LeitingLogin.LoginData
            if (verifyData != null && verifyData.detail != null) {
                ret.userName = verifyData.detail.userName; // nickName
                ret.detail = verifyData.detail;
            }
        }
        else if (channel == HermesChannels.ivy) {
            // verifyData=IvyLogin.IvyUserInfo
            if (verifyData != null) {
                ret.userName = verifyData.name;
                ret.detail = verifyData;
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