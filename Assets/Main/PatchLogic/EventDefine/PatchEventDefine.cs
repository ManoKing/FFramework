using GameFramework;
using GameFramework.Event;

public class PatchEventDefine
{
    /// <summary>
    /// 补丁包初始化失败
    /// </summary>
    public class InitializeFailed : GameEventArgs
    {
        public static readonly int EventId = typeof(InitializeFailed).GetHashCode();
        public override int Id
        {
            get
            {
                return EventId;
            }
        }

        public static void SendEventMessage(object sender)
        {
            var eventManager = GameFrameworkEntry.GetModule<IEventManager>();
            eventManager.FireNow(sender, Create());
        }

        public static InitializeFailed Create()
        {
            InitializeFailed InitializeFailedArgs = ReferencePool.Acquire<InitializeFailed>();
            return InitializeFailedArgs;
        }

        public override void Clear()
        {
             
        }
    }

    /// <summary>
    /// 补丁流程步骤改变
    /// </summary>
    public class PatchStatesChange : GameEventArgs
    {
        public string Tips;

        public static readonly int EventId = typeof(PatchStatesChange).GetHashCode();
        public override int Id
        {
            get
            {
                return EventId;
            }
        }

        public static void SendEventMessage(object sender, string tips)
        {
            var eventManager = GameFrameworkEntry.GetModule<IEventManager>();
            eventManager.FireNow(sender, Create(tips));
        }

        public static PatchStatesChange Create(string tips)
        {
            PatchStatesChange PatchStatesChangeArgs = ReferencePool.Acquire<PatchStatesChange>();
            PatchStatesChangeArgs.Tips = tips;
            return PatchStatesChangeArgs;
        }

        public override void Clear()
        {
            
        }
    }

    /// <summary>
    /// 发现更新文件
    /// </summary>
    public class FoundUpdateFiles : GameEventArgs
    {
        public int TotalCount;
        public long TotalSizeBytes;

        public static readonly int EventId = typeof(FoundUpdateFiles).GetHashCode();
        public override int Id
        {
            get
            {
                return EventId;
            }
        }

        public static void SendEventMessage(object sender, int totalCount, long totalSizeBytes)
        {
            var eventManager = GameFrameworkEntry.GetModule<IEventManager>();
            eventManager.FireNow(sender, Create(totalCount, totalSizeBytes));
        }

        public static FoundUpdateFiles Create(int totalCount, long totalSizeBytes)
        {
            FoundUpdateFiles FoundUpdateFilesArgs = ReferencePool.Acquire<FoundUpdateFiles>();
            FoundUpdateFilesArgs.TotalCount = totalCount;
            FoundUpdateFilesArgs.TotalSizeBytes = totalSizeBytes;
            return FoundUpdateFilesArgs;
        }


        public override void Clear()
        {
             
        }
    }

    /// <summary>
    /// 下载进度更新
    /// </summary>
    public class DownloadProgressUpdate : GameEventArgs
    {
        public int TotalDownloadCount;
        public int CurrentDownloadCount;
        public long TotalDownloadSizeBytes;
        public long CurrentDownloadSizeBytes;

        public static readonly int EventId = typeof(DownloadProgressUpdate).GetHashCode();
        public override int Id
        {
            get
            {
                return EventId;
            }
        }

        public static void SendEventMessage(object sender, int totalDownloadCount, int currentDownloadCount, long totalDownloadSizeBytes, long currentDownloadSizeBytes)
        {
            var eventManager = GameFrameworkEntry.GetModule<IEventManager>();
            eventManager.FireNow(sender, Create(totalDownloadCount, currentDownloadCount, totalDownloadSizeBytes, currentDownloadSizeBytes));
        }

        public static DownloadProgressUpdate Create(int totalDownloadCount, int currentDownloadCount, long totalDownloadSizeBytes, long currentDownloadSizeBytes)
        {
            DownloadProgressUpdate DownloadProgressUpdateArgs = ReferencePool.Acquire<DownloadProgressUpdate>();
            DownloadProgressUpdateArgs.TotalDownloadCount = totalDownloadCount;
            DownloadProgressUpdateArgs.CurrentDownloadCount = currentDownloadCount;
            DownloadProgressUpdateArgs.TotalDownloadSizeBytes = totalDownloadSizeBytes;
            DownloadProgressUpdateArgs.CurrentDownloadSizeBytes = currentDownloadSizeBytes;
            return DownloadProgressUpdateArgs;
        }

        public override void Clear()
        {
             
        }
    }

    /// <summary>
    /// 资源版本号更新失败
    /// </summary>
    public class PackageVersionUpdateFailed : GameEventArgs
    {
        public static readonly int EventId = typeof(PackageVersionUpdateFailed).GetHashCode();
        public override int Id
        {
            get
            {
                return EventId;
            }
        }

        public static void SendEventMessage(object sender)
        {
            var eventManager = GameFrameworkEntry.GetModule<IEventManager>();
            eventManager.FireNow(sender, Create());
        }

        public static PackageVersionUpdateFailed Create()
        {
            PackageVersionUpdateFailed PackageVersionUpdateFailedArgs = ReferencePool.Acquire<PackageVersionUpdateFailed>();
            return PackageVersionUpdateFailedArgs;
        }

        public override void Clear()
        {
             
        }
    }

    /// <summary>
    /// 补丁清单更新失败
    /// </summary>
    public class PatchManifestUpdateFailed : GameEventArgs
    {
        public static readonly int EventId = typeof(PatchManifestUpdateFailed).GetHashCode();
        public override int Id
        {
            get
            {
                return EventId;
            }
        }

        public static void SendEventMessage(object sender)
        {
            var eventManager = GameFrameworkEntry.GetModule<IEventManager>();
            eventManager.FireNow(sender, Create());
        }

        public static PatchManifestUpdateFailed Create()
        {
            PatchManifestUpdateFailed PatchManifestUpdateFailedArgs = ReferencePool.Acquire<PatchManifestUpdateFailed>();
            return PatchManifestUpdateFailedArgs;
        }

        public override void Clear()
        {
             
        }
    }

    /// <summary>
    /// 网络文件下载失败
    /// </summary>
    public class WebFileDownloadFailed : GameEventArgs
    {
        public string FileName;
        public string Error;

        public static readonly int EventId = typeof(WebFileDownloadFailed).GetHashCode();
        public override int Id
        {
            get
            {
                return EventId;
            }
        }

        public static void SendEventMessage(object sender, string fileName, string error)
        {
            var eventManager = GameFrameworkEntry.GetModule<IEventManager>();
            eventManager.FireNow(sender, Create(fileName, error));
        }

        public static WebFileDownloadFailed Create(string fileName, string error)
        {
            WebFileDownloadFailed WebFileDownloadFailedArgs = ReferencePool.Acquire<WebFileDownloadFailed>();
            WebFileDownloadFailedArgs.FileName = fileName;
            WebFileDownloadFailedArgs.Error = error;
            return WebFileDownloadFailedArgs;
        }

        public override void Clear()
        {
             
        }
    }
}