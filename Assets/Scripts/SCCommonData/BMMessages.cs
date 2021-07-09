using System.Collections;
using System.Collections.Generic;
using Data;

namespace Data
{
    public class MCharacter
    {
        public int id;
        public FVector3 pos;
        public FVector3 moveDir;
        public int walkableId;
    }
    public class MBattleData
    {
        public List<MCharacter> characters;
    }

    public class BMMsgPlayerLogin : ISerializable
    {
        public int battleId;
        public int playerId;
        public string token;
    }
    public class BMResPlayerLogin : ISerializable
    {
        // scene data!
        public int battleId;
        public int mapId;
        public int characterId;
        public MBattleData battleData;
    }
    
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