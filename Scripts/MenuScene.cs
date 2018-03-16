using Godot;
using System;

public class MenuScene : Node
{
	Autoload globals;

	private AudioStreamPlayer bgm;
	private MenuHUD hud;

    public override void _Ready()
    {
		this.globals = (Autoload)GetNode("/root/Autoload");

		bgm = GetNode("BGM") as AudioStreamPlayer;
		hud = GetNode("HUD") as MenuHUD;
		
		hud.Connect("StartGame",this, "NewGame");
		
		SetProcess(false);
        bgm.Play();
    }

    public override void _Process(float delta)
    {
		
    }
	
	public void NewGame()
    {
		bgm.Stop();
		this.globals.GotoScene(this, "BlankScene");
	}
}
