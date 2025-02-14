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

    [Signal]
    public delegate void DeathEventHandler();

    [Export] public Dictionary<int, int>? AmmoCount { get; set; }

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
            var bridge = _obstacles.GetBridgeWithStatus(_GetCurrentTilePosition(), _tileMap, isUp: false);
            var bullInPit = _obstacles.GetBullWithStatus(_GetCurrentTilePosition(), _tileMap, false);
            if (bridge == null && bullInPit == null)
            {
                EmitSignal(SignalName.Death);
            }
        }


        _isMoving = false;
        ResetAfterSpecialMove();
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

        if (AmmoCount == null)
        {
            AmmoCount = new Dictionary<int, int>();
            AmmoCount.Add(1, 2);
            AmmoCount.Add(2, 2);
        }

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
            _directionSprite.Visible = false;
            _directionSprite = GetNode<Sprite2D>("Arrow");
            _spriteAnimation.Play(_weapon.Animations["idle"]);
        }

        if (Input.IsActionJustPressed("whip"))
        {
            _weapon = new Whip();
            _directionSprite.Visible = false; // Fix bow direction sprite still showing
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
        if (Input.IsActionPressed("move_down"))
        {
            MovePlayer(Vector2I.Down);
        }
        else if (Input.IsActionPressed("move_up"))
        {
            MovePlayer(Vector2I.Up);
        }
        else if (Input.IsActionPressed("move_left"))
        {
            MovePlayer(Vector2I.Left);
        }
        else if (Input.IsActionPressed("move_right"))
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
                 && AmmoCount[(int)_weapon.Name] != 0
                 && _selectedDirection != Vector2I.Zero)
        {
            GetNode<AudioStreamPlayer>("Sounds/BowShoot").Play();
            AmmoCount[(int)_weapon.Name]--;
            MovePlayer(_selectedDirection);
        }
    }

    private void _ProcessOmnidirectionalSpecialMove()
    {
        if (Input.IsActionJustPressed("move_confirm") && AmmoCount[(int)_weapon.Name] != 0)
        {
            GetNode<AudioStreamPlayer>("Sounds/WhipSound").Play();
            ProcessSpecialMove();
            AmmoCount[(int)_weapon.Name]--;
            ResetAfterSpecialMove();
        }
    }

    public void ResetAfterSpecialMove()
    {
        UpdateHud();
        _weapon = new Wizard();
        _directionSprite.Visible = false;
        _spriteAnimation.Play(_weapon.Animations["idle"]);
        _selectedDirection = Vector2I.Zero;
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


        var bridge = _obstacles.GetBridgeWithStatus(targetTile, _tileMap, isUp: false);
        var bridgeInPath = _obstacles.GetBridgeWithStatus(targetTile, _tileMap, isUp: true);

        var bullInTheWay = _obstacles.GetBullWithStatus(targetTile, _tileMap);
        var bullInPit = _obstacles.GetBullWithStatus(targetTile, _tileMap, false);
        var tileType = targetTileData.GetCustomData("type").AsString();
        var walkableFloor = (tileType == "floor" || tileType == "door") && bridgeInPath == null;
        var filledPit = tileType == "pit" && (bullInPit != null || bridge != null);

        if (bullInTheWay != null || (bullInPit != null && tileType == "pit"))
        {
            var bullSound = GetNode<AudioStreamPlayer>("Sounds/BullInteract");
            if (!bullSound.Playing) bullSound.Play();
        }

        var walkable = walkableFloor && bullInTheWay == null || filledPit;

        if (walkable)
        {
            // We don't care about the result, we just press it if its there 
            _obstacles.IsHitButton(targetTile, _tileMap);
            return targetTile;
        }

        return _GetCurrentTilePosition();
    }


    public Vector2I GetBowTarget(Vector2I direction)
    {
        var targetTile = _GetCurrentTilePosition();
        var targetTileData = _tileMap.GetCellTileData(targetTile + direction);

        var bull = _obstacles.GetBullWithStatus(targetTile, _tileMap);
        var bridge = _obstacles.GetBridgeWithStatus(targetTile + direction, _tileMap);

        // Stoppin' criteria is hitting a wall, bull, or bridge
        while (targetTileData.GetCustomData("type").AsString() != "wall" &&
               bull == null && bridge == null)
        {
            targetTile += direction;
            bull = _obstacles.GetBullWithStatus(targetTile, _tileMap);
            bridge = _obstacles.GetBridgeWithStatus(targetTile + direction, _tileMap);
            targetTileData = _tileMap.GetCellTileData(targetTile + direction);
        }

        if (bull != null)
        {
            GetNode<AudioStreamPlayer>("Sounds/BullDeath").Play();
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