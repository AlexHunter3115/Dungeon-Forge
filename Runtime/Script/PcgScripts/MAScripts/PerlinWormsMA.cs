using System.Collections.Generic;
using UnityEngine;


namespace DungeonForge.AlgoScript
{
    using DungeonForge.Utils;

    public class PerlinWormsMA : MonoBehaviour, IUndoInteraction
    {
        [HideInInspector]
        public PCGManager pcgManager;

        [HideInInspector]
        public HashSet<DFTile> wormsTiles = new HashSet<DFTile>();

        // main algo specific
        [HideInInspector]
        public int offsetX;

        [HideInInspector]
        public int offsetY;

        [HideInInspector]
        public float scale;

        [HideInInspector]
        public int octaves;

        [HideInInspector]
        public float persistance;

        [HideInInspector]
        public float lacunarity;

        [HideInInspector]
        public List<List<DFTile>> rooms = new List<List<DFTile>>();

        [HideInInspector]
        public bool allowedBack;
        [HideInInspector]
        public bool allowedForward;

        [HideInInspector]
        public int currStateIndex = 0;
        [HideInInspector]
        public int numberOfWorms = 0;


        [HideInInspector]
        public DFEditorUtil.PathFindingType pathFindingType;
        [HideInInspector]
        public bool pathType = false;

        [HideInInspector]
        public List<Edge> edges = new List<Edge>();

        public enum UI_STATE
        {
            WORM_CREATION,
            EXTRA_ROOM,
            PATHING,
            GENERATE
        }
        [HideInInspector]
        public UI_STATE currUiState = UI_STATE.WORM_CREATION;


        public void DeleteLastSavedRoom()
        {
            if (currUiState == UI_STATE.EXTRA_ROOM)
                rooms.RemoveAt(rooms.Count - 1);
            else if (currUiState == UI_STATE.WORM_CREATION)
                numberOfWorms--;
        }

        public void InspectorAwake()
        {
            pcgManager = this.transform.GetComponent<PCGManager>();
            pcgManager.UndoInteraction = this;
        }

    }
}