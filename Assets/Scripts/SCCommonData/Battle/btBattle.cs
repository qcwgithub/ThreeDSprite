using System.Collections.Generic;
using UnityEngine;
using System;

namespace Data
{
    public class btBattle
    {
        public btTilemapData tilemapData;
        public Dictionary<string, btTilesetConfig> tilesetConfigs;
        public Dictionary<int, btObject> DictObjects = new Dictionary<int, btObject>();
        public List<btIWalkable> walkables = new List<btIWalkable>();
        public List<btIObstacle> obstacles = new List<btIObstacle>();
        public List<btTree> trees = new List<btTree>();
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

        public btObject GetObject(int id)
        {
            btObject obj;
            if (!this.DictObjects.TryGetValue(id, out obj))
            {
                return null;
            }
            return obj;
        }

        private List<int> toRemoves = new List<int>();
        public void RemoveObject(int id)
        {
            btObject obj = this.GetObject(id);
            if (obj == null)
            {
                return;
            }
        }

        public void Move(btCharacter lChar, Vector3 delta)
        {
            Vector3 from = lChar.Pos;
            float y = 0f;
            btIWalkable preWalkable = lChar.Walkable;

            if (lChar.Walkable != null)
            {
                PredictMoveResult result = lChar.Walkable.PredictMove(from, delta);
                if (!result.OutOfRange)
                {
                    //Debug.Log("OutOfRange")
                    y = result.Y;
                }
                else
                {
                    Debug.Log("Out of range, " + lChar.Walkable.ToString());
                    lChar.Walkable = null;
                }
            }

            // find a new walkables
            int index = 0;
            if (lChar.Walkable != null)
            {
                index = lChar.Collidings.FindIndex(_ => _.obj is btIWalkable && (_.obj as btIWalkable) == preWalkable);
                if (index >= 0)
                {
                    index++;
                }
            }
            if (index >= 0)
            {
                for (; index < lChar.Collidings.Count; index++)
                {
                    btIWalkable walkable = lChar.Collidings[index].obj as btIWalkable;
                    if (walkable == null)
                    {
                        continue;
                    }
                    if (walkable == preWalkable)
                    {
                        continue;
                    }
                    if (walkable.CanAccept(from, delta))
                    {
                        lChar.Walkable = walkable;
                        PredictMoveResult result = walkable.PredictMove(from, delta);
                        y = result.Y;
                        break;
                    }
                }
            }

            if (lChar.Walkable != null)
            {
                delta.y = y - from.y;

                // limit by obstacles
                for (int i = 0; i < lChar.Collidings.Count; i++)
                {
                    btIObstacle ob = lChar.Collidings[i].obj as btIObstacle;
                    if (ob != null && ob.LimitMove(from, ref delta))
                    {
                        break;
                    }
                }

                lChar.Pos = from + delta;
            }
            else if (!lChar.EverHasWalkable)
            {
                lChar.Pos = from + delta;
            }
            else
            {
                // keep position unchanged
            }
        }

        private bool updating = false;
        public void Update()
        {
            this.updating = true;
            Qu3eApi.SceneStep(physicsScene);
            this.updating = false;
        }

        public void OnDestroy()
        {
            Qu3eApi.SceneDestroy(this.physicsScene);
        }
    }
}