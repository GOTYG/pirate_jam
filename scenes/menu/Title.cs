using Godot;
using System;

namespace PirateJam.scenes.menu;

public partial class Title : CanvasLayer
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void LoadMainScene()
	{
		GetTree().ChangeSceneToFile("res://scenes/menu/main.tscn");
	}
	
	public void Quit()
	{
		GetTree().Quit();
	}
}
