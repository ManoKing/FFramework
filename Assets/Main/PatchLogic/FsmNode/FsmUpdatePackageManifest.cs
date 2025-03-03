using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniFramework.Machine;
using YooAsset;
using Cysharp.Threading.Tasks;

/// <summary>
/// 更新资源清单
/// </summary>
public class FsmUpdatePackageManifest : IStateNode
{
    private StateMachine _machine;

    void IStateNode.OnCreate(StateMachine machine)
    {
        _machine = machine;
    }
    void IStateNode.OnEnter()
    {
        PatchEventDefine.PatchStatesChange.SendEventMessage("更新资源清单！");
        UpdateManifest();
    }
    void IStateNode.OnUpdate()
    {
    }
    void IStateNode.OnExit()
    {
    }

    private async UniTask UpdateManifest()
    {
        var packageName = (string)_machine.GetBlackboardValue("PackageName");
        var packageVersion = (string)_machine.GetBlackboardValue("PackageVersion");
        var package = YooAssets.GetPackage(packageName);
        var operation = package.UpdatePackageManifestAsync(packageVersion);

        await UniTask.WaitUntil(() => operation.IsDone);

        if (operation.Status != EOperationStatus.Succeed)
        {
            Debug.LogWarning(operation.Error);
            PatchEventDefine.PatchManifestUpdateFailed.SendEventMessage();
            return;
        }
        else
        {
            _machine.ChangeState<FsmCreatePackageDownloader>();
        }
    }
}