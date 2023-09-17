using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class UnitSystem : Node
{

    [Signal]
    public delegate void UnitSelectedEventHandler(Unit unit);

    [Signal]
    public delegate void UnitDeselectedEventHandler();

    [Signal]
    public delegate void ActionSelectedEventHandler(BaseAction action);

    [Signal]
    public delegate void ActionDeselectedEventHandler();

    // [Signal]
    // public delegate void ActionCompletedEventHandler();
    
    [Signal]
    public delegate void BuildingSelectedEventHandler(BuildingDefinition buildingDef);

    public static UnitSystem Instance { get; private set; }

    [Export] private PackedScene unitActionMenu;
    
    
    private Unit selectedUnit;
    private Vector2I originalPosition;
    private BaseAction selectedAction;

    private Node baseUI;
    private AudioStreamPlayer audioPlayer;

    public override void _Ready()
    {
        if (Instance != null)
        {
            GD.Print("There is already an instance of UnitSystem??");
            QueueFree();
            return;
        }
        Instance = this;
        
        baseUI = GetNode<Node>("/root/main/UI"); // TODO: should I really be doing this?
        audioPlayer = GetNode<AudioStreamPlayer>("AudioStreamPlayer");
        audioPlayer.Stream = ResourceLoader.Load<AudioStreamWav>("res://Assets/Sounds/error.wav");

        ActionEventBus.Instance.ActionCompleted += HandleActionCompleted;
        GD.Print("UnitSystem._Ready()");
    }

    public bool IsUnitSelected() => selectedUnit != null;

    private void SetSelectedUnit(Unit unit)
    {
        selectedUnit = unit;
        originalPosition = unit.GetGridPosition();
        // emit UnitSelected BEFORE ActionSelected so the action has a chance...
        // ...to calculate its valid positions BEFORE TileHighlighter asks for them
        EmitSignal(SignalName.UnitSelected, unit);
        SetSelectedAction(unit.GetAction<MoveAction>());
    }

    public bool IsActionSelected() => selectedAction != null;

    public BaseAction GetSelectedAction() => selectedAction;

    public void SetSelectedAction(BaseAction action)
    {
        GD.Print($"Setting selected action to {action.Name}");
        selectedAction = action;
        EmitSignal(SignalName.ActionSelected, selectedAction);
    }

    private void DeselectAction()
    {
        selectedAction = null;
        EmitSignal(SignalName.ActionDeselected);
    }
    
    private void TrySelectUnit(Vector2I pos)
    {
        if (!Level.Instance.IsOccupied(pos)) return;
        Unit unit = Level.Instance.GetUnit(pos);
        if (unit.IsExhausted()) return;
        SetSelectedUnit(unit);
    }

    private void DeselectUnit(bool resetPosition = false)
    {
        // move the unit back to its starting position
        if (resetPosition)
        {
            selectedUnit.SetGridPosition(originalPosition);
            selectedUnit.SetMoved(false);
        }
        selectedUnit = null;
        selectedAction = null;
        EmitSignal(SignalName.UnitDeselected);
    }

    private void HandleActionCompleted()
    {
        DeselectAction();
        if (selectedUnit.IsExhausted())
        {
            DeselectUnit();
            return;
        }
        // show the menu again
        Vector2 position = Level.Instance.MapToLocal(selectedUnit.GetGridPosition());
        List<BaseAction> availableActions = selectedUnit.GetActions().Where(x => x.IsActionAvailable()).ToList();
        CreateActionMenuUI(position, availableActions);
    }

    private void CreateActionMenuUI(Vector2 position, List<BaseAction> availableActions)
    {
        UnitActionMenu menu = unitActionMenu.Instantiate<UnitActionMenu>();
        menu.Position = position;
        menu.SetActions(availableActions);
        menu.ActionSelected += SetSelectedAction;
        menu.MenuClosed += HandleMenuClosed;
        baseUI.AddChild(menu);
    }

    private void HandleMenuClosed()
    {
        // the menu was closed without choosing an action, so reset the selectedUnit's position
        DeselectUnit(true);
    }

    // use UnhandledInput so that any GUI showing on top of the level can consume the input first
    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event.IsActionPressed("left click"))
        {
            InputEventMouseButton e = (InputEventMouseButton) @event;
            Vector2I clickPos = Level.Instance.GetGridPosition(e.Position);

            if (!IsUnitSelected()) // try selecting a unit
            {
                TrySelectUnit(clickPos);
            }
            else // see if wherever we clicked is valid
            {
                if (!IsActionSelected()) return;
                if (!selectedAction.IsValidPosition(clickPos))
                {
                    audioPlayer.Play(); // TODO: UnitSystem shouldn't be responsible for playing audio
                    return;
                }
                selectedAction.TakeAction(clickPos);
            }
        }else if (@event.IsActionPressed("right click"))
        {
            if (!IsUnitSelected()) return;
            DeselectUnit(true);
        }
    }
}
