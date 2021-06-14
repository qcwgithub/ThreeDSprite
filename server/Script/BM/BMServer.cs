using Data;
using System.IO;

namespace Script
{
    public class BMServer : Server
    {
        public BMData bmData
        {
            get
            {
                return (BMData)this.data;
            }
        }

        public override void OnLoad(DataEntry dataEntry, int id, int version)
        {
            base.OnLoad(dataEntry, id, version);
            base.AddHandler<BMServer>();
        }

        public override void OnUnload()
        {
            base.OnUnload();
        }
    }
}