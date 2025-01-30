using System.Collections.Generic;
using Godot;

namespace PirateJam.scenes.player;

public enum SpecialMoveCategory
{
    None,
    Targeted,
    OmniDirectional,
}

public abstract class PlayerWeapon
{
    public abstract WeaponType Name { get; }
    public abstract Dictionary<string, StringName> Animations { get; }

    public abstract SpecialMoveCategory SpecialMoveCategoryMode { get; }

    public abstract int WeaponSpeed { get; }
}

public class Wizard : PlayerWeapon
{
    public override WeaponType Name => WeaponType.Wizard;

    public override SpecialMoveCategory SpecialMoveCategoryMode => SpecialMoveCategory.None;

    public override int WeaponSpeed => 2;

    public override Dictionary<string, StringName> Animations { get; } = new()
    {
        { "idle", "sword_idle" },
    };
}

public class Bow : PlayerWeapon
{
    public override WeaponType Name => WeaponType.Bow;

    public override SpecialMoveCategory SpecialMoveCategoryMode => SpecialMoveCategory.Targeted;

    public override int WeaponSpeed => 4;


    public override Dictionary<string, StringName> Animations { get; } = new()
    {
        { "idle", "bow_idle" },
        { "shoot", "bow_shoot" },
    };
}

public class Whip : PlayerWeapon
{
    public override WeaponType Name => WeaponType.Whip;

    public override int WeaponSpeed => 1;

    public override SpecialMoveCategory SpecialMoveCategoryMode => SpecialMoveCategory.OmniDirectional;

    public override Dictionary<string, StringName> Animations { get; } = new()
    {
        { "idle", "whip_idle" },
    };
}