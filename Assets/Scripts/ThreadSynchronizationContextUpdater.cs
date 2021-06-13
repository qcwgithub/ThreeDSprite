using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThreadSynchronizationContextUpdater : MonoBehaviour
{
    void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
    // Update is called once per frame
    void Update()
    {
        ET.ThreadSynchronizationContext.Instance.Update();
    }
}
