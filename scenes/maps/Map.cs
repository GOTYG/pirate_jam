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
        player.Move(startPostion.Position);

        // Example of how to get tile information:
        Vector2I firstTile = Vector2I.Zero; // Tile at 0,0
        var tiledata = map.GetCellTileData(firstTile);
        var type = tiledata.GetCustomData("type"); // This is the property assigned in the TileSet
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
    }
}