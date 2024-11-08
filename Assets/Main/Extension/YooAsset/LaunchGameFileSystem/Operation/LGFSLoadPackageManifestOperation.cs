using UnityEngine;
using YooAsset;

internal class LGFSLoadPackageManifestOperation : FSLoadPackageManifestOperation
{
    private enum ESteps
    {
        None,
        RequestRemotePackageHash,
        LoadRemotePackageManifest,
        RequestCachePackageHash,
        LoadCachePackageManifest,
        Done,
    }

    private readonly LaunchGameFileSystem _fileSystem;
    private readonly string _packageVersion;
    private readonly int _timeout;
    private LGFSDownloadPackageHashOperation _requestRemotePackageHashOp;
    private LGFSDownloadPackageManifestOperation _loadRemotePackageManifestOp;
    private LGFSDownloadCacheHashOperation _requestCachePackageHashOp;
    private LGFSDownloadCacheManifestOperation _loadCachePackageManifestOp;
    private ESteps _steps = ESteps.None;

    
    public LGFSLoadPackageManifestOperation(LaunchGameFileSystem fileSystem, string packageVersion, int timeout)
    {
        _fileSystem = fileSystem;
        _packageVersion = packageVersion;
        _timeout = timeout;
    }
    internal override void InternalOnStart()
    {
        _steps = ESteps.RequestCachePackageHash;
    }
    internal override void InternalOnUpdate()
    {
        if (_steps == ESteps.None || _steps == ESteps.Done)
            return;

        if (_steps == ESteps.RequestCachePackageHash)
        {
            if (_requestCachePackageHashOp == null)
            {
                _requestCachePackageHashOp = new LGFSDownloadCacheHashOperation(_fileSystem, _packageVersion);
                OperationSystem.StartOperation(_fileSystem.PackageName, _requestCachePackageHashOp);
            }

            if (_requestCachePackageHashOp.IsDone == false)
                return;

            if (_requestCachePackageHashOp.Status == EOperationStatus.Succeed)
            {
                // 加载缓存hash成功
                _steps = ESteps.LoadCachePackageManifest;
            }
            else
            {
                // 加载缓存失败，去下载本地
                _steps = ESteps.RequestRemotePackageHash;
            }
        }

        if (_steps == ESteps.LoadCachePackageManifest)
        {
            if (_loadCachePackageManifestOp == null)
            {
                _loadCachePackageManifestOp = new LGFSDownloadCacheManifestOperation(_fileSystem, _packageVersion, _requestCachePackageHashOp.PackageHash);
                OperationSystem.StartOperation(_fileSystem.PackageName, _loadCachePackageManifestOp);
            }

            if (_loadCachePackageManifestOp.IsDone == false)
                return;

            if (_loadCachePackageManifestOp.Status == EOperationStatus.Succeed)
            {
                // 加载缓存成功
                _steps = ESteps.Done;
                Manifest = _loadCachePackageManifestOp.Manifest;
                Status = EOperationStatus.Succeed;
            }
            else
            {
                // 加载缓存失败，去下载本地
                _steps = ESteps.RequestRemotePackageHash;
            }
        }

        if (_steps == ESteps.RequestRemotePackageHash)
        {
            if (_requestRemotePackageHashOp == null)
            {
                _requestRemotePackageHashOp = new LGFSDownloadPackageHashOperation(_fileSystem, _packageVersion);
                OperationSystem.StartOperation(_fileSystem.PackageName, _requestRemotePackageHashOp);
            }

            if (_requestRemotePackageHashOp.IsDone == false)
                return;

            if (_requestRemotePackageHashOp.Status == EOperationStatus.Succeed)
            {
                _steps = ESteps.LoadRemotePackageManifest;
            }
            else
            {
                _steps = ESteps.Done;
                Status = EOperationStatus.Failed;
                Error = _requestRemotePackageHashOp.Error;
            }
        }

        if (_steps == ESteps.LoadRemotePackageManifest)
        {
            if (_loadRemotePackageManifestOp == null)
            {
                string packageHash = _requestRemotePackageHashOp.PackageHash;
                _loadRemotePackageManifestOp = new LGFSDownloadPackageManifestOperation(_fileSystem, _packageVersion, packageHash);
                OperationSystem.StartOperation(_fileSystem.PackageName, _loadRemotePackageManifestOp);
            }

            Progress = _loadRemotePackageManifestOp.Progress;
            if (_loadRemotePackageManifestOp.IsDone == false)
                return;

            if (_loadRemotePackageManifestOp.Status == EOperationStatus.Succeed)
            {
                _steps = ESteps.Done;
                Manifest = _loadRemotePackageManifestOp.Manifest;
                Status = EOperationStatus.Succeed;
            }
            else
            {
                _steps = ESteps.Done;
                Status = EOperationStatus.Failed;
                Error = _loadRemotePackageManifestOp.Error;
            }
        }
    }
}