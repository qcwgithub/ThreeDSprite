using System.Collections.Generic;
using UnityEngine;
using System;
using MessagePack;

namespace Data
{
    [MessagePackObject]
    public class btBattle
    {
        [Key(0)]
        public int mapId;
        
        [IgnoreMember]
        public Dictionary<int, btObject> objects = new Dictionary<int, btObject>();
        
        [IgnoreMember]
        public List<btIWalkable> walkables = new List<btIWalkable>();
        // public List<btIObstacle> obstacles = new List<btIObstacle>();
        // public List<btTree> trees = new List<btTree>();
        
        [IgnoreMember]
        public Dictionary<int, btCharacter> characters = new Dictionary<int, btCharacter>();
        public btCharacter GetCharacter(int characterId)
        {
            btCharacter character;
            if (this.characters.TryGetValue(characterId, out character))
            {
                return character;
            }
            return null;
        }
        
        [IgnoreMember]
        public Dictionary<IntPtr, btObject> body2Objects = new Dictionary<IntPtr, btObject>();

        [IgnoreMember]
        public IntPtr physicsScene = IntPtr.Zero;

        [IgnoreMember]
        public float[] tempForPosition = new float[3];

        [IgnoreMember]
        public Qu3eApi.ContactDelegate onBeginContactDel;
        
        [IgnoreMember]
        public Qu3eApi.ContactDelegate onEndContactDel;
        
        [IgnoreMember]
        public bool updating = false;
    }
}