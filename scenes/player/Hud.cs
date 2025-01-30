using Godot;
using System;
using Godot.Collections;
using PirateJam.scenes.player;

namespace PirateJam.scenes.player;

public partial class Hud : CanvasLayer
{
	private Label _bowCounts;
	private Label _whipCounts;
	
	public override void _Ready()
	{
		_bowCounts = GetNode<Label>("AmmoCounts/BowAmmo/BowCount");
		_whipCounts = GetNode<Label>("AmmoCounts/WhipAmmo/WhipCount");
	}

	public override void _Process(double delta)
	{
		
	}

	public void UpdateAmmoCounts(Dictionary<int, int> ammoCounts)
	{
		_bowCounts.Text = ammoCounts[(int)WeaponType.Bow].ToString();
		_whipCounts.Text = ammoCounts[(int)WeaponType.Whip].ToString();
	}
	
}
