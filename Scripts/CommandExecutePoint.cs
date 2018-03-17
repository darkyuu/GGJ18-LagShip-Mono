using Godot;
using System;

public class CommandExecutePoint : Area2D
{
	[Signal]
    public delegate void CallRight();
    [Signal]
    public delegate void CallLeft();
    [Signal]
    public delegate void CallShoot();

    Autoload globals;

    public override void _Ready()
    {
		this.globals = (Autoload)GetNode("/root/Autoload");

        AddToGroup("commands");
        SetProcess(true);
    }

   public override void _Process(float delta)
   {

   }

    public void OnCommandExecutePointBodyEntered(RigidBody2D body)
    {
        if(body.GetName().Find("TurnRightCommand") != -1)
            this.EmitSignal("CallRight");
        else if(body.GetName().Find("TurnLeftCommand") != -1)
            this.EmitSignal("CallLeft");
        else 
            this.EmitSignal("CallShoot");

        this.globals.currentCommandBuffer -= 1;
        body.QueueFree();
    }
}
