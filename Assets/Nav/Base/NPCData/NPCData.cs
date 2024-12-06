using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCData : MonoBehaviour
{
    public Transform initCarPos;
    public Transform initCarPart1;
    public Transform initCarPart2;
    public Transform initCarPart3;
    public Transform initCarPart4;
    public Transform posDoor;
    public Transform posDoorEnd;
    public Transform posPlay;
    public Transform posPlayEnd;
    public Transform carEnd;
    public Transform posPartDoor1;
    public Transform posPartDoor2;
    public Transform posPartDoor3;
    public Transform posPartDoor4;


    // 数据结构设计
    public Dictionary<Transform, bool> partList; // 停车场信息
    public Queue<NPCDecisionCar> carQueue; // 大门排队车辆
    public Dictionary<Transform, bool> carPosDic; // 大门排队位置

    public Queue<GameObject> personDoorQueue; // npc大门排队
    public Queue<GameObject> personPlayQueue; // npc游乐排队
}
