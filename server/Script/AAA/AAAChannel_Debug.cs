
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Data;

namespace Script
{
    public class AAAChannel_Debug : IScript<AAAServer>
    {
        public AAAServer server { get; set; }
        public async Task<MyResponse> verifyAccount(string channelUserId, Dictionary<string, object> verifyData)
        {
            if (!this.server.baseScript.isDevelopment())
            {
                return ECode.Error;
            }
            var res = new AAAVerifyAccountResult
            {
                accountMustExist = false,
                data = null
            };

            return new MyResponse(ECode.Success, res);
        }
    }
}