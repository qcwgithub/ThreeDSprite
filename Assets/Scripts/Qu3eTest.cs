using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using Script;

public enum q3BodyType
{
    eStaticBody,
    eDynamicBody,
    eKinematicBody
}

public enum q3TransformOperation
{
    ePostion,
    eRotation,
    eBoth,
}


public class Qu3eTest : MonoBehaviour
{
    public BoxCollider ColliderA;
    public BoxCollider ColliderB;

    IntPtr m_scene;
    IntPtr m_bodyA;
    Vector3 m_posA;
    Quaternion m_rotA;

    IntPtr m_bodyB;
    Vector3 m_posB;
    Quaternion m_rotB;

    // class BodyInfo
    // {
    //     BoxCollider collider;
    //     IntPtr body;
    //     Vector3
    // }

    void Start()
    {
        m_scene = Qu3eApi.CreateScene();
        Debug.Log("scene = " + m_scene);

        Qu3eApi.SceneSetContactListener(m_scene, new Qu3eApi.ContactDelegate(this.OnBeginContact), new Qu3eApi.ContactDelegate(this.OnEndContact));

        {
            var posA = ColliderA.transform.position;
            m_bodyA = Qu3eApi.SceneAddBody(m_scene, q3BodyType.eDynamicBody, posA.x, posA.y, posA.z);
            m_posA = posA;
            m_rotA = ColliderA.transform.rotation;

            var extendsA = ColliderA.bounds.extents;
            Qu3eApi.BodyAddBox(m_bodyA, 0f, 0f, 0f, extendsA.x, extendsA.y, extendsA.z);
            Debug.Log(string.Format("bodyA: {0}, pos: ({1},{2},{3}), extends: ({4}, {5}, {6})", m_bodyA, posA.x, posA.y, posA.z, extendsA.x, extendsA.y, extendsA.z));
        }

        {
            var posB = ColliderB.transform.position;
            m_bodyB = Qu3eApi.SceneAddBody(m_scene, q3BodyType.eDynamicBody, posB.x, posB.y, posB.z);
            m_posB = posB;
            m_rotB = ColliderB.transform.rotation;

            var extendsB = ColliderB.bounds.extents;
            Qu3eApi.BodyAddBox(m_bodyB, 0f, 0f, 0f, extendsB.x, extendsB.y, extendsB.z);
            Debug.Log(string.Format("bodyB: {0}, pos: ({1},{2},{3}), extends: ({4}, {5}, {6})", m_bodyB, posB.x, posB.y, posB.z, extendsB.x, extendsB.y, extendsB.z));
        }

        // DestroyScene(m_scene);
    }

    void Update()
    {
        if (m_scene != IntPtr.Zero)
        {
            {
                var posA = ColliderA.transform.position;
                var rotA = ColliderA.transform.rotation;
                if (posA != m_posA || rotA != m_rotA)
                {
                    m_posA = posA;
                    m_rotA = rotA;
                    Qu3eApi.BodySetTransform(m_bodyA, q3TransformOperation.eBoth, new float[] { posA.x, posA.y, posA.z, rotA.x, rotA.y, rotA.z, rotA.w });
                    Debug.Log(string.Format("bodyA pos: ({0},{1},{2}), rot: ({3}, {4}, {5}, {6})", posA.x, posA.y, posA.z, rotA.x, rotA.y, rotA.z, rotA.w));
                }
            }

            {
                var posB = ColliderB.transform.position;
                var rotB = ColliderB.transform.rotation;
                if (posB != m_posB || rotB != m_rotB)
                {
                    m_posB = posB;
                    m_rotB = rotB;
                    Qu3eApi.BodySetTransform(m_bodyB, q3TransformOperation.eBoth, new float[] { posB.x, posB.y, posB.z, rotB.x, rotB.y, rotB.z, rotB.w });
                    Debug.Log(string.Format("bodyB pos: ({0},{1},{2}), rot: ({3}, {4}, {5}, {6})", posB.x, posB.y, posB.z, rotB.x, rotB.y, rotB.z, rotB.w));
                }
            }

            Qu3eApi.SceneStep(m_scene);
        }
    }

    void OnDestroy()
    {
        if (m_scene != IntPtr.Zero)
        {
            Qu3eApi.SceneDestroy(m_scene);
            m_scene = IntPtr.Zero;

            m_bodyA = IntPtr.Zero;
            m_bodyB = IntPtr.Zero;
        }
    }

    void OnBeginContact(IntPtr bodyA, IntPtr boxA, IntPtr bodyB, IntPtr boxB)
    {
        Debug.LogWarning(string.Format("OnBeginContact"));
    }

    void OnEndContact(IntPtr bodyA, IntPtr boxA, IntPtr bodyB, IntPtr boxB)
    {
        Debug.LogWarning(string.Format("OnEndContact"));
    }

}
