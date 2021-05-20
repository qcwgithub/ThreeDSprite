
using System.Collections;

public class AAAChannel_Debug : IScript {
    public Server server { get; set; }
    public IEnumerator verifyAccount(string channelUserId, string token, string code, MyResponse r)
    {
        if (!this.server.baseScript.isDevelopment()) {
            r.err = ECode.Error;
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