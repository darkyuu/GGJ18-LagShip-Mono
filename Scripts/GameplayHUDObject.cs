using Godot;
using System;

public class GameplayHUDObject : CanvasLayer
{
    private Timer messageTimer;
    private Label messageLabel;

    public override void _Ready()
    {
        messageLabel = GetNode("MessageLabel") as Label;
        messageTimer = GetNode("MessageTimer") as Timer;
    }

    public void ShowMesssage(string message)
    {
        messageLabel.Text = message;
        messageLabel.Show();
        messageTimer.Start();
    }

    public void OnMessageTimerTimeout()
    {
        messageLabel.Hide();
        messageLabel.Text = "";
    }
}

