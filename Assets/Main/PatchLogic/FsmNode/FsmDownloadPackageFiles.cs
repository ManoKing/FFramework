using System.Collections;
using UnityEngine;
using UniFramework.Machine;
using YooAsset;
using Cysharp.Threading.Tasks;

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
        PatchEventDefine.PatchStatesChange.SendEventMessage("开始下载补丁文件！");
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
        downloader.OnDownloadErrorCallback = PatchEventDefine.WebFileDownloadFailed.SendEventMessage;
        downloader.OnDownloadProgressCallback = PatchEventDefine.DownloadProgressUpdate.SendEventMessage;
        downloader.BeginDownload();

        await UniTask.WaitUntil(() => downloader.IsDone);

        // 检测下载结果
        if (downloader.Status != EOperationStatus.Succeed)
            return;

        _machine.ChangeState<FsmDownloadPackageOver>();
    }
}