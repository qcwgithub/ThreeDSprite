using System.Collections.Generic;
using UnityEngine;
using System;
using Leopotam.Ecs;

namespace Data
{
    public class btBattle
    {
        public EcsWorld ecsWorld;
        public EcsSystems updateSystems;

        public btTilemapData tilemapData;
        public Dictionary<string, btTilesetConfig> tilesetConfigs;
        public Dictionary<int, btObject> objects = new Dictionary<int, btObject>();
        public List<btIWalkable> walkables = new List<btIWalkable>();
        public List<btIObstacle> obstacles = new List<btIObstacle>();
        public List<btTree> trees = new List<btTree>();
        public Dictionary<int, btCharacter> characters = new Dictionary<int, btCharacter>();
        public Dictionary<IntPtr, btObject> body2Objects = new Dictionary<IntPtr, btObject>();

        public IntPtr physicsScene = IntPtr.Zero;
        public IntPtr AddBody(btObject who, q3BodyType bodyType, Vector3 position)
        {
            var body = Qu3eApi.SceneAddBody(physicsScene, bodyType, position.x, position.y, position.z);
            this.body2Objects.Add(body, who);
            return body;
        }
        public void AddBox(IntPtr body, Vector3 position, Vector3 extends)
        {
            Qu3eApi.BodyAddBox(body, position.x, position.y, position.z, extends.x, extends.y, extends.z);
        }

        private float[] tempForPosition = new float[3];
        public void SetBodyPosition(IntPtr body, Vector3 position)
        {
            tempForPosition[0] = position.x;
            tempForPosition[1] = position.y;
            tempForPosition[2] = position.z;
            Qu3eApi.BodySetTransform(body, q3TransformOperation.ePostion, tempForPosition);
        }

        public Qu3eApi.ContactDelegate onBeginContactDel;
        public Qu3eApi.ContactDelegate onEndContactDel;
        public bool updating = false;
    }
}