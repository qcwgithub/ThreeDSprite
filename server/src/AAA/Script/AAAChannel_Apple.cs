
using System.Collections;

public class AAAChannel_Apple : IScript
{
    public Server server { get; set; }
    public IEnumerator verifyAccount(string channelUserId, string token, string code, MyResponse r)
    {
        var res = new AAAVerifyAccountResult
        {
            accountMustExist = false,
            data = null
        };

        if (!this.server.scUtils.checkArgs("SS", token, code))
        {
            res.accountMustExist = true;
        }

        r.err = ECode.Success;
        r.res = res;
        yield break;
    }
}