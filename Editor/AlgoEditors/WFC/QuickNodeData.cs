using DS.Enumerations;
using System;
using UnityEngine;



[Serializable]
public class QuickNodeData
{
    public Vector2 position;
    public string nodeGuid;
    public string IndexTile;
    public bool IsOpenRight;
    public bool IsOpenLeft;
    public bool IsOpenAbove;
    public bool IsOpenBelow;
    public DSDialogueType dialogueType;
}
