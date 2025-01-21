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
        
        var animatedSprite2D = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
        string[] mobTypes = animatedSprite2D.SpriteFrames.GetAnimationNames();
        animatedSprite2D.Play(mobTypes[1]);
    }

    public static Vector2I ConvertPosition(Vector2 vec)
    {
        var intX = Mathf.FloorToInt(vec.X) - 49;
        var intY = Mathf.FloorToInt(vec.Y) - 49;

        return new Vector2I((intX / 32) + 1, (intY / 32) + 1);
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        var moveTo = Position;
        bool isMoved = false;
        if (Input.IsActionJustPressed("move_down"))
        {
            moveTo.Y += 32;
            isMoved = true;
            Rotation = Mathf.Pi / 2;
        }

        if (Input.IsActionJustPressed("move_up"))
        {
            moveTo.Y -= 32;
            isMoved = true;
            Rotation = Mathf.Pi*3/2;
        }

        if (Input.IsActionJustPressed("move_left"))
        {
            moveTo.X -= 32;
            isMoved = true;
            Rotation = Mathf.Pi;
        }

        if (Input.IsActionJustPressed("move_right"))
        {
            moveTo.X += 32;
            isMoved = true;
            Rotation = 0;
        }

        if (!isMoved)
        {
            return;
        }

        //TODO getting parent node is pretty ugly, is there a better way?
        var map = GetParent().GetNode("Map") as TileMapLayer;

        var tileDataIndex = ConvertPosition(moveTo);
        var tiledata = map.GetCellTileData(tileDataIndex);
        var customData = tiledata.GetCustomData("type").As<string>();

        
        if (customData == "floor")
        {
            Move(moveTo);
        }
    }

    public void Move(Vector2 position)
    {
        Position = position;
        GD.Print($"Moving to: {Position}");
    }
}