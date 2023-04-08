using System;
using System.Collections.Generic;
using UnityEngine;



[Serializable]

public class TilesRuleSet : ScriptableObject
{
    public List<TileRuleSet> FloorTiles = new List<TileRuleSet>();
    public List<TileRuleSet> CeilingTiles = new List<TileRuleSet>();
    public List<TileRuleSet> WallsTiles = new List<TileRuleSet>();
}



[Serializable]
public class TileRuleSet 
{
    public GameObject Tile;
    public int occurance = 1;
}