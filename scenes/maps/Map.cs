using Godot;
using System;
using PirateJam.scenes.player;

namespace PirateJam.scenes.maps;

public partial class Map : Node2D
{
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        var player = GetNode<Player>("Player");
        var map = GetNode<TileMapLayer>("Map");
        var startPostion = GetNode<Marker2D>("StartMarker");
        player.Position = startPosition.Position;
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
    }
}
