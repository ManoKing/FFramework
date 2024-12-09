using UnityEngine;

public class NPCDecisionCar
{
    public int count;
    private GameObject car;
    /// <summary>
    /// 创建车，去停车场
    /// </summary>
    /// <param name="factoryManager"></param>
    /// <param name="npcData"></param>
    /// <param name="part"></param>
    public void SpawnCarToPart(Transform part)
    {
        // 创建车
        var car = FactoryManager.Instance.CreateCar("", CarQueueData.instance.initCarPos.position, Quaternion.identity);
        var navCar = car.GetComponent<NPCCarNavigation>();

        // 车出发，去停车场还是去排队
        navCar.SetDestination(part);

        // 车抵达, 创建人
        navCar.Arrive = () =>
        {
            count = 0;
            for (int i = 0; i < 2; i++)
            {
                new NPCDecisionPerson().SpawnPerson(part, navCar, this);
            }
        };
    }

    /// <summary>
    /// 创建车，去排队
    /// </summary>
    /// <param name="factoryManager"></param>
    /// <param name="npcData"></param>
    /// <param name="part"></param>
    public void SpawnCarToQueue(Transform part)
    {
        // 创建车
        car = FactoryManager.Instance.CreateCar("", CarQueueData.instance.initCarPos.position, Quaternion.identity);
        var navCar = car.GetComponent<NPCCarNavigation>();

        // 车出发，去停车场还是去排队
        navCar.SetDestination(part);
        CarQueueData.instance.carQueue.Enqueue(this);
    }

    /// <summary>
    /// 排队车前移动
    /// </summary>
    /// <param name="factoryManager"></param>
    /// <param name="npcData"></param>
    /// <param name="part"></param>
    public void QueueCarForward(Transform part)
    {
        var navCar = car.GetComponent<NPCCarNavigation>();

        // 车出发，去停车场还是去排队
        navCar.SetDestination(part);
    }

    /// <summary>
    /// 排队的车去停车场
    /// </summary>
    /// <param name="factoryManager"></param>
    /// <param name="npcData"></param>
    /// <param name="part"></param>
    public void QueueCarEnterPart(Transform part)
    {
        var navCar = car.GetComponent<NPCCarNavigation>();
        navCar.isArrive = false;
        // 车出发，去停车场还是去排队
        navCar.SetDestination(part);

        // 车排队
        navCar.Arrive = () =>
        {
            count = 0;
            for (int i = 0; i < 2; i++)
            {
                new NPCDecisionPerson().SpawnPerson(part, navCar, this);
            }
        };

    }
}
