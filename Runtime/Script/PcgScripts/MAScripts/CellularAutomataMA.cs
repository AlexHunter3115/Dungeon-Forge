using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


namespace DungeonForge.AlgoScript
{

    using DungeonForge.Utils;

    public class CellularAutomataMA : MonoBehaviour, IUndoInteraction
    {
        [HideInInspector]
        public PCGManager pcgManager;


        //specific to main algo
        [HideInInspector]
        public int iterations;

        [HideInInspector]
        public bool startFromMiddle = false;

        [HideInInspector]
        public bool alreadyPassed;


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



        public enum UI_STATE
        {
            MAIN_ALGO,
            ROOM_GEN,
            EXTRA_ROOM_GEN,
            PATHING,
            GENERATION
        }
        [HideInInspector]
        public UI_STATE state;


        public void DeleteLastSavedRoom()
        {
            if (state == UI_STATE.EXTRA_ROOM_GEN)
                rooms.RemoveAt(rooms.Count - 1);
        }

        public void InspectorAwake()
        {
            pcgManager = this.transform.GetComponent<PCGManager>();
            pcgManager.UndoInteraction = this;
        }

    }
}