using Godot;
using System;

public class AsteroidObject : RigidBody2D
{	
    Autoload globals;

    private AnimatedSprite explosionSpriteEffect;
    private Sprite sprite;
    private CollisionShape2D collision;
    private AudioStreamPlayer explosionSound;

    private Vector2 velocity;
    private int rotationSpeed;
    private Vector2 extents;

    public override void _Ready()
    {
        this.globals = (Autoload)GetNode("/root/Autoload");

        explosionSpriteEffect = GetNode("ExplosionSpriteEffect") as AnimatedSprite;
        sprite = GetNode("Sprite") as Sprite;
        collision = GetNode("Collision") as CollisionShape2D;
        explosionSound = GetNode("ExplosionSound") as AudioStreamPlayer;

        AddToGroup("asteroids");
        explosionSpriteEffect.Visible = false;
    }

    public override void _Process(float delta)
    {
        if(this.globals.paused)
            this.Sleeping = true;
    }

    public void ExplodeItSelf()
    {
        sprite.Visible = false;
        collision.Disabled = true;
        this.Sleeping = true;

        explosionSpriteEffect.Visible = true;
        explosionSpriteEffect.Play();
        explosionSound.Play();
   }

   public void OnExplosionSpriteEffectAnimationFinished()
   {
       QueueFree();
   }
}
