using Data;
using Script;

public abstract class OnMessageBase
{
    public abstract void Handle(object msg);
    protected T CastMsg<T>(object msg) where T : class
    {
        return (T)msg;
    }
}