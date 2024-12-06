using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;
using System;

public class PersonQueueManager : MonoBehaviour
{
    public float spacing = 0.9f; // 每个 NPC 间的间距
    public float releaseInterval = 2f;// 放行间隔
    public int releaseCount = 1; // 放行数量

    private Transform queueStart; // 队伍起点
    private Transform nextPos; // 放行位置
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
        Vector3 targetPosition = queueStart.position - queueStart.forward * (npcQueue.Count * spacing);

        NavMeshAgent agent = npc.GetComponent<NavMeshAgent>();
        agent.SetDestination(targetPosition);

        npcQueue.Add(new NPCData(npc, targetPosition));
    }

    public void SetReleaseTargetPosition(Transform nextpos)
    {
        this.nextPos = nextpos;
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

            if (releasedNPCs.Count >= releaseCount)
            {
                for (int i = 0; i < releaseCount; i++)
                {

                    NPCData npcData = releasedNPCs[i];
                    

                    // 排队结束
                    if (nextPos.GetComponent<NpcPointManager>() != null)
                    {
                        nextPos.GetComponent<NpcPointManager>().AddToQueue(npcData.npc);
                    }
                    else
                    {
                        // 回到车里
                        Debug.LogError("没有决策点");
                        npcData.npc.GetComponent<NPCPersonNavigation>().Arrive();
                    }
                    npcQueue.Remove(npcData);
                }

                RearrangeQueue();
            }
        }
    }

    private bool HasReachedPosition(GameObject npc, Vector3 targetPosition)
    {
        NavMeshAgent agent = npc.GetComponent<NavMeshAgent>();
        return !agent.pathPending && agent.remainingDistance <= agent.stoppingDistance;
    }

    private void RearrangeQueue()
    {
        for (int i = 0; i < npcQueue.Count; i++)
        {
            NPCData npcData = npcQueue[i];
            Vector3 newTargetPosition = queueStart.position - queueStart.forward * (i * spacing);
            npcData.targetPosition = newTargetPosition;

            NavMeshAgent agent = npcData.npc.GetComponent<NavMeshAgent>();
            agent.SetDestination(newTargetPosition);
        }
    }
   

}
