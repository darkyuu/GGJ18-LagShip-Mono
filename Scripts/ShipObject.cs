using Godot;
using System;

public class ShipObject : Area2D
{
	[Signal]
    public delegate void Hit();

    [Export]
    public PackedScene bullets;
    private CollisionShape2D collision;
    private AnimatedSprite explosionSpriteEffect;
    private Node bulletContainer;
    private AudioStreamPlayer shootSound;
    private Position2D muzzle;
    private Sprite sprite; 

    private double currentDirection = Math.PI/2;
    private Vector2 currentPosition;

    public override void _Ready()
    {
        collision = GetNode("Collision") as CollisionShape2D;
        explosionSpriteEffect = GetNode("ExplosionSpriteEffect") as AnimatedSprite;
        bulletContainer = GetNode("BulletContainer") as Node;
        shootSound = GetNode("ShootSound") as AudioStreamPlayer;
        muzzle = GetNode("Muzzle") as Position2D;
        sprite = GetNode("Sprite") as Sprite; 

        Hide();
        Vector2 screenSize = GetViewportRect().Size;
        currentPosition = new Vector2(screenSize.x/2, screenSize.y/2);
        collision.Disabled = true;
        explosionSpriteEffect.Visible = false;
        Start();
        SetProcess(false);
        //SetProcess(true); //for test in GameObject
    }

    public override void _Process(float delta)
    {
        if (Input.IsActionJustPressed("player_shoot"))
            Shoot();
        if (Input.IsActionJustPressed("ui_right"))
            TurnRight();
        if (Input.IsActionJustPressed("ui_left"))
            TurnLeft();
    }

    public void Start()
    {
        SetPosition(currentPosition);
        SetRotation((float)currentDirection);
        Show();
        collision.Disabled = false;
    }

    private void TrimDirectionInRadian()
    {
        if(currentDirection >= 2*Math.PI)
            currentDirection = 0;
        else if(currentDirection <= -2*Math.PI)
            currentDirection = 0;
    }

    public void TurnLeft()
    {
        currentDirection -= Math.PI/4;
        TrimDirectionInRadian();
        SetRotation((float)currentDirection);        
    }

    public void TurnRight()
    {
        currentDirection += Math.PI/4;
        TrimDirectionInRadian();
        SetRotation((float)currentDirection);
    }

    public void Shoot()
    {
        PlayerBulletObject bulletObj = (PlayerBulletObject)bullets.Instance();
        bulletContainer.AddChild(bulletObj);
        bulletObj.StartAt(GetRotation(), muzzle.GetGlobalPosition());
        shootSound.Play();
    }

    public void OnShipBodyEntered(Area2D area)
    {
        collision.Disabled = true;
        sprite.Hide();
        explosionSpriteEffect.Visible = true;
        explosionSpriteEffect.Play();
        this.EmitSignal("Hit");
    }

    public void OnExplosionSpriteEffectAnimationFinished()
    {
    	explosionSpriteEffect.Hide();
    }

}
