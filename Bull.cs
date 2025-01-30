using Godot;
using System;
using PirateJam.scenes.player;

namespace PirateJam;

public partial class Bull : Sprite2D, IInteractable
{
    private TileMapLayer _tileMap;
    private bool _isMoving;
    private Vector2 _globalTargetPosition;
    private Obstacles _obstacles;
    private Player _player;

    public bool IsInteractable { get; set; }


    public void OnPlayerWhip(Vector2I whipPos)
    {
        if (!IsInteractable)
        {
            return;
        }

        _isMoving = true;
        var deltaX = whipPos.X - Position.X;
        var deltaY = whipPos.Y - Position.Y;
        Vector2I direction;


        if (Math.Abs(deltaX) > Math.Abs(deltaY))
        {
            if (Mathf.Sign(deltaX) == 1) direction = Vector2I.Left;
            else direction = Vector2I.Right;
        }
        else
        {
            if (Mathf.Sign(deltaY) == 1) direction = Vector2I.Up;
            else direction = Vector2I.Down;
        }

        Rotation = Mathf.Atan2(direction.Y, direction.X);

        //TODO update rotation/mirror image

        var currentTile = _GetCurrentTilePosition();
        var curTileData = _tileMap.GetCellTileData(currentTile);
        var nextTileData = _tileMap.GetCellTileData(currentTile + direction);

        var bull = _obstacles.IsHitBull(currentTile + direction, _tileMap);
        var bridge = _obstacles.IsHitBridge(currentTile + direction, _tileMap);


        while (nextTileData.GetCustomData("type").AsString() != "wall" &&
               curTileData.GetCustomData("type").AsString() != "pit" && bull == null && bridge == null)
        {
            curTileData = nextTileData;
            currentTile += direction;
            bull = _obstacles.IsHitBull(currentTile + direction, _tileMap);
            bridge = _obstacles.IsHitBridge(currentTile + direction, _tileMap);
            _obstacles.IsHitButton(currentTile, _tileMap);
            nextTileData = _tileMap.GetCellTileData(currentTile + direction);
        }

        if (curTileData.GetCustomData("type").AsString() == "pit")
        {
            //TODO: Don't hardcode and make new tile (filled pit)
            // var floorTileAtlasLoc = new Vector2I(2, 3);
            // _tileMap.SetCell(currentTile, 0, atlasCoords: floorTileAtlasLoc, 0);
            IsInteractable = false;
        }

        _globalTargetPosition = _tileMap.MapToLocal(currentTile);
    }

    public override void _PhysicsProcess(double delta)
    {
        if (!_isMoving) return;

        if (GlobalPosition != _globalTargetPosition)
        {
            GlobalPosition = GlobalPosition.MoveToward(_globalTargetPosition, 2); //TODO: not hardcoded delta
            return;
        }


        _isMoving = false;
    }

    private Vector2I _GetCurrentTilePosition()
    {
        return _tileMap.LocalToMap(GlobalPosition);
    }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        var parent = GetParent();
        _tileMap = parent.GetParent().GetNode<TileMapLayer>("Map");
        _player = parent.GetParent().GetNode<Player>("Player");
        _player.Whip += OnPlayerWhip;
        _obstacles = parent as Obstacles;
        IsInteractable = true;
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
    }
}