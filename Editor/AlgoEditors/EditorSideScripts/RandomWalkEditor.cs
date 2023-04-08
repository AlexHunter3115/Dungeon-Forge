

namespace DungeonForge.Editor
{
    using System.Collections.Generic;
    using UnityEditor;
    using UnityEngine;
    using DungeonForge.Utils;
    using DungeonForge.AlgoScript;

    [CustomEditor(typeof(RandomWalkMA))]
    public class RandomWalkEditor : Editor
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
            RandomWalkMA mainScript = (RandomWalkMA)target;

            #region explanation

            showRules = EditorGUILayout.BeginFoldoutHeaderGroup(showRules, "Instructions");

            if (showRules)
            {
                GUILayout.TextArea("You have choosen RandomWalk algorithm as your initial algorithm\n\nExplenation: a \"Head\" moves in a random direction at each step\n\nStep 1: Decide how many iterations the algorithm will have to generate the base map and its sub parameters." +
                    "\n\nStep 2: To round up the rough edges you can decide to use Cellular Automata to help smooth things out." +
                    "\n\nStep 3: It is possible small rooms that are not your what you are looking for have been geenrated, delete them using by setting up the minimum amount of tiles a room should have." +
                    "\n\nStep 4: Depending on the amount of rooms you are able to create corridors by choosing the wanted pathFinding algorithm and the algortihm which decideds which room is connected to which." +
                    "\n\nStep 5: Generate the algorithm using the tileSet provided or create the blank gameobject whihc can then be exported and manipulated");
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
                        mainScript.allowedBack = false;

                        mainScript.iterations = (int)EditorGUILayout.Slider(new GUIContent() { text = "Iterations", tooltip = "This is how many times the head of the algorithm is going to move" }, mainScript.iterations, (mainScript.pcgManager.gridArr.GetLength(1) * mainScript.pcgManager.gridArr.GetLength(0)) * 0.3f, (mainScript.pcgManager.gridArr.GetLength(1) * mainScript.pcgManager.gridArr.GetLength(0)) * 0.9f);

                        mainScript.startFromMiddle = EditorGUILayout.Toggle(new GUIContent() { text = "Should The algo start from the middle", tooltip = "Should the head of the algorithm start from the middle of the canvas or a random position?" }, mainScript.startFromMiddle); ;
                        mainScript.alreadyPassed = EditorGUILayout.Toggle(new GUIContent() { text = "Overlap cells count", tooltip = mainScript.alreadyPassed == true ? "When the head of the walker goes over an already populated cells the iteration still counts" : "When the head of the walker goes over an already populated cells the iteration does not count" }, mainScript.alreadyPassed);

                        if (GUILayout.Button("Generate RandomWalk Randomisation"))// gen something
                        {
                            DFGeneralUtil.RestartGrid(mainScript.pcgManager.gridArr);
                            DFAlgoBank.RandomWalk(mainScript.pcgManager.gridArr,mainScript.iterations, !mainScript.alreadyPassed, randomStart: !mainScript.startFromMiddle);
                            mainScript.pcgManager.Plane.GetComponent<Renderer>().sharedMaterial.mainTexture = DFGeneralUtil.SetUpTextBiColAnchor(mainScript.pcgManager.gridArr);

                            mainScript.allowedForward = true;
                        }
                    }
                    break;

                case DFEditorUtil.UI_STATE.CA:
                    {
                        mainScript.allowedForward = true;
                        mainScript.allowedBack = true;

                        DFEditorUtil.CellularAutomataEditorSection(mainScript.pcgManager, ref mainScript.neighboursNeeded);
                    }
                    break;

                case DFEditorUtil.UI_STATE.ROOM_GEN:
                    {
                        mainScript.allowedBack = true;

                        List<List<DFTile>> rooms;
                        if (DFEditorUtil.CalculateRoomsEditorSection(mainScript.pcgManager, ref mainScript.minSize, out rooms))
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

                        DFEditorUtil.ExtraRoomEditorSelection(mainScript.pcgManager, mainScript.rooms,ref radius,ref height,ref width);
                    }
                    break;

                case DFEditorUtil.UI_STATE.PATHING:

                    #region corridor making region

                    DFEditorUtil.GenerateCorridorsEditorSection(mainScript.pcgManager, mainScript.rooms, ref mainScript.allowedForward, ref mainScript.allowedBack, ref corridorThickness
                     , ref selGridConnectionType, ref selGridPathGenType, ref useWeights, ref bezierOndulation, ref mainScript.pathType, ref randomAddCorr, ref deadEndAmount, ref deadEndCorridorThickness, ref deadEndOndulation, ref mainScript.edges);


                    #endregion

                    break;

                case DFEditorUtil.UI_STATE.GENERATION:
                    {
                        mainScript.allowedBack = true;

                        DFEditorUtil.SaveGridDataToGenerate(mainScript.pcgManager, saveMapFileName, out saveMapFileName);
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