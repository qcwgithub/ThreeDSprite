namespace Data
{
    public interface IScriptEntry
    {
        bool OnLoad(string[] args, GlobalData global);
    }
}