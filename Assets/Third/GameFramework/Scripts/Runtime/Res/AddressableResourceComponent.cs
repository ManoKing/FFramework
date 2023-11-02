using GameFramework;
using GameFramework.Download;
using GameFramework.FileSystem;
using GameFramework.ObjectPool;
using GameFramework.Resource;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityGameFramework.Runtime;

/// <summary>
/// Addressable资源组件。
/// </summary>
[DisallowMultipleComponent]
[AddComponentMenu("Game Framework/AddressableResource")]
public class AddressableResourceComponent : GameFrameworkComponent, IResourceManager
{
    private const int DefaultPriority = 0;
    private IResourceManager m_ResourceManager = null;

    public event EventHandler<GameFramework.Resource.ResourceApplySuccessEventArgs> ResourceApplySuccess;
    public event EventHandler<GameFramework.Resource.ResourceApplyFailureEventArgs> ResourceApplyFailure;
    public event EventHandler<GameFramework.Resource.ResourceUpdateStartEventArgs> ResourceUpdateStart;
    public event EventHandler<GameFramework.Resource.ResourceUpdateChangedEventArgs> ResourceUpdateChanged;
    public event EventHandler<GameFramework.Resource.ResourceUpdateSuccessEventArgs> ResourceUpdateSuccess;
    public event EventHandler<GameFramework.Resource.ResourceUpdateFailureEventArgs> ResourceUpdateFailure;

    /// <summary>
    /// 卸载资源。
    /// </summary>
    /// <param name="asset">要卸载的资源。</param>
    public void UnloadAsset(object asset)
    {
        m_ResourceManager.UnloadAsset(asset);
    }

    /// <summary>
    /// 游戏框架组件初始化。
    /// </summary>
    protected override void Awake()
    {
        base.Awake();
    }

    private void Start()
    {
        BaseComponent baseComponent = GameEntry.GetComponent<BaseComponent>();
        if (baseComponent == null)
        {
            Log.Fatal("Base component is invalid.");
            return;
        }

        var m_EditorResourceMode = baseComponent.EditorResourceMode;
        m_ResourceManager = m_EditorResourceMode ? baseComponent.EditorResourceHelper : GameFrameworkEntry.GetModule<IResourceManager>();
        if (m_ResourceManager == null)
        {
            Log.Fatal("Resource manager is invalid.");
            return;
        }
    }

    /// <summary>
    /// 强制执行释放未被使用的资源。
    /// </summary>
    /// <param name="performGCCollect">是否使用垃圾回收。</param>
    public void ForceUnloadUnusedAssets(bool performGCCollect)
    {
        throw new NotSupportedException("ForceUnloadUnusedAssets");
    }

    /// <summary>
    /// 使用单机模式并初始化资源。
    /// </summary>
    /// <param name="initResourcesCompleteCallback">使用单机模式并初始化资源完成时的回调函数。</param>
    public void InitResources(InitResourcesCompleteCallback initResourcesCompleteCallback)
    {
        throw new NotSupportedException("InitResources");
    }

    /// <summary>
    /// 获取正在更新的资源组。
    /// </summary>
    public IResourceGroup UpdatingResourceGroup
    {
        get
        {
            throw new NotSupportedException("UpdatingResourceGroup");
        }
    }

    /// <summary>
    /// 使用可更新模式并检查资源。
    /// </summary>
    /// <param name="ignoreOtherVariant">是否忽略处理其它变体的资源，若不忽略，将会移除其它变体的资源。</param>
    /// <param name="checkResourcesCompleteCallback">使用可更新模式并检查资源完成时的回调函数。</param>
    public void CheckResources(CheckResourcesCompleteCallback checkResourcesCompleteCallback)
    {
        throw new NotSupportedException("CheckResources");
    }

    /// <summary>
    /// 获取资源组。
    /// </summary>
    /// <param name="resourceGroupName">要获取的资源组名称。</param>
    /// <returns>要获取的资源组。</returns>
    public IResourceGroup GetResourceGroup(string resourceGroupName)
    {
        throw new NotSupportedException("GetResourceGroup");
    }


    /// <summary>
    /// 获取资源模式。
    /// </summary>
    public ResourceMode ResourceMode
    {
        get
        {
            return ResourceMode.Unspecified;
        }
    }

    /// <summary>
    /// 使用可更新模式并更新版本资源列表。
    /// </summary>
    /// <param name="versionListLength">版本资源列表大小。</param>
    /// <param name="versionListHashCode">版本资源列表哈希值。</param>
    /// <param name="versionListZipLength">版本资源列表压缩后大小。</param>
    /// <param name="versionListZipHashCode">版本资源列表压缩后哈希值。</param>
    /// <param name="updateVersionListCallbacks">版本资源列表更新回调函数集。</param>
    public void UpdateVersionList(int versionListLength, int versionListHashCode, int versionListZipLength, int versionListZipHashCode, UpdateVersionListCallbacks updateVersionListCallbacks)
    {
        throw new NotSupportedException("UpdateVersionList");
    }

    /// <summary>
    /// 设置当前变体。
    /// </summary>
    /// <param name="currentVariant">当前变体。</param>
    public void SetCurrentVariant(string currentVariant)
    {
        throw new NotSupportedException("SetCurrentVariant");
    }


    /// <summary>
    /// 使用可更新模式并更新指定资源组的资源。
    /// </summary>
    /// <param name="resourceGroupName">要更新的资源组名称。</param>
    /// <param name="updateResourcesCompleteCallback">使用可更新模式并更新指定资源组完成时的回调函数。</param>
    public void UpdateResources(string resourceGroupName, UpdateResourcesCompleteCallback updateResourcesCompleteCallback)
    {
        throw new NotSupportedException("UpdateResources");
    }

    /// <summary>
    /// 检查版本资源列表。
    /// </summary>
    /// <param name="latestInternalResourceVersion">最新的内部资源版本号。</param>
    /// <returns>检查版本资源列表结果。</returns>
    public CheckVersionListResult CheckVersionList(int latestInternalResourceVersion)
    {
        throw new NotSupportedException("CheckVersionList");
    }

    public void SetReadOnlyPath(string readOnlyPath)
    {
        throw new NotImplementedException();
    }

    public void SetReadWritePath(string readWritePath)
    {
        throw new NotImplementedException();
    }

    public void SetResourceMode(ResourceMode resourceMode)
    {
        throw new NotImplementedException();
    }

    public void SetObjectPoolManager(IObjectPoolManager objectPoolManager)
    {
        throw new NotImplementedException();
    }

    public void SetFileSystemManager(IFileSystemManager fileSystemManager)
    {
        throw new NotImplementedException();
    }

    public void SetDownloadManager(IDownloadManager downloadManager)
    {
        throw new NotImplementedException();
    }

    public void SetDecryptResourceCallback(DecryptResourceCallback decryptResourceCallback)
    {
        throw new NotImplementedException();
    }

    public void SetResourceHelper(IResourceHelper resourceHelper)
    {
        throw new NotImplementedException();
    }

    public void AddLoadResourceAgentHelper(ILoadResourceAgentHelper loadResourceAgentHelper)
    {
        throw new NotImplementedException();
    }

    public void CheckResources(bool ignoreOtherVariant, CheckResourcesCompleteCallback checkResourcesCompleteCallback)
    {
        throw new NotImplementedException();
    }

    public void ApplyResources(string resourcePackPath, ApplyResourcesCompleteCallback applyResourcesCompleteCallback)
    {
        throw new NotImplementedException();
    }

    public void UpdateResources(UpdateResourcesCompleteCallback updateResourcesCompleteCallback)
    {
        throw new NotImplementedException();
    }

    public bool VerifyResourcePack(string resourcePackPath)
    {
        throw new NotImplementedException();
    }

    public HasAssetResult HasAsset(string assetName)
    {
        throw new NotImplementedException();
    }
    /// <summary>
    /// 异步加载资源。
    /// </summary>
    /// <param name="assetName">要加载资源的名称。</param>
    /// <param name="loadAssetCallbacks">加载资源回调函数集。</param>
    public void LoadAsset(string assetName, LoadAssetCallbacks loadAssetCallbacks)
    {
        LoadAsset(assetName, null, DefaultPriority, loadAssetCallbacks, null);
    }

    /// <summary>
    /// 异步加载资源。
    /// </summary>
    /// <param name="assetName">要加载资源的名称。</param>
    /// <param name="assetType">要加载资源的类型。</param>
    /// <param name="loadAssetCallbacks">加载资源回调函数集。</param>
    public void LoadAsset(string assetName, Type assetType, LoadAssetCallbacks loadAssetCallbacks)
    {
        LoadAsset(assetName, assetType, DefaultPriority, loadAssetCallbacks, null);
    }

    /// <summary>
    /// 异步加载资源。
    /// </summary>
    /// <param name="assetName">要加载资源的名称。</param>
    /// <param name="priority">加载资源的优先级。</param>
    /// <param name="loadAssetCallbacks">加载资源回调函数集。</param>
    public void LoadAsset(string assetName, int priority, LoadAssetCallbacks loadAssetCallbacks)
    {
        LoadAsset(assetName, null, priority, loadAssetCallbacks, null);
    }

    /// <summary>
    /// 异步加载资源。
    /// </summary>
    /// <param name="assetName">要加载资源的名称。</param>
    /// <param name="loadAssetCallbacks">加载资源回调函数集。</param>
    /// <param name="userData">用户自定义数据。</param>
    public void LoadAsset(string assetName, LoadAssetCallbacks loadAssetCallbacks, object userData)
    {
        LoadAsset(assetName, null, DefaultPriority, loadAssetCallbacks, userData);
    }

    /// <summary>
    /// 异步加载资源。
    /// </summary>
    /// <param name="assetName">要加载资源的名称。</param>
    /// <param name="assetType">要加载资源的类型。</param>
    /// <param name="priority">加载资源的优先级。</param>
    /// <param name="loadAssetCallbacks">加载资源回调函数集。</param>
    public void LoadAsset(string assetName, Type assetType, int priority, LoadAssetCallbacks loadAssetCallbacks)
    {
        LoadAsset(assetName, assetType, priority, loadAssetCallbacks, null);
    }

    /// <summary>
    /// 异步加载资源。
    /// </summary>
    /// <param name="assetName">要加载资源的名称。</param>
    /// <param name="assetType">要加载资源的类型。</param>
    /// <param name="loadAssetCallbacks">加载资源回调函数集。</param>
    /// <param name="userData">用户自定义数据。</param>
    public void LoadAsset(string assetName, Type assetType, LoadAssetCallbacks loadAssetCallbacks, object userData)
    {
        LoadAsset(assetName, assetType, DefaultPriority, loadAssetCallbacks, userData);
    }

    /// <summary>
    /// 异步加载资源。
    /// </summary>
    /// <param name="assetName">要加载资源的名称。</param>
    /// <param name="priority">加载资源的优先级。</param>
    /// <param name="loadAssetCallbacks">加载资源回调函数集。</param>
    /// <param name="userData">用户自定义数据。</param>
    public void LoadAsset(string assetName, int priority, LoadAssetCallbacks loadAssetCallbacks, object userData)
    {
        LoadAsset(assetName, null, priority, loadAssetCallbacks, userData);
    }

    /// <summary>
    /// 异步加载资源。
    /// </summary>
    /// <param name="assetName">要加载资源的名称。</param>
    /// <param name="assetType">要加载资源的类型。</param>
    /// <param name="priority">加载资源的优先级。</param>
    /// <param name="loadAssetCallbacks">加载资源回调函数集。</param>
    /// <param name="userData">用户自定义数据。</param>
    public void LoadAsset(string assetName, Type assetType, int priority, LoadAssetCallbacks loadAssetCallbacks, object userData)
    {
        var startTime = DateTime.UtcNow;
        Addressables.LoadAssetAsync<UnityEngine.Object>(assetName).Completed += result => {
            float elapseSeconds = (float)(DateTime.UtcNow - startTime).TotalSeconds;
            loadAssetCallbacks.LoadAssetSuccessCallback(assetName, result.Result, elapseSeconds, userData);
        };
    }

    public void LoadScene(string sceneAssetName, LoadSceneCallbacks loadSceneCallbacks)
    {
        throw new NotImplementedException();
    }

    public void LoadScene(string sceneAssetName, int priority, LoadSceneCallbacks loadSceneCallbacks)
    {
        throw new NotImplementedException();
    }

    public void LoadScene(string sceneAssetName, LoadSceneCallbacks loadSceneCallbacks, object userData)
    {
        throw new NotImplementedException();
    }

    public void LoadScene(string sceneAssetName, int priority, LoadSceneCallbacks loadSceneCallbacks, object userData)
    {
        throw new NotImplementedException();
    }

    public void UnloadScene(string sceneAssetName, UnloadSceneCallbacks unloadSceneCallbacks)
    {
        throw new NotImplementedException();
    }

    public void UnloadScene(string sceneAssetName, UnloadSceneCallbacks unloadSceneCallbacks, object userData)
    {
        throw new NotImplementedException();
    }

    public string GetBinaryPath(string binaryAssetName)
    {
        throw new NotImplementedException();
    }

    public bool GetBinaryPath(string binaryAssetName, out bool storageInReadOnly, out bool storageInFileSystem, out string relativePath, out string fileName)
    {
        throw new NotImplementedException();
    }

    public int GetBinaryLength(string binaryAssetName)
    {
        throw new NotImplementedException();
    }

    public void LoadBinary(string binaryAssetName, LoadBinaryCallbacks loadBinaryCallbacks)
    {
        throw new NotImplementedException();
    }

    public void LoadBinary(string binaryAssetName, LoadBinaryCallbacks loadBinaryCallbacks, object userData)
    {
        throw new NotImplementedException();
    }

    public byte[] LoadBinaryFromFileSystem(string binaryAssetName)
    {
        throw new NotImplementedException();
    }

    public int LoadBinaryFromFileSystem(string binaryAssetName, byte[] buffer)
    {
        throw new NotImplementedException();
    }

    public int LoadBinaryFromFileSystem(string binaryAssetName, byte[] buffer, int startIndex)
    {
        throw new NotImplementedException();
    }

    public int LoadBinaryFromFileSystem(string binaryAssetName, byte[] buffer, int startIndex, int length)
    {
        throw new NotImplementedException();
    }

    public byte[] LoadBinarySegmentFromFileSystem(string binaryAssetName, int length)
    {
        throw new NotImplementedException();
    }

    public byte[] LoadBinarySegmentFromFileSystem(string binaryAssetName, int offset, int length)
    {
        throw new NotImplementedException();
    }

    public int LoadBinarySegmentFromFileSystem(string binaryAssetName, byte[] buffer)
    {
        throw new NotImplementedException();
    }

    public int LoadBinarySegmentFromFileSystem(string binaryAssetName, byte[] buffer, int length)
    {
        throw new NotImplementedException();
    }

    public int LoadBinarySegmentFromFileSystem(string binaryAssetName, byte[] buffer, int startIndex, int length)
    {
        throw new NotImplementedException();
    }

    public int LoadBinarySegmentFromFileSystem(string binaryAssetName, int offset, byte[] buffer)
    {
        throw new NotImplementedException();
    }

    public int LoadBinarySegmentFromFileSystem(string binaryAssetName, int offset, byte[] buffer, int length)
    {
        throw new NotImplementedException();
    }

    public int LoadBinarySegmentFromFileSystem(string binaryAssetName, int offset, byte[] buffer, int startIndex, int length)
    {
        throw new NotImplementedException();
    }

    public bool HasResourceGroup(string resourceGroupName)
    {
        throw new NotImplementedException();
    }

    public IResourceGroup GetResourceGroup()
    {
        throw new NotImplementedException();
    }

    public TaskInfo[] GetAllLoadAssetInfos()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// 获取或设置资源更新下载地址。
    /// </summary>
    public string UpdatePrefixUri
    {
        get
        {
            throw new NotSupportedException("UpdatePrefixUri");
        }
        set
        {
            throw new NotSupportedException("UpdatePrefixUri");
        }
    }


    /// <summary>
    /// 获取当前资源适用的游戏版本号。
    /// </summary>
    public string ApplicableGameVersion
    {
        get
        {
            throw new NotSupportedException("ApplicableGameVersion");
        }
    }

    /// <summary>
    /// 获取当前内部资源版本号。
    /// </summary>
    public int InternalResourceVersion
    {
        get
        {
            throw new NotSupportedException("InternalResourceVersion");
        }
    }

    public string ReadOnlyPath => throw new NotImplementedException();

    public string ReadWritePath => throw new NotImplementedException();

    public string CurrentVariant => throw new NotImplementedException();

    public PackageVersionListSerializer PackageVersionListSerializer => throw new NotImplementedException();

    public UpdatableVersionListSerializer UpdatableVersionListSerializer => throw new NotImplementedException();

    public ReadOnlyVersionListSerializer ReadOnlyVersionListSerializer => throw new NotImplementedException();

    public ReadWriteVersionListSerializer ReadWriteVersionListSerializer => throw new NotImplementedException();

    public ResourcePackVersionListSerializer ResourcePackVersionListSerializer => throw new NotImplementedException();

    public int AssetCount => throw new NotImplementedException();

    public int ResourceCount => throw new NotImplementedException();

    public int ResourceGroupCount => throw new NotImplementedException();

    public int GenerateReadWriteVersionListLength { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

    public string ApplyingResourcePackPath => throw new NotImplementedException();

    public int ApplyWaitingCount => throw new NotImplementedException();

    public int UpdateRetryCount { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

    public int UpdateWaitingCount => throw new NotImplementedException();

    public int UpdateCandidateCount => throw new NotImplementedException();

    public int UpdatingCount => throw new NotImplementedException();

    public int LoadTotalAgentCount => throw new NotImplementedException();

    public int LoadFreeAgentCount => throw new NotImplementedException();

    public int LoadWorkingAgentCount => throw new NotImplementedException();

    public int LoadWaitingTaskCount => throw new NotImplementedException();

    public float AssetAutoReleaseInterval { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public int AssetCapacity { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public float AssetExpireTime { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public int AssetPriority { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public float ResourceAutoReleaseInterval { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public int ResourceCapacity { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public float ResourceExpireTime { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public int ResourcePriority { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
}
