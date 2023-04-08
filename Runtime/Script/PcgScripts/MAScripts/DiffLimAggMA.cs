using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


namespace DungeonForge.AlgoScript
{

    public class DiffLimAggMA : MonoBehaviour
    {

        [HideInInspector]
        public PCGManager pcgManager;

        [HideInInspector]
        public bool generatedBool;
        [HideInInspector]
        public bool allowedBack;

        public void InspectorAwake()
        {
            pcgManager = this.transform.GetComponent<PCGManager>();
        }

    }
}