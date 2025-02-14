using Godot;
using PirateJam.scenes.player;

namespace PirateJam.scenes.maps;

public partial class Map : Node2D
{
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        var player = GetNode<Player>("Player");
        var startMarker = GetNode<Marker2D>("StartMarker");
        player.Position = startMarker.Position;
    }
}