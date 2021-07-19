using Data;
namespace Script
{
    public abstract class GameScript : GameScriptBase
    {
        public abstract int getTimeS();
        public abstract int getTodayTimeS(int hour, int minute, int second);
        public abstract int setHours(int timeMs, int hour, int minute, int second);


        public ECode ChangeChannelCheck(IProfileInput input, MsgChangeChannel msg, ResChangeChannel res)
        {
            if (!MyChannels.isValidChannel(msg.channel1) || !MyChannels.isValidChannel(msg.channel2))
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

        public void ChangeChannelExecute(IProfileInput input, MsgChangeChannel msg, ResChangeChannel res)
        {
            // if (res.loginReward > 0) {
            //     input.profile.loginReward = res.loginReward;
            // }
            // if (res.userName != null && res.userName.Length > 0)
            // {
            //     input.profile.userName = res.userName;
            // }
        }

        public ECode ChangeCharacterCheck(IProfileInput input, MsgChangeCharacter msg, ResChangeCharacter res)
        {
            res.characterConfigId = msg.characterConfigId;
            return ECode.Success;
        }

        public void ChangeCharacterExecute(IProfileInput input, MsgChangeCharacter msg, ResChangeCharacter res)
        {
            input.profile.characterConfigId = res.characterConfigId;
        }
    }
}