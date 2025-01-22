using Godot;
using System;
using PirateJam.scenes.maps;

namespace PirateJam.scenes.player;

public partial class Player : Area2D
{
    private PlayerWeapon _weapon;
    private TileMapLayer _tileMap;
    private AnimatedSprite2D _spriteAnimation;
    private bool _isMoving;
    private Vector2 _globalTargetPosition;
    private Vector2I _selectedDirection;
    private Sprite2D _directionSprite;

    public override void _PhysicsProcess(double delta)
    {
        if(!_isMoving) return;

        if (GlobalPosition != _globalTargetPosition)
        {
            GlobalPosition = GlobalPosition.MoveToward(_globalTargetPosition, 2); //TODO: not hardcoded delta
            return;
        }

        if (_tileMap.GetCellTileData(_GetCurrentTilePosition()).GetCustomData("type").AsString() == "door")
        {
            GD.Print("CUM");
            // TODO Level Cumplete
        }
        
        _isMoving = false;
        // TODO: still retains the selected direction, wanted behavior?
    }
    
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        _weapon = new Sword();
        _directionSprite= GetNode<Sprite2D>("Arrow");
        _spriteAnimation = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
        _spriteAnimation.Play(_weapon.Animations["idle"]);
        _tileMap = GetParent().GetNode<TileMapLayer>("Map");
    }
    
    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        if (Input.IsActionJustPressed("restart"))
            GetTree().ReloadCurrentScene();
        
        if (_isMoving) return;
        
        if (Input.IsActionJustPressed("move_down"))
            SelectDirection(Vector2I.Down);
        else if (Input.IsActionJustPressed("move_up")) 
            SelectDirection(Vector2I.Up);
        else if (Input.IsActionJustPressed("move_left")) 
            SelectDirection(Vector2I.Left);
        else if (Input.IsActionJustPressed("move_right")) 
            SelectDirection(Vector2I.Right);
        else if (Input.IsActionJustPressed("move_confirm"))
            MovePlayer(_selectedDirection);
    }

    public void SelectDirection(Vector2I direction)
    {
        _selectedDirection = direction;
        _directionSprite.Position = direction;
        _directionSprite.Rotation = Mathf.Atan2(direction.Y, direction.X) ;
        _directionSprite.Visible = true;
        
        Vector2I currTile = _GetCurrentTilePosition();
        _directionSprite.GlobalPosition = _tileMap.MapToLocal(currTile + direction);
    }
    
    
    public void MovePlayer(Vector2I direction)
    {
        _isMoving = true;
        _directionSprite.Visible = false;
        
        Vector2I currTile = _GetCurrentTilePosition();
        Vector2I targetPosition = _weapon.GetTargetPosition(direction, _tileMap, currTile);
        
        // Move in linear format
        _globalTargetPosition = _tileMap.MapToLocal(targetPosition);
    }

    private Vector2I _GetCurrentTilePosition()
    {
        return _tileMap.LocalToMap(GlobalPosition);
    }

    
}