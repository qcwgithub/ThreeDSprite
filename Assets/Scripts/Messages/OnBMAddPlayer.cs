using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Data;
using Script;

public class OnBMAddPlayer : OnMessageBase
{
    public override void Handle(object msg_)
    {
        var res = this.CastMsg<BMMsgAddPlayer>(msg_);
        BMPlayerInfo p = res.player;
        sc.battleScene.mainScript.addPlayer(sc.battleScene.battle, p.playerId, p.battleId);
    }
}
