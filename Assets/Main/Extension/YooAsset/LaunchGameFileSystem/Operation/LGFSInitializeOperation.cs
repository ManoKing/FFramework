
using YooAsset;

internal partial class LGFSInitializeOperation : FSInitializeFileSystemOperation
{
    private readonly LaunchGameFileSystem _fileSystem;

    public LGFSInitializeOperation(LaunchGameFileSystem fileSystem)
    {
        _fileSystem = fileSystem;
    }
    internal override void InternalOnStart()
    {
        Status = EOperationStatus.Succeed;
    }
    internal override void InternalOnUpdate()
    {
    }
}
