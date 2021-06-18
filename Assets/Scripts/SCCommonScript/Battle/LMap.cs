using System.Collections.Generic;
using UnityEngine;
using System;
using Script;

public class LMap
{
    public LMapData Data { get; private set; }
    public Dictionary<int, LObject> DictObjects = new Dictionary<int, LObject>();
    private List<IWalkable> Walkables = new List<IWalkable>();
    private List<IObstacle> Obstacles = new List<IObstacle>();
    private List<LTree> Trees = new List<LTree>();
    private HashSet<int> needUpdates = new HashSet<int>();
    Dictionary<IntPtr, LObject> body2Objects = new Dictionary<IntPtr, LObject>();

    IntPtr physicsScene = IntPtr.Zero;
    public IntPtr AddBody(LObject who, q3BodyType bodyType, Vector3 position)
    {
        var body = Qu3eApi.SceneAddBody(physicsScene, bodyType, position.x, position.y, position.z);
        body2Objects.Add(body, who);
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

    Qu3eApi.ContactDelegate OnBeginContactDel;
    Qu3eApi.ContactDelegate OnEndContactDel;
    public LMap(LMapData data)
    {
        this.Data = data;

        physicsScene = Qu3eApi.CreateScene();

        OnBeginContactDel = new Qu3eApi.ContactDelegate(this.OnBeginContact);
        OnEndContactDel = new Qu3eApi.ContactDelegate(this.OnEndContact);
        Qu3eApi.SceneSetContactListener(physicsScene, OnBeginContactDel, OnEndContactDel);

        for (int i = 0; i < data.Floors.Length; i++)
        {
            LFloorData floorData = data.Floors[i];
            LFloor floor = new LFloor(this, floorData);
            this.Walkables.Add(floor);
            this.DictObjects.Add(floor.Id, floor);
        }

        for (int i = 0; i < data.Stairs.Length; i++)
        {
            LStairData stairData = data.Stairs[i];
            LStair stair = new LStair(this, stairData);
            this.Walkables.Add(stair);
            this.DictObjects.Add(stair.Id, stair);
        }

        for (int i = 0; i < data.BoxObstacles.Length; i++)
        {
            LBoxObstacleData obData = data.BoxObstacles[i];
            LBoxObstacle obstacle = new LBoxObstacle(this, obData);
            this.Obstacles.Add(obstacle);
            this.DictObjects.Add(obstacle.Id, obstacle);
        }

        for (int i = 0; i < data.Trees.Length; i++)
        {
            LTreeData treeData = data.Trees[i];
            LTree tree = new LTree(this, treeData);
            this.Trees.Add(tree);
            this.DictObjects.Add(tree.Id, tree);
        }

        foreach (var kv in this.DictObjects)
        {
            kv.Value.AddToPhysicsScene();
        }
    }

    void OnBeginContact(IntPtr bodyA, IntPtr boxA, IntPtr bodyB, IntPtr boxB)
    {
        LObject objectA;
        if (!body2Objects.TryGetValue(bodyA, out objectA))
        {
            return;
        }

        LObject objectB;
        if (!body2Objects.TryGetValue(bodyB, out objectB))
        {
            return;
        }
        Debug.Log(string.Format("OnBeginContact {0} - {1}", objectA, objectB));
        objectA.Collidings.Add(new LObject_Time { obj = objectB });
        objectB.Collidings.Add(new LObject_Time { obj = objectA });
    }

    void OnEndContact(IntPtr bodyA, IntPtr boxA, IntPtr bodyB, IntPtr boxB)
    {
        // Debug.LogWarning(string.Format("OnEndContact"));

        LObject objectA;
        if (!body2Objects.TryGetValue(bodyA, out objectA))
        {
            return;
        }

        LObject objectB;
        if (!body2Objects.TryGetValue(bodyB, out objectB))
        {
            return;
        }

        Debug.Log(string.Format("OnEndContact {0} * {1}", objectA, objectB));
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

    public LObject GetObject(int id)
    {
        LObject obj;
        if (!this.DictObjects.TryGetValue(id, out obj))
        {
            return null;
        }
        return obj;
    }

    private List<int> toRemoves = new List<int>();
    public void RemoveObject(int id)
    {
        LObject obj = this.GetObject(id);
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

    public void Move(LCharacter lChar, Vector3 delta)
    {
        Vector3 from = lChar.Pos;
        float y = 0f;
        IWalkable preWalkable = lChar.Walkable;

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
            index = lChar.Collidings.FindIndex(_ => _.obj is IWalkable && (_.obj as IWalkable) == preWalkable);
            if (index >= 0)
            {
                index++;
            }
        }
        if (index >= 0)
        {
            for (; index < lChar.Collidings.Count; index++)
            {
                IWalkable walkable = lChar.Collidings[index].obj as IWalkable;
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
        }

        // limit by obstacles
        for (int i = 0; i < lChar.Collidings.Count; i++)
        {
            IObstacle ob = lChar.Collidings[i].obj as IObstacle;
            if (ob != null && ob.LimitMove(from, ref delta))
            {
                break;
            }
        }

        // old implement
        //for (int i = 0; i < this.Obstacles.Count; i++)
        //{
        //    if (this.Obstacles[i].LimitMove(from, ref delta))
        //    {
        //        break;
        //    }
        //}

        lChar.Pos = from + delta;
    }

    // public IWalkable RandomWalkable()
    // {
    //     int index = Random.Range(0, this.Walkables.Count);
    //     return this.Walkables[index];
    // }

    public void AddCharacter(LCharacter lChar)
    {
        this.DictObjects.Add(lChar.Id, lChar);
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
}
