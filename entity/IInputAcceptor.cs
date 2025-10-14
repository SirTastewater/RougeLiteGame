using Godot;
using RougeLiteGame.entity.behavior;

namespace RougeLiteGame.entity;

/// <summary>
/// Defines a contract for classes that handle input events, typically for entities. Provides a mechanism to accept and
/// process input events such as keyboard and mouse actions.
/// </summary>
public interface IInputAcceptor
{
    /// <summary>
    /// Processes and handles player input events. Enables interaction with input
    /// devices like mouse and keyboard, modifies behaviors <see cref="Behavior"/> based on input events.
    /// </summary>
    /// <param name="event">The input event to process, such as mouse movements or key presses.</param>
    void AcceptInput(InputEvent @event);
}