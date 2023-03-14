using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class GraphViewDataCont : ScriptableObject
{
    public List<NodeData> nodeData = new List<NodeData>();
    public List<QuickNodeData> quickNodeData = new List<QuickNodeData>();
    public List<NodeLinkData> nodeLinkData = new List<NodeLinkData>();
}

