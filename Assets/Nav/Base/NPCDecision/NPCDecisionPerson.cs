using UnityEngine;

public class NPCDecisionPerson
{
    public void SpawnPerson(Transform part, NPCCarNavigation navCar, NPCDecisionCar decision)
    {
        // 人出生
        var person = FactoryManager.Instance.CreatePerson("", part.position, Quaternion.identity);

        // 创建出人，人要带有数据，要去哪些游乐场
        var personNav = person.GetComponent<NPCPersonNavigation>();
        personNav.initPos = part;
        personNav.ReturnStart = () => { EndReturn(part, person, decision, navCar); };

        // 人排队, 到大门决策点; (大门入口有多个，决策去那个大门【需要拿到当前排队信息，都排满也要过去】)
        // 也可以让npc直接过去，选择那个入口
        QueueEntranceManager.instance.queueEntranceEnter.AddToQueue(person);

    }

    public void EndReturn(Transform part, GameObject person, NPCDecisionCar decision, NPCCarNavigation navCar)
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
                navCar.SetDestination(CarQueueData.instance.carEnd);
                navCar.isArrive = false;

                CarQueueData.instance.partList[part] = true;

                // 车消失
                navCar.Arrive = () =>
                {
                    navCar.Pool();
                };
            }
        };
    }
}
