using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarQueueData : MonoBehaviour
{
    public static CarQueueData instance;
   
    // 车初始化点
    public Transform initCarPos;
    // 停车位置
    public List<Transform> carPartPosList = new List<Transform>();
    // 车的排队位置
    public List<Transform> carPartDoorList = new List<Transform>();
    // 车的消失点
    public Transform carEnd;

    // 车，数据结构设计
    public Dictionary<Transform, bool> partList; // 停车场信息
    public Queue<NPCDecisionCar> carQueue; // 大门排队车辆
    public Dictionary<Transform, bool> carPosDic; // 大门排队位置

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        // 初始化停车场数据
        partList = new Dictionary<Transform, bool>();
        for (int i = 0; i < carPartPosList.Count; i++)
        {
            partList.Add(carPartPosList[i], true);
        }

        // 初始化进入停车场排队位置数据
        carPosDic = new Dictionary<Transform, bool>();
        for (int i = 0; i < carPartDoorList.Count; i++)
        {
            carPosDic.Add(carPartDoorList[i], true);
        }

        carQueue = new Queue<NPCDecisionCar>(); // 排队车辆

    }
}
