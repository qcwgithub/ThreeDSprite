using System.Collections;
using System.Collections.Generic;
using Data;
using MessagePack;
using UnityEngine;

namespace Data
{
    ///////////////////////////////////////////////////////////

    [MessagePackObject]
    public class BMMsgPlayerLogin
    {
        [Key(0)]
        public int battleId;
        [Key(1)]
        public int playerId;
        [Key(2)]
        public string token;
    }

    [MessagePackObject]
    public class BMResPlayerLogin
    {
        [Key(0)]
        public BMBattleInfo battle;
    }

    [MessagePackObject]
    public class BMMsgAddPlayer
    {
        [Key(0)]
        public BMPlayerInfo player;
    }

    [MessagePackObject]
    public class BMMsgAddCharacter
    {
        [Key(0)]
        public btCharacter character;
    }

    ///////////////////////////////////////////////////////////
    
    [MessagePackObject]
    public class BMMsgMove
    {
        // public MessageCode code => MessageCode.BMMsgMove;

        public static BMMsgMove shared = new BMMsgMove();

        [Key(0)]
        public Vector3 moveDir;
        [Key(1)]
        public int id;
    }

    [MessagePackObject]
    public class BMMsgCharacterMove
    {        
        [Key(0)]
        public int characterId;
        [Key(1)]
        public Vector3 moveDir;
    }

    

    [MessagePackObject]
    public class BMMsgDebugGetCharacterPosition
    {
        [Key(0)]
        public int characterId;
    }

    [MessagePackObject]
    public class BMResDebugGetCharacterPosition
    {
        [Key(0)]
        public Vector3 position;
    }
}