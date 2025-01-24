using Godot;
using System.Collections.Generic;
using System.Linq;
using PirateJam;

public partial class Obstacles : Node
{
	private List<Bull> _interactables { get; set; }
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_interactables = new List<Bull>();
		var children = GetChildren();
		foreach (var child in children)
		{
			//TODO there's probably a cleaner way to do this
			if (child is Bull bull)
			{
				_interactables.Add(bull);
			}
		}
	}

	public List<Bull> GetInteractables()
	{
		return _interactables.Where(bull => bull.IsInteractable).ToList();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
