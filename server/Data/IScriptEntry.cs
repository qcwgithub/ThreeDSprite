using System.Collections.Generic;

namespace Data
{
    public interface IScriptEntry
    {
        bool OnLoad(Dictionary<string, string> args, DataEntry global, int version);
        int GetVersion();
        void OnUnload();
    }
}