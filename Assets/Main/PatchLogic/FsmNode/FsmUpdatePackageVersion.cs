using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniFramework.Machine;
using YooAsset;
using System;
using Cysharp.Threading.Tasks;

/// <summary>
/// 更新资源版本号
/// </summary>
internal class FsmUpdatePackageVersion : IStateNode
{
    private StateMachine _machine;

    void IStateNode.OnCreate(StateMachine machine)
    {
        _machine = machine;
    }
    async void IStateNode.OnEnter()
    {
        PatchEventDefine.PatchStatesChange.SendEventMessage("获取最新的资源版本 !");
        await UpdatePackageVersion();
    }
    void IStateNode.OnUpdate()
    {
    }
    void IStateNode.OnExit()
    {
    }

    private async UniTask UpdatePackageVersion()
    {
        var packageName = (string)_machine.GetBlackboardValue("PackageName");
        var package = YooAssets.GetPackage(packageName);
        var operation = package.RequestPackageVersionAsync();

        await UniTask.WaitUntil(() => operation.IsDone);

        if (operation.Status != EOperationStatus.Succeed)
        {
            Debug.LogWarning(operation.Error);
            PatchEventDefine.PackageVersionUpdateFailed.SendEventMessage();
        }
        else
        {
            if (packageName == "Launch")
            {
                // 注意：下载完成之后再保存本地版本
                PlayerPrefs.SetString("GAME_VERSION", operation.PackageVersion);
            }
            Debug.Log($"Request package version : {operation.PackageVersion}");
            _machine.SetBlackboardValue("PackageVersion", operation.PackageVersion);
            _machine.ChangeState<FsmUpdatePackageManifest>();
        }
    }
}