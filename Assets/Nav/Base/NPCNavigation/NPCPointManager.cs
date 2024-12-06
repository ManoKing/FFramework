using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using System;

public class NpcPointManager : MonoBehaviour
{
    public Transform queuePoint; // 排队点
    public float releaseInterval = 0.1f;// 放行间隔
    public Transform nextQueueStart; // 下一个决策点

    private Transform queueStart; // 决策点
    private List<NPCData> npcQueue = new List<NPCData>();

    private class NPCData
    {
        public GameObject npc;
        public Vector3 targetPosition;

        public NPCData(GameObject npc, Vector3 targetPosition)
        {
            this.npc = npc;
            this.targetPosition = targetPosition;
        }
    }

    private void Start()
    {
        queueStart = this.transform;
        StartCoroutine(ReleaseNPCs());
    }

    public void AddToQueue(GameObject npc)
    {
        NavMeshAgent agent = npc.GetComponent<NavMeshAgent>();
        agent.SetDestination(queueStart.position);

        npcQueue.Add(new NPCData(npc, queueStart.position));
    }

    private IEnumerator ReleaseNPCs()
    {
        while (true)
        {
            yield return new WaitForSeconds(releaseInterval);

            List<NPCData> releasedNPCs = new List<NPCData>();
            foreach (var npcData in npcQueue)
            {
                if (HasReachedPosition(npcData.npc, npcData.targetPosition))
                {
                    releasedNPCs.Add(npcData);
                }
            }

            for (int i = 0; i < releasedNPCs.Count; i++)
            {
                NPCData npcData = releasedNPCs[i];
                npcQueue.Remove(npcData);

                queuePoint.GetComponent<PersonQueueManager>().AddToQueue(npcData.npc);
                queuePoint.GetComponent<PersonQueueManager>().SetReleaseTargetPosition(nextQueueStart);
            }
        }
    }

    private bool HasReachedPosition(GameObject npc, Vector3 targetPosition)
    {
        NavMeshAgent agent = npc.GetComponent<NavMeshAgent>();
        return !agent.pathPending && agent.remainingDistance <= agent.stoppingDistance;
    }


}
