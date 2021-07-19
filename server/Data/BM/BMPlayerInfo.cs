using MessagePack;

namespace Data
{
    public partial class BMPlayer
    {
        [IgnoreMember]
        public string token;
        
        [IgnoreMember]
        public TcpClientData socket;
        // public int battleId;
        // public int characterId;
    }
}