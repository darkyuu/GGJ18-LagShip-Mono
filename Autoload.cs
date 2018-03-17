using Godot;
using System;
using System.Collections.Generic;

public class Autoload : Node
{
	public int score = 123;
	public bool paused = false;
	public int currentCommandBuffer = 0;
	public String playerState = "";
	public int level = 0;
	public Random randomGenerator = null;

	public float[] commandVelocity = new float[]{
		1600, 1600, 1400, 1400, 1000, 1000, 800, 800, 800, 800
	};

	public float commandMinimumVelocity = 300;

	public float[] commandLatencyFactor = new float[]{
		0.5f, 1.0f, 1.5f, 2.0f, 2.5f, 3.0f, 3.5f, 4.0f, 4.5f, 5.0f
	};

	public float[] levelSpawnTime = new float[]{
		3, 3, 3, 2.5f, 2.5f, 2.5f, 2, 2, 2, 2
	};

	public float[] asteroidInitialVelocity = new float[]{
		50, 50, 50, 
		70, 70,  
		30, 30, 30
	};

	public double[] aimToShipRotation = new double[]{
		Math.PI/4, Math.PI/2, 3*Math.PI/4,
		0, Math.PI,
		7*Math.PI/4, 3*Math.PI/2, 5*Math.PI/4
	};

	public void GotoScene(Node source, string destinationSceneName)
	{
		source.GetTree().ChangeScene("res://Scenes/"+destinationSceneName+".tscn");
	}

	public void Randomize()
	{
		if(randomGenerator == null)
			randomGenerator = new Random();
	}

	public float GetCommandVelocityForCurrentLevel()
	{
		return commandVelocity[GetLevelForUseAsIndex()-1];
	}

	public float GetCommandLatencyFactorForCurrentLevel()
	{
		return commandLatencyFactor[GetLevelForUseAsIndex()-1];
	}

	public float GetLevelSpawnTimeForCurrentLevel()
	{
		return levelSpawnTime[GetLevelForUseAsIndex()-1];
	}

	public int GetLevelForUseAsIndex()
	{
		if(level > 10)
			return 10;
		else
			return level;
	}

	public List<int> GenerateSpawnPattern()
	{
		List<int> result = new List<int>();
		int counter = 0;
		int maxCounter = randomGenerator.Next() % 2 + 3;

		if (level==1 || level ==2)
			maxCounter = level;
		else
			maxCounter = randomGenerator.Next() % 2 + 3;

		while(counter < maxCounter)
		{
			int val = randomGenerator.Next() % 8 + 1;
			if(result.IndexOf(val) == -1)
			{
				result.Add(val);
				counter++;
			}
		}

		return result;
	}

}
