
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

public class AAAChannel_Ivy : IScript {
    public Server server { get; set; }
    public async Task<MyResponse> verifyAccount(string channelUserId, Dictionary<string, object> verifyData)
    {
        var res = new AAAVerifyAccountResult {
            accountMustExist = false,
            data = null
        };

        return new MyResponse(ECode.Success, res);
    }
}