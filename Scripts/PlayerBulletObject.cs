using Godot;
using System;

public class PlayerBulletObject : Area2D
{
    [Export]
    public int speed = 1000;

    private Vector2 velocity;

    public override void _Ready()
    {
        SetProcess(true);
    }

   public override void _Process(float delta)
   {
       SetPosition(GetPosition() + velocity * delta);       
   }

    public void StartAt(Double direction, Vector2 position)
    {
        SetRotation((float)direction);
        SetPosition(position);
        velocity = new Vector2(speed, 0).Rotated((float)(direction - Math.PI/2));        
    }

    public void OnPlayerBulletBodyEntered(AsteroidObject area)
    {
        if(area.IsInGroup("asteroids"))
        {
            QueueFree();
            area.ExplodeItSelf();
        }
    }
}
