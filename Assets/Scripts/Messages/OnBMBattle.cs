using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Data;
using Script;

public class OnBMBattle : OnMessageBase
{
    public override void Handle(object msg_)
    {
        var msg = this.CastMsg<BMMsgBattle>(msg_);
        Debug.LogFormat("OnBMBattle battleIdId({0})", msg.battle.battleId);

        BMBattleInfo battle = msg.battle;

        CBattleScene bs = sc.battleScene;
        bs.battleId = battle.battleId;
        bs.mapId = battle.mapId;
        bs.myCharacerId = 0;
        
        BattleScript.initBattleScripts(bs, bs);

        /////////////////////////////////////////////////////////////////////////////

        BattleScript.loadMap(new Script.JsonUtils(), bs, bs.mapId,
            mapId =>
            {
                string mapPath = CBattleScene.baseDir + "/map" + mapId;
                Debug.Log("Loading map " + mapPath + ".tmx");
                TextAsset textAsset = Resources.Load<TextAsset>(mapPath + ".tmx");
                if (textAsset == null)
                {
                    throw new Exception("load map error 1");
                }
                return textAsset.text;
            },
            tileset =>
            {
                string tilesetPath = CBattleScene.baseDir + "/" + tileset;// + ".json";
                TextAsset tilesetAsset = Resources.Load<TextAsset>(tilesetPath);
                if (tilesetAsset == null)
                {
                    throw new Exception(tilesetPath + " not imported");
                }
                return tilesetAsset.text;
            });

        /////////////////////////////////////////////////////////////////////////////

        bs.battle = bs.createScript.newBattle(battle.battleId, battle.mapId);
        foreach (var kv in battle.playerDict)
        {
            BMPlayerInfo p = kv.Value;
            bs.mainScript.addPlayer(bs.battle, p.playerId, p.battleId);
        }

        foreach (var kv in battle.characters)
        {
            btCharacter c = kv.Value;
            btCharacter c2 = bs.mainScript.addCharacter(bs.battle, c.id, c.playerId, c.walkableId, c.pos, c.moveDir);

            if (c.playerId == bs.myPlayerId)
            {
                bs.myCharacerId = c.id;
                bs.myCharacter = c2;
            }
        }

        bs.ApplyBattle(bs.battle);
    }
}
