using Godot;
using System;

public partial class Bridge : AnimatedSprite2D
{
    public bool IsInteractable { get; set; }


    public void GoUp()
    {
        IsInteractable = true;
        Play("up");
    }

    public void GoDown()
    {
        IsInteractable = false;
        Play("down");
    }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
    }


    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
    }
}