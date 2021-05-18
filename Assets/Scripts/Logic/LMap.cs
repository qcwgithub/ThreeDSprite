using System.Collections.Generic;
using UnityEngine;

public class LMap
{
    public LMapData Data { get; private set; }
    public Dictionary<int, LObject> DictObjects = new Dictionary<int, LObject>();
    public List<IWalkable> Walkables = new List<IWalkable>();
    public List<IObstacle> Obstacles = new List<IObstacle>();

    public LMap(LMapData data)
    {
        this.Data = data; 
        for (int i = 0; i < data.Floors.Length; i++)
        {
            LFloorData floorData = data.Floors[i];
            LFloor floor = new LFloor(floorData);
            this.Walkables.Add(floor);
            this.DictObjects.Add(floor.Id, floor);
        }

        for (int i = 0; i < data.Stairs.Length; i++)
        {
            LStairData stairData = data.Stairs[i];
            LStair stair = new LStair(stairData);
            this.Walkables.Add(stair);
            this.DictObjects.Add(stair.Id, stair);
        }

        for (int i = 0; i < data.BoxObstacles.Length; i++)
        {
            LBoxObstacleData obData = data.BoxObstacles[i];
            LBoxObstacle obstacle = new LBoxObstacle(obData);
            if (obstacle.Data.Walkable)
            {
                this.Walkables.Add(obstacle);
            }
            this.Obstacles.Add(obstacle);
            this.DictObjects.Add(obstacle.Id, obstacle);
        }
    }


    public void Move(CCharacter character, Vector3 delta)
    {
        Vector3 from = character.Pos;
        float y = 0f;
        IWalkable preWalkable = character.Walkable;

        if (character.Walkable != null)
        {
            PredictMoveResult result = character.Walkable.PredictMove(from, delta);
            if (!result.OutOfRange)
            {
                //Debug.Log("OutOfRange")
                y = result.Y;
            }
            else
            {
                Debug.Log("Out of range, " + ((LObject)character.Walkable).Id);
                character.Walkable = null;
            }
        }

        if (character.Walkable == null)
        {
            for (int i = 0; i < this.Walkables.Count; i++)
            {
                if (this.Walkables[i] != preWalkable && this.Walkables[i].CanAccept(from, delta))
                {
                    character.Walkable = this.Walkables[i];
                    PredictMoveResult result = this.Walkables[i].PredictMove(from, delta);
                    y = result.Y;
                    break;
                }
            }
        }

        if (character.Walkable != null)
        {
            delta.y = y - from.y;
        }

        for (int i = 0; i < this.Obstacles.Count; i++)
        {
            if (this.Obstacles[i].LimitMove(from, ref delta))
            {
                break;
            }
        }

        character.Pos = from + delta;
    }

    public IWalkable RandomWalkable()
    {
        int index = Random.Range(0, this.Walkables.Count);
        return this.Walkables[index];
    }
}
