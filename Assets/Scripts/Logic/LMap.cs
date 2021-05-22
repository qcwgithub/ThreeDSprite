using System.Collections.Generic;
using UnityEngine;

public class LMap
{
    public LMapData Data { get; private set; }
    public Dictionary<int, LObject> DictObjects = new Dictionary<int, LObject>();
    private List<IWalkable> Walkables = new List<IWalkable>();
    private List<IObstacle> Obstacles = new List<IObstacle>();
    private List<LTree> Trees = new List<LTree>();
    public BoundsOctree<LObject> Octree { get; private set; }
    private HashSet<int> needUpdates = new HashSet<int>();
    public LMap(LMapData data)
    {
        this.Data = data; 
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

        // create octree
        this.Octree = new BoundsOctree<LObject>(15, Vector3.zero, 1f, 1.2f);
        foreach (var kv in this.DictObjects)
        {
            kv.Value.AddToOctree(this.Octree);
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

        if (lChar.Walkable == null)
        {
            // find a new walkables
            for (int i = 0; i < lChar.Collidings.Count; i++)
            {
                IWalkable walkable = lChar.Collidings[i] as IWalkable;
                if (walkable == null || walkable == preWalkable)
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

            // old implement
            //for (int i = 0; i < this.Walkables.Count; i++)
            //{
            //    if (this.Walkables[i] != preWalkable && this.Walkables[i].CanAccept(from, delta))
            //    {
            //        lChar.Walkable = this.Walkables[i];
            //        PredictMoveResult result = this.Walkables[i].PredictMove(from, delta);
            //        y = result.Y;
            //        break;
            //    }
            //}
        }

        if (lChar.Walkable != null)
        {
            delta.y = y - from.y;
        }

        // limit by obstacles
        for (int i = 0; i < lChar.Collidings.Count; i++)
        {
            IObstacle ob = lChar.Collidings[i] as IObstacle;
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

    public IWalkable RandomWalkable()
    {
        int index = Random.Range(0, this.Walkables.Count);
        return this.Walkables[index];
    }

    public void AddCharacter(LCharacter lChar)
    {
        this.DictObjects.Add(lChar.Id, lChar);
    }

    public void AddNeedUpdate(int id)
    {
        this.needUpdates.Add(id);
    }

    private bool updating = false;
    public void Update()
    {
        this.updating = true;
        foreach (var id in this.needUpdates)
        {
            this.GetObject(id).Update();
        }
        this.updating = false;
    }
}
