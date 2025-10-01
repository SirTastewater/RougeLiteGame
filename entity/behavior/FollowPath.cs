using Godot;

namespace RougeLiteGame.entity.behavior;

[GlobalClass] public partial class FollowPath : IdleBehavior
{
    
    [Export(PropertyHint.NodePathValidTypes, "PathFollow3D")] private NodePath _path;
    private PathFollow3D _path3D;
    [Export(PropertyHint.Range, "1,30")]
    private float _samplePathPointDistance = 5;

    protected override void Init()
    {
        _path3D = GetNode<PathFollow3D>(_path);
        SetTargetPosition(_path3D.GlobalPosition);
        base.Init();
    }

    public override Vector3 Process(double delta)
    {
        if (!IsFacingTarget())
        {
            RotateTowards(delta);
        }
        
        if (!Controller.IsNavigationFinished()) return FollowControllerTarget();
        
        _path3D.Progress += _samplePathPointDistance;
        Vector3 targetPosition = _path3D.GlobalPosition;
        SetTargetPosition(targetPosition);

        return FollowControllerTarget();
    }

    public override bool IsSneaking()
    {
        return !IsFacingTarget();
    }
}