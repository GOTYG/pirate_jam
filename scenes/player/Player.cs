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

    public Vector2I ConvertPosition(Vector2 vec)
    {
        var intX = Mathf.FloorToInt(vec.X) - 49 +1;
        var intY = Mathf.FloorToInt(vec.Y) - 49+1;

        return new Vector2I(intX / 32, intY / 32);


    }
    
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

        var tileDataIndex = ConvertPosition(Position);
        var tiledata = map.GetCellTileData(tileDataIndex);
        var type = tiledata.GetCustomData("type"); 
        GD.Print(tileDataIndex);
        GD.Print(type);
        
        // if ( type == "wall")
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