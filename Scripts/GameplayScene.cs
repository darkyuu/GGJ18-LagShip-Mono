using Godot;
using System;
using System.Collections.Generic;

public class GameplayScene : Node
{
    [Export]
    public PackedScene rightCommands;
    [Export]
    public PackedScene leftCommands;
    [Export]
    public PackedScene shootCommands;
    [Export]
    public PackedScene asteroids;
    [Export]
    public PackedScene blackHoles;
    [Export]
    public int maxCommandBuffer = 15;
    [Export]
    public float velocity = 200;
    [Export]
    public int maxCommandFrameCounter = 10;

    private Node asteroidPool;
    private Position2D commandStartPoint;
    private AudioStreamPlayer deathSound; 
    private AudioStreamPlayer bgm;
    private ShipObject ship;
    private GameOverHUD gameOverHUD; 
    private GameplayHUDObject gameplayHUD;
    private Timer messageTimer;
    private Node blackHolePool;
    private Timer spawnTimer;
    private CommandExecutePoint commandExecutePoint;
    Autoload globals;

    private int currentCommandFrameCounter;
    private int listSelectedIndex = 1;
    private List<int> spawnPatterns = new List<int>();

    public override void _Ready()
    {
        asteroidPool = GetNode("AsteroidPool") as Node;
        commandStartPoint = GetNode("CommandStartPoint") as Position2D;
        deathSound = GetNode("DeathSound") as AudioStreamPlayer;
        bgm = GetNode("BGM") as AudioStreamPlayer;
        gameplayHUD = GetNode("GameplayHUD") as GameplayHUDObject;
        gameOverHUD = GetNode("GameOverHUD") as GameOverHUD;
        ship = GetNode("Ship") as ShipObject;
        commandExecutePoint = GetNode("CommandExecutePoint") as CommandExecutePoint;

        messageTimer = GetNode("GameplayHUD/MessageTimer") as Timer;
        blackHolePool = GetNode("BlackHolePool") as Node;
        spawnTimer = GetNode("SpawnTimer") as Timer;

        this.globals = (Autoload)GetNode("/root/Autoload");

        this.globals.Randomize();
        ship.Connect("Hit", this, "GameOver");
        gameOverHUD.Connect("BackToMainMenu",this, "GotoMainmenu");
        gameOverHUD.Connect("RestartGame",this, "RestartGameplay");
        commandExecutePoint.Connect("CallRight",this, "ForceShipTurnRight");
        commandExecutePoint.Connect("CallLeft",this, "ForceShipTurnLeft");
        commandExecutePoint.Connect("CallShoot",this, "ForceShipShootLaser");

	    SetProcess(true);
	    NewGame();
	    StartNextLevel();
    }

    public override void _Process(float delta)
    {
        if(currentCommandFrameCounter == maxCommandFrameCounter)
            InputFromPlayer();
        else
            currentCommandFrameCounter += 1;

        if(this.globals.playerState.Equals("play") && asteroidPool.GetChildCount() == 0)
            StartNextLevel();
    }

    private void InputFromPlayer()
    {
        if(this.globals.currentCommandBuffer < maxCommandBuffer)
        {
            if(Input.IsActionJustPressed("ui_right"))
                CreateCommand("right");
            else if(Input.IsActionJustPressed("ui_left"))
                CreateCommand("left");
            else if(Input.IsActionJustPressed("player_shoot"))
                CreateCommand("shoot");
        }
    }

    private void CreateCommand(string command)
    {
        RigidBody2D commandObj = null;
        if(command.Equals("left"))
            commandObj = (RigidBody2D)leftCommands.Instance();
        else if(command.Equals("right"))
            commandObj = (RigidBody2D)rightCommands.Instance();
        else if(command.Equals("shoot"))
            commandObj = (RigidBody2D)shootCommands.Instance();
        
        if(commandObj != null)
        {
            AddChild(commandObj);
            float direction = 0;
            commandObj.Position = commandStartPoint.Position;
            velocity = this.globals.GetCommandVelocityForCurrentLevel();
            int baseValue = 5;
            velocity -= this.globals.GetCommandLatencyFactorForCurrentLevel()/baseValue*velocity;
            velocity = Mathf.Clamp(velocity, this.globals.commandMinimumVelocity, this.globals.commandVelocity[0]);

            commandObj.SetLinearVelocity(new Vector2(velocity, 0).Rotated(direction));

            this.globals.currentCommandBuffer += 1;
            currentCommandFrameCounter = 0;
        }
    }

    private void ForceShipTurnLeft()
    {
        ((ShipObject)GetNode("Ship")).TurnLeft();
    }

    private void ForceShipTurnRight()
    {
        ((ShipObject)GetNode("Ship")).TurnRight();
    }

    private void ForceShipShootLaser()
    {
        ((ShipObject)GetNode("Ship")).Shoot();
    }

    private void NewGame()
    {
        currentCommandFrameCounter = 0;
        deathSound.Stop();
        bgm.Play();
        this.globals.paused = false;
        gameOverHUD.Hide(); 
        SetProcess(true);
    }

	private void RestartGameplay()
    {
        this.globals.level = 0;
        ClearRemainingAsteroid(asteroidPool);
        ((Sprite)GetNode("Ship/Sprite")).Show();
        ship.Start();
        NewGame();
        StartNextLevel();
    }

    private void StartNextLevel()
    {
        this.globals.playerState = "wait";
        this.globals.level += 1;

        messageTimer.WaitTime = this.globals.GetLevelSpawnTimeForCurrentLevel();
        gameplayHUD.ShowMesssage("WAVE "+this.globals.level);
        
        string latencyString = (this.globals.GetCommandLatencyFactorForCurrentLevel() * 100).ToString();
        Label latencyLabel = GetNode("BarSet/LatencyLabel") as Label;
        latencyLabel.Text = "Latency\n"+ latencyString;


        spawnPatterns.Clear();
        spawnPatterns = this.globals.GenerateSpawnPattern();
        foreach (var i in spawnPatterns)
            SetBlackholeEffectToSpawnPosition(i);

        SetSpawnTimerAfterMessageTimerIsFinish();
    }

    private async void SetSpawnTimerAfterMessageTimerIsFinish()
    {
        await ToSignal(messageTimer, "timeout");
        foreach (BlackHoleObject a in blackHolePool.GetChildren())
            a.QueueFree();
        spawnTimer.WaitTime = 0.1f;
        spawnTimer.Start();
    }

    private void SetBlackholeEffectToSpawnPosition(int positionIndex)
    {
        Vector2 tempStartPosition = ((Position2D)GetNode("ObstacleSpawnPositions/"+positionIndex.ToString())).GetPosition();
        BlackHoleObject bh = (BlackHoleObject)blackHoles.Instance();
        blackHolePool.AddChild(bh);
        bh.Position = tempStartPosition;
        bh.Play();
    }

    private void SetAsteroidToSpawnPosition(int positionIndex)
    {
        Vector2 tempStartPosition = ((Position2D)GetNode("ObstacleSpawnPositions/"+positionIndex.ToString())).GetPosition();
        float aimToPosition = (float)this.globals.aimToShipRotation[positionIndex-1];

        AsteroidObject ast = (AsteroidObject)asteroids.Instance();
        asteroidPool.AddChild(ast);
        ast.Position = tempStartPosition;
        ast.Rotation = 0;
        ast.SetLinearVelocity(new Vector2(this.globals.asteroidInitialVelocity[positionIndex -1], 0).Rotated(aimToPosition));
    }

    private void ClearRemainingAsteroid(Node astPool)
    {
        foreach (Node a in astPool.GetChildren())
            a.QueueFree();
    }

    private void GameOver()
    {
        deathSound.Play();
        bgm.Stop();
        SetProcess(false);
        this.globals.paused = true;
        spawnTimer.Stop();
        gameOverHUD.Show();
    }

    private void GotoMainmenu()
    {
        this.globals.level = 0;
        this.globals.GotoScene(gameOverHUD, "MenuScene");
    }

    public void OnSpawnTimerTimeout()
    {
        foreach (var i in spawnPatterns)
            SetAsteroidToSpawnPosition(i);
            
        this.globals.playerState = "play";
    }
}