﻿using GameFramework;
using GameFramework.Fsm;
using ProcedureOwner = GameFramework.Fsm.IFsm<PatchOperation>;

/// <summary>
/// 下载完毕
/// </summary>
internal class FsmDownloadPackageOver : FsmState<PatchOperation>, IReference
{
    private PatchOperation owner;

    protected override void OnEnter(ProcedureOwner procedureOwner)
    {
        base.OnEnter(procedureOwner);

        owner = procedureOwner.Owner;

        ChangeState<FsmClearPackageCache>(procedureOwner);
    }
    public static FsmDownloadPackageOver Create()
    {
        FsmDownloadPackageOver state = ReferencePool.Acquire<FsmDownloadPackageOver>();
        return state;
    }
    public void Clear()
    {
        throw new System.NotImplementedException();
    }
}