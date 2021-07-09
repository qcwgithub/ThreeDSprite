using System.Collections;
using System.Collections.Generic;
using Data;
using MessagePack;
using UnityEngine;

namespace Data
{
    ///////////////////////////////////////////////////////////

    public class BMMsgPlayerLogin : ISerializable
    {
        public int battleId;
        public int playerId;
        public string token;
    }
    public class MCharacter
    {
        public int characterId;
        public Vector3 pos;
        public Vector3 moveDir;
        public int walkableId;
    }
    public class BMResPlayerLogin : ISerializable
    {
        public BMBattleInfo battle;

        // key = playerId
        public Dictionary<int, MCharacter> characterDict;
    }

    ///////////////////////////////////////////////////////////
    
    [MessagePackObject]
    public class BMMsgMove : ISerializable
    {
        public static BMMsgMove shared = new BMMsgMove();
        [Key(0)]
        public Vector3 moveDir;
    }

    [MessagePackObject]
    public class BMResMove : ISerializable
    {
        [Key(0)]
        public int characterId;
        [Key(1)]
        public Vector3 moveDir;
    }
}