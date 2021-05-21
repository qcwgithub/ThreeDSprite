
using System.Collections;
using System.Collections.Generic;

public class AAAChannel_Debug : IScript {
    public Server server { get; set; }
    public IEnumerator verifyAccount(string channelUserId, Dictionary<string, object> verifyData, MyResponse r)
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