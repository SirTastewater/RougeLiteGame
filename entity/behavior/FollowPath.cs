using Godot;

namespace RougeLiteGame.entity.behavior;

[GlobalClass] public partial class FollowPath : NavigationIdleBehavior
{
    
    [Export(PropertyHint.NodePathValidTypes, "PathFollow3D")] private NodePath _path;
    private PathFollow3D _path3D;
    [Export(PropertyHint.Range, "1,30")]
    private float _samplePathPointDistance = 5;

    private Path3D _path33;

    protected override void Init()
    {
        _path3D = GetNode<PathFollow3D>(_path);
        SetTargetPosition(_path3D.GlobalPosition);
    }

    protected override Vector3 GetNextPosition()
    {
        // TODO instead of sampling the path by a path follow node,maybe use the points of the path directly
        // Instead of using the path follow node we could use the points from the path directly.
        // It would reduce the computation as it doesn't always set's a new target position. 
        // However, it makes the path not as dynamic as sampling points. I still need to think about more
        // positives. Let's discuss this later.
        
        _path3D.Progress += _samplePathPointDistance;
        return _path3D.GlobalPosition;
    }

    public override bool IsSneaking()
    {
        return !IsFacingTarget();
    }
}