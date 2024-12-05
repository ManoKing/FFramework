using Cysharp.Threading.Tasks;
using GameFramework.Data;
using System;
using System.Collections.Generic;
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

        npcData.partList = new Dictionary<Transform, bool>();
        npcData.partList.Add(npcData.initCarPart1, true);
        npcData.partList.Add(npcData.initCarPart2, true);
        npcData.partList.Add(npcData.initCarPart3, true);
        npcData.partList.Add(npcData.initCarPart4, true);

        // 固定时间创建一辆车
        InvokeRepeating(nameof(LoopCar), 0f, 15f);

    }

    private void LoopCar()
    {
        foreach (var item in npcData.partList)
        {
            if (item.Value)
            {
                new NPCDecisionCar().SpawnCar(factoryManager, npcData, item.Key);
                break;
            }
        }
        
    }
}
