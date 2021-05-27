
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Data;

public class AAAChannel_Apple : IScript
{
    public Server server { get; set; }
    public Task<MyResponse> verifyAccount(string channelUserId, Dictionary<string, object> verifyData)
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

        return Task.FromResult(new MyResponse(ECode.Success, res));
    }
}