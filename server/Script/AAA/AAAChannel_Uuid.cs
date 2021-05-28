
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Data;

namespace Script
{
    public class AAAChannel_Uuid : IServerScript<AAAServer>
    {
        public AAAServer server { get; set; }

        public Task<MyResponse> verifyAccount(string channelUserId, Dictionary<string, object> verifyData)
        {
            if (channelUserId.Length != 36)
            {
                return Task.FromResult(new MyResponse(ECode.InvalidChannelUserId));
            }
            var res = new AAAVerifyAccountResult
            {
                accountMustExist = false,
                data = null
            };
            return Task.FromResult(new MyResponse(ECode.Success, res));
        }
    }
}