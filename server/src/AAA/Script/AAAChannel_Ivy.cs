
using System.Collections;
using System.Collections.Generic;

public class AAAChannel_Ivy : IScript {
    public Server server { get; set; }
    public IEnumerator verifyAccount(string channelUserId, Dictionary<string, object> verifyData, MyResponse r)
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