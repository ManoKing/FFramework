using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QueueEntranceManager : MonoBehaviour 
{
    public static QueueEntranceManager instance;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }
    // 人要分三类

    // 入口大门入口点
    public QueueEntrance queueEntranceEnter;
    // 出口大门入口点
    public QueueEntrance queueEntranceExit;
    // 所有游乐场入口点
    public List<QueueEntrance> queueEntranceList = new List<QueueEntrance>();

}
