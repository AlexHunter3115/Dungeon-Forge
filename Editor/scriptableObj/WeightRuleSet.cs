using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeightRuleSet : ScriptableObject
{
    [Header("Chooses the weighs for the Pathfinding algorithms")]

    public float VOID = 0;
    public float FLOORROOM = 0;
    public float WALL = 0;
    public float ROOF = 0;
    public float FLOORCORRIDOR = 0;
    public float AVOID = 0;
}
