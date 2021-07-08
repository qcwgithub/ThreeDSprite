using Data;
using Script;

public class OnBMResMove : OnMessageBase
{
    public override void Handle(object msg_)
    {
        var msg = this.CastMsg<BMResMove>(msg_);
    }
}