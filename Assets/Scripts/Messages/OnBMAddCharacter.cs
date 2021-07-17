using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Data;
using Script;

public class OnBMAddCharacter : OnMessageBase
{
    public override void Handle(object msg_)
    {
        var res = this.CastMsg<BMMsgAddCharacter>(msg_);
        Debug.LogFormat("OnBMAddCharacter characterId({0}) playerId({1})", res.character.id, res.character.playerId);
        btCharacter c = res.character;
        btCharacter c2 = sc.battleScene.mainScript.addCharacter(sc.battleScene.battle, c.id, c.playerId, c.walkableId, c.pos, c.moveDir);
        sc.battleScene.ApplyCharacter(c2);
    }
}
