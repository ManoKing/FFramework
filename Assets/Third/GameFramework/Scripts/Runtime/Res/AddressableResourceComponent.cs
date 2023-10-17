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

public class AddressableResourceComponent : MonoBehaviour, IResourceManager
{
    public string ReadOnlyPath => throw new NotImplementedException();

    public string ReadWritePath => throw new NotImplementedException();

    public ResourceMode ResourceMode => throw new NotImplementedException();

    public string CurrentVariant => throw new NotImplementedException();

    public PackageVersionListSerializer PackageVersionListSerializer => throw new NotImplementedException();

    public UpdatableVersionListSerializer UpdatableVersionListSerializer => throw new NotImplementedException();

    public ReadOnlyVersionListSerializer ReadOnlyVersionListSerializer => throw new NotImplementedException();

    public ReadWriteVersionListSerializer ReadWriteVersionListSerializer => throw new NotImplementedException();

    public ResourcePackVersionListSerializer ResourcePackVersionListSerializer => throw new NotImplementedException();

    public string ApplicableGameVersion => throw new NotImplementedException();

    public int InternalResourceVersion => throw new NotImplementedException();

    public int AssetCount => throw new NotImplementedException();

    public int ResourceCount => throw new NotImplementedException();

    public int ResourceGroupCount => throw new NotImplementedException();

    public string UpdatePrefixUri { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public int GenerateReadWriteVersionListLength { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

    public string ApplyingResourcePackPath => throw new NotImplementedException();

    public int ApplyWaitingCount => throw new NotImplementedException();

    public int UpdateRetryCount { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

    public IResourceGroup UpdatingResourceGroup => throw new NotImplementedException();

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

    public event EventHandler<ResourceApplySuccessEventArgs> ResourceApplySuccess;
    public event EventHandler<ResourceApplyFailureEventArgs> ResourceApplyFailure;
    public event EventHandler<ResourceUpdateStartEventArgs> ResourceUpdateStart;
    public event EventHandler<ResourceUpdateChangedEventArgs> ResourceUpdateChanged;
    public event EventHandler<ResourceUpdateSuccessEventArgs> ResourceUpdateSuccess;
    public event EventHandler<ResourceUpdateFailureEventArgs> ResourceUpdateFailure;

    public void AddLoadResourceAgentHelper(ILoadResourceAgentHelper loadResourceAgentHelper)
    {
        throw new NotImplementedException();
    }

    public void ApplyResources(string resourcePackPath, ApplyResourcesCompleteCallback applyResourcesCompleteCallback)
    {
        throw new NotImplementedException();
    }

    public void CheckResources(bool ignoreOtherVariant, CheckResourcesCompleteCallback checkResourcesCompleteCallback)
    {
        throw new NotImplementedException();
    }

    public CheckVersionListResult CheckVersionList(int latestInternalResourceVersion)
    {
        throw new NotImplementedException();
    }

    public TaskInfo[] GetAllLoadAssetInfos()
    {
        throw new NotImplementedException();
    }

    public int GetBinaryLength(string binaryAssetName)
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

    public IResourceGroup GetResourceGroup()
    {
        throw new NotImplementedException();
    }

    public IResourceGroup GetResourceGroup(string resourceGroupName)
    {
        throw new NotImplementedException();
    }

    public HasAssetResult HasAsset(string assetName)
    {
        throw new NotImplementedException();
    }

    public bool HasResourceGroup(string resourceGroupName)
    {
        throw new NotImplementedException();
    }

    public void InitResources(InitResourcesCompleteCallback initResourcesCompleteCallback)
    {
        throw new NotImplementedException();
    }

    public void LoadAsset(string assetName, LoadAssetCallbacks loadAssetCallbacks)
    {
        throw new NotImplementedException();
    }

    public void LoadAsset(string assetName, Type assetType, LoadAssetCallbacks loadAssetCallbacks)
    {
        throw new NotImplementedException();
    }

    public void LoadAsset(string assetName, int priority, LoadAssetCallbacks loadAssetCallbacks)
    {
        throw new NotImplementedException();
    }

    public void LoadAsset(string assetName, LoadAssetCallbacks loadAssetCallbacks, object userData)
    {
        throw new NotImplementedException();
    }

    public void LoadAsset(string assetName, Type assetType, int priority, LoadAssetCallbacks loadAssetCallbacks)
    {
        throw new NotImplementedException();
    }

    public void LoadAsset(string assetName, Type assetType, LoadAssetCallbacks loadAssetCallbacks, object userData)
    {
        throw new NotImplementedException();
    }

    public void LoadAsset(string assetName, int priority, LoadAssetCallbacks loadAssetCallbacks, object userData)
    {
        var startTime = DateTime.UtcNow;
        Addressables.LoadAssetAsync<GameObject>(assetName).Completed += result => {
            float elapseSeconds = (float)(DateTime.UtcNow - startTime).TotalSeconds;
            loadAssetCallbacks.LoadAssetSuccessCallback(assetName, result, elapseSeconds, userData);
        };
    }

    public void LoadAsset(string assetName, Type assetType, int priority, LoadAssetCallbacks loadAssetCallbacks, object userData)
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

    public void SetCurrentVariant(string currentVariant)
    {
        throw new NotImplementedException();
    }

    public void SetDecryptResourceCallback(DecryptResourceCallback decryptResourceCallback)
    {
        throw new NotImplementedException();
    }

    public void SetDownloadManager(IDownloadManager downloadManager)
    {
        throw new NotImplementedException();
    }

    public void SetFileSystemManager(IFileSystemManager fileSystemManager)
    {
        throw new NotImplementedException();
    }

    public void SetObjectPoolManager(IObjectPoolManager objectPoolManager)
    {
        throw new NotImplementedException();
    }

    public void SetReadOnlyPath(string readOnlyPath)
    {
        throw new NotImplementedException();
    }

    public void SetReadWritePath(string readWritePath)
    {
        throw new NotImplementedException();
    }

    public void SetResourceHelper(IResourceHelper resourceHelper)
    {
        throw new NotImplementedException();
    }

    public void SetResourceMode(ResourceMode resourceMode)
    {
        throw new NotImplementedException();
    }

    public void UnloadAsset(object asset)
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

    public void UpdateResources(UpdateResourcesCompleteCallback updateResourcesCompleteCallback)
    {
        throw new NotImplementedException();
    }

    public void UpdateResources(string resourceGroupName, UpdateResourcesCompleteCallback updateResourcesCompleteCallback)
    {
        throw new NotImplementedException();
    }

    public void UpdateVersionList(int versionListLength, int versionListHashCode, int versionListZipLength, int versionListZipHashCode, UpdateVersionListCallbacks updateVersionListCallbacks)
    {
        throw new NotImplementedException();
    }

    public bool VerifyResourcePack(string resourcePackPath)
    {
        throw new NotImplementedException();
    }
}
