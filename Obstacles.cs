using Godot;
using System.Collections.Generic;
using System.Linq;
using PirateJam;

namespace PirateJam;

public partial class Obstacles : Node
{
    private List<Bull> _bulls { get; set; }
    private List<Bridge> _bridges { get; set; }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        _bulls = new List<Bull>();
        _bridges = new List<Bridge>();

        var children = GetChildren();
        foreach (var child in children)
        {
            //TODO there's probably a cleaner way to do this
            if (child is Bull bull) _bulls.Add(bull);
            else if (child is Bridge bridge) _bridges.Add(bridge);
        }
    }

    public List<Bull> GetInteractableBulls()
    {
        return _bulls.Where(bull => bull.IsInteractable).ToList();
    }

    public List<Bridge> GetInteractableBridges()
    {
        return _bridges.Where(bridge => bridge.IsInteractable).ToList();
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
    }
}