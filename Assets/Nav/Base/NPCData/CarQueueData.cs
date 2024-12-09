using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarQueueData : MonoBehaviour
{
    public static CarQueueData instance;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    // 车初始化点
    public Transform initCarPos;
    // 停车位置
    public Transform initCarPart1;
    public Transform initCarPart2;
    public Transform initCarPart3;
    public Transform initCarPart4;
    // 车的排队位置
    public Transform posPartDoor1;
    public Transform posPartDoor2;
    public Transform posPartDoor3;
    public Transform posPartDoor4;
    // 车的消失点
    public Transform carEnd;


    // 车，数据结构设计
    public Dictionary<Transform, bool> partList; // 停车场信息
    public Queue<NPCDecisionCar> carQueue; // 大门排队车辆
    public Dictionary<Transform, bool> carPosDic; // 大门排队位置

}
