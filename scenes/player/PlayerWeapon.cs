using System.Collections.Generic;
using Godot;

namespace PirateJam.scenes.player;

public abstract class SpecialWeapon : PlayerWeapon
{
    public abstract void SpecialMove();
}

public abstract class PlayerWeapon
{
    public bool HasSpecial { get; set; }
    public abstract Dictionary<string, StringName> Animations { get; }
    public abstract Vector2I GetTargetPosition(Vector2I direction, TileMapLayer tileMap, Vector2I playerPosition);
}

public class Sword : PlayerWeapon
{
    public override Dictionary<string, StringName> Animations { get; } = new()
    {
        { "idle", "sword_idle" },
    };

    public override Vector2I GetTargetPosition(Vector2I direction, TileMapLayer tileMap, Vector2I playerPosition)
    {
        var targetTile = playerPosition + direction;
        var targetTileData = tileMap.GetCellTileData(targetTile);

        return targetTileData.GetCustomData("type").AsString() != "floor" ? playerPosition : targetTile;
    }
}

public class Bow : PlayerWeapon
{
    public override Dictionary<string, StringName> Animations { get; } = new()
    {
        { "idle", "bow_idle" },
        { "shoot", "bow_shoot" },
    };

    public override Vector2I GetTargetPosition(Vector2I direction, TileMapLayer tileMap, Vector2I playerPosition)
    {
        var targetTile = playerPosition;
        var targetTileData = tileMap.GetCellTileData(targetTile + direction);

        while (targetTileData.GetCustomData("type").AsString() != "wall")
        {
            targetTile += direction;
            targetTileData = tileMap.GetCellTileData(targetTile + direction);
        }

        return targetTile;
    }
}

public class Whip : SpecialWeapon
{
    public Whip()
    {
        HasSpecial = true;
    }
    public override Dictionary<string, StringName> Animations { get; } = new()
    {
        { "idle", "whip_idle" },
    };

    public override Vector2I GetTargetPosition(Vector2I direction, TileMapLayer tileMap, Vector2I playerPosition)
    {
        return playerPosition;
    }

    public override void SpecialMove()
    {
        GD.Print("ooooush!");
    }
}