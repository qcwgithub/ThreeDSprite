using System.Collections;
using System.Collections.Generic;
using Data;

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
        public FVector3 pos;
        public FVector3 moveDir;
        public int walkableId;
    }
    public class BMResPlayerLogin : ISerializable
    {
        public BMBattleInfo battle;

        // key = playerId
        public Dictionary<int, MCharacter> characterDict;
    }

    ///////////////////////////////////////////////////////////
    
    public class BMMsgMove : ISerializable
    {
        public static BMMsgMove shared = new BMMsgMove();
        
        public FVector3 moveDir;
    }

    public class BMResMove : ISerializable
    {
        public int characterId;
        public FVector3 moveDir;
    }
}