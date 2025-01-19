using Godot;
using System;

namespace PirateJam.scenes.player;

public partial class Player : Area2D
{
	private readonly PlayerWeapon _weapon = new PlayerWeapon();
	private Vector2 _screenSize;
	private TileMapLayer _tileMap;
	
	public PlayerWeapon Weapon { get; set; }
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_screenSize = GetViewportRect().Size;
		_weapon.WeaponName = "Sword";
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void Start(Vector2 startPositionPosition)
	{
		Position = startPositionPosition;
		GD.Print($"Starting position: {Position}");
	}
}
