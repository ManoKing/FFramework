using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using QFramework;
using UnityEngine.UI;
using UnityEngine.U2D;

public static class AddressableExtension
{
    public static void SetSprite(this Image self, string atlasName, string spriteName)
    {
        var atals = AddressableManager.Instance.LoadAsset<SpriteAtlas>(atlasName);
        self.sprite = atals.GetSprite(spriteName);
    }

    public static void SetSprite(this Image self, string spriteName)
    {
        self.sprite = AddressableManager.Instance.LoadAsset<Sprite>(spriteName);
    }

    public static void SetTexture(this RawImage self, string textureName)
    {
        self.texture = AddressableManager.Instance.LoadAsset<Texture>(textureName);
    }

}
public class AddressableManager : Singleton<AddressableManager>
{
    private AddressableManager() { }
    private Dictionary<string, AsyncOperationHandle> caches = new Dictionary<string, AsyncOperationHandle>();

    public T LoadAsset<T>(string address) where T : UnityEngine.Object
    {
        if (caches.ContainsKey(address))
        {
            return caches[address].Result as T;
        }
        else
        {
            var handle = Addressables.LoadAssetAsync<T>(address);
            var obj = handle.WaitForCompletion();

            caches.Add(address, handle);
            return obj as T;
        }
    }

    public void LoadAsset<T>(string address, Action<T> onComplete, Action onFailed = null) where T : UnityEngine.Object
    {
        if (caches.ContainsKey(address))
        {
            var handle = this.caches[address];
            if (handle.IsDone)
            {
                if (onComplete != null)
                {
                    onComplete(caches[address].Result as T);
                }
            }
            else
            {
                handle.Completed += (result) =>
                {
                    if (result.Status == AsyncOperationStatus.Succeeded)
                    {
                        var obj = result.Result as T;
                        if (onComplete != null)
                        {
                            onComplete(obj);
                        }
                    }
                    else
                    {
                        if (onFailed != null)
                        {
                            onFailed();
                        }

                        Debug.LogError("Load " + address + " failed!");
                    }
                };
            }
        }
        else
        {
            var handle = Addressables.LoadAssetAsync<T>(address);
            handle.Completed += (result) =>
            {
                if (result.Status == AsyncOperationStatus.Succeeded)
                {
                    var obj = result.Result as T;
                    if (onComplete != null)
                    {
                        onComplete(obj);
                    }
                }
                else
                {
                    if (onFailed != null)
                    {
                        onFailed();
                    }

                    Debug.LogError("Load " + address + " failed!");
                }
            };
            caches.Add(address, handle);
        }
    }

    private void AddCompleted<T>(AsyncOperationHandle handle,Action<T> onComplete, Action onFailed = null)where T : UnityEngine.Object
    {
        handle.Completed += (result) =>
        {
            if (result.Status == AsyncOperationStatus.Succeeded)
            {
                var obj = result.Result as T;
                if (onComplete != null)
                {
                    onComplete(obj);
                }
            }
            else
            {
                if (onFailed != null)
                {
                    onFailed();
                }

                Debug.LogError("Load "  + " failed!");
            }
        };
    }
}