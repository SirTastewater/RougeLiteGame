using Godot;
using RougeLiteGame.entity.behavior.reaction;
using RougeLiteGame.entity.player;
using RougeLiteGame.logger;

namespace RougeLiteGame.entity.behavior.idle;

[GlobalClass] public partial class FollowPath : NavigationIdleBehavior
{
    private static readonly ILogger Logger = LoggerFactory.GetLogger<FollowPath>();
    
    [Export(PropertyHint.NodePathValidTypes, "PathFollow3D")] private NodePath _path;
    [Export(PropertyHint.NodePathValidTypes, "Player")] private NodePath player;
    private PathFollow3D _path3D;
    
    [Export(PropertyHint.Range, "0.1,30")]
    private float _samplePathPointDistance = 1; // The higher this value, the more the path is ignored, but the target position is updated less frequently.
    
    protected override void Init()
    {
        _path3D = GetNode<PathFollow3D>(_path);
        SetTargetPosition(_path3D.GlobalPosition);
    }

    protected override Vector3 GetNextPosition()
    {
        // turns out, calculating a new target position is not computaional expensive
        // "afaik it is threaded, so it should be pretty cheap." - nancok
        
        _path3D.Progress += _samplePathPointDistance;
        Logger.Info("Progress: {} ", _path3D.Progress);
        if (_path3D.Progress > 20)
        {
            Controller.SetBehavior(new ChaseBehaviour(GetNode<Player>(player)));
            return Vector3.Zero;
        }
        return _path3D.GlobalPosition;
    }

    public override bool IsSneaking()
    {
        return !IsFacingTarget();
    }
}