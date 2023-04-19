using System.Collections.Generic;
using UnityEngine;

namespace DungeonForge.AlgoScript
{
    using DungeonForge.Utils;

    public class PerlinNoiseMA : MonoBehaviour, IUndoInteraction
    {
        [HideInInspector]
        public PCGManager pcgManager;

        // main algo specific
        private int offsetX;
        public int OffsetX
        {
            get { return offsetX; }
            set { offsetX = value; }
        }

        private int offsetY;
        public int OffsetY
        {
            get { return offsetY; }
            set { offsetY = value; }
        }


        private float scale;
        public float Scale
        {
            get { return scale; }
            set { scale = value; }
        }

        private int octaves;
        public int Octaves
        {
            get { return octaves; }
            set { octaves = value; }
        }

        private float persistance;
        public float Persistance
        {
            get { return persistance; }
            set { persistance = value; }
        }

        private float lacunarity;
        public float Lacunarity
        {
            get { return lacunarity; }
            set { lacunarity = value; }
        }

        private float threshold;
        public float Threshold
        {
            get { return threshold; }
            set { threshold = value; }
        }





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
        public DFEditorUtil.PathFindingType pathFindingType;


        [HideInInspector]
        public DFEditorUtil.UI_STATE currUiState = DFEditorUtil.UI_STATE.MAIN_ALGO;

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