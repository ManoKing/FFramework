using UnityEngine;

public abstract class GameObjectFactory
{
    public abstract GameObject CreateObject(string objectId, Vector3 position, Quaternion rotation);
}

