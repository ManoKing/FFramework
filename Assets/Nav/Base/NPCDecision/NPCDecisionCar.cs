using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;

public class NPCDecisionCar 
{
    public int count;
    private GameObject carDoor;
    public void SpawnCar(FactoryManager factoryManager, NPCData npcData, Transform part)
    {
        // 创建车
        var car = factoryManager.CreateCar("", npcData.initCarPos.position, Quaternion.identity);
        var navCar = car.GetComponent<NPCCarNavigation>();

        // 车出发，去停车场还是去排队
        navCar.SetDestination(part);

        // 车排队


        // 车抵达, 创建人
        navCar.Arrive = () =>
        {
            count = 0;
            for (int i = 0; i < 2; i++)
            {
                new NPCDecisionPerson().SpawnPerson(factoryManager, npcData, part, navCar, this);
            }
            
        };
    }

    public void SpawnCarList(FactoryManager factoryManager, NPCData npcData, Transform part)
    {
        // 创建车
        carDoor = factoryManager.CreateCar("", npcData.initCarPos.position, Quaternion.identity);
        var navCar = carDoor.GetComponent<NPCCarNavigation>();

        // 车出发，去停车场还是去排队
        navCar.SetDestination(part);
        npcData.carQueue.Enqueue(this);

        // 车排队
        navCar.Arrive = () =>
        {
            
        };

    }

    public void SpawnCarForwardList(FactoryManager factoryManager, NPCData npcData, Transform part)
    {
        var navCar = carDoor.GetComponent<NPCCarNavigation>();

        // 车出发，去停车场还是去排队
        navCar.SetDestination(part);

        // 车排队
        navCar.Arrive = () =>
        {

        };

    }

    public void SpawnCarListEnter(FactoryManager factoryManager, NPCData npcData, Transform part)
    {
        var navCar = carDoor.GetComponent<NPCCarNavigation>();
        navCar.isArrive = false;
        // 车出发，去停车场还是去排队
        navCar.SetDestination(part);

        // 车排队
        navCar.Arrive = () =>
        {
            count = 0;
            for (int i = 0; i < 2; i++)
            {
                new NPCDecisionPerson().SpawnPerson(factoryManager, npcData, part, navCar, this);
            }
        };

    }
}
