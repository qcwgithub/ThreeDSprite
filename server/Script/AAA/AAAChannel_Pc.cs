
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Data;

namespace Script
{
    public class AAAChannel_Pc : IServerScript<AAAServer>
    {
        public AAAServer server { get; set; }
        public Task<MyResponse> verifyAccount(string channelUserId, Dictionary<string, object> verifyData)
        {
            if (!this.server.isDevelopment())
            {
                return ECode.Error.toTask();
            }
            var res = new AAAVerifyAccountResult
            {
                accountMustExist = false,
                data = null
            };

            return new MyResponse(ECode.Success, res).toTask();
        }
    }
}