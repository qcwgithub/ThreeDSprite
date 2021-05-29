namespace Data
{
    public interface IScriptEntry
    {
        bool OnLoad(string[] args, DataEntry global);
        void OnUnload();
    }
}