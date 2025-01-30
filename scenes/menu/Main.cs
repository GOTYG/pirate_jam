using Godot;
using PirateJam.scenes.player;

namespace PirateJam.scenes.menu;

public partial class Main : Node
{
    private const string FileBegin = "res://scenes/maps/map_";

    [Export] public int CurrentScene { get; set; } = 1;

    public override void _Ready()
    {
        _SetLevel(CurrentScene);
    }

    public override void _Process(double delta)
    {
        if (Input.IsActionJustPressed("restart")) _ResetLevel();
    }

    // Set the level to the given level number.
    //
    // This is a wrapper around _ActivateLevel that installs the correct signal handler
    // into the newly created player in the to-be-activated level.
    private void _SetLevel(int index)
    {
        _ActivateLevel(index);

        // Get the player in the newly activated level and install the signal handler
        // for the NextLevel signal.
        GetNode<Player>($"Level{index}/Player").NextLevel += () =>
        {
            _DeactivateLevel(CurrentScene);
            _SetLevel(++CurrentScene);
        };
    }

    // Reset the current level.
    private void _ResetLevel()
    {
        _DeactivateLevel(CurrentScene);
        _SetLevel(CurrentScene);
    }

    // Remove a level from the scene tree.
    private void _DeactivateLevel(int index)
    {
        if (GetNodeOrNull("Level" + index) is { } old)
        {
            RemoveChild(old);
            old.QueueFree();
        }
    }

    // Load a level from the file system and add it to the scene tree.
    private void _ActivateLevel(int index)
    {
        var scenePath = FileBegin + index + ".tscn";
        var scene = ResourceLoader.Load<PackedScene>(scenePath);
        var level = scene.Instantiate();
        AddChild(level);
        level.SetName("Level" + CurrentScene);
    }
}