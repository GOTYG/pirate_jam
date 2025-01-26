using Godot;
using System;
using System.Collections.Generic;

namespace PirateJam;
public partial class Button : AnimatedSprite2D, IInteractable
{
	public bool IsInteractable { get; set; }
	private List<Bridge> _bridges { get; set; }
	[Export]
	public int BridgeId { get; set; }


	public void Press()
	{
		foreach (var bridge in _bridges)
		{
			Play("pressed");
			bridge.ToggleBridge();
		}
	}
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		IsInteractable = true;
		_bridges = new List<Bridge>();
		foreach (var node in GetParent().GetChildren())
		{
			if (node is Bridge bridge)
			{
				_bridges.Add(bridge);
			}
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
