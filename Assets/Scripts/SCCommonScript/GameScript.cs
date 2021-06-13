using Data;
namespace Script
{
    public abstract class GameScript : GameScriptBase
    {
        public abstract int getTime();
        public abstract int getTodayTime(int hour, int minute, int second, int ms);
        public abstract int setHours(int timeMs, int hour, int minute, int second, int ms);


        public ECode changeChannelCheck(IProfileInput input, MsgChangeChannel msg, ResChangeChannel res)
        {
            if (!this.scripts.scUtils.isValidChannelType(msg.channel1) || !this.scripts.scUtils.isValidChannelType(msg.channel2))
            {
                return ECode.InvalidChannel;
            }
            if (msg.channel1 != MyChannels.uuid)
            {
                // 只允许由 uuid 换成其他的
                return ECode.Error;
            }
            if (msg.channel2 == MyChannels.uuid)
            {
                return ECode.Error;
            }

            // init
            res.channel2Exist = false;
            res.loginReward = 0;
            return ECode.Success;
        }

        public void changeChannelExecute(IProfileInput input, MsgChangeChannel msg, ResChangeChannel res)
        {
            // if (res.loginReward > 0) {
            //     input.profile.loginReward = res.loginReward;
            // }
            // if (res.userName != null && res.userName.Length > 0)
            // {
            //     input.profile.userName = res.userName;
            // }
        }
    }
}