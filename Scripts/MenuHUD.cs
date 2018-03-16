using Godot;
using System;

public class MenuHUD : CanvasLayer
{
	[Signal]
    public delegate void StartGame();

    public override void _Ready()
    {

    }
	
	public void OnStartButtonPressed()
	{
		this.EmitSignal("StartGame");
	}
}









