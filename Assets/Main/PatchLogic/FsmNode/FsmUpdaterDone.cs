using ProcedureOwner = GameFramework.Fsm.IFsm<PatchOperation>;
using GameFramework.Fsm;
using GameFramework;

/// <summary>
/// 流程更新完毕
/// </summary>
internal class FsmUpdaterDone : FsmState<PatchOperation>, IReference
{
    private PatchOperation owner;
    protected override void OnEnter(ProcedureOwner procedureOwner)
    {
        base.OnEnter(procedureOwner);
        owner = procedureOwner.Owner;
        owner.SetStatus();
    }

    public static FsmUpdaterDone Create()
    {
        FsmUpdaterDone state = ReferencePool.Acquire<FsmUpdaterDone>();
        return state;
    }

    public void Clear()
    {
        throw new System.NotImplementedException();
    }
}