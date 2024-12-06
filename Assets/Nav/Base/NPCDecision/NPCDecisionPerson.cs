using Cysharp.Threading.Tasks;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.AI;

public class NPCDecisionPerson 
{
    public void SpawnPerson(FactoryManager factoryManager, NPCData npcData, Transform part, NPCCarNavigation navCar, NPCDecisionCar decision)
    {
        
        var person = factoryManager.CreatePerson("", part.position, Quaternion.identity);
        var personNav = person.GetComponent<NPCPersonNavigation>();

        personNav.Arrive = () => { EndReturn(part, person, decision, navCar, npcData); };
        // 人排队, 到大门决策点
        npcData.posDoorEnd.GetComponent<NpcPointManager>().AddToQueue(person);


    }

    public void EndReturn(Transform part, GameObject person, NPCDecisionCar decision, NPCCarNavigation navCar, NPCData npcData)
    {

        var personNav = person.GetComponent<NPCPersonNavigation>();
        //人离开
        personNav.SetDestination(part);
        personNav.isArrive = false;
        ////人抵达
        personNav.ArriveX = () =>
        {
            // 人消失
            personNav.Pool();
            ++decision.count;
            if (decision.count == 2)
            {
                // 车离开
                navCar.SetDestination(npcData.carEnd);
                navCar.isArrive = false;

                npcData.partList[part] = true;

                // 车消失
                navCar.Arrive = () =>
                {
                    navCar.Pool();
                };
            }
        };
    }
}
