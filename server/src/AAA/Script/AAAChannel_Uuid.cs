
using System.Collections;

public class AAAChannel_Uuid : IScript {
    public Server server { get; set; }

    public IEnumerator verifyAccount(string channelUserId, MyResponse r)
    {
        if (channelUserId.Length != 36) {
            r.err = ECode.InvalidChannelUserId;
            yield break;
        }
        var res = new AAAVerifyAccountResult {
            accountMustExist = false,
            data = null
        };
        r.err = ECode.Success;
        r.res = res;
    }
}