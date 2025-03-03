using System.Collections;
using UnityEngine;
using UniFramework.Machine;
using YooAsset;
using Cysharp.Threading.Tasks;
using static PatchEventDefine;

/// <summary>
/// 下载更新文件
/// </summary>
public class FsmDownloadPackageFiles : IStateNode
{
    private StateMachine _machine;

    void IStateNode.OnCreate(StateMachine machine)
    {
        _machine = machine;
    }
    async void IStateNode.OnEnter()
    {
        PatchEventDefine.PatchStatesChange.SendEventMessage(this, "开始下载补丁文件！");
        await BeginDownload();
    }
    void IStateNode.OnUpdate()
    {
    }
    void IStateNode.OnExit()
    {
    }

    private async UniTask BeginDownload()
    {
        var downloader = (ResourceDownloaderOperation)_machine.GetBlackboardValue("Downloader");
        downloader.OnDownloadErrorCallback = WebFileDownloadFailed;
        downloader.OnDownloadProgressCallback = DownloadProgressUpdate;
        downloader.BeginDownload();

        await UniTask.WaitUntil(() => downloader.IsDone);

        // 检测下载结果
        if (downloader.Status != EOperationStatus.Succeed)
            return;

        _machine.ChangeState<FsmDownloadPackageOver>();
    }

    public void DownloadProgressUpdate(int totalDownloadCount, int currentDownloadCount, long totalDownloadSizeBytes, long currentDownloadSizeBytes)
    {
        PatchEventDefine.DownloadProgressUpdate.SendEventMessage(this, totalDownloadCount, currentDownloadCount, totalDownloadSizeBytes,  currentDownloadSizeBytes);
    }

    public void WebFileDownloadFailed(string fileName, string error)
    {
        PatchEventDefine.WebFileDownloadFailed.SendEventMessage(this, fileName, error);
    }
}