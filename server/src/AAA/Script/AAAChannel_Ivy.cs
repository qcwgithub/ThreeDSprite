
public class AAAChannel_Ivy : IScript {
    Server server;
    *verifyAccount(string channelUserId, verifyData: { string token, string code }) {
        AAAVerifyAccountResult res = {
            accountMustExist: false,
            data: null
        }
        return new MyResponse(ECode.Success, res);
    }
}