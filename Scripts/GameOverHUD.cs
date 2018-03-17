using Godot;
using System;

public class GameOverHUD : CanvasLayer
{
    [Signal]
    public delegate void RestartGame();
    [Signal]
    public delegate void BackToMainMenu();

    
    private Button restartBtn;
    private Button backtomainBtn;
    private Label gameOver; 

    public override void _Ready()
    {
        restartBtn = GetNode("restartBtn") as Button;
        backtomainBtn = GetNode("backtomainBtn") as Button;
        gameOver = GetNode("gameOver") as Label;        
    }

    public void Show()
    {
        restartBtn.Show();
        backtomainBtn.Show();
        gameOver.Show();
    }

    public void Hide()
    {
        restartBtn.Hide();
        backtomainBtn.Hide();
        gameOver.Hide();
    }

    public void OnRestartBtnPressed()
    {
        this.EmitSignal("RestartGame");
    }

    public void OnBacktomainBtnPressed()
    {
    	this.EmitSignal("BackToMainMenu");
    }
}
