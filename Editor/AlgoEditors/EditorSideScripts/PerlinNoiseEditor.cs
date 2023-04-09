namespace DungeonForge.Editor
{
    using System.Collections.Generic;
    using UnityEditor;
    using UnityEngine;
    using DungeonForge.Utils;
    using DungeonForge.AlgoScript;

    [CustomEditor(typeof(PerlinNoiseMA))]
    public class PerlinNoiseEditor : Editor
    {
        bool showRules = false;

        bool useWeights = false;
       // bool DjAvoidWalls = false;

        int corridorThickness = 2;

        int selGridConnectionType = 0;
        int selGridPathGenType = 0;

        int randomAddCorr = 0;

        int bezierOndulation = 20;
        int deadEndOndulation = 20;

        int deadEndAmount = 0;
        int deadEndCorridorThickness = 3;

        int radius = 10;

        int width = 10;
        int height = 10;

        string saveMapFileName = "";

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            PerlinNoiseMA mainScript = (PerlinNoiseMA)target;

            #region explanation

            showRules = EditorGUILayout.BeginFoldoutHeaderGroup(showRules, "Instructions");

            if (showRules)
            {
                GUILayout.TextArea("You have choosen Perlin noise Generation  ---   to write");

            }

            if (!Selection.activeTransform)
            {
                showRules = false;
            }

            EditorGUILayout.EndFoldoutHeaderGroup();

            #endregion

            DFEditorUtil.SpacesUILayout(4);

            #region Main Algo


            //mainScript.OffsetX = (int)EditorGUILayout.Slider(new GUIContent() { text = "Perlin Offset X", tooltip = "" }, mainScript.OffsetX, 0, 10000);
            //mainScript.OffsetY = (int)EditorGUILayout.Slider(new GUIContent() { text = "Perlin Offset Y", tooltip = "" }, mainScript.OffsetY, 0, 10000);


            //mainScript.Scale = EditorGUILayout.Slider(new GUIContent() { text = "Perlin Scale", tooltip = "" }, mainScript.Scale, 3f, 35f);
            //mainScript.Octaves = (int)EditorGUILayout.Slider(new GUIContent() { text = "Perlin Octaves", tooltip = "" }, mainScript.Octaves, 1, 8);

            //mainScript.Persistance = EditorGUILayout.Slider(new GUIContent() { text = "Perlin Persitance", tooltip = "" }, mainScript.Persistance, 0.1f, 0.9f);
            //mainScript.Lacunarity = EditorGUILayout.Slider(new GUIContent() { text = "Perlin Lacunarity", tooltip = "" }, mainScript.Lacunarity, 0.5f, 10f);

            //mainScript.Threshold = EditorGUILayout.Slider(new GUIContent() { text = "Max Threshold", tooltip = "" }, mainScript.Threshold, 0.1f, 0.9f);


            //if (GUILayout.Button("Generate Noise"))
            //{
            //    mainScript.PcgManager.gridArr = AlgosUtils.PerlinNoise2D(mainScript.PcgManager.gridArr, mainScript.Scale, mainScript.Octaves, mainScript.Persistance, mainScript.Lacunarity, mainScript.OffsetX, mainScript.OffsetY, mainScript.Threshold);

            //    mainScript.PcgManager.Plane.GetComponent<Renderer>().sharedMaterial.mainTexture = GeneralUtil.SetUpTextBiColAnchor(mainScript.PcgManager.gridArr,true);

            //    mainScript.Started = true;
            //}


            #endregion


            switch (mainScript.currUiState)
            {
                case DFEditorUtil.UI_STATE.MAIN_ALGO:
                    {
                        mainScript.allowedBack = false;
                        mainScript.OffsetX = (int)EditorGUILayout.Slider(new GUIContent() { text = "Perlin Offset X", tooltip = "" }, mainScript.OffsetX, 0, 10000);
                        mainScript.OffsetY = (int)EditorGUILayout.Slider(new GUIContent() { text = "Perlin Offset Y", tooltip = "" }, mainScript.OffsetY, 0, 10000);


                        mainScript.Scale = EditorGUILayout.Slider(new GUIContent() { text = "Perlin Scale", tooltip = "" }, mainScript.Scale, 3f, 35f);
                        mainScript.Octaves = (int)EditorGUILayout.Slider(new GUIContent() { text = "Perlin Octaves", tooltip = "" }, mainScript.Octaves, 1, 8);

                        mainScript.Persistance = EditorGUILayout.Slider(new GUIContent() { text = "Perlin Persitance", tooltip = "" }, mainScript.Persistance, 0.1f, 0.9f);
                        mainScript.Lacunarity = EditorGUILayout.Slider(new GUIContent() { text = "Perlin Lacunarity", tooltip = "" }, mainScript.Lacunarity, 0.5f, 10f);

                        mainScript.Threshold = EditorGUILayout.Slider(new GUIContent() { text = "Max Threshold", tooltip = "" }, mainScript.Threshold, 0.1f, 0.9f);

                        if (GUILayout.Button("Generate Noise"))
                        {
                            DFAlgoBank.PerlinNoise(mainScript.pcgManager.gridArr, mainScript.Scale, mainScript.Octaves, mainScript.Persistance, mainScript.Lacunarity, mainScript.OffsetX, mainScript.OffsetY, mainScript.Threshold);

                            mainScript.pcgManager.Plane.GetComponent<Renderer>().sharedMaterial.mainTexture = DFGeneralUtil.SetUpTextBiColAnchor(mainScript.pcgManager.gridArr, true);

                            mainScript.allowedForward = true;
                        }
                    }
                    break;

                case DFEditorUtil.UI_STATE.CA:
                    {
                        mainScript.allowedForward = true;
                        mainScript.allowedBack = true;

                        DFEditorUtil.CellularAutomataEditorSection(mainScript.pcgManager,ref mainScript.neighboursNeeded);
                    }
                    break;

                case DFEditorUtil.UI_STATE.ROOM_GEN:
                    {
                        mainScript.allowedBack = true;

                        List<List<DFTile>> rooms;
                        if (DFEditorUtil.CalculateRoomsEditorSection(mainScript.pcgManager,ref mainScript.minSize, out rooms))
                        {
                            mainScript.allowedForward = true;
                        }

                        if (rooms != null)
                        {
                            mainScript.rooms = rooms;
                        }
                    }
                    break;

                case DFEditorUtil.UI_STATE.EXTRA_ROOM_GEN:
                    {
                        mainScript.allowedForward = true;
                        mainScript.allowedBack = false;

                        DFEditorUtil.ExtraRoomEditorSelection(mainScript.pcgManager, mainScript.rooms, ref radius, ref height, ref width);
                    }
                    break;

                case DFEditorUtil.UI_STATE.PATHING:

                    #region corridor making region
                    mainScript.allowedBack = true;

                    DFEditorUtil.GenerateCorridorsEditorSection(mainScript.pcgManager, mainScript.rooms, ref mainScript.allowedForward, ref mainScript.allowedBack, ref corridorThickness
                     , ref selGridConnectionType, ref selGridPathGenType, ref useWeights, ref bezierOndulation, ref mainScript.pathType, ref randomAddCorr, ref deadEndAmount, ref deadEndCorridorThickness, ref deadEndOndulation, ref mainScript.edges);


                    #endregion

                    break;

                case DFEditorUtil.UI_STATE.GENERATION:
                    {
                        mainScript.allowedBack = true;

                        DFEditorUtil.SaveGridDataToGenerateEditorSection(mainScript.pcgManager, saveMapFileName, out saveMapFileName);
                    }

                    break;

                default:
                    break;
            }



            if (mainScript.currUiState != DFEditorUtil.UI_STATE.GENERATION)
            {
                DFEditorUtil.SpacesUILayout(4);

                EditorGUI.BeginDisabledGroup(mainScript.allowedBack == false);

                if (GUILayout.Button(new GUIContent() { text = "Go Back", tooltip = mainScript.allowedForward == true ? "Press this to go back one step" : "You cant go back" }))// gen something
                {
                    mainScript.pcgManager.ClearUndos();
                    mainScript.allowedBack = false;
                    mainScript.currStateIndex--;
                    mainScript.currUiState = (DFEditorUtil.UI_STATE)mainScript.currStateIndex;
                }

                EditorGUI.EndDisabledGroup();



                EditorGUI.BeginDisabledGroup(mainScript.allowedForward == false);

                if (GUILayout.Button(new GUIContent() { text = "Continue", tooltip = mainScript.allowedForward == true ? "Press this to continue to the next step" : "You need to finish this step to continue" }))// gen something
                {
                    mainScript.pcgManager.ClearUndos();
                    mainScript.allowedForward = false;
                    mainScript.currStateIndex++;
                    mainScript.currUiState = (DFEditorUtil.UI_STATE)mainScript.currStateIndex;

                }

                EditorGUI.EndDisabledGroup();
            }





        }
    }
}