using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class ThreadSynchronizationContextUpdater : MonoBehaviour
{
    void Awake()
    {
        ET.ThreadSynchronizationContext.Instance = new ET.ThreadSynchronizationContext(Thread.CurrentThread.ManagedThreadId);
        DontDestroyOnLoad(gameObject);
    }
    // Update is called once per frame
    void Update()
    {
        ET.ThreadSynchronizationContext.Instance.Update();
    }
}
