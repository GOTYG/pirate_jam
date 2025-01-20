using Godot;
using System;
using PirateJam.scenes.maps;

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

    public ConvertPosition
    
    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        var moveTo = Position;
        if (Input.IsActionJustPressed("move_down"))
        {
            moveTo.Y += 32;
        }

        if (Input.IsActionJustPressed("move_up"))
        {
            moveTo.Y -= 32;
        }

        if (Input.IsActionJustPressed("move_left"))
        {
            moveTo.X -= 32;
        }

        if (Input.IsActionJustPressed("move_right"))
        {
            moveTo.X += 32;
        }

        var map = GetParent().GetNode("Map") as TileMapLayer;
        
        var tileDataIndex = new Vector2I(Mathf.FloorToInt(moveTo.X - 49),
            Mathf.FloorToInt(moveTo.Y -49 ));
        var tiledata = map.GetCellTileData(tileDataIndex);
        // // var type = tiledata.GetCustomData("type"); // This is the property assigned in the TileSet
        GD.Print(tileDataIndex);
        GD.Print(tiledata);
        
        // if ( typeof(type == "string")
        // {
        //     
        // }
        Move(moveTo);
    }

    public void Move(Vector2 startPositionPosition)
    {
        Position = startPositionPosition;
        GD.Print($"Moving to: {Position}");
    }
}