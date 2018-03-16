using Godot;
using System;

public class Autoload : Node
{
	public int score = 123;

	public void GotoScene(Node source, string destinationSceneName)
	{
		source.GetTree().ChangeScene("res://Scenes/"+destinationSceneName+".tscn");
	}


}
