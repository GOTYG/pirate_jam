using System;
using Godot;
using System.Collections.Generic;
using System.Linq;
using PirateJam;

namespace PirateJam;

public partial class Obstacles : Node
{
    private List<Bull> _bulls { get; set; }
    private List<Bridge> _bridges { get; set; }
    private List<Button> _buttons { get; set; }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        _bulls = new List<Bull>();
        _bridges = new List<Bridge>();
        _buttons = new List<Button>();

        var children = GetChildren();
        foreach (var child in children)
        {
            //TODO there's probably a cleaner way to do this
            if (child is Bull bull) _bulls.Add(bull);
            else if (child is Bridge bridge) _bridges.Add(bridge);
            else if (child is Button button) _buttons.Add(button);
        }
    }

    public List<Bull> GetInteractableBulls()
    {
        return _bulls.Where(bull => bull.IsInteractable).ToList();
    }

    public List<Bridge> GetBridges(bool isUp=true)
    {
        return _bridges.Where(bridge => bridge.IsInteractable == isUp).ToList();
    }

    public List<Button> GetInteractableButtons()
    {
        return _buttons.Where(button => button.IsInteractable).ToList();
    }

    public Button? IsHitButton(Vector2I tile, TileMapLayer tileMap)
    {
        var buttons = GetInteractableButtons();
        foreach (var button in buttons)
        {
            if (tile == tileMap.LocalToMap(button.Position))
            {
                button.Press();
                return button;
            }
        }

        return null;
    }


    public Bridge? IsHitBridge(Vector2I tile, TileMapLayer tileMap,bool isUp=true)
    {
        var bridges = GetBridges(isUp);

        foreach (var obstacle in bridges)
        {
            if (tile == tileMap.LocalToMap(obstacle.Position))
            {
                return obstacle;
            }
        }

        return null;
    }

    public Bull? IsHitBull(Vector2I tile, TileMapLayer tileMap)
    {
        var bulls = GetInteractableBulls();

        foreach (var obstacle in bulls)
        {
            if (tile == tileMap.LocalToMap(obstacle.Position))
            {
                return obstacle;
            }
        }

        return null;
    }


    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
    }
}