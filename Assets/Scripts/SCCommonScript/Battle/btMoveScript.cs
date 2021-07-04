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

                Vector3 from = character.Pos;
                float y = 0f;
                btIWalkable preWalkable = character.Walkable;

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
                        Debug.Log("Out of range, " + character.Walkable.ToString());
                        character.Walkable = null;
                    }
                }

                // find a new walkables
                int index = 0;
                if (character.Walkable != null)
                {
                    index = character.collidings.FindIndex(_ => _.obj is btIWalkable && (_.obj as btIWalkable) == preWalkable);
                    if (index >= 0)
                    {
                        index++;
                    }
                }
                if (index >= 0)
                {
                    for (; index < character.collidings.Count; index++)
                    {
                        btIWalkable walkable = character.collidings[index].obj as btIWalkable;
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
                            character.Walkable = walkable;
                            PredictMoveResult result = walkable.PredictMove(from, delta);
                            y = result.Y;
                            break;
                        }
                    }
                }

                if (character.Walkable != null)
                {
                    delta.y = y - from.y;

                    // limit by obstacles
                    for (int i = 0; i < character.collidings.Count; i++)
                    {
                        btIObstacle ob = character.collidings[i].obj as btIObstacle;
                        if (ob != null && ob.LimitMove(from, ref delta))
                        {
                            break;
                        }
                    }

                    character.Pos = from + delta;
                }
                else if (!character.EverHasWalkable)
                {
                    character.Pos = from + delta;
                }
                else
                {
                    // keep position unchanged
                }
            }
        }
    
        public void PredictMove()
        {

        }
    }
}