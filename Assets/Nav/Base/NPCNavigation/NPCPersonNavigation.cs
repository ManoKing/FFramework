using System;
using UnityEngine;
using UnityEngine.AI;

public class NPCPersonNavigation : MonoBehaviour
{
    [HideInInspector]
    public NavMeshAgent agent;
    [HideInInspector]
    public bool isArrive = true;
    public Action Arrive;
    public Action ArriveX;
    void Awake()
    {
        agent = this.GetComponent<NavMeshAgent>();
    }

    public void SetDestination(Transform pos)
    {
        agent.destination = pos.position;//自动导航
    }

    void Update()
    {
        if (!agent.pathPending && agent.remainingDistance < 0.5f && !isArrive)
        {
            isArrive = true;
            ArriveX?.Invoke();
        }
    }

    public void Pool()
    {
        Destroy(gameObject);
    }

}
