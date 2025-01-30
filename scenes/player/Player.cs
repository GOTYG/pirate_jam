using System;
using Godot;
using Godot.Collections;

namespace PirateJam.scenes.player;

public partial class Player : Area2D
{
    [Signal]
    public delegate void WhipEventHandler(Vector2I pos);

    [Signal]
    public delegate void NextLevelEventHandler();

    [Export] public Dictionary<int, int> AmmoCount;


    private PlayerWeapon _weapon;
    private Hud _hud;
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
            GlobalPosition = GlobalPosition.MoveToward(_globalTargetPosition, _weapon.WeaponSpeed);
            return;
        }

        var curTile = _tileMap.GetCellTileData(_GetCurrentTilePosition());
        if (curTile.GetCustomData("type").AsString() == "door")
        {
            EmitSignal(SignalName.NextLevel);
        }

        if (curTile.GetCustomData("type").AsString() == "pit")
        {
            var bridge = _obstacles.IsHitBridge(_GetCurrentTilePosition(), _tileMap, isUp: false);
            var bullInPit = _obstacles.IsHitBull(_GetCurrentTilePosition(), _tileMap, false);
            if (bridge == null && bullInPit == null) GD.Print("die");
        }


        _isMoving = false;
    }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        _weapon = new Wizard();
        _hud = GetNode<Hud>("HUD");
        _directionSprite = GetNode<Sprite2D>("Arrow");
        _spriteAnimation = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
        _spriteAnimation.Play(_weapon.Animations["idle"]);
        _tileMap = GetParent().GetNode<TileMapLayer>("Map");
        _obstacles = GetParent().GetNode<Obstacles>("Obstacles");
        UpdateHud();
    }

    private void UpdateHud()
    {
        _hud.UpdateAmmoCounts(AmmoCount);
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        if (_isMoving)
            return;

        if (Input.IsActionJustPressed("sword"))
        {
            _weapon = new Wizard();
            _spriteAnimation.Play(_weapon.Animations["idle"]);
            _directionSprite.Visible = false;
        }

        if (Input.IsActionJustPressed("bow"))
        {
            _weapon = new Bow();
            if (_directionSprite != null) _directionSprite.Visible = false;
            _directionSprite = GetNode<Sprite2D>("Arrow");
            _spriteAnimation.Play(_weapon.Animations["idle"]);
        }

        if (Input.IsActionJustPressed("whip"))
        {
            _weapon = new Whip();
            _directionSprite = GetNode<Sprite2D>("Omni");
            _directionSprite.Position = Position;
            _directionSprite.Visible = true;
            _spriteAnimation.Play(_weapon.Animations["idle"]);
            Rotation = 0;
        }


        switch (_weapon.SpecialMoveCategoryMode)
        {
            case SpecialMoveCategory.None:
                _ProcessNoSpecialMove();
                break;
            case SpecialMoveCategory.Targeted:
                _ProcessTargetedSpecialMove();
                break;
            case SpecialMoveCategory.OmniDirectional:
                _ProcessOmnidirectionalSpecialMove();
                break;
        }
    }

    private void _ProcessNoSpecialMove()
    {
        if (Input.IsActionJustPressed("move_down"))
        {
            MovePlayer(Vector2I.Down);
        }
        else if (Input.IsActionJustPressed("move_up"))
        {
            MovePlayer(Vector2I.Up);
        }
        else if (Input.IsActionJustPressed("move_left"))
        {
            MovePlayer(Vector2I.Left);
        }
        else if (Input.IsActionJustPressed("move_right"))
        {
            MovePlayer(Vector2I.Right);
        }
    }

    private void _ProcessTargetedSpecialMove()
    {
        if (Input.IsActionJustPressed("move_down"))
        {
            SelectDirection(Vector2I.Down);
        }
        else if (Input.IsActionJustPressed("move_up"))
        {
            SelectDirection(Vector2I.Up);
        }
        else if (Input.IsActionJustPressed("move_left"))
        {
            SelectDirection(Vector2I.Left);
        }
        else if (Input.IsActionJustPressed("move_right"))
        {
            SelectDirection(Vector2I.Right);
        }
        else if (Input.IsActionJustPressed("move_confirm") 
                 && AmmoCount[(int)_weapon.Name] > 0 
                 && _selectedDirection != Vector2I.Zero)
        {
            AmmoCount[(int)_weapon.Name]--;
            _directionSprite.Visible = false;
            MovePlayer(_selectedDirection);
            UpdateHud();
        }
    }

    private void _ProcessOmnidirectionalSpecialMove()
    {
        if (Input.IsActionJustPressed("move_confirm") && AmmoCount[(int)_weapon.Name] > 0)
        {
            ProcessSpecialMove();
            AmmoCount[(int)_weapon.Name]--;
            UpdateHud();
        }
    }

    public void SelectDirection(Vector2I direction)
    {
        _selectedDirection = direction;
        _directionSprite.Position = direction;
        Rotation = Mathf.Atan2(direction.Y, direction.X);
        _directionSprite.Rotation = Mathf.Atan2(direction.Y, direction.X);
        _directionSprite.Visible = true;

        var currTile = _GetCurrentTilePosition();
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

        // We don't care about the result, we just press it if its there 
        _obstacles.IsHitButton(targetTile, _tileMap);
        var bridge = _obstacles.IsHitBridge(targetTile, _tileMap, isUp: false);
        var bullInTheWay = _obstacles.IsHitBull(targetTile, _tileMap);
        var bullInPit = _obstacles.IsHitBull(targetTile, _tileMap, false);
        var tileType = targetTileData.GetCustomData("type").AsString();
        var walkable = tileType == "floor" || tileType == "door" || bridge != null;
        var filledPit = tileType == "pit" && bullInPit != null && _tileMap.LocalToMap(bullInPit.Position) == targetTile;

        return walkable && bullInTheWay == null || filledPit
            ? targetTile
            : _GetCurrentTilePosition();
    }


    public Vector2I GetBowTarget(Vector2I direction)
    {
        var targetTile = _GetCurrentTilePosition();
        var targetTileData = _tileMap.GetCellTileData(targetTile + direction);

        var bull = _obstacles.IsHitBull(targetTile, _tileMap);
        var bridge = _obstacles.IsHitBridge(targetTile + direction, _tileMap);

        // Stoppin' criteria is hitting a wall, bull, or bridge
        while (targetTileData.GetCustomData("type").AsString() != "wall" &&
               bull == null && bridge == null)
        {
            targetTile += direction;
            bull = _obstacles.IsHitBull(targetTile, _tileMap);
            bridge = _obstacles.IsHitBridge(targetTile + direction, _tileMap);
            targetTileData = _tileMap.GetCellTileData(targetTile + direction);
        }

        if (bull != null)
        {
            bull.Hide();
            bull.IsInteractable = false;
        }

        return targetTile;
    }

    public Vector2I GetTargetPosition(Vector2I direction)
    {
        switch (_weapon.Name)
        {
            case WeaponType.Wizard: return GetSwordTarget(direction);
            case WeaponType.Bow: return GetBowTarget(direction);
            case WeaponType.Whip:
            default:
                return GetWhipTarget();
        }
    }

    public void MovePlayer(Vector2I direction)
    {
        _isMoving = true;
        Rotation = Mathf.Atan2(direction.Y, direction.X);

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
        }
    }

    private Vector2I _GetCurrentTilePosition()
    {
        return _tileMap.LocalToMap(GlobalPosition);
    }
}