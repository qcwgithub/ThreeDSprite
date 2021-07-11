using MessagePack;
namespace Data
{
    [MessagePackObject]
    public sealed class Loc
    {
        [Key(0)]
        public int id;
        [Key(1)]
        public string inIp;
        [Key(2)]
        public string outIp;
        [Key(3)]
        public string outDomain;
        [Key(4)]
        public int inPort;
        [Key(5)]
        public int outPort;
    }
}