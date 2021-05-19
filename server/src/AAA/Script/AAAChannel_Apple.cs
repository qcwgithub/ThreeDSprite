
public class AAAChannel_Apple : IScript {
    Server server;
    *verifyAccount(string channelUserId, verifyData: { string token, string code }) {
        if (verifyData == null) {
            return MyResponse.create(ECode.InvalidParam);
        }
        AAAVerifyAccountResult res = {
            accountMustExist: false,
            data: null
        }

        if (!this.server.scUtils.checkArgs("SS", verifyData.token, verifyData.code)) {
            res.accountMustExist = true;
        }

        return new MyResponse(ECode.Success, res);
    }
}