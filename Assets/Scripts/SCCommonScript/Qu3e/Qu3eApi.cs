using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

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

public class Qu3eApi
{
    [DllImport("qu3e.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr CreateScene();

    [DllImport("qu3e.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern void SceneDestroy(IntPtr scene);

    public delegate void ContactDelegate(IntPtr bodyA, IntPtr boxA, IntPtr bodyB, IntPtr boxB);
    [DllImport("qu3e.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern void SceneSetContactListener(IntPtr scene, ContactDelegate onBeginContact, ContactDelegate onEndContact);

    [DllImport("qu3e.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr SceneStep(IntPtr scene);

    [DllImport("qu3e.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr SceneAddBody(IntPtr scene, q3BodyType bodyType, float x, float y, float z);

    [DllImport("qu3e.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern void BodySetTransform(IntPtr body, q3TransformOperation operation, float[] values);

    [DllImport("qu3e.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern void BodySetToAwake(IntPtr body);
    [DllImport("qu3e.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern void BodySetToSleep(IntPtr body);

    [DllImport("qu3e.dll", CallingConvention = CallingConvention.Cdecl)]
    public static extern void BodyAddBox(IntPtr body, float x, float y, float z, float extend_x, float extend_y, float extend_z);
}
