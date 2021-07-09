using Data;
using Script;

public class OnBMResMove : OnMessageBase
{
    public override void Handle(object msg_)
    {
        var res = this.CastMsg<BMResMove>(msg_);
        sc.battleScene.moveScript.characterMove(
            sc.battleScene.battle, res.characterId, FVector3.ToVector3(res.moveDir));
    }
}