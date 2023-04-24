using System.Collections.Generic;
using UnityEngine;

namespace DungeonForge.AlgoScript
{
    using DungeonForge.Utils;

    public class VoronoiMA : MonoBehaviour, IUndoInteraction
    {
        [HideInInspector]
        public PCGManager pcgManager;

        [HideInInspector]
        public DFEditorUtil.UI_STATE currUiState = DFEditorUtil.UI_STATE.MAIN_ALGO;

        [HideInInspector]
        public bool allowedBack = false;
        [HideInInspector]
        public bool allowedForward = false;

        [HideInInspector]
        public int currStateIndex = 0;

        [HideInInspector]
        public List<List<DFTile>> rooms = new List<List<DFTile>>();


        [HideInInspector]
        public DFEditorUtil.PathFindingType pathFindingType;
        [HideInInspector]
        public bool pathType = false;

        [HideInInspector]
        public List<Edge> edges = new List<Edge>();

        [HideInInspector]
        public bool typeOfVoronoi = false;
        
        public void DeleteLastSavedRoom()
        {
            if (currUiState == DFEditorUtil.UI_STATE.EXTRA_ROOM_GEN)
                rooms.RemoveAt(rooms.Count - 1);
        }

        public void InspectorAwake()
        {
            pcgManager = this.transform.GetComponent<PCGManager>();
            pcgManager.UndoInteraction = this;
        }
    }
}