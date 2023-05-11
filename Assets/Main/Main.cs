using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;
public class Main : MonoBehaviour
{
    public Text loadText;
    public Slider loadSlider;

    void Start()
    {
        StartCoroutine(UpdateAddressablesContent()); 
    }

    void StartGame()
    {
        gameObject.AddComponent<LoadDll>();
    }

    IEnumerator UpdateAddressablesContent()
    {
        Debug.Log("UpdateAddressablesContent");
        var initHandle = Addressables.InitializeAsync();
        yield return initHandle;

        var handler = Addressables.CheckForCatalogUpdates(false);
        yield return handler;

        var catalogs = handler.Result;
        Debug.Log($"need update catalog:{catalogs.Count}"); 
        foreach (var catalog in catalogs)
        {
            Debug.Log(catalog);
        }

        if (catalogs.Count > 0)
        {
            var updateHandle = Addressables.UpdateCatalogs(catalogs, false);
            yield return updateHandle;

            var locators = updateHandle.Result;
            foreach (var locator in locators)
            {
                foreach (var key in locator.Keys)
                {
                    Debug.Log($"update {key}");
                }
            }
        }

        string downkey = "Pre";
        var sizeHandle = Addressables.GetDownloadSizeAsync(downkey);
        yield return sizeHandle;
        long totalDownloadSize = sizeHandle.Result;

        Debug.Log("NEED downLoad size:" + totalDownloadSize);    
        if (totalDownloadSize > 0)
        {
            //var downloadHandle = Addressables.DownloadDependenciesAsync(downkey, Addressables.MergeMode.Union);
            var downloadHandle = Addressables.DownloadDependenciesAsync(downkey, false);
            while (downloadHandle.Status == AsyncOperationStatus.None)
            {
                float percent = downloadHandle.PercentComplete;  

                var status = downloadHandle.GetDownloadStatus();
                float progress = status.Percent;    
                Debug.LogError($"已经下载：{(int)(totalDownloadSize * percent)}/{totalDownloadSize}");
                Debug.LogError($"{progress * 100:0.0}" + "%"); 
                loadText.text = $"正在为您下载和校验配置，请耐心等待：" + $"{progress * 100:0.0}" + "%";
                loadSlider.gameObject.SetActive(true);
                loadSlider.value = progress;  
                yield return null;
            }

            if (downloadHandle.IsDone)
            {
                Debug.LogError("已经下载完成！！！");
            }
            if (downloadHandle.Status == AsyncOperationStatus.Succeeded)
            {
                Addressables.Release(downloadHandle);
            }
        }
        Debug.Log("已经下载完成，准备进入游戏");
        Addressables.Release(handler);
        Debug.Log("释放hander成功");
        yield return null;
        StartGame();
    }

}
