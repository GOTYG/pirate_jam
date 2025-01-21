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

    public override void _PhysicsProcess(double delta)
    {
        if(!_isMoving) return;

        if (GlobalPosition != _globalTargetPosition)
        {
            GlobalPosition = GlobalPosition.MoveToward(_globalTargetPosition, 2);
            return;
        }
        
        _isMoving = false;
    }
    
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        _weapon = new Bow();
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
            MovePlayer(Vector2I.Down);
        else if (Input.IsActionJustPressed("move_up")) 
            MovePlayer(Vector2I.Up);
        else if (Input.IsActionJustPressed("move_left")) 
            MovePlayer(Vector2I.Left);
        else if (Input.IsActionJustPressed("move_right")) 
            MovePlayer(Vector2I.Right);
    }

    public void MovePlayer(Vector2I direction)
    {
        _isMoving = true;
        Vector2I currTile = _tileMap.LocalToMap(GlobalPosition);
        Vector2I targetPosition = _weapon.GetTargetPosition(direction, _tileMap, currTile);
        
        // Move in linear format
        _globalTargetPosition = _tileMap.MapToLocal(targetPosition);
    }

    
}