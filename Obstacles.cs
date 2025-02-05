using Godot;
using System.Collections.Generic;
using System.Linq;
using PirateJam.scenes.player;

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
            if (child is Bull bull) _bulls.Add(bull);
            else if (child is Bridge bridge) _bridges.Add(bridge);
            else if (child is Button button) _buttons.Add(button);
        }

        GetNode<Player>("../Player").Whip += OnPlayerWhip;
    }

    public void OnPlayerWhip(Vector2I whipPosition)
    {
        var claimedPits = new List<Vector2I>();
        var bulls = GetInteractableBulls();
        bulls.Sort(delegate(Bull a, Bull b)
        {
            var aDeltaX = whipPosition.X - a.Position.X;
            var aDeltaY = whipPosition.Y - a.Position.Y;
            
            var bDeltaX = whipPosition.X - b.Position.X;
            var bDeltaY = whipPosition.Y - b.Position.Y;
            
            var aHyp = Mathf.Pow(aDeltaX, 2) + Mathf.Pow(aDeltaY, 2);
            var bHyp = Mathf.Pow(bDeltaX, 2) + Mathf.Pow(bDeltaY, 2);

            if (aHyp > bHyp) return -1;
            return 1;
        });
  
        foreach (var bull in bulls )
        {
            var maybeClaimed = bull.MoveDueToWhip(whipPosition, claimedPits);
            if (maybeClaimed is { } claimed) claimedPits.Add(claimed);
        }
    }

    public List<Bull> GetInteractableBulls(bool status = true)
    {
        return _bulls.Where(bull => bull.IsInteractable == status).ToList();
    }

    public List<Bridge> GetBridges(bool isUp = true)
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


    public Bridge? GetBridgeWithStatus(Vector2I tile, TileMapLayer tileMap, bool isUp = true)
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

    public Bull? GetBullWithStatus(Vector2I tile, TileMapLayer tileMap, bool isInteractable = true)
    {
        var bulls = GetInteractableBulls(isInteractable);

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