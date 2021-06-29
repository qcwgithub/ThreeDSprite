using System.Collections.Generic;
using UnityEngine;
using System;
using Script;

public class btScene
{
    public btTilemapData data { get; private set; }
    Dictionary<string, btTilesetConfig> tilesetConfigs;
    public Dictionary<int, btObject> DictObjects = new Dictionary<int, btObject>();
    private List<btIWalkable> Walkables = new List<btIWalkable>();
    private List<btIObstacle> Obstacles = new List<btIObstacle>();
    private List<btTree> Trees = new List<btTree>();
    private HashSet<int> needUpdates = new HashSet<int>();
    Dictionary<IntPtr, btObject> body2Objects = new Dictionary<IntPtr, btObject>();

    IntPtr physicsScene = IntPtr.Zero;
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

    Qu3eApi.ContactDelegate onBeginContactDel;
    Qu3eApi.ContactDelegate onEndContactDel;
    public btScene(btTilemapData data, Dictionary<string, btTilesetConfig> tilesetConfigs)
    {
        this.data = data;
        this.tilesetConfigs = tilesetConfigs;

        this.physicsScene = Qu3eApi.CreateScene();

        this.onBeginContactDel = new Qu3eApi.ContactDelegate(this.OnBeginContact);
        this.onEndContactDel = new Qu3eApi.ContactDelegate(this.OnEndContact);
        Qu3eApi.SceneSetContactListener(physicsScene, this.onBeginContactDel, this.onEndContactDel);

        Vector3 mapOffset = Vector3.zero; // to do ?

        foreach (btTileLayerData layerData in data.layerDatas)
        {
            Vector3 layerOffset = FVector3.ToVector3(layerData.offset);

            if (layerData.objectType == btObjectType.floor)
            {
                Vector3 min = Vector3.zero;
                Vector3 max = Vector3.zero;
                bool first = true;
                foreach (btTileData tileData in layerData.tileDatas)
                {
                    btTileConfig tileConfig = tilesetConfigs[tileData.tileset].tiles[tileData.tileId];
                    if (tileConfig.shape == btShape.xz)
                    {
                        Vector3 mi = FVector3.ToVector3(tileData.position);
                        Vector3 ma = new Vector3(
                            mi.x + tileConfig.size.x,
                            mi.y + tileConfig.size.y,
                            mi.z + tileConfig.size.z);

                        if (first || mi.x < min.x) min.x = mi.x;
                        if (first || mi.y < min.y) min.y = mi.y;
                        if (first || mi.z < min.z) min.z = mi.z;

                        if (first || ma.x > max.x) max.x = ma.x;
                        if (first || ma.y > max.y) max.y = ma.y;
                        if (first || ma.z > max.z) max.z = ma.z;

                        first = false;
                    }
                }

                Vector3 worldMin = min + mapOffset + layerOffset;
                Vector3 worldMax = max + mapOffset + layerOffset;

                btFloor floor = new btFloor(this, layerData.id, worldMin, worldMax);
                this.Walkables.Add(floor);
                this.DictObjects.Add(floor.id, floor);
            }
            else if (layerData.objectType == btObjectType.stair)
            {
                Vector3 min = Vector3.zero;
                Vector3 max = Vector3.zero;
                bool first = true;
                foreach (btTileData tileData in layerData.tileDatas)
                {
                    btTileConfig tileConfig = tilesetConfigs[tileData.tileset].tiles[tileData.tileId];
                    if (tileConfig.shape == btShape.cube)
                    {
                        Vector3 mi = FVector3.ToVector3(tileData.position);
                        Vector3 ma = new Vector3(
                            mi.x + tileConfig.size.x,
                            mi.y + tileConfig.size.y,
                            mi.z + tileConfig.size.z);

                        if (first || mi.x < min.x) min.x = mi.x;
                        if (first || mi.y < min.y) min.y = mi.y;
                        if (first || mi.z < min.z) min.z = mi.z;

                        if (first || ma.x > max.x) max.x = ma.x;
                        if (first || ma.y > max.y) max.y = ma.y;
                        if (first || ma.z > max.z) max.z = ma.z;

                        first = false;
                    }
                }

                Vector3 worldMin = min + mapOffset + layerOffset;
                Vector3 worldMax = max + mapOffset + layerOffset;

                btStair stair = new btStair(this, layerData.id, layerData.stairDir, worldMin, worldMax);
                this.Walkables.Add(stair);
                this.DictObjects.Add(stair.id, stair);
            }

            foreach (btTileData tileData in layerData.tileDatas)
            {
                btTileConfig tileConfig = tilesetConfigs[tileData.tileset].tiles[tileData.tileId];
                switch (tileConfig.objectType)
                {
                    case btObjectType.box_obstacle:
                        {
                            btBoxObstacle obstacle = new btBoxObstacle(this, mapOffset + layerOffset, tileData, tileConfig);
                            this.Obstacles.Add(obstacle);
                            this.DictObjects.Add(obstacle.id, obstacle);
                        }
                        break;

                    case btObjectType.tree:
                        {
                            btTree tree = new btTree(this, mapOffset + layerOffset, tileData, tileConfig);
                            this.Trees.Add(tree);
                            this.DictObjects.Add(tree.id, tree);
                        }
                        break;
                }
            }
        }

        foreach (var kv in this.DictObjects)
        {
            kv.Value.AddToPhysicsScene();
        }
    }

    void OnBeginContact(IntPtr bodyA, IntPtr boxA, IntPtr bodyB, IntPtr boxB)
    {
        btObject objectA;
        if (!body2Objects.TryGetValue(bodyA, out objectA))
        {
            return;
        }

        btObject objectB;
        if (!body2Objects.TryGetValue(bodyB, out objectB))
        {
            return;
        }
        Debug.Log(string.Format("OnBeginContact {0} - {1}", objectA, objectB));
        objectA.Collidings.Add(new btObject_Time { obj = objectB });
        objectB.Collidings.Add(new btObject_Time { obj = objectA });
    }

    void OnEndContact(IntPtr bodyA, IntPtr boxA, IntPtr bodyB, IntPtr boxB)
    {
        // Debug.LogWarning(string.Format("OnEndContact"));

        btObject objectA;
        if (!body2Objects.TryGetValue(bodyA, out objectA))
        {
            return;
        }

        btObject objectB;
        if (!body2Objects.TryGetValue(bodyB, out objectB))
        {
            return;
        }

        Debug.Log(string.Format("OnEndContact {0} x {1}", objectA, objectB));
        for (int i = 0; i < objectA.Collidings.Count; i++)
        {
            if (objectA.Collidings[i].obj == objectB)
            {
                objectA.Collidings.RemoveAt(i);
                i--;
            }
        }

        for (int i = 0; i < objectB.Collidings.Count; i++)
        {
            if (objectB.Collidings[i].obj == objectA)
            {
                objectB.Collidings.RemoveAt(i);
                i--;
            }
        }
    }

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

    //public bool FindAWalkable(LCharacter lChar)
    //{
    //    for (int i = 0; i < lChar.Collidings.Count; i++)
    //    {
    //        IWalkable walkable = lChar.Collidings[i] as IWalkable;
    //        if (walkable != null && walkable != preWalkable && walkable.CanAccept(from, delta))
    //        {
    //            lChar.Walkable = walkable;
    //            PredictMoveResult result = walkable.PredictMove(from, delta);
    //            y = result.Y;
    //            break;
    //        }
    //    }
    //}

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

        // old implement
        //for (int i = 0; i < this.Obstacles.Count; i++)
        //{
        //    if (this.Obstacles[i].LimitMove(from, ref delta))
        //    {
        //        break;
        //    }
        //}

    }

    // public IWalkable RandomWalkable()
    // {
    //     int index = Random.Range(0, this.Walkables.Count);
    //     return this.Walkables[index];
    // }

    public void AddCharacter(btCharacter lChar)
    {
        this.DictObjects.Add(lChar.id, lChar);
        lChar.AddToPhysicsScene();
    }

    public void AddNeedUpdate(int id)
    {
        this.needUpdates.Add(id);
    }

    private bool updating = false;
    public void Update()
    {
        this.updating = true;
        // foreach (var id in this.needUpdates)
        // {
        //     this.GetObject(id).Update();
        // }
        Qu3eApi.SceneStep(physicsScene);
        this.updating = false;
    }

    public void OnDestroy()
    {
        Qu3eApi.SceneDestroy(this.physicsScene);
    }
}
