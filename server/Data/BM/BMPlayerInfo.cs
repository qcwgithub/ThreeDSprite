using Newtonsoft.Json;

namespace Data
{
    public partial class BMPlayerInfo
    {
        [JsonIgnore]
        public string token;
        
        [JsonIgnore]
        public TcpClientData socket;
        // public int battleId;
        // public int characterId;
    }
}