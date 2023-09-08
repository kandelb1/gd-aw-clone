using Godot;
using System;
using System.Collections.Generic;

public partial class ValidActionPositionsCache : Node
{
    public static ValidActionPositionsCache Instance { get; private set; }

    private List<Vector2I> validPositions;

    public override void _Ready()
    {
        GD.Print("ValidActionPositionsCache _Ready()");
        if (Instance != null)
        {
            GD.Print("There is already an instance of ValidActionPositionsCache??");
            QueueFree();
            return;
        }
        Instance = this;

        // ValidMovesCache is an autoload, so it'll load and subscribe to this signal before anything else does
        UnitSystem.Instance.ActionSelected += HandleActionSelected;
    }

    private void HandleActionSelected(BaseAction action)
    {
        GD.Print("Action Cache HandleActionSelected()");
        validPositions = action.GetValidPositions();
    }

    public List<Vector2I> GetCachedPositions() => validPositions;
}
