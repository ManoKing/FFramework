using GameFramework;
using GameFramework.Event;
using UniFramework.Machine;
using YooAsset;

public class PatchOperation : GameAsyncOperation
{
    private enum ESteps
    {
        None,
        Update,
        Done,
    }

    private IEventManager eventManager = GameFrameworkEntry.GetModule<IEventManager>();
    private readonly StateMachine _machine;
    private ESteps _steps = ESteps.None;

    public PatchOperation(string packageName, string buildPipeline, EPlayMode playMode)
    {
        // 注册监听事件
        eventManager.Subscribe(UserEventDefine.UserTryInitialize.EventId, EventMessage);
        eventManager.Subscribe(UserEventDefine.UserBeginDownloadWebFiles.EventId, EventMessage);
        eventManager.Subscribe(UserEventDefine.UserTryUpdatePackageVersion.EventId, EventMessage);
        eventManager.Subscribe(UserEventDefine.UserTryUpdatePatchManifest.EventId, EventMessage);
        eventManager.Subscribe(UserEventDefine.UserTryDownloadWebFiles.EventId, EventMessage);

        // 创建状态机
        _machine = new StateMachine(this);
        _machine.AddNode<FsmInitializePackage>();
        _machine.AddNode<FsmUpdatePackageVersion>();
        _machine.AddNode<FsmUpdatePackageManifest>();
        _machine.AddNode<FsmCreatePackageDownloader>();
        _machine.AddNode<FsmDownloadPackageFiles>();
        _machine.AddNode<FsmDownloadPackageOver>();
        _machine.AddNode<FsmClearPackageCache>();
        _machine.AddNode<FsmUpdaterDone>();

        _machine.SetBlackboardValue("PackageName", packageName);
        _machine.SetBlackboardValue("PlayMode", playMode);
        _machine.SetBlackboardValue("BuildPipeline", buildPipeline);
    }

    protected override void OnStart()
    {
        _steps = ESteps.Update;
        _machine.Run<FsmInitializePackage>();
    }
    protected override void OnUpdate()
    {
        if (_steps == ESteps.None || _steps == ESteps.Done)
            return;

        if (_steps == ESteps.Update)
        {
            _machine.Update();
            if (_machine.CurrentNode == typeof(FsmUpdaterDone).FullName)
            {
                //_eventGroup.RemoveAllListener();
                Status = EOperationStatus.Succeed;
                _steps = ESteps.Done;
            }
        }
    }
    protected override void OnAbort()
    {
    }

    private void EventMessage(object sender, GameEventArgs args)
    {
        if (args is UserEventDefine.UserTryInitialize)
        {
            _machine.ChangeState<FsmInitializePackage>();
        }
        else if (args is UserEventDefine.UserBeginDownloadWebFiles)
        {
            _machine.ChangeState<FsmDownloadPackageFiles>();
        }
        else if (args is UserEventDefine.UserTryUpdatePackageVersion)
        {
            _machine.ChangeState<FsmUpdatePackageVersion>();
        }
        else if (args is UserEventDefine.UserTryUpdatePatchManifest)
        {
            _machine.ChangeState<FsmUpdatePackageManifest>();
        }
        else if (args is UserEventDefine.UserTryDownloadWebFiles)
        {
            _machine.ChangeState<FsmCreatePackageDownloader>();
        }
        else
        {
            throw new System.NotImplementedException($"{args.GetType()}");
        }
    }
}