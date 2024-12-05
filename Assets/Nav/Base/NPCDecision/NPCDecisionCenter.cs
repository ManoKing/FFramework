using Cysharp.Threading.Tasks;
using GameFramework.Data;
using System;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;

public class NPCDecisionCenter : MonoBehaviour
{
    
    private FactoryManager factoryManager;
    private NPCData npcData;

    
    void Start()
    {
        // 初始化工厂管理器
        factoryManager = new FactoryManager();

        // 获取数据
        npcData = GetComponent<NPCData>();

        // 初始化停车场数据
        npcData.partList = new Dictionary<Transform, bool>();
        npcData.partList.Add(npcData.initCarPart1, true);
        npcData.partList.Add(npcData.initCarPart2, true);
        npcData.partList.Add(npcData.initCarPart3, true);
        npcData.partList.Add(npcData.initCarPart4, true);

        // 初始化进入停车场排队位置数据
        npcData.carPosDic = new Dictionary<Transform, bool>();
        npcData.carPosDic.Add(npcData.posPartDoor1, true);
        npcData.carPosDic.Add(npcData.posPartDoor2, true);
        npcData.carPosDic.Add(npcData.posPartDoor3, true);
        npcData.carPosDic.Add(npcData.posPartDoor4, true);

        npcData.carQueue = new Queue<NPCDecisionCar>(); // 排队车辆

        npcData.personDoorQueue = new Queue<GameObject>(); // 大门排队
        npcData.personPlayQueue = new Queue<GameObject>(); // 游乐排队

        // 固定时间创建一辆车
        InvokeRepeating(nameof(LoopCar), 0f, 3f);

    }

    private void LoopCar()
    {
        var spot = FindEmptyParkingSpot();
        if (spot != null)// 有停车位
        {
            npcData.partList[spot] = false;
            if (npcData.carQueue.Count > 0) // 排队车辆进入停车场
            {
                npcData.carQueue.Dequeue().SpawnCarListEnter(factoryManager, npcData, spot);

                if (npcData.carQueue.Count < npcData.carPosDic.Count) // 排队车辆少于排队车辆限制，初始化车辆进入排队
                {
                    // 所有车辆前移 TODO 数据结构不好处理
                    int index = 0;
                    foreach (var item in npcData.carQueue)
                    {
                        if (index == 0)
                        {
                            item.SpawnCarForwardList(factoryManager, npcData, npcData.posPartDoor1);
                        }
                        else if (index == 1)
                        {
                            item.SpawnCarForwardList(factoryManager, npcData, npcData.posPartDoor2);
                        }
                        else if (index == 2)
                        {
                            item.SpawnCarForwardList(factoryManager, npcData, npcData.posPartDoor3);
                        }
                        index++;
                    }

                    

                    // 从后方开进一个
                    new NPCDecisionCar().SpawnCarList(factoryManager, npcData, npcData.posPartDoor4);
                }
            }
            else // 初始化车辆进入
            {
                new NPCDecisionCar().SpawnCar(factoryManager, npcData, spot);
            }

        }
        else // 进入排队队列
        {
            // 排队大于排队队列，不再产生
            if (npcData.carQueue.Count <= npcData.carPosDic.Count) 
            {
                var doorPos = FindEmptyParkingDoor();
                if (doorPos != null)
                {
                    new NPCDecisionCar().SpawnCarList(factoryManager, npcData, doorPos);
                    npcData.carPosDic[doorPos] = false;
                }
            }
        }
    }

    private Transform FindEmptyParkingDoor()
    {
        foreach (var item in npcData.carPosDic)
        {
            if (item.Value)
            {
                return item.Key;
            }
        }
        return null;
    }

    private Transform FindEmptyParkingSpot()
    {
        foreach (var item in npcData.partList)
        {
            if (item.Value)
            {
                return item.Key;
            }
        }
        return null;
    }
}
