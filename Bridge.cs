using Godot;
using System;

namespace PirateJam;

public partial class Bridge : AnimatedSprite2D, IInteractable
{
    public bool IsInteractable { get; set; }

    [Export] public bool IsUp { get; set; }

    [Export] public int Id { get; set; }


    public void ToggleBridge()
    {
        if (IsUp) GoDown();
        else GoUp();
    }

    public void GoUp()
    {
        IsInteractable = true;
        Play("up");
        IsUp = true;
    }

    public void GoDown()
    {
        IsInteractable = false;
        IsUp = false;
        Play("down");
    }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        if (IsUp) GoUp();
        else GoDown();
    }


    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
    }
}