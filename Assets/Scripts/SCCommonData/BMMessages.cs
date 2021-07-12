using System.Collections;
using System.Collections.Generic;
using Data;
using MessagePack;
using UnityEngine;

namespace Data
{
    ///////////////////////////////////////////////////////////

    [MessagePackObject]
    public class BMMsgPlayerLogin : IJsonSerializable
    {
        [Key(0)]
        public int battleId;
        [Key(1)]
        public int playerId;
        [Key(2)]
        public string token;
    }

    [MessagePackObject]
    public class MCharacter
    {
        [Key(0)]
        public int characterId;
        [Key(1)]
        public Vector3 pos;
        [Key(2)]
        public Vector3 moveDir;
        [Key(3)]
        public int walkableId;
    }

    [MessagePackObject]
    public class BMResPlayerLogin : IJsonSerializable
    {
        [Key(0)]
        public BMBattleInfo battle;

        // key = playerId
        [Key(1)]
        public Dictionary<int, MCharacter> characterDict;
    }

    ///////////////////////////////////////////////////////////
    
    [MessagePackObject]
    public class BMMsgMove : IJsonSerializable
    {
        // public MessageCode code => MessageCode.BMMsgMove;

        public static BMMsgMove shared = new BMMsgMove();

        [Key(0)]
        public Vector3 moveDir;
    }

    [MessagePackObject]
    public class BMResMove : IJsonSerializable
    {
        // public MessageCode code => MessageCode.BMResMove;
        
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