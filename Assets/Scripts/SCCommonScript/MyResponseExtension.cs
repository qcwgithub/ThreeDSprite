using System;
using System.Threading.Tasks;
using Data;

namespace Script
{
    public static class MyResponseExtension
    {
        public static Task<MyResponse> toTask(this MyResponse @this)
        {
            return Task.FromResult<MyResponse>(@this);
        }
        public static Task<MyResponse> toTask(this ECode e)
        {
            return Task.FromResult<MyResponse>(new MyResponse(e, null));
        }
    }
}