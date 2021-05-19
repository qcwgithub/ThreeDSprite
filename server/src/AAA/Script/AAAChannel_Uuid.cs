
public class AAAChannel_Uuid : IScript {
    Server server;
    *verifyAccount(string channelUserId, object verifyData) {
        if (channelUserId.length !== 36) {
            return MyResponse.create(ECode.InvalidChannelUserId);
        }
        AAAVerifyAccountResult res = {
            accountMustExist: false,
            data: null
        }
        return new MyResponse(ECode.Success, res);
    }
}