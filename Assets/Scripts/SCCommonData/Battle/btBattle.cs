using System.Collections.Generic;
using UnityEngine;
using System;

namespace Data
{
    public class btBattle
    {
        public int mapId;
        
        public Dictionary<int, btObject> objects = new Dictionary<int, btObject>();
        public List<btIWalkable> walkables = new List<btIWalkable>();
        // public List<btIObstacle> obstacles = new List<btIObstacle>();
        // public List<btTree> trees = new List<btTree>();
        public Dictionary<int, btCharacter> characters = new Dictionary<int, btCharacter>();
        public Dictionary<IntPtr, btObject> body2Objects = new Dictionary<IntPtr, btObject>();

        public IntPtr physicsScene = IntPtr.Zero;

        public float[] tempForPosition = new float[3];

        public Qu3eApi.ContactDelegate onBeginContactDel;
        public Qu3eApi.ContactDelegate onEndContactDel;
        public bool updating = false;
    }
}