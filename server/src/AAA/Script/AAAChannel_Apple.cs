
using System.Collections;
using System.Collections.Generic;

public class AAAChannel_Apple : IScript
{
    public Server server { get; set; }
    public IEnumerator verifyAccount(string channelUserId, Dictionary<string, object> verifyData, MyResponse r)
    {
        var res = new AAAVerifyAccountResult
        {
            accountMustExist = false,
            data = null
        };

        if (verifyData == null || !verifyData.ContainsKey("token") || !verifyData.ContainsKey("code"))
        {
            res.accountMustExist = true;
        }

        r.err = ECode.Success;
        r.res = res;
        yield break;
    }
}