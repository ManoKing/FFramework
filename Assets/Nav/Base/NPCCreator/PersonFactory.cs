using UnityEngine;

public class PersonFactory : GameObjectFactory
{
    private string resourcePath = "AgentPerson"; // 人物资源路径

    public override GameObject CreateObject(string objectId, Vector3 position, Quaternion rotation)
    {
        // 加载人物模型
        GameObject prefab = Resources.Load<GameObject>($"{resourcePath}{objectId}");
        if (prefab == null)
        {
            Debug.LogError($"Person with ID {objectId} not found in Resources/{resourcePath}");
            return null;
        }

        // 实例化人物
        return Object.Instantiate(prefab, position, rotation);
    }
}
