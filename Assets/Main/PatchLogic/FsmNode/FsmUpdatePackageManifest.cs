using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ProcedureOwner = GameFramework.Fsm.IFsm<PatchOperation>;
using YooAsset;
using Cysharp.Threading.Tasks;
using GameFramework.Fsm;
using GameFramework;

/// <summary>
/// 更新资源清单
/// </summary>
public class FsmUpdatePackageManifest : FsmState<PatchOperation>, IReference
{
    private PatchOperation owner;
    protected override void OnEnter(ProcedureOwner procedureOwner)
    {
        base.OnEnter(procedureOwner);

        owner = procedureOwner.Owner;

        PatchEventDefine.PatchStatesChange.SendEventMessage(this, "更新资源清单！");
        UpdateManifest(procedureOwner);
    }

    private async UniTask UpdateManifest(ProcedureOwner procedureOwner)
    {
        var packageName = owner.packageName;
        var packageVersion = owner.packageVersion;
        var package = YooAssets.GetPackage(packageName);
        var operation = package.UpdatePackageManifestAsync(packageVersion);

        await UniTask.WaitUntil(() => operation.IsDone);

        if (operation.Status != EOperationStatus.Succeed)
        {
            Debug.LogWarning(operation.Error);
            PatchEventDefine.PatchManifestUpdateFailed.SendEventMessage(this);
            return;
        }
        else
        {
            ChangeState<FsmCreatePackageDownloader>(procedureOwner);
        }
    }
    public static FsmUpdatePackageManifest Create()
    {
        FsmUpdatePackageManifest state = ReferencePool.Acquire<FsmUpdatePackageManifest>();
        return state;
    }
    public void Clear()
    {
        throw new System.NotImplementedException();
    }
}