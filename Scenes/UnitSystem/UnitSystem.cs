using Godot;
using System;
using System.Linq;

public partial class UnitSystem : Node
{

    [Signal]
    public delegate void UnitDeselectedEventHandler();

    [Signal]
    public delegate void ActionSelectedEventHandler(BaseAction action);
    
    [Signal]
    public delegate void ActionTakenEventHandler();

    public static UnitSystem Instance { get; private set; }

    private PackedScene unitActionMenu;
    
    
    private Unit selectedUnit;
    private BaseAction selectedAction;

    private Node baseUI;

    public override void _Ready()
    {
        GD.Print("UnitSystem _Ready()");
        if (Instance != null)
        {
            GD.Print("There is already an instance of UnitSystem??");
            QueueFree();
            return;
        }
        Instance = this;

        unitActionMenu = (PackedScene) GD.Load("res://Scenes/UI/UnitActionMenu/UnitActionMenu.tscn");
        baseUI = GetNode<Node>("/root/main/UI"); // TODO: should I really be doing this?
        GD.Print($"Base ui: {baseUI.Name}");
    }

    public override void _Input(InputEvent @event)
    {
        if (@event.IsActionPressed("left click"))
        {
            // GD.Print("left click");
            InputEventMouseButton e = (InputEventMouseButton) @event;
            Vector2I clickPos = Level.Instance.GetGridPosition(e.Position);

            if (selectedUnit != null)
            {
                if (selectedAction == null) return;
                if (ValidActionPositionsCache.Instance.GetCachedPositions().Contains(clickPos))
                {
                    // take the action
                    selectedAction.TakeAction(clickPos);
                    DeselectUnit();
                }
                else
                {
                    // deselect unit
                    DeselectUnit();
                }
            }
            else
            {
                // otherwise try to select the unit on the grid position we clicked
                if (!Level.Instance.IsOccupied(clickPos)) return;
                Unit unit = Level.Instance.GetUnit(clickPos);
                if (unit == selectedUnit) return;
                if (unit.IsExhausted()) return;
                
                GD.Print($"clicked on {unit.Name}");
                selectedUnit = unit;
                UnitActionMenu menu = unitActionMenu.Instantiate() as UnitActionMenu;
                menu.Position = unit.GetPosition() + new Vector2(10, 0);
                menu.SetActions(unit.GetActions());
                menu.ActionSelected += HandleActionSelected;
                menu.MenuClosed += DeselectUnit;
                baseUI.AddChild(menu);
            }
        }else if (@event.IsActionPressed("right click")) // lets use right click to cancel for now
        {
            DeselectUnit();
        }
    }

    public override void _Process(double delta)
    {
        selectedAction?.Update(delta);
    }

    private void HandleActionSelected(BaseAction action)
    {
        GD.Print($"action selected: {action.GetActionName()}");
        selectedAction = action;
        EmitSignal(SignalName.ActionSelected, action);
    }

    public void DeselectUnit()
    {
        selectedUnit = null;
        selectedAction = null;
        EmitSignal(SignalName.UnitDeselected);
    }

    public bool IsUnitSelected() => selectedUnit != null;

    public Unit GetSelectedUnit() => selectedUnit;

    public BaseAction GetSelectedAction() => selectedAction;
}
