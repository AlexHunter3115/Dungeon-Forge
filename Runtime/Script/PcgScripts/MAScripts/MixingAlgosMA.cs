
using DungeonForge.Utils;
using System.Collections.Generic;
using UnityEngine;


namespace DungeonForge.AlgoScript
{
    public class MixingAlgosMA : MonoBehaviour
    {
        [HideInInspector]
        public PCGManager PcgManager;

        [HideInInspector]
        public List<List<DFTile>> rooms = new List<List<DFTile>>();

        public void InspectorAwake()
        {
            PcgManager = this.transform.GetComponent<PCGManager>();
        }
    }
}
