using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using YooAsset;

public static class LaunchGameFileSystemCreater
{
    public static FileSystemParameters CreateLaunchGameFileSystemParameters()
    {
        string fileSystemClass = $"{nameof(LaunchGameFileSystem)},YooAsset.RuntimeExtension";
        var fileSystemParams = new FileSystemParameters(fileSystemClass, null);
        return fileSystemParams;
    }
}


internal class LaunchGameFileSystem : IFileSystem
{
    protected string _manifestFileRoot;
    protected string _manifestCacheFileRoot;

    /// <summary>
    /// Key:资源包GUID Value:缓存路径
    /// </summary>
    private readonly Dictionary<string, string> _cacheFilePaths = new Dictionary<string, string>(10000);
    private string _fileCacheRoot = string.Empty;
    private string _saveFileRoot = string.Empty;

    /// <summary>
    /// 包裹名称
    /// </summary>
    public string PackageName { private set; get; }

    /// <summary>
    /// 文件根目录
    /// </summary>
    public string FileRoot
    {
        get
        {
            return _fileCacheRoot;
        }
    }

    /// <summary>
    /// 文件数量
    /// </summary>
    public int FileCount
    {
        get
        {
            return 0;
        }
    }

    #region 自定义参数
    /// <summary>
    /// 自定义参数：远程服务接口
    /// </summary>
    public IRemoteServices RemoteServices { private set; get; } = null;
    #endregion


    public LaunchGameFileSystem()
    {
    }

    public virtual FSLoadPackageManifestOperation LoadPackageManifestAsync(string packageVersion, int timeout)
    {
        var operation = new LGFSLoadPackageManifestOperation(this, packageVersion, timeout);
        OperationSystem.StartOperation(PackageName, operation);
        return operation;
    }

    public virtual void SetParameter(string name, object value)
    {
        if (name == "REMOTE_SERVICES")
        {
            RemoteServices = (IRemoteServices)value;
        }
        else
        {
            YooLogger.Warning($"Invalid parameter : {name}");
        }
    }
    public virtual void OnCreate(string packageName, string rootDirectory)
    {
        PackageName = packageName;

        var _packageRoot = PathUtility.Combine(Application.streamingAssetsPath, YooAssetSettingsData.Setting.DefaultYooFolderName);
        _manifestFileRoot = PathUtility.Combine(_packageRoot, packageName);

        var _manifestCacheFilePath = PathUtility.Combine(GetPersistentCachePath(), packageName);
        _manifestCacheFileRoot = PathUtility.Combine(_manifestCacheFilePath, DefaultCacheFileSystemDefine.ManifestFilesFolderName);
        _saveFileRoot = PathUtility.Combine(_manifestCacheFilePath, DefaultCacheFileSystemDefine.SaveFilesFolderName);
    }

    public string GetPackageHashFilePath(string packageVersion)
    {
        string fileName = YooAssetSettingsData.GetPackageHashFileName(PackageName, packageVersion);
        return PathUtility.Combine(_manifestFileRoot, fileName);
    }
    public string GetPackageManifestFilePath(string packageVersion)
    {
        string fileName = YooAssetSettingsData.GetManifestBinaryFileName(PackageName, packageVersion);
        return PathUtility.Combine(_manifestFileRoot, fileName);
    }

    public string GetCachePackageHashFilePath(string packageVersion)
    {
        string fileName = YooAssetSettingsData.GetPackageHashFileName(PackageName, packageVersion);
        return PathUtility.Combine(_manifestCacheFileRoot, fileName);
    }
    public string GetCachePackageManifestFilePath(string packageVersion)
    {
        string fileName = YooAssetSettingsData.GetManifestBinaryFileName(PackageName, packageVersion);
        return PathUtility.Combine(_manifestCacheFileRoot, fileName);
    }

    public string GetBuildinPackageVersionFilePath()
    {
        string fileName = YooAssetSettingsData.GetPackageVersionFileName(PackageName);
        return PathUtility.Combine(_manifestFileRoot, fileName);
    }

    /// <summary>
    /// 加载资源路径
    /// </summary>
    /// <param name="packageVersion"></param>
    /// <returns></returns>
    public string GetCacheFileLoadPath(string packageVersion)
    {
        string fileName = YooAssetSettingsData.GetManifestBinaryFileName(PackageName, packageVersion);
        return PathUtility.Combine(_manifestCacheFileRoot, fileName);
    }

    public string GetDataFilePath(PackageBundle bundle)
    {
        string folderName = bundle.FileHash.Substring(0, 2);
        var filePath = PathUtility.Combine(_saveFileRoot, folderName, bundle.BundleGUID, DefaultCacheFileSystemDefine.SaveBundleDataFileName);
        //if (AppendFileExtension)
        //    filePath += bundle.FileExtension;

        return filePath;
    }

    public string GetPersistentCachePath()
    {
#if UNITY_EDITOR
        // 注意：为了方便调试查看，编辑器下把存储目录放到项目里。
        string projectPath = Path.GetDirectoryName(UnityEngine.Application.dataPath);
        projectPath = PathUtility.RegularPath(projectPath);
        return PathUtility.Combine(projectPath, YooAssetSettingsData.Setting.DefaultYooFolderName);
#elif UNITY_STANDALONE
            return PathUtility.Combine(UnityEngine.Application.dataPath, YooAssetSettingsData.Setting.DefaultYooFolderName);
#else
            return PathUtility.Combine(UnityEngine.Application.persistentDataPath, YooAssetSettingsData.Setting.DefaultYooFolderName);	
#endif
    }
    public virtual void OnUpdate()
    {
    }

    public virtual bool Belong(PackageBundle bundle)
    {
        return true;
    }

    public virtual bool NeedDownload(PackageBundle bundle)
    {
        if (Belong(bundle) == false)
            return false;

        return Exists(bundle) == false;
    }
    public virtual bool NeedUnpack(PackageBundle bundle)
    {
        return false;
    }
    public virtual bool NeedImport(PackageBundle bundle)
    {
        return false;
    }

    public FSInitializeFileSystemOperation InitializeFileSystemAsync()
    {
        // 验证文件，TODO
        var operation = new LGFSInitializeOperation(this);
        OperationSystem.StartOperation(PackageName, operation);
        return operation;
    }

    public FSClearAllBundleFilesOperation ClearAllBundleFilesAsync()
    {
        throw new NotImplementedException();
    }

    public FSClearUnusedBundleFilesOperation ClearUnusedBundleFilesAsync(PackageManifest manifest)
    {
        throw new NotImplementedException();
    }

    public FSDownloadFileOperation DownloadFileAsync(PackageBundle bundle, DownloadParam param)
    {
        throw new NotImplementedException();
    }

    public FSLoadBundleOperation LoadBundleFile(PackageBundle bundle)
    {
        var operation = new LGFSLoadBundleOperation(this, bundle);
        OperationSystem.StartOperation(PackageName, operation);
        return operation;
    }

    public void UnloadBundleFile(PackageBundle bundle, object result)
    {
        AssetBundle assetBundle = result as AssetBundle;
        if (assetBundle == null)
            return;

        if (assetBundle != null)
            assetBundle.Unload(true);

    }

    public bool Exists(PackageBundle bundle)
    {
        return true;
    }

    public byte[] ReadFileData(PackageBundle bundle)
    {
        throw new NotImplementedException();
    }

    public string ReadFileText(PackageBundle bundle)
    {
        throw new NotImplementedException();
    }

    public FSRequestPackageVersionOperation RequestPackageVersionAsync(bool appendTimeTicks, int timeout)
    {
        var operation = new LGFSLoadPackageVersionOperation(this);
        OperationSystem.StartOperation(PackageName, operation);
        return operation;
    }
}