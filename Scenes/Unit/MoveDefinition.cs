using Godot;
using Godot.Collections;

[GlobalClass]
public partial class MoveDefinition : Resource
{

    [Export] private int moveDistance;
    [Export] private Dictionary<string, int> terrainMoveCosts;

    // public MoveDefinition()
    // {
    //     // default move dist is 3 I guess
    //     moveDistance = 3;
    //     // default move costs are 1 for everything 
    //     terrainMoveCosts = new Dictionary<string, int>()
    //     {
    //         {"sea", 1},
    //         {"river", 1},
    //         {"road", 1},
    //         {"plains", 1},
    //         {"mountains", 1},
    //         {"woods", 1},
    //     };
    // }

    public int GetMoveDistance() => moveDistance;

    public void SetMoveDistance(int moveDistance) => this.moveDistance = moveDistance;

    public int GetMoveCostForTerrain(string terrain)
    {
        return terrainMoveCosts.ContainsKey(terrain) ? terrainMoveCosts[terrain] : 0;
    }
}
