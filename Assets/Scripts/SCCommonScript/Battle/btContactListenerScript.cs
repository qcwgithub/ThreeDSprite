using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Data;

namespace Script
{
    public class btContactListenerScript : btScriptBase
    {
        public void initListner(BMBattleInfo battle)
        {
            battle.onBeginContactDel = new Qu3eApi.ContactDelegate(
                (IntPtr bodyA, IntPtr boxA, IntPtr bodyB, IntPtr boxB) => this.onBeginContact(battle, bodyA, boxA, bodyB, boxB));

            battle.onEndContactDel = new Qu3eApi.ContactDelegate(
                (IntPtr bodyA, IntPtr boxA, IntPtr bodyB, IntPtr boxB) => this.onEndContact(battle, bodyA, boxA, bodyB, boxB));

            Qu3eApi.SceneSetContactListener(battle.physicsScene, battle.onBeginContactDel, battle.onEndContactDel);
        }

        void onBeginContact(BMBattleInfo battle, IntPtr bodyA, IntPtr boxA, IntPtr bodyB, IntPtr boxB)
        {
            btObject objectA;
            if (!battle.body2Objects.TryGetValue(bodyA, out objectA))
            {
                return;
            }

            btObject objectB;
            if (!battle.body2Objects.TryGetValue(bodyB, out objectB))
            {
                return;
            }
            Debug.Log(string.Format("OnBeginContact {0} - {1}", objectA, objectB));
            objectA.collidings.Add(objectB);
            objectB.collidings.Add(objectA);
        }

        void onEndContact(BMBattleInfo battle, IntPtr bodyA, IntPtr boxA, IntPtr bodyB, IntPtr boxB)
        {
            // Debug.LogWarning(string.Format("OnEndContact"));

            btObject objectA;
            if (!battle.body2Objects.TryGetValue(bodyA, out objectA))
            {
                return;
            }

            btObject objectB;
            if (!battle.body2Objects.TryGetValue(bodyB, out objectB))
            {
                return;
            }

            Debug.Log(string.Format("OnEndContact {0} x {1}", objectA, objectB));
            for (int i = 0; i < objectA.collidings.Count; i++)
            {
                if (objectA.collidings[i] == objectB)
                {
                    objectA.collidings.RemoveAt(i);
                    i--;
                }
            }

            for (int i = 0; i < objectB.collidings.Count; i++)
            {
                if (objectB.collidings[i] == objectA)
                {
                    objectB.collidings.RemoveAt(i);
                    i--;
                }
            }
        }
    }
}