using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using System;

public class QueueEntrance : MonoBehaviour
{
    public Transform queuePoint; // 排队点
    public float releaseInterval = 0.1f;// 放行间隔

    private Transform queueStart; // 决策点
    private List<GameObject> npcQueue = new List<GameObject>();


    private void Start()
    {
        queueStart = this.transform;
        StartCoroutine(ReleaseNPCs());
    }

    public void AddToQueue(GameObject npc)
    {
        NavMeshAgent agent = npc.GetComponent<NavMeshAgent>();
        agent.SetDestination(queueStart.position);

        npcQueue.Add(npc);
    }

    private IEnumerator ReleaseNPCs()
    {
        while (true)
        {
            yield return new WaitForSeconds(releaseInterval);

            List<GameObject> releasedNPCs = new List<GameObject>();
            foreach (var npcData in npcQueue)
            {
                if (HasReachedPosition(npcData))
                {
                    releasedNPCs.Add(npcData);
                }
            }

            for (int i = 0; i < releasedNPCs.Count; i++)
            {
                GameObject npcData = releasedNPCs[i];
                npcQueue.Remove(npcData);

                // 做决策，离开还是进入队伍
                var navigationInfo = npcData.GetComponent<NPCPersonNavigation>();
                if (queuePoint == null)
                {
                    Debug.LogError("经过出口大门");
                    // 经过点
                    navigationInfo.ReturnStart();
                }
                else
                {
                    var personQueueManager = queuePoint.GetComponent<PersonQueueManager>();
                    if (personQueueManager.npcQueue.Count < 3) // 排队队伍不满
                    {
                        personQueueManager.AddToQueue(npcData);
                        // 获取到npc信息，下一个点的信息 // TODO
                        personQueueManager.SetReleaseTargetPosition(navigationInfo.GetNextPointInfo());
                    }
                    else // 排队队伍满了
                    {
                        Debug.LogError("队伍满了，离开");
                        // 获取到npc信息，离开的信息
                        navigationInfo.ReturnStart();
                    }
                }
            }
        }
    }

    private bool HasReachedPosition(GameObject npc)
    {
        NavMeshAgent agent = npc.GetComponent<NavMeshAgent>();
        return !agent.pathPending && agent.remainingDistance <= agent.stoppingDistance;
    }


}
