using Godot;
using System;
using System.Collections.Generic;

namespace PirateJam;

public partial class Bull : Sprite2D, IInteractable
{
    private TileMapLayer _tileMap;
    private bool _isMoving;
    private Vector2 _globalTargetPosition;
    private Obstacles _obstacles;

    public bool IsInteractable { get; set; }

    public Vector2I? MoveDueToWhip(Vector2I whipPos, List<Vector2I> claimedPits)
    {
        _isMoving = true;
        var deltaX = whipPos.X - Position.X;
        var deltaY = whipPos.Y - Position.Y;

        Vector2I direction;
        if (Math.Abs(deltaX) > Math.Abs(deltaY))
        {
            direction = Mathf.Sign(deltaX) == 1 ? Vector2I.Left : Vector2I.Right;
        }
        else
        {
            direction = Mathf.Sign(deltaY) == 1 ? Vector2I.Up : Vector2I.Down;
        }

        Rotation = Mathf.Atan2(direction.Y, direction.X);


        var targetTile = _CalculateMovement(direction, claimedPits);
        var targetTileData = _tileMap.GetCellTileData(targetTile);
        _globalTargetPosition = _tileMap.MapToLocal(targetTile);

        if (targetTileData.GetCustomData("type").AsString() == "pit")
        {
            var bull = _obstacles.GetBullWithStatus(targetTile, _tileMap, false);
            if (bull == null) IsInteractable = false;


            return targetTile;
        }

        return null;
    }


    private Vector2I _CalculateMovement(Vector2I direction, List<Vector2I> claimedPits)
    {
        var currentTile = _GetCurrentTilePosition();
        while (true)
        {
            var curTileData = _tileMap.GetCellTileData(currentTile);
            var nextTileData = _tileMap.GetCellTileData(currentTile + direction);

            var isNextToWall = nextTileData.GetCustomData("type").AsString() == "wall";
            var bullInPath = _obstacles.GetBullWithStatus(currentTile + direction, _tileMap);
            var bridgeInPath = _obstacles.GetBridgeWithStatus(currentTile + direction, _tileMap, isUp: true);
            var wouldHitImpass = isNextToWall || (bullInPath != null) || (bridgeInPath != null);


            var isOverPit = curTileData.GetCustomData("type").AsString() == "pit";
            var bullInIt = _obstacles.GetBullWithStatus(currentTile, _tileMap, false) != null;
            var bullClaimedIt = claimedPits.Contains(currentTile);
            var bridgeOverIt = _obstacles.GetBridgeWithStatus(currentTile, _tileMap, isUp: false) != null;
            var isInPit = isOverPit && !bullInIt && !bullClaimedIt && !bridgeOverIt;

            _obstacles.IsHitButton(currentTile, _tileMap);


            if (wouldHitImpass || isInPit) break;
            currentTile += direction;
        }

        return currentTile;
    }

    public override void _PhysicsProcess(double delta)
    {
        if (!_isMoving) return;

        if (GlobalPosition != _globalTargetPosition)
        {
            GlobalPosition = GlobalPosition.MoveToward(_globalTargetPosition, 2); //TODO: not hardcoded delta
            return;
        }

        var curTile = _tileMap.GetCellTileData(_GetCurrentTilePosition());
        if (curTile.GetCustomData("type").AsString() == "pit")
        {
            var bridge = _obstacles.GetBridgeWithStatus(_GetCurrentTilePosition(), _tileMap, isUp: false);
            var bullInPit = _obstacles.GetBullWithStatus(_GetCurrentTilePosition(), _tileMap, false);
            if (bridge == null && bullInPit != null && GetInstanceId() == bullInPit.GetInstanceId())
                Scale = new Vector2(.8f, .8f);
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
        _obstacles = parent as Obstacles;
        IsInteractable = true;
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
    }
}