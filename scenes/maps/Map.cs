using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using PirateJam.scenes.player;

namespace PirateJam.scenes.maps;

public partial class Map : Node2D
{
    private const string FILE_BEGIN = "res://scenes/maps/map_";

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        var player = GetNode<Player>("Player");
        var map = GetNode<TileMapLayer>("Map");
        var startPosition = GetNode<Marker2D>("StartMarker");
        player.Position = startPosition.Position;
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
    }

    public void NextLevel(string currMapPath)
    {
        var levelNum = currMapPath.ReplaceN(".tscn", "").Split('_').Last().ToInt();
        var nextLevelPath = FILE_BEGIN + (levelNum + 1) + ".tscn";

        PackedScene nextMapScene = ResourceLoader.Load<PackedScene>(nextLevelPath);
        if (nextMapScene != null)
        {
            Node nextMap = nextMapScene.Instantiate();
            Node parent = GetParent();

            if (parent != null)
            {
                parent.AddChild(nextMap);
                QueueFree();
            }
        }
        else
        {
            GD.PrintErr($"Failed to load next map: {nextLevelPath}. Are you at the last level?");
        }
    }
}