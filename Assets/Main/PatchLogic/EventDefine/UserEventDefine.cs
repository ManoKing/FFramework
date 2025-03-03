using GameFramework;
using GameFramework.Event;
using static PatchEventDefine;

public class UserEventDefine
{
    /// <summary>
    /// 用户尝试再次初始化资源包
    /// </summary>
    public class UserTryInitialize : GameEventArgs
    {
        public static readonly int EventId = typeof(UserTryInitialize).GetHashCode();
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

        public static UserTryInitialize Create()
        {
            UserTryInitialize UserTryInitializedArgs = ReferencePool.Acquire<UserTryInitialize>();
            return UserTryInitializedArgs;
        }

        public override void Clear()
        {
             
        }
    }

    /// <summary>
    /// 用户开始下载网络文件
    /// </summary>
    public class UserBeginDownloadWebFiles : GameEventArgs
    {
        public static readonly int EventId = typeof(UserBeginDownloadWebFiles).GetHashCode();
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

        public static UserBeginDownloadWebFiles Create()
        {
            UserBeginDownloadWebFiles UserBeginDownloadWebFilesArgs = ReferencePool.Acquire<UserBeginDownloadWebFiles>();
            return UserBeginDownloadWebFilesArgs;
        }

        public override void Clear()
        {
             
        }
    }

    /// <summary>
    /// 用户尝试再次更新静态版本
    /// </summary>
    public class UserTryUpdatePackageVersion : GameEventArgs
    {
        public static readonly int EventId = typeof(UserTryUpdatePackageVersion).GetHashCode();
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

        public static UserTryUpdatePackageVersion Create()
        {
            UserTryUpdatePackageVersion UserTryUpdatePackageVersionArgs = ReferencePool.Acquire<UserTryUpdatePackageVersion>();
            return UserTryUpdatePackageVersionArgs;
        }

        public override void Clear()
        {
             
        }
    }

    /// <summary>
    /// 用户尝试再次更新补丁清单
    /// </summary>
    public class UserTryUpdatePatchManifest : GameEventArgs
    {
        public static readonly int EventId = typeof(UserTryUpdatePatchManifest).GetHashCode();
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

        public static UserTryUpdatePatchManifest Create()
        {
            UserTryUpdatePatchManifest UserTryUpdatePatchManifestArgs = ReferencePool.Acquire<UserTryUpdatePatchManifest>();
            return UserTryUpdatePatchManifestArgs;
        }

        public override void Clear()
        {
             
        }
    }

    /// <summary>
    /// 用户尝试再次下载网络文件
    /// </summary>
    public class UserTryDownloadWebFiles : GameEventArgs
    {
        public static readonly int EventId = typeof(UserTryDownloadWebFiles).GetHashCode();
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

        public static UserTryDownloadWebFiles Create()
        {
            UserTryDownloadWebFiles UserTryDownloadWebFilesArgs = ReferencePool.Acquire<UserTryDownloadWebFiles>();
            return UserTryDownloadWebFilesArgs;
        }

        public override void Clear()
        {
             
        }
    }
}