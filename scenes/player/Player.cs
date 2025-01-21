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
        _tileMap = GetParent().GetNode("Map") as TileMapLayer;
        var animatedSprite2D = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
        string[] weaponSprites = animatedSprite2D.SpriteFrames.GetAnimationNames();
        animatedSprite2D.Play(weaponSprites[1]);
    }

    public static Vector2I ConvertPosition(Vector2 vec)
    {
        var intX = Mathf.FloorToInt(vec.X) - 49;
        var intY = Mathf.FloorToInt(vec.Y) - 49;

        return new Vector2I((intX / 32) + 1, (intY / 32) + 1);
    }

    public enum Direction
    {
        North,
        East,
        South,
        West,
    }


    public void BowMovement(Direction direction)
    {
        var moveTo = Position;

        while (true)
        {
            switch (direction)
            {
                case Direction.North:
                    moveTo.Y -= 32;
                    break;
                case Direction.East:
                    moveTo.X += 32;
                    break;
                case Direction.South:
                    moveTo.Y += 32;
                    break;
                case Direction.West:
                    moveTo.X -= 32;
                    break;
            }


            var tileDataIndex = ConvertPosition(moveTo);
            var tiledata = _tileMap.GetCellTileData(tileDataIndex);
            var customData = tiledata.GetCustomData("type").As<string>();


            if (customData == "floor")
            {
                Move(moveTo);
            }


            if (customData == "wall")
            {
                break;
            }
        }
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        bool isMoved = false;

        Direction direction = Direction.East;
        if (Input.IsActionJustPressed("move_down"))
        {
            direction = Direction.South;
            isMoved = true;
            Rotation = Mathf.Pi / 2;
        }

        if (Input.IsActionJustPressed("move_up"))
        {
            direction = Direction.North;

            isMoved = true;
            Rotation = Mathf.Pi * 3 / 2;
        }

        if (Input.IsActionJustPressed("move_left"))
        {
            direction = Direction.West;

            isMoved = true;
            Rotation = Mathf.Pi;
        }

        if (Input.IsActionJustPressed("move_right"))
        {
            direction = Direction.East;

            isMoved = true;
            Rotation = 0;
        }

        if (!isMoved)
        {
            return;
        }


        BowMovement(direction);
    }

    public void Move(Vector2 startPositionPosition)
    {
        Position = startPositionPosition;
        GD.Print($"Moving to: {Position}");
    }
}