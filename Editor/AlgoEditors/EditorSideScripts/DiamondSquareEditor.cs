namespace DungeonForge.Editor
{
    using System.Collections.Generic;
    using UnityEditor;
    using UnityEngine;
    using DungeonForge.Utils;
    using DungeonForge.AlgoScript;
    [CustomEditor(typeof(DiamondSquareMA))]
    public class DiamondSquareEditor : Editor
    {
        bool showRules = false;

        bool useWeights = false;
        //bool DjAvoidWalls = false;

        int corridorThickness = 2;

        int selGridConnectionType = 0;
        int selGridPathGenType = 0;

        int randomAddCorr = 0;

        int bezierOndulation = 20;
        int deadEndOndulation = 20;

        int deadEndAmount = 0;

        int radius = 10;

        int width = 10;
        int height = 10;

        string saveMapFileName = "";

        int heightDSA = 4;
        int roughnessDSA = 4;

        float weightClamp = 0.5f;

        int power = 6;


        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            DiamondSquareMA mainScript = (DiamondSquareMA)target;


            #region explanation

            showRules = EditorGUILayout.BeginFoldoutHeaderGroup(showRules, "Introduction");

            if (showRules)
            {
                GUILayout.TextArea("The Diamond-Square algorithm is a terrain generation algorithm that generates height maps through a recursive process. By interpreting height values as walls or floors, this algorithm can be used to create dungeons with a fractal-like appearance. It is particularly useful for generating dungeons with a more chaotic or unpredictable layout.\n\n Visit the wiki for more informations: https://github.com/AlessandroBufalino3115/Dungeon-Forge/wiki/Using-the-Pack#5-diamond-square");
            }

            if (!Selection.activeTransform)
            {
                showRules = false;
            }

            EditorGUILayout.EndFoldoutHeaderGroup();

            #endregion

            DFEditorUtil.SpacesUILayout(4);

            switch (mainScript.currUiState)
            {
                case DFEditorUtil.UI_STATE.MAIN_ALGO:
                    {
                        EditorGUILayout.HelpBox("To run this algorithm a specific size of a map is needed, use the slider below", MessageType.Warning);

                        power = (int)EditorGUILayout.Slider(new GUIContent() { text = "Size", tooltip = "Size of the canvas" }, power, 6, 10);
                        GUILayout.TextArea($"The current size of the new plane is will be {Mathf.Pow(2, power) + 1} by {Mathf.Pow(2, power) + 1}");

                        mainScript.allowedBack = false;
                        DFEditorUtil.SpacesUILayout(1);
                        heightDSA = (int)EditorGUILayout.Slider(new GUIContent() { text = "Height", tooltip = "Height determines the size of the grid in the Diamond-Square Algorithm. Larger values create a more detailed terrain with higher resolution." }, heightDSA, 4, 16);
                        roughnessDSA = (int)EditorGUILayout.Slider(new GUIContent() { text = "Roughness", tooltip = "Roughness controls the level of terrain roughness in the Diamond-Square Algorithm. Higher values result in a more rugged and irregular terrain surface." }, roughnessDSA, 1, 16);
                        weightClamp = EditorGUILayout.Slider(new GUIContent() { text = "Threashold", tooltip = "the minimum value as a percentage of the height that will be recognised as a tile" }, weightClamp, 0.2f, 0.8f);

                        if (GUILayout.Button("Generate Diamond Square Noise"))// gen something
                        {
                            DFGeneralUtil.RestartGrid(mainScript.pcgManager.gridArr);

                            mainScript.pcgManager.height = (int)Mathf.Pow(2, power) + 1;
                            mainScript.pcgManager.width = (int)Mathf.Pow(2, power) + 1;

                            mainScript.pcgManager.CreatePlane();

                            DFAlgoBank.DiamondSquare(heightDSA, -heightDSA, roughnessDSA, mainScript.pcgManager.gridArr);

                            float minWeight = Mathf.Lerp(-heightDSA, heightDSA, weightClamp);

                            for (int y = 0; y < mainScript.pcgManager.gridArr.GetLength(1); y++)
                            {
                                for (int x = 0; x < mainScript.pcgManager.gridArr.GetLength(0); x++)
                                {
                                    if (mainScript.pcgManager.gridArr[x, y].tileWeight > minWeight)
                                    {
                                        mainScript.pcgManager.gridArr[x, y].tileWeight = 1;
                                    }
                                    else
                                    {
                                        mainScript.pcgManager.gridArr[x, y].tileWeight = 0;
                                    }
                                }
                            }

                            mainScript.allowedForward = true;

                            mainScript.pcgManager.Plane.GetComponent<Renderer>().sharedMaterial.mainTexture = DFGeneralUtil.SetUpTextBiColAnchor(mainScript.pcgManager.gridArr);
                        }
                    }
                    break;

                case DFEditorUtil.UI_STATE.CA:
                    {
                        mainScript.allowedForward = true;
                        mainScript.allowedBack = true;

                        DFEditorUtil.CellularAutomataEditorSection(mainScript.pcgManager,ref  mainScript.neighboursNeeded);
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
                     , ref selGridConnectionType, ref selGridPathGenType, ref useWeights, ref bezierOndulation, ref mainScript.pathType, ref randomAddCorr, ref deadEndAmount, ref deadEndOndulation, ref mainScript.edges);

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
