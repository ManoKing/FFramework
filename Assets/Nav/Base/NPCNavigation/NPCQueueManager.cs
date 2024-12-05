using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NPCQueueManager : MonoBehaviour
{
    public Transform startPoint; // A 点
    public Transform queueStartPoint; // 队列起点（B 点）
    public Transform nextDestination; // 队列结束后的目标
    public float queueSpacing = 2f; // 队列间距
    public float waitTime = 2f; // 每个 NPC 的等待时间

    private Queue<NavMeshAgent> npcQueue = new Queue<NavMeshAgent>();

    void Start()
    {
        // 将场景中的所有 NPC 加入队列
        foreach (Transform child in transform)
        {
            NavMeshAgent agent = child.GetComponent<NavMeshAgent>();
            if (agent != null)
            {
                npcQueue.Enqueue(agent);
                StartCoroutine(HandleNPC(agent));
            }
        }
    }

    private IEnumerator HandleNPC(NavMeshAgent agent)
    {
        // 1. 从起点移动到队列起点
        agent.SetDestination(queueStartPoint.position);
        yield return new WaitUntil(() => agent.remainingDistance <= agent.stoppingDistance);

        // 2. 等待并排队
        while (npcQueue.Peek() != agent)
        {
            yield return null; // 等待轮到自己
        }

        // 3. 设置排队位置
        Vector3 queuePosition = queueStartPoint.position + Vector3.back * queueSpacing * (npcQueue.Count - 1);
        agent.SetDestination(queuePosition);
        yield return new WaitUntil(() => agent.remainingDistance <= agent.stoppingDistance);

        // 4. 等待指定时间
        yield return new WaitForSeconds(waitTime);

        // 5. 移动到下一个目标点
        agent.SetDestination(nextDestination.position);
        npcQueue.Dequeue();
    }
}
