using Cysharp.Threading.Tasks;
using UnityEngine;

public class NPCDecisionPerson 
{
    public void SpawnPerson(FactoryManager factoryManager, NPCData npcData, Transform part, NPCCarNavigation navCar, NPCDecisionCar decision)
    {
        
        var person = factoryManager.CreatePerson("", part.position, Quaternion.identity);
        var personNav = person.GetComponent<NPCPersonNavigation>();

        // 人出发
        personNav.SetDestination(npcData.posDoor);
 
        // 人抵达
        personNav.Arrive = async () =>
        {

            // 人排队


            // 排队结束
            await UniTask.WaitForSeconds(2f);


            // 人出发
            personNav.SetDestination(npcData.posPlay);
            personNav.isArrive = false;

            // 人抵达
            personNav.Arrive = async () =>
            {
                // 人排队

                // 排队结束
                await UniTask.WaitForSeconds(2f);
                // 人玩

                // 玩结束


                // 人离开
                personNav.SetDestination(part);
                personNav.isArrive = false;
                // 人抵达
                personNav.Arrive = () =>
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
            };
        };

    }
}
