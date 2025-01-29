using System.Collections.Generic;
using Godot;

namespace PirateJam.scenes.player;

public abstract class PlayerWeapon
{
    public abstract WeaponType Name { get; }
    public abstract Dictionary<string, StringName> Animations { get; }

    public abstract bool DirectionSpriteEnabled { get; }
}

public class Sword : PlayerWeapon
{
    public override WeaponType Name => WeaponType.Sword;

    public override bool DirectionSpriteEnabled => true;

    public override Dictionary<string, StringName> Animations { get; } = new()
    {
        { "idle", "sword_idle" },
    };
}

public class Bow : PlayerWeapon
{
    public override WeaponType Name => WeaponType.Bow;
    
    public override bool DirectionSpriteEnabled => true;

    public override Dictionary<string, StringName> Animations { get; } = new()
    {
        { "idle", "bow_idle" },
        { "shoot", "bow_shoot" },
    };
}

public class Whip : PlayerWeapon
{
    public override WeaponType Name => WeaponType.Whip;
    
    public override bool DirectionSpriteEnabled => false;

    public override Dictionary<string, StringName> Animations { get; } = new()
    {
        { "idle", "whip_idle" },
    };
}