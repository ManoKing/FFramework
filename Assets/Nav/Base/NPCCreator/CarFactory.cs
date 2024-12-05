using UnityEngine;

public class CarFactory : GameObjectFactory
{
    private string resourcePath = "AgentCar"; // 车辆资源路径

    public override GameObject CreateObject(string objectId, Vector3 position, Quaternion rotation)
    {
        // 加载车辆模型
        GameObject prefab = Resources.Load<GameObject>($"{resourcePath}{objectId}");
        if (prefab == null)
        {
            Debug.LogError($"Vehicle with ID {objectId} not found in Resources/{resourcePath}");
            return null;
        }

        // 实例化车辆
        return Object.Instantiate(prefab, position, rotation);
    }
}
