using System.Collections.Generic;

namespace Data
{
    public interface IScriptEntry
    {
        bool needUpdate { get; }
        void Update(float dt);
        bool OnLoad(Dictionary<string, string> args, DataEntry global, int version);
        int GetVersion();
        void OnUnload();
    }
}