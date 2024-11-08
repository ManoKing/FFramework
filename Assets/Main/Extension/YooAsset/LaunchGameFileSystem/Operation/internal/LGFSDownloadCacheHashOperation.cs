using System.IO;
using UnityEngine;

namespace YooAsset
{
    internal class LGFSDownloadCacheHashOperation : AsyncOperationBase
    {
        private enum ESteps
        {
            None,
            LoadPackageHash,
            Done,
        }

        private readonly LaunchGameFileSystem _fileSystem;
        private readonly string _packageVersion;
        private ESteps _steps = ESteps.None;

        /// <summary>
        /// °ü¹ü¹þÏ£Öµ
        /// </summary>
        public string PackageHash { private set; get; }


        internal LGFSDownloadCacheHashOperation(LaunchGameFileSystem fileSystem, string packageVersion)
        {
            _fileSystem = fileSystem;
            _packageVersion = packageVersion;
        }
        internal override void InternalOnStart()
        {
            _steps = ESteps.LoadPackageHash;
        }
        internal override void InternalOnUpdate()
        {
            if (_steps == ESteps.None || _steps == ESteps.Done)
                return;

            if (_steps == ESteps.LoadPackageHash)
            {
                string filePath = _fileSystem.GetCachePackageHashFilePath(_packageVersion);
                if (File.Exists(filePath) == false)
                {
                    _steps = ESteps.Done;
                    Status = EOperationStatus.Failed;
                    Error = $"Can not found cache package hash file : {filePath}";
                    return;
                }

                PackageHash = FileUtility.ReadAllText(filePath);
                if (string.IsNullOrEmpty(PackageHash))
                {
                    _steps = ESteps.Done;
                    Status = EOperationStatus.Failed;
                    Error = $"Cache package hash file content is empty !";
                }
                else
                {
                    _steps = ESteps.Done;
                    Status = EOperationStatus.Succeed;
                }
            }
        }
    }
}