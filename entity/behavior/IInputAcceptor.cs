using Godot;

namespace RougeLiteGame.entity.behavior;

public interface IInputAcceptor
{
    void AcceptInput(InputEvent @event);
}