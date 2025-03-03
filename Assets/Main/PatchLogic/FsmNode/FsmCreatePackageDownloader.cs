using GameFramework;
using GameFramework.Fsm;
using UnityEngine;
using YooAsset;
using ProcedureOwner = GameFramework.Fsm.IFsm<PatchOperation>;

/// <summary>
/// 创建文件下载器
/// </summary>
public class FsmCreatePackageDownloader : FsmState<PatchOperation>, IReference
{
    private PatchOperation owner;
    protected override void OnEnter(ProcedureOwner procedureOwner)
    {
        base.OnEnter(procedureOwner);

        owner = procedureOwner.Owner;

        PatchEventDefine.PatchStatesChange.SendEventMessage(this, "创建补丁下载器！");
        CreateDownloader(procedureOwner);
    }
    void CreateDownloader(ProcedureOwner procedureOwner)
    {
        var packageName = owner.packageName;
        var package = YooAssets.GetPackage(packageName);
        int downloadingMaxNum = 10;
        int failedTryAgain = 3;
        var downloader = package.CreateResourceDownloader(downloadingMaxNum, failedTryAgain);
        owner.resourceDownloaderOperation = downloader;

        if (downloader.TotalDownloadCount == 0)
        {
            Debug.Log("Not found any download files !");
            ChangeState<FsmUpdaterDone>(procedureOwner);
        }
        else
        {
            // 发现新更新文件后，挂起流程系统
            // 注意：开发者需要在下载前检测磁盘空间不足
            //int totalDownloadCount = downloader.TotalDownloadCount;
            //long totalDownloadBytes = downloader.TotalDownloadBytes;
            //PatchEventDefine.FoundUpdateFiles.SendEventMessage(totalDownloadCount, totalDownloadBytes);

            ChangeState<FsmDownloadPackageFiles>(procedureOwner);
        }
    }
    public static FsmCreatePackageDownloader Create()
    {
        FsmCreatePackageDownloader state = ReferencePool.Acquire<FsmCreatePackageDownloader>();
        return state;
    }
    public void Clear()
    {
        throw new System.NotImplementedException();
    }
}