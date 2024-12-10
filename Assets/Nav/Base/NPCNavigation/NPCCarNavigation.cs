using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class NPCCarNavigation : MonoBehaviour
{
    [HideInInspector]
    public NavMeshAgent agent;
    [HideInInspector]
    public bool isArrive;
    public Action Arrive;
    [HideInInspector]
    public Transform parkingSpot; // 停车位
    private bool isAligning;
    private bool isEnd;
    void Awake()
    {
        agent = this.GetComponent<NavMeshAgent>();
    }

    public void SetDestination(Transform pos, bool isEnd = false)
    {
        this.isEnd = isEnd;
        isAligning = true;
        parkingSpot = pos;
        agent.isStopped = false;
        agent.destination = pos.position;//自动导航
    }

    void Update()
    {
        if (!agent.pathPending && agent.remainingDistance < 0.5f)
        {
            if (!isArrive)
            {
                isArrive = true;
                Arrive?.Invoke();
            }
            if (isAligning && !isEnd)
            {
                AlignToParkingSpot();
            }
        }
    }

    // 对齐停车位
    private void AlignToParkingSpot()
    {
        
        agent.isStopped = true;

        StartCoroutine(AlignCarRoutine());
    }


    // 对齐车身到停车位正前方
    private IEnumerator AlignCarRoutine()
    {
        // 目标方向
        Quaternion targetRotation = Quaternion.LookRotation(parkingSpot.forward, Vector3.up);

        // 平滑旋转
        while (Quaternion.Angle(transform.rotation, targetRotation) > 0.1f)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5);
            yield return null;
        }

        transform.rotation = targetRotation;
        isAligning = false;
        Debug.LogError("停车完成！");
    }

    /// <summary>
    /// TODO 对象池
    /// </summary>
    public void Pool()
    {
        Destroy(gameObject);
    }
}
