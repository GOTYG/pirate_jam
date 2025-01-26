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

    public List<Bridge> GetInteractableBridges()
    {
        return _bridges.Where(bridge => bridge.IsInteractable).ToList();
    }

    public List<Button> GetInteractableButtons()
    {
        return _buttons.Where(button => button.IsInteractable).ToList();
    }

    public bool IsSwordHitObstacle(Vector2I tile, TileMapLayer tileMap)
    {
        var buttons = GetInteractableButtons();
        foreach (var button in buttons)
        {
            if (tile == tileMap.LocalToMap(button.Position))
            {
                button.Press();
                return true;
            }
        }

        return false;
    }

    public bool IsBowHitObstacle(Vector2I tile, TileMapLayer tileMap, Vector2I direction)
    {
        var bulls = GetInteractableBulls();
        var bridges = GetInteractableBridges();

        foreach (var obstacle in bulls)
        {
            if (tile == tileMap.LocalToMap(obstacle.Position))
            {
                obstacle.Hide();
                obstacle.IsInteractable = false;
                return true;
            }
        }

        foreach (var obstacle in bridges)
        {
            if (tile + direction == tileMap.LocalToMap(obstacle.Position))
            {
                return true;
            }
        }

        return false;
    }


    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
    }
}