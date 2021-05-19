
public class AAAChannel_Debug : IScript {
    Server server;
    *verifyAccount(string channelUserId, object verifyData) {
        if (!this.server.baseScript.isDevelopment()) {
            return MyResponse.create(ECode.Error);
        }
        AAAVerifyAccountResult res = {
            accountMustExist: false,
            data: null
        }
        return new MyResponse(ECode.Success, res);
    }
}