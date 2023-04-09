using System.Collections.Generic;
using UnityEngine;
using System;

namespace DungeonForge.AlgoScript
{
    public class WFCRuleDecipher : MonoBehaviour
    {
        [HideInInspector]
        public Texture2D texture;

        public GameObject[] tileSet = new GameObject[0];

        [SerializeField]
        public List<WFCTileRule> ruleSet = new List<WFCTileRule>();

        [SerializeField]
        public List<TextColToObj> colToIntList = new List<TextColToObj>();

        [HideInInspector]
        public bool useText = false;


        [HideInInspector]
        public string ruleSetFileName = "";
        [HideInInspector]
        public string tileSetFileName = "";
    }

    [Serializable]
    public class WFCTileRule
    {
        public GameObject mainAsset;

        public int assetIdx;

        public List<int> allowedObjLeft = new List<int>();
        public List<int> allowedObjRight = new List<int>();
        public List<int> allowedObjAbove = new List<int>();
        public List<int> allowedObjBelow = new List<int>();
    }

    [Serializable]
    public class TextColToObj
    {
        public Color pixelColor;
        public int indexTileSet;
    }
}

