using UnityEngine;

public class NPCDecisionPerson
{
    public void SpawnPerson(FactoryManager factoryManager, NPCData npcData, Transform part, NPCCarNavigation navCar, NPCDecisionCar decision)
    {
        // 人出生
        var person = factoryManager.CreatePerson("", part.position, Quaternion.identity);
        var personNav = person.GetComponent<NPCPersonNavigation>();
        personNav.initPos = part;
        // 创建出人，人要带有数据，要去哪些游乐场


        personNav.ReturnStart = () => { EndReturn(part, person, decision, navCar, npcData); };
        // 人排队, 到大门决策点; (大门入口有多个，决策去那个大门【需要拿到当前排队信息，都排满也要过去】)
        // 也可以让npc直接过去，选择那个入口
        QueueEntranceManager.instance.queueEntranceEnter.AddToQueue(person);

    }

    public void EndReturn(Transform part, GameObject person, NPCDecisionCar decision, NPCCarNavigation navCar, NPCData npcData)
    {

        var personNav = person.GetComponent<NPCPersonNavigation>();
        //人离开
        personNav.SetDestination(part);
        personNav.isReturn = false;
        //人抵达
        personNav.ReturnEnd = () =>
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
