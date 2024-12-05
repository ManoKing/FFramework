using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ParkingLotManager : MonoBehaviour
{
    public Transform entrance; // 停车场入口位置
    public Transform exit; // 出口位置
    public Transform[] parkingSpots; // 停车位
    public int maxVehicles = 5; // 停车场最大车位数量

    private Queue<GameObject> vehicleQueue = new Queue<GameObject>(); // 车辆排队队列

    private Queue<GameObject> personQueue = new Queue<GameObject>(); // 离开排队队列

    private List<GameObject> parkedVehicles = new List<GameObject>(); // 当前停在车位的车辆

    public GameObject vehiclePrefab; // 车辆预制体
    public GameObject personPrefab; // 人物预制体

    void Start()
    {
        StartCoroutine(SpawnVehicles());
    }

    // 每 10 秒生成一辆车
    private IEnumerator SpawnVehicles()
    {
        while (true)
        {
            GameObject vehicle = Instantiate(vehiclePrefab, GetSpawnPoint(), Quaternion.identity);
            MoveToEntrance(vehicle);
            yield return new WaitForSeconds(10f);
        }
    }

    // 获取车辆生成点
    private Vector3 GetSpawnPoint()
    {
        return new Vector3(Random.Range(-10, -5), 0, Random.Range(-10, -5)); // 自定义生成点范围
    }

    // 车辆移动到入口
    private void MoveToEntrance(GameObject vehicle)
    {
        NavMeshAgent agent = vehicle.GetComponent<NavMeshAgent>();
        agent.SetDestination(entrance.position);

        // 到达入口后检查停车场状态
        StartCoroutine(HandleVehicleAtEntrance(vehicle));
    }

    private IEnumerator HandleVehicleAtEntrance(GameObject vehicle)
    {
        NavMeshAgent agent = vehicle.GetComponent<NavMeshAgent>();

        yield return new WaitUntil(() => agent.remainingDistance <= agent.stoppingDistance);

        if (parkedVehicles.Count < maxVehicles)
        {
            // 停车场未满，找到空停车位
            Transform spot = FindEmptyParkingSpot();
            if (spot != null)
            {
                ParkVehicle(vehicle, spot);
            }
        }
        else
        {
            // 停车场已满，加入排队队列
            vehicleQueue.Enqueue(vehicle);
        }
    }

    private Transform FindEmptyParkingSpot()
    {
        foreach (Transform spot in parkingSpots)
        {
            if (!parkedVehicles.Exists(v => Vector3.Distance(v.transform.position, spot.position) < 1f))
            {
                return spot;
            }
        }
        return null;
    }

    private void ParkVehicle(GameObject vehicle, Transform spot)
    {
        NavMeshAgent agent = vehicle.GetComponent<NavMeshAgent>();
        agent.SetDestination(spot.position);

        parkedVehicles.Add(vehicle);
        StartCoroutine(HandlePassengers(vehicle));
    }

    private IEnumerator HandlePassengers(GameObject vehicle)
    {
        NavMeshAgent agent = vehicle.GetComponent<NavMeshAgent>();

        yield return new WaitUntil(() => agent.remainingDistance <= agent.stoppingDistance);

        // 生成两个乘客
        for (int i = 0; i < 2; i++)
        {
            GameObject person = Instantiate(personPrefab, vehicle.transform.position, Quaternion.identity);
            MovePersonToExit(person);
        }
    }

    private void MovePersonToExit(GameObject person)
    {
        NavMeshAgent agent = person.GetComponent<NavMeshAgent>();
        agent.SetDestination(exit.position);
        personQueue.Enqueue(person);

        StartCoroutine(HandlePersonQueue());
    }

    private IEnumerator HandlePersonQueue()
    {
        while (personQueue.Count > 0)
        {
            GameObject person = personQueue.Dequeue();

            // 等待 5 秒再移动
            yield return new WaitForSeconds(5f);

            NavMeshAgent agent = person.GetComponent<NavMeshAgent>();
            agent.SetDestination(exit.position);

            // 销毁人物到达出口后
            StartCoroutine(DestroyPersonAtExit(person));
        }
    }

    private IEnumerator DestroyPersonAtExit(GameObject person)
    {
        NavMeshAgent agent = person.GetComponent<NavMeshAgent>();

        yield return new WaitUntil(() => agent.remainingDistance <= agent.stoppingDistance);

        Destroy(person);
    }

}

