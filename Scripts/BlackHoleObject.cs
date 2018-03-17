using Godot;
using System;

public class BlackHoleObject : AnimatedSprite
{
    public override void _Ready()
    {
        
    }

    public void OnBlackHoleAnimationFinished()
    {
        Hide();
        QueueFree();
    }
}
