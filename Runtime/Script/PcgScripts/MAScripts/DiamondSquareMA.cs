using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace DungeonForge.AlgoScript
{
    using DungeonForge.Utils;

    public class DiamondSquareMA : MonoBehaviour, IUndoInteraction
    {
        [HideInInspector]
        public PCGManager pcgManager;

        //general

        [HideInInspector]
        public bool pathType = false;

        [HideInInspector]
        public int neighboursNeeded = 3;

        [HideInInspector]
        public int typeOfTri;

        [HideInInspector]
        public int minSize = 40;

        [HideInInspector]
        public List<List<DFTile>> rooms = new List<List<DFTile>>();

        [HideInInspector]
        public List<Edge> edges = new List<Edge>();

        [HideInInspector]
        public bool allowedBack;
        [HideInInspector]
        public bool allowedForward;
        [HideInInspector]
        public int currStateIndex = 0;



        [HideInInspector]
        public DFGeneralUtil.PathFindingType pathFindingType;

        [HideInInspector]
        public DFGeneralUtil.UI_STATE currUiState = DFGeneralUtil.UI_STATE.MAIN_ALGO;

        public void DeleteLastSavedRoom()
        {
            if (currUiState == DFGeneralUtil.UI_STATE.EXTRA_ROOM_GEN)
                rooms.RemoveAt(rooms.Count - 1);
        }

        public void InspectorAwake()
        {
            pcgManager = this.transform.GetComponent<PCGManager>();
            pcgManager.UndoInteraction = this;
        }

    }
}