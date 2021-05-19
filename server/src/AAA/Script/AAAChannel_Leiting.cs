
public class AAAChannel_Leiting : IScript {
    Server server;

    // https://wiki.g-bits.com/pages/viewpage.action?pageId=559682334
    // https://stackoverflow.com/questions/6158933/how-is-an-http-post-request-made-in-node-js
    *verifyAccount(string channelUserId, object verifyData) {
        if (verifyData == null) {
            return MyResponse.create(ECode.InvalidParam);
        }
        var token = verifyData.token;
        var game = verifyData.game;
        var channelNo = verifyData.channelNo;
        if (!this.server.baseScript.checkArgs("SSSS", token, game, channelNo, channelUserId)) {
            return MyResponse.create(ECode.InvalidParam);
        }

        var waiter = new WaitCallBack().init(() => {
            var url = `https://login.jysyx.net/login/verify/verify_token?token=${token}&game=${game}&channelNo=${channelNo}`;
            https.get(url, (res: http.IncomingMessage) => {
                if (res.statusCode != 200) {
                    waiter.finish(new MyResponse(ECode.VerifyAccountErrorStatusCode, { error: "res.statusCode=" + res.statusCode }));
                    return;
                }

                // _trunk 值举例：{"status":"success","data":"{\"isGuest\":0,\"registerTime\":1592377740000,\"auth\":1,\"idCard\":\"79e04930cc9fc2c88757f515f7fc78fb\",\"age\":33,\"sid\":\"m3xtqqwo\"}"}
                res.on("data", _trunk => {
                    try {
                        var trunk = JSON.parse(_trunk);
                        if (trunk.status != "success") {
                            waiter.finish(new MyResponse(ECode.VerifyAccountErrorResponse, { error: trunk.message }));
                        }
                        else {
                            AAAVerifyAccountResult res = {
                                accountMustExist: false,
                                data: new AAALoginResponseData()
                            };

                            var _d = JSON.parse(trunk.data);
                            if (!this.initAAALoginResponseData(res.data, _d)) {
                                waiter.finish(new MyResponse(ECode.VerifyAccountErrorResponse, { error: "data check failed, " + JSON.stringify(_d) }));
                            }
                            else if (channelUserId != channelNo + "_" + res.data.sid) {
                                waiter.finish(new MyResponse(ECode.InvalidChannelUserId, { error: "channelUserId != channelNo + _ + res.sid" }));
                            }
                            else {
                                waiter.finish(new MyResponse(ECode.Success, { error: null, res: res }));
                            }
                        }
                    }
                    catch (ex) {
                        waiter.finish(new MyResponse(ECode.Error, { error: ex.message }));
                    }
                });

            }).on("error", (e: Error) => {
                waiter.finish(new MyResponse(ECode.Error, { error: e.message }));
            });
        });
        return yield waiter;
    }

    private initAAALoginResponseData(_this: AAALoginResponseData, d: AAALoginResponseData/* fake type */): boolean {
        if (d == null) {
            return false;
        }
        if (!this.server.baseScript.checkArgs("siisii", d.sid, d.registerTime, d.isGuest, d.idCard, d.age, d.auth)) {
            return false;
        }
        _this.sid = d.sid;
        _this.registerTime = d.registerTime;
        _this.isGuest = d.isGuest;
        _this.idCard = d.idCard;
        _this.age = d.age;
        _this.auth = d.auth;
        return true;
    }
}