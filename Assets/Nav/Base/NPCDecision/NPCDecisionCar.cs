using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
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
    public void SpawnCar(FactoryManager factoryManager, Transform part)
    {
        // 创建车
        var car = factoryManager.CreateCar("", CarQueueData.instance.initCarPos.position, Quaternion.identity);
        var navCar = car.GetComponent<NPCCarNavigation>();

        // 车出发，去停车场还是去排队
        navCar.SetDestination(part);

        // 车抵达, 创建人
        navCar.Arrive = () =>
        {
            count = 0;
            for (int i = 0; i < 2; i++)
            {
                new NPCDecisionPerson().SpawnPerson(factoryManager, part, navCar, this);
            }
        };
    }

    /// <summary>
    /// 创建车，去排队
    /// </summary>
    /// <param name="factoryManager"></param>
    /// <param name="npcData"></param>
    /// <param name="part"></param>
    public void SpawnCarList(FactoryManager factoryManager, Transform part)
    {
        // 创建车
        car = factoryManager.CreateCar("", CarQueueData.instance.initCarPos.position, Quaternion.identity);
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
    public void SpawnCarForwardList(FactoryManager factoryManager, Transform part)
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
    public void SpawnCarListEnter(FactoryManager factoryManager, Transform part)
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
                new NPCDecisionPerson().SpawnPerson(factoryManager, part, navCar, this);
            }
        };

    }
}
