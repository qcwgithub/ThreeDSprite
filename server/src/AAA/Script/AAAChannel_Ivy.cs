
using System.Collections;

public class AAAChannel_Ivy : IScript {
    public Server server { get; set; }
    public IEnumerator verifyAccount(string channelUserId, string token, string code, MyResponse r)
    {
        var res = new AAAVerifyAccountResult {
            accountMustExist = false,
            data = null
        };

        r.err = ECode.Success;
        r.res = res;
        yield break;
    }
}