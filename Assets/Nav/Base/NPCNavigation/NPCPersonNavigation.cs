using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NPCPersonNavigation : MonoBehaviour
{
    [HideInInspector]
    public NavMeshAgent agent;
    [HideInInspector]
    public bool isReturn = true;
    // npc返回出发
    public Action ReturnStart;
    // npc返回结束
    public Action ReturnEnd;

    // npc导航信息
    [HideInInspector]
    public Transform initPos;

    private int index = 0;
    
    void Awake()
    {
        agent = this.GetComponent<NavMeshAgent>();
    }

    public void SetDestination(Transform pos)
    {
        agent.destination = pos.position;//自动导航
    }

    /// <summary>
    /// 获取下一个导航点
    /// </summary>
    public QueueEntrance GetNextPointInfo()
    {
        if (index >= QueueEntranceManager.instance.queueEntranceList.Count)
        {
            // 返回停车场
            return null;
        }
        else
        {
            // 提供下一个排队点
            var data = QueueEntranceManager.instance.queueEntranceList[index];
            index++;
            return data;
        }
    }

    void Update()
    {
        if (!isReturn && !agent.pathPending && agent.remainingDistance < 0.5f )
        {
            isReturn = true;
            ReturnEnd?.Invoke();
        }
    }

    /// <summary>
    /// 加入对象池 TODO
    /// </summary>
    public void Pool()
    {
        Destroy(gameObject);
    }

}
