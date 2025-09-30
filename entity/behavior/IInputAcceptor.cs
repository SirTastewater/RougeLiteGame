using Godot;

namespace RougeLiteGame.entity.behavior;

public interface IInputAcceptor
{
    void Input(InputEvent @event);
}