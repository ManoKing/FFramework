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
    public Transform posPlay;
    public Transform carEnd;


    // 数据结构设计
    public Dictionary<Transform, bool> partList;

}
