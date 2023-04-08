using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace DungeonForge.AlgoScript
{
    using DungeonForge.Utils;

    public class DelunaryMA : MonoBehaviour, IUndoInteraction
    {
        [HideInInspector]
        public PCGManager pcgManager;

        [HideInInspector]
        public bool readyToGen = false;

        [HideInInspector]
        public List<List<DFTile>> rooms = new List<List<DFTile>>();


        public enum UI_STATE
        {
            STAGE_1,
            STAGE_2,
            GENERATION
        }

        [HideInInspector]
        public UI_STATE state;


        [HideInInspector]
        public bool allowedForward = false;
        [HideInInspector]
        public bool allowedBack = false;

        [HideInInspector]
        public int currStateIndex;


        [HideInInspector]
        public bool generatedCorridors = false;


        public void InspectorAwake()
        {
            pcgManager = transform.GetComponent<PCGManager>();
            pcgManager.UndoInteraction = this;
        }

        public void DeleteLastSavedRoom()
        {
            if (state == UI_STATE.STAGE_1)
                rooms.RemoveAt(rooms.Count - 1);

            if (state == UI_STATE.STAGE_2)
                generatedCorridors = false;

        }
    }
}