using Godot;

namespace RougeLiteGame.entity.behavior.idle;

[GlobalClass] public partial class FollowPath : NavigationIdleBehavior
{
    [Export(PropertyHint.NodePathValidTypes, "PathFollow3D")] private NodePath _path;
    private PathFollow3D _path3D;
    
    [Export(PropertyHint.Range, "1,30")]
    private float _samplePathPointDistance = 1; // The higher this value, the more the path is ignored, but the target position is updated less frequently.
    
    protected override void Init()
    {
        _path3D = GetNode<PathFollow3D>(_path);
        SetTargetPosition(_path3D.GlobalPosition);
    }

    protected override Vector3 GetNextPosition()
    {
        // turns out, calculating a new target position is computaional not expensive
        // "afaik it is threaded, so it should be pretty cheap." - nancok
        
        _path3D.Progress += _samplePathPointDistance;
        return _path3D.GlobalPosition;
    }

    public override bool IsSneaking()
    {
        return !IsFacingTarget();
    }
}