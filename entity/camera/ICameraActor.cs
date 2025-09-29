using Godot;

namespace RougeLiteGame.entity.camera;

/// <summary>
/// Defines an interface for objects that can interact with a camera controller.
/// </summary>
/// <remarks>
/// Implementations of this interface are expected to define how camera orientation is
/// applied and to provide information about their current yaw and pitch values. This
/// interface is primarily used to allow camera systems to interact with entities in the game.
/// </remarks>
public interface ICameraActor
{
    /// <summary>
    /// Applies the given camera orientation (yaw and pitch) to an entity or actor
    /// implementing the ICameraActor interface. Updates the entity's rotation based
    /// on the provided yaw value, optionally skipping the update based on internal conditions.
    /// </summary>
    /// <param name="camera">The camera node to use for orientation logic.</param>   
    /// <param name="yaw">The yaw angle in radians to apply to the entity's orientation.</param>
    /// <param name="pitch">The pitch angle in radians to be used for the orientation logic.</param>
    void CameraProcess(Node3D camera, float yaw, float pitch);
}