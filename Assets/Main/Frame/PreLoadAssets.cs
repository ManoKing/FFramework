using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.AddressableAssets.ResourceLocators;
using System;

public class PreLoadAssets : MonoBehaviour
{
    private const string preloadLabel = "Pre";
    public Slider slider;
    public Text preText;
    public UnityAction<float> ProgressEvent;
    public UnityAction<bool> CompletionEvent;
    private AsyncOperationHandle downloadHandle;
    private AsyncOperationHandle sceneHandle;
    private AsyncOperationHandle initHandle;
    private AsyncOperationHandle<List<string>> catalogUpdatesHandle;
    private AsyncOperationHandle<List<IResourceLocator>> updateHandle;
    private AsyncOperationHandle<long> getDownloadSizeHandle;

    void Start()
    {
        ProgressEvent = PreProgress;
        CompletionEvent = Preompletion;

        initHandle = Addressables.InitializeAsync();
        initHandle.Completed += _ => {
            if (initHandle.Status == AsyncOperationStatus.Succeeded)
            {
                Addressables.Release(initHandle);
                catalogUpdatesHandle = Addressables.CheckForCatalogUpdates(false);
                catalogUpdatesHandle.Completed += _ => {
                    Debug.Log("check catalog status " + catalogUpdatesHandle.Status);
                    if (catalogUpdatesHandle.Status == AsyncOperationStatus.Succeeded)
                    {
                        List<string> catalogs = new List<string>();
                        catalogs.AddRange(catalogUpdatesHandle.Result);
                        if (catalogs != null && catalogs.Count > 0)
                        {
                            foreach (var catalog in catalogs)
                            {
                                Debug.Log("catalog  " + catalog);
                            }
                            Debug.Log("download catalog start "); ;
                            Addressables.Release(catalogUpdatesHandle);
                            updateHandle = Addressables.UpdateCatalogs(catalogs, false);
                            updateHandle.Completed += _ => {
                                if (updateHandle.Status == AsyncOperationStatus.Succeeded)
                                {/*
                                    foreach (var item in updateHandle.Result)
                                    {
                                        Debug.LogError("catalog result " + item.LocatorId);
                                        foreach (var key in item.Keys)
                                        {
                                            Debug.LogError("catalog key " + key);
                                        }
                                    }*/
                                    Debug.LogError("download catalog finish " + updateHandle.Status);
                                    GetDownloadSize();
                                }
                                else
                                {
                                    Debug.LogError("download catalog status " + updateHandle.Status);
                                }
                                Addressables.Release(updateHandle);
                            };
                        }
                        else
                        {
                            Addressables.Release(catalogUpdatesHandle);
                            Debug.Log("dont need update catalogs");
                            LoadScene();
                        }
                    }
                };
            }
            else
            {
                Addressables.Release(initHandle);
                Debug.LogError("init addressable error");
            }
        };
    }

    public void PreProgress(float pre)
    {
        slider.value = pre;
        preText.text = "Check for resource updates " + (int)(pre * 100)  + "%";
    }

    public void Preompletion(bool isDown)
    {
        if (isDown)
        {
            LoadScene();
        }
    }

    void LoadScene()
    {
        var go = new GameObject("LoadDll");
        var temp = go.AddComponent<LoadDll>();
        DontDestroyOnLoad(go);
        temp.loadScene = () => {
            sceneHandle = Addressables.LoadSceneAsync("Main");
            sceneHandle.Completed += _ => {
                GameObject.Destroy(gameObject);
            };
        };
    }

    private void GetDownloadSize()
    {
        getDownloadSizeHandle = Addressables.GetDownloadSizeAsync(preloadLabel);
        getDownloadSizeHandle.Completed += _ => {
            Debug.LogWarning("加载文件大小：" + getDownloadSizeHandle.Result);
            if (getDownloadSizeHandle.Result > 0)
            {
                Addressables.Release(getDownloadSizeHandle);
                StartCoroutine(PreLoadGameAssets());
            }
            else
            {
                Addressables.Release(getDownloadSizeHandle);
                LoadScene();
            }
        };
    }

    IEnumerator PreLoadGameAssets()
    {
        downloadHandle = Addressables.DownloadDependenciesAsync(preloadLabel, false);
        float progress = 0;

        while (downloadHandle.Status == AsyncOperationStatus.None)
        {
            float percentageComplete = downloadHandle.GetDownloadStatus().Percent;
            if (percentageComplete > progress * 1.1) // Report at most every 10% or so
            {
                progress = percentageComplete; // More accurate %
                ProgressEvent.Invoke(progress);
            }
            yield return null;
        }

        CompletionEvent.Invoke(downloadHandle.Status == AsyncOperationStatus.Succeeded);
        Addressables.Release(downloadHandle); //Release the operation handle
    }

    private float catalogUpdatesPercent = 0;
    private float updateResourcePercent = 0;
    // Update is called once per frame
    void Update()
    {
        if (sceneHandle.IsValid())
        {
            slider.value = (float)Math.Round((double)sceneHandle.PercentComplete, 2);
            preText.text = "Load resource progress " + (int)(sceneHandle.PercentComplete * 100) + "%";
            Debug.Log("Load sence progress " + sceneHandle.PercentComplete * 100 + "%");
        }

        if (initHandle.IsValid())
        {
            slider.value = (float)Math.Round((double)initHandle.PercentComplete, 2);
            preText.text = "Initialization  progress " + (int)(initHandle.PercentComplete * 100) + "%";
            Debug.Log("Initialization  progress  " + initHandle.PercentComplete * 100 + "%");
        }

        if (catalogUpdatesHandle.IsValid())
        {
            if (catalogUpdatesHandle.PercentComplete == 0) // 模拟更新Catalog
            {
                catalogUpdatesPercent += 0.0005f;
                if (catalogUpdatesPercent > 0.99f)
                {
                    catalogUpdatesPercent = 0.99f;
                }
                slider.value =  (float)Math.Round((double)catalogUpdatesPercent, 2);
                preText.text = "Catalog Updates progress " + (int)(catalogUpdatesPercent * 100) + "%";
            }
            else
            {
                slider.value = catalogUpdatesHandle.PercentComplete;
                preText.text = "Catalog Updates progress " + (int)(catalogUpdatesHandle.PercentComplete * 100) + "%";
                Debug.Log("Catalog Updates progress " + catalogUpdatesHandle.PercentComplete * 100 + "%");
            }           
        }

        if (updateHandle.IsValid())
        {
            if (updateHandle.PercentComplete == 0) // 模拟加载资源
            {
                updateResourcePercent += 0.0005f;
                if (updateResourcePercent > 0.99f)
                {
                    updateResourcePercent = 0.99f;
                }
                slider.value = (float)Math.Round((double)updateResourcePercent, 2);
                preText.text = "Update resource progress " + (int)(updateResourcePercent * 100) + "%";
            }
            else
            {
                slider.value = (float)Math.Round((double)updateHandle.PercentComplete, 2);
                preText.text = "Update resource progress " + (int)(updateHandle.PercentComplete * 100) + "%";
                Debug.Log("Update resource progress " + updateHandle.PercentComplete * 100 + "%");
            }
            
        }

        if (getDownloadSizeHandle.IsValid())
        {
            slider.value = (float)Math.Round((double)getDownloadSizeHandle.PercentComplete, 2);
            preText.text = "Get down load size progress " + (int)(getDownloadSizeHandle.PercentComplete * 100) + "%";
            Debug.Log("Get down load size progress " + getDownloadSizeHandle.PercentComplete * 100 + "%");
        }


    }
}
