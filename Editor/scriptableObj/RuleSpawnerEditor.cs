using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.VersionControl;
using UnityEngine;



namespace DungeonForge
{
    public class RuleSpawnerEditor : Editor
    {
        //https://hugecalf-studios.github.io/unity-lessons/lessons/editor/menuitem/
        [MenuItem("PCG Algorithms/Spawn WFC Rule", priority = 24)]
        private static void SpawnWFCRule()
        {
            var asset = CreateInstance<GraphViewDataCont>();

            CheckResourceFolder();
            CheckMainAlgoFolder();

            if (!AssetDatabase.IsValidFolder("Assets/Resources/Resources_Algorithms/WFC_Rule_Sets"))
            {
                AssetDatabase.CreateFolder("Assets/Resources/Resources_Algorithms", "WFC_Rule_Sets");
                AssetDatabase.Refresh();
            }

            AssetDatabase.CreateAsset(asset, $"Assets/Resources/Resources_Algorithms/WFC_Rule_Sets/NewWFCRuleSet.asset");
            AssetDatabase.SaveAssets();
        }

        [MenuItem("PCG Algorithms/Spawn L-System Rule", priority = 25)]
        private static void SpawnLSystemRule()
        {
            var asset = CreateInstance<LSystemRuleObj>();


            CheckResourceFolder();
            CheckMainAlgoFolder();

            if (!AssetDatabase.IsValidFolder("Assets/Resources/Resources_Algorithms/L_system_Rule_Sets"))
            {
                AssetDatabase.CreateFolder("Assets/Resources/Resources_Algorithms", "L_system_Rule_Sets");
                AssetDatabase.Refresh();
            }

            AssetDatabase.CreateAsset(asset, $"Assets/Resources/Resources_Algorithms/L_system_Rule_Sets/NewLSystemRuleSet.asset");
            AssetDatabase.SaveAssets();
        }


        [MenuItem("PCG Algorithms/Spawn Weight Rule", priority = 26)]
        private static void SpawnWeightRule()
        {
            var asset = CreateInstance<WeightRuleSet>();


            CheckResourceFolder();
            CheckMainAlgoFolder();

            if (!AssetDatabase.IsValidFolder("Assets/Resources/Resources_Algorithms/Weight_Pathfinding_RuleSet"))
            {
                AssetDatabase.CreateFolder("Assets/Resources/Resources_Algorithms", "Weight_Pathfinding_RuleSet");
                AssetDatabase.Refresh();
            }

            AssetDatabase.CreateAsset(asset, $"Assets/Resources/Resources_Algorithms/Weight_Pathfinding_RuleSet/NewWeightRuleSet.asset");
            AssetDatabase.SaveAssets();
        }


        [MenuItem("PCG Algorithms/Spawn TileSet Rule", priority = 27)]
        private static void SpawnTileSetRule()
        {
            var asset = CreateInstance<TilesRuleSet>();

            CheckResourceFolder();
            CheckMainAlgoFolder();

            if (!AssetDatabase.IsValidFolder("Assets/Resources/Resources_Algorithms/Tile_Sets_Ruleset"))
            {
                AssetDatabase.CreateFolder("Assets/Resources/Resources_Algorithms", "Tile_Sets_Ruleset");
                AssetDatabase.Refresh();
            }

            AssetDatabase.CreateAsset(asset, $"Assets/Resources/Resources_Algorithms/Tile_Sets_Ruleset/NewTileSetRuleSet.asset");
            AssetDatabase.SaveAssets();
        }


        private static void CheckResourceFolder()
        {
            if (!AssetDatabase.IsValidFolder("Assets/Resources"))
            {
                AssetDatabase.CreateFolder("Assets", "Resources");
                AssetDatabase.Refresh();
            }
        }

        private static void CheckMainAlgoFolder()
        {
            if (!AssetDatabase.IsValidFolder("Assets/Resources/Resources_Algorithms"))
            {
                AssetDatabase.CreateFolder("Assets/Resources", "Resources_Algorithms");
                AssetDatabase.Refresh();
            }
        }
    }
}