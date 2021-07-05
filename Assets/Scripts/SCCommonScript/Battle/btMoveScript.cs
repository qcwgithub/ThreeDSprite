using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Data;

namespace Script
{
    public class btMoveScript : btScriptBase
    {
        public ECode characterMove(btCharacter character, Vector3 moveDir)
        {
            if (!(moveDir.x >= -1f && moveDir.x <= 1f && moveDir.y >= -1f && moveDir.y <= 1f && moveDir.z >= -1f && moveDir.z <= 1f))
            {
                return ECode.InvalidParam;
            }

            character.moveDir = moveDir;
            return ECode.Success;
        }
        public void characterStopMove(btCharacter character)
        {
            character.moveDir = Vector3.zero;
        }

        public bool isWalkable(btObjectType objectType)
        {
            switch (objectType)
            {
                case btObjectType.floor:
                case btObjectType.stair:
                    return true;
                default:
                    return false;
            }
        }

        public bool isObstacle(btObjectType objectType)
        {
            switch (objectType)
            {
                case btObjectType.box_obstacle:
                    return true;
                default:
                    return false;
            }
        }

        public void setObjectPosition(btBattle battle, btIHasPosition iHasPos, Vector3 pos)
        {
            iHasPos.prePos = iHasPos.pos;
            iHasPos.pos = pos;
            btObject obj = (btObject)iHasPos;
            if (obj.body != IntPtr.Zero)
            {
                var tempForPosition = battle.tempForPosition;
                tempForPosition[0] = pos.x;
                tempForPosition[1] = pos.y;
                tempForPosition[2] = pos.z;
                Qu3eApi.BodySetTransform(obj.body, q3TransformOperation.ePostion, tempForPosition);
            }
        }

        public void update(btBattle battle, float dt)
        {
            foreach (var kv in battle.characters)
            {
                btCharacter character = kv.Value;
                if (character.moveDir == Vector3.zero)
                {
                    continue;
                }

                Vector3 delta = 5f * dt * character.moveDir;

                Vector3 from = character.pos;
                // character.prePos = character.pos;
                float y = 0f;
                btIWalkable preWalkable = character.walkable;

                if (character.walkable != null)
                {
                    PredictMoveResult result = this.predictMove(character.walkable, from, delta);
                    if (!result.outOfRange)
                    {
                        //Debug.Log("OutOfRange")
                        y = result.y;
                    }
                    else
                    {
                        Debug.Log("Out of range, " + character.walkable.ToString());
                        character.walkable = null;
                    }
                }

                btIWalkable newWalkable;
                this.findNewWalkable(preWalkable, character.collidings, from, delta, out newWalkable);
                if (newWalkable != null)
                {
                    character.walkable = newWalkable;
                    PredictMoveResult newMoveResult = this.predictMove(newWalkable, from, delta);
                    y = newMoveResult.y;
                }

                if (character.walkable != null)
                {
                    delta.y = y - from.y;
                    this.setObjectPosition(battle, character, from + delta);
                }
                else
                {
                    // keep pos unchanged
                    character.walkable = preWalkable;
                }

                // if (character.walkable != null)
                // {
                //     delta.y = y - from.y;

                //     // limit by obstacles
                //     for (int i = 0; i < character.collidings.Count; i++)
                //     {
                //         btIObstacle ob = character.collidings[i].obj as btIObstacle;
                //         if (ob != null && ob.LimitMove(from, ref delta))
                //         {
                //             break;
                //         }
                //     }

                //     character.Pos = from + delta;
                // }
                // else if (!character.EverHasWalkable)
                // {
                //     character.Pos = from + delta;
                // }
                // else
                // {
                //     // keep position unchanged
                // }
            }
        }

        bool checkXZOutOfRange(btObject obj, Vector3 pos)
        {
            bool outOfRange = false;
            if (pos.x < obj.worldMin.x)
            {
                pos.x = obj.worldMin.x;
                outOfRange = true;
            }
            else if (pos.x > obj.worldMax.x)
            {
                pos.x = obj.worldMax.x;
                outOfRange = true;
            }

            if (pos.z < obj.worldMin.z)
            {
                pos.z = obj.worldMin.z;
                outOfRange = true;
            }
            else if (pos.z > obj.worldMax.z)
            {
                pos.z = obj.worldMax.z;
                outOfRange = true;
            }
            return outOfRange;
        }

        float stairZtoY(btStair stair, float z)
        {
            float t = (z - stair.worldMin.z) / (stair.worldMax.z - stair.worldMin.z);
            float y = UnityEngine.Mathf.Lerp(stair.worldMin.y, stair.worldMax.y, t);
            return y;
        }

        float stairXtoY(btStair stair, float x)
        {
            if (stair.dir == StairDir.left_low_right_high)
            {
                float t = (x - stair.worldMin.x) / (stair.worldMax.x - stair.worldMin.x);
                float y = UnityEngine.Mathf.Lerp(stair.worldMin.y, stair.worldMax.y, t);
                return y;
            }
            else
            {
                float t = (x - stair.worldMin.x) / (stair.worldMax.x - stair.worldMin.x);
                float y = UnityEngine.Mathf.Lerp(stair.worldMax.y, stair.worldMin.y, t);
                return y;
            }
        }

        float stairXZtoY(btStair stair, float x, float z)
        {
            switch (stair.dir)
            {
                case StairDir.front_back:
                    return this.stairZtoY(stair, z);
                //break;
                case StairDir.left_high_right_low:
                case StairDir.left_low_right_high:
                default:
                    return this.stairXtoY(stair, x);
                    //break;
            }
        }

        Vector3 floorRandomPos(btFloor floor)
        {
            Vector3 pos;
            pos.x = UnityEngine.Random.Range(floor.worldMin.x, floor.worldMax.x);
            pos.z = UnityEngine.Random.Range(floor.worldMin.z, floor.worldMax.z);
            pos.y = floor.worldMin.y;
            return pos;
        }

        bool floorCanAccept(btFloor floor, Vector3 from, Vector3 delta)
        {
            Vector3 to = from + delta;
            if (this.checkXZOutOfRange(floor, to))
            {
                return false;
            }

            float y = floor.worldMin.y;

            if (delta.y < 0 && from.y > y && to.y <= y)
            {
                return true;
            }
            if (Mathf.Abs(y - to.y) > 0.1f)
            {
                return false;
            }
            return true;
        }
        bool stairCanAccept(btStair stair, Vector3 from, Vector3 delta)
        {
            Vector3 to = from + delta;
            if (this.checkXZOutOfRange(stair, to))
            {
                return false;
            }

            float y = this.stairXZtoY(stair, to.x, to.z);
            if (delta.y < 0 && from.y > y && to.y <= y)
            {
                return true;
            }
            if (Mathf.Abs(to.y - y) > 0.1f)
            {
                return false;
            }
            return true;
        }

        bool canAccept(btObject obj, Vector3 from, Vector3 delta)
        {
            bool canAccept = false;
            switch (obj.type)
            {
                case btObjectType.floor:
                    {
                        canAccept = this.floorCanAccept((btFloor)obj, from, delta);
                    }
                    break;

                case btObjectType.stair:
                    {
                        canAccept = this.stairCanAccept((btStair)obj, from, delta);
                    }
                    break;
                default:
                    break;
            }
            return canAccept;
        }

        PredictMoveResult predictMove(btIWalkable walkable, Vector3 from, Vector3 delta)
        {
            PredictMoveResult result = default;
            Vector3 to = from + delta;

            btObject obj = walkable as btObject;

            switch (obj.type)
            {
                case btObjectType.floor:
                    {
                        if (this.checkXZOutOfRange(obj, to))
                        {
                            result.outOfRange = true;
                        }
                        else
                        {
                            result.y = obj.worldMin.y;
                        }
                    }
                    break;

                case btObjectType.stair:
                    {
                        if (this.checkXZOutOfRange(obj, to))
                        {
                            result.outOfRange = true;
                        }
                        else
                        {
                            result.y = this.stairXZtoY((btStair)obj, to.x, to.z);
                        }
                    }
                    break;

                case btObjectType.character:
                case btObjectType.box_obstacle:
                case btObjectType.tree:
                case btObjectType.wall:
                default:
                    break;
            }

            return result;
        }

        void findNewWalkable(btIWalkable preWalkable, List<btObject> collidings, Vector3 from, Vector3 delta,
            out btIWalkable newWalkable)
        {
            newWalkable = null;

            // find a new walkables
            int index = 0;
            if (preWalkable != null)
            {
                // 只找后面的，防止不断在 2 个 walkable 中切换
                index = collidings.FindIndex(_ => _ is btIWalkable && (_ as btIWalkable) == preWalkable);
                if (index >= 0)
                {
                    index++;
                }
                else
                {
                    // throw new System.Exception();
                    // newWalkable = null;
                    return;
                }
            }

            for (; index < collidings.Count; index++)
            {
                btIWalkable walkable = collidings[index] as btIWalkable;
                if (walkable == null)
                {
                    continue;
                }
                if (walkable == preWalkable)
                {
                    continue;
                }

                if (this.canAccept((btObject)walkable, from, delta))
                {
                    newWalkable = walkable;
                    break;
                }
            }
        }

        public void randomWalkable(btBattle battle, out btIWalkable walkable, out Vector3 pos)
        {
            walkable = null;
            pos = Vector3.zero;

            for (int i = 0; i < battle.walkables.Count; i++)
            {
                if (battle.walkables[i] is btFloor floor)
                {
                    walkable = floor;
                    pos = this.floorRandomPos(floor);
                    break;
                }
            }
        }
    }
}