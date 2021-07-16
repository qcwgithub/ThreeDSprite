using Data;
using Script;

public class OnBMCharacterMove : OnMessageBase
{
    public override void Handle(object msg_)
    {
        var res = this.CastMsg<BMMsgCharacterMove>(msg_);
        sc.battleScene.moveScript.characterMove(
            sc.battleScene.battle, res.characterId, res.moveDir);
    }
}