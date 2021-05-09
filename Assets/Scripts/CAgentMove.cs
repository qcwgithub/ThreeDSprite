using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class CAgentMove : MonoBehaviour
{
    public Spine.Unity.SkeletonAnimation Skel;
    private Vector3 originalScale;

    public Transform TEST;
    
    NavMeshAgent agent;
    private void Awake()
    {
        this.agent = this.GetComponent<NavMeshAgent>();
        this.originalScale = this.transform.localScale;
    }
    private void Update()
    {
        if (this.TEST != null && Input.GetKeyDown(KeyCode.M))
        {
            this.agent.SetDestination(this.TEST.position);
            return;
        }

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        if (x != 0f || z != 0f)
        {
            //Vector3 pos = this.transform.position;
            this.agent.Move(Time.deltaTime * this.agent.speed * new Vector3(x, 0f, z));

            this.Skel.AnimationName = "run";
            this.Skel.loop = true;

            Vector3 scale = this.originalScale;
            if (x < 0f)
            {
                scale.x = -scale.x;
            }
            //this.transform.localScale = scale;
        }
        else
        {
            this.Skel.AnimationName = "idle";
            this.Skel.loop = true;
        }
    }
}
