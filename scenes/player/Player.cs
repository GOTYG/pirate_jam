using System.Collections.Generic;
using Godot;
using PirateJam.scenes.maps;

namespace PirateJam.scenes.player;

public partial class Player : Area2D
{
    [Signal]
    public delegate void WhipEventHandler();

    [Signal]
    public delegate void NextLevelEventHandler();

    private PlayerWeapon _weapon;
    private TileMapLayer _tileMap;
    private AnimatedSprite2D _spriteAnimation;
    private bool _isMoving;
    private Vector2 _globalTargetPosition;
    private Vector2I _selectedDirection;
    private Sprite2D _directionSprite;
    private Obstacles _obstacles;
    

    public override void _PhysicsProcess(double delta)
    {
        if (!_isMoving) return;

        if (GlobalPosition != _globalTargetPosition)
        {
            GlobalPosition = GlobalPosition.MoveToward(_globalTargetPosition, 2); //TODO: not hardcoded delta
            return;
        }

        if (_tileMap.GetCellTileData(_GetCurrentTilePosition()).GetCustomData("type").AsString() == "door")
        {
            EmitSignal(SignalName.NextLevel, GetParent().SceneFilePath);
        }

        _isMoving = false;
    }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        _weapon = new Sword();
        _directionSprite = GetNode<Sprite2D>("Arrow");
        _spriteAnimation = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
        _spriteAnimation.Play(_weapon.Animations["idle"]);
        _tileMap = GetParent().GetNode<TileMapLayer>("Map");
        _obstacles = GetParent().GetNode<Obstacles>("Obstacles");
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        if (Input.IsActionJustPressed("restart"))
            GetTree().ReloadCurrentScene();

        if (Input.IsActionJustPressed("sword"))
        {
            _weapon = new Sword();
            _spriteAnimation.Play(_weapon.Animations["idle"]);
        }

        if (Input.IsActionJustPressed("bow"))
        {
            _weapon = new Bow();
            _spriteAnimation.Play(_weapon.Animations["idle"]);
        }

        if (Input.IsActionJustPressed("whip"))
        {
            _weapon = new Whip();
            _spriteAnimation.Play(_weapon.Animations["idle"]);
        }


        if (_isMoving)
            return;

        if (Input.IsActionJustPressed("move_down"))
            SelectDirection(Vector2I.Down);
        else if (Input.IsActionJustPressed("move_up"))
            SelectDirection(Vector2I.Up);
        else if (Input.IsActionJustPressed("move_left"))
            SelectDirection(Vector2I.Left);
        else if (Input.IsActionJustPressed("move_right"))
            SelectDirection(Vector2I.Right);
        else if (Input.IsActionJustPressed("move_confirm") && _directionSprite.Visible)
        {
            ProcessSpecialMove();

            MovePlayer(_selectedDirection);
        }
    }

    public void SelectDirection(Vector2I direction)
    {
        _selectedDirection = direction;
        _directionSprite.Position = direction;
        _directionSprite.Rotation = Mathf.Atan2(direction.Y, direction.X);
        _directionSprite.Visible = true;

        Vector2I currTile = _GetCurrentTilePosition();
        _directionSprite.GlobalPosition = _tileMap.MapToLocal(currTile + direction);
    }


    public Vector2I GetWhipTarget()
    {
        return _GetCurrentTilePosition();
    }

    public Vector2I GetSwordTarget(Vector2I direction)
    {
        var targetTile = _GetCurrentTilePosition() + direction;
        var targetTileData = _tileMap.GetCellTileData(targetTile);

        return targetTileData.GetCustomData("type").AsString() != "floor" ? _GetCurrentTilePosition() : targetTile;
    }

    public Vector2I GetBowTarget(Vector2I direction)
    {
        var targetTile = _GetCurrentTilePosition();
        GD.Print(targetTile);
        GD.Print( _tileMap.MapToLocal(targetTile));

        var obstacles = _obstacles.GetInteractables();
        GD.Print(obstacles);
        var targetTileData = _tileMap.GetCellTileData(targetTile + direction);
        

        while (targetTileData.GetCustomData("type").AsString() != "wall")
        {
            foreach (var obstacle in obstacles)
            {
                GD.Print(_tileMap.LocalToMap(obstacle.Position));
                if (targetTile == _tileMap.LocalToMap(obstacle.Position))
                {
                    GD.Print("hit!");
                }
            }
            targetTile += direction;
            targetTileData = _tileMap.GetCellTileData(targetTile + direction);
        }

        return targetTile;
    }

    public Vector2I GetTargetPosition(Vector2I direction)
    {
        switch (_weapon.Name)
        {
            case WeaponType.Sword: return GetSwordTarget(direction);
            case WeaponType.Bow: return GetBowTarget(direction);
            case WeaponType.Whip:
            default:
                return GetWhipTarget();
        }
    }

    public void MovePlayer(Vector2I direction)
    {
        _isMoving = true;
        _directionSprite.Visible = false;

        var targetPosition = GetTargetPosition(direction);

        // Move in linear format
        _globalTargetPosition = _tileMap.MapToLocal(targetPosition);
    }

    public void ProcessSpecialMove()
    {
        switch (_weapon.Name)
        {
            case WeaponType.Whip:
                EmitSignal(SignalName.Whip, Position);
                break;
            default:
                break;
        }
    }

    private Vector2I _GetCurrentTilePosition()
    {
        return _tileMap.LocalToMap(GlobalPosition);
    }
}