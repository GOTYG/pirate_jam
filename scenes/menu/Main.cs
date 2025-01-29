using Godot;
using PirateJam.scenes.player;

namespace PirateJam.scenes.menu;

public partial class Main : Node
{
    private const string FileBegin = "res://scenes/maps/map_";
    private int _currentScene = 1;

    public override void _Ready()
    {
        _SetLevel(_currentScene);

        GetNode<Player>("Level1/Player").NextLevel += () => { _SetLevel(_currentScene++); };
    }

    public override void _Process(double delta)
    {
        if (Input.IsActionJustPressed("restart")) _ResetLevel();
    }

    private void _SetLevel(int index)
    {
        _ActivateLevel(_LoadLevel(index), clearPrevious: true);
    }

    private void _ResetLevel()
    {
        _ActivateLevel(_LoadLevel(_currentScene), clearPrevious: false);
    }

    private static Node _LoadLevel(int index)
    {
        var scenePath = FileBegin + index + ".tscn";
        var scene = ResourceLoader.Load<PackedScene>(scenePath);
        return scene.Instantiate();
    }

    private void _ActivateLevel(Node level, bool clearPrevious = true)
    {
        AddChild(level);
        level.SetName("Level" + _currentScene);

        if (_currentScene == 1 || !clearPrevious) return;

        var old = GetNode("Level" + (_currentScene - 1));
        RemoveChild(old);
        old.QueueFree();
    }
}