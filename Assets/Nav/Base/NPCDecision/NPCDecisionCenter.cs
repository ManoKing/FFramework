using System.Collections.Generic;
using UnityEngine;

public class NPCDecisionCenter : MonoBehaviour
{
    private CarQueueData npcData;

    void Start()
    {
        // 获取数据
        npcData = CarQueueData.instance;

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
                npcData.carQueue.Dequeue().QueueCarEnterPart(spot);
                if (npcData.carQueue.Count < npcData.carPosDic.Count) // 排队车辆少于排队车辆限制，初始化车辆进入排队
                {
                    // 所有车辆前移
                    int index = 0;
                    foreach (var item in npcData.carQueue)
                    {
                        item.QueueCarForward(npcData.carPartDoorList[index]);
                        index++;
                    }
                    // 从后方开进一个
                    new NPCDecisionCar().SpawnCarToQueue(npcData.carPartDoorList[index]);
                }
            }
            else // 初始化车辆进入
            {
                new NPCDecisionCar().SpawnCarToPart(spot);
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
                    new NPCDecisionCar().SpawnCarToQueue(doorPos);
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
