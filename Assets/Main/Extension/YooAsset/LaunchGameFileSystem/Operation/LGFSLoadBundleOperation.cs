using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using YooAsset;

internal class LGFSLoadBundleOperation : FSLoadBundleOperation
{
    private enum ESteps
    {
        None,
        LoadBundleFile,
        Done,
    }

    private readonly LaunchGameFileSystem _fileSystem;
    private readonly PackageBundle _bundle;
    private UnityWebRequest _webRequest;
    private ESteps _steps = ESteps.None;

    internal LGFSLoadBundleOperation(LaunchGameFileSystem fileSystem, PackageBundle bundle)
    {
        _fileSystem = fileSystem;
        _bundle = bundle;
    }
    internal override void InternalOnStart()
    {
        _steps = ESteps.LoadBundleFile;
    }
    internal override void InternalOnUpdate()
    {
        if (_steps == ESteps.None || _steps == ESteps.Done)
            return;

        if (_steps == ESteps.LoadBundleFile)
        {
            if (_bundle.Encrypted)
            {
                //Result = _fileSystem.LoadEncryptedAssetBundle(_bundle);
                Debug.LogError("Encrypted TODO");
            }
            else
            {
                string filePath = _fileSystem.GetDataFilePath(_bundle);
                if (!File.Exists(filePath))
                {
                    filePath = _fileSystem.GetCacheFileLoadPath(_bundle.BundleGUID);
                    if (!File.Exists(filePath))
                    {
                        _steps = ESteps.Done;
                        Status = EOperationStatus.Failed;
                        return;
                    }
                }

                Result = AssetBundle.LoadFromFile(filePath);
                _steps = ESteps.Done;
                Status = EOperationStatus.Succeed;
            }
        }
    }
    internal override void InternalWaitForAsyncComplete()
    {
        if (_steps != ESteps.Done)
        {
            _steps = ESteps.Done;
            Status = EOperationStatus.Failed;
            Error = "WebGL platform not support sync load method !";
            UnityEngine.Debug.LogError(Error);
        }
    }
    public override void AbortDownloadOperation()
    {
    }

   
}