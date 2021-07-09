using System.Collections.Generic;
using UnityEngine;
using System;
using Newtonsoft.Json;

namespace Data
{
    public class btBattle
    {
        public int mapId;
        
        [JsonIgnore]
        public Dictionary<int, btObject> objects = new Dictionary<int, btObject>();
        
        [JsonIgnore]
        public List<btIWalkable> walkables = new List<btIWalkable>();
        // public List<btIObstacle> obstacles = new List<btIObstacle>();
        // public List<btTree> trees = new List<btTree>();
        
        [JsonIgnore]
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
        
        [JsonIgnore]
        public Dictionary<IntPtr, btObject> body2Objects = new Dictionary<IntPtr, btObject>();

        [JsonIgnore]
        public IntPtr physicsScene = IntPtr.Zero;

        [JsonIgnore]
        public float[] tempForPosition = new float[3];

        [JsonIgnore]
        public Qu3eApi.ContactDelegate onBeginContactDel;
        
        [JsonIgnore]
        public Qu3eApi.ContactDelegate onEndContactDel;
        
        [JsonIgnore]
        public bool updating = false;
    }
}