namespace DungeonForge.Editor
{
    using System.Collections.Generic;
    using UnityEditor;
    using UnityEngine;
    using DungeonForge.Utils;
    using DungeonForge.AlgoScript;


    [CustomEditor(typeof(CellularAutomataMA))]
    public class CellularAutomataEditor : Editor
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
        int deadEndCorridorThickness = 3;

        int radius = 10;

        int width = 10;
        int height = 10;

        string saveMapFileName = "";

        bool started = false;
        float ranVal = 0.5f;

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            CellularAutomataMA mainScript = (CellularAutomataMA)target;


            #region explanation

            showRules = EditorGUILayout.BeginFoldoutHeaderGroup(showRules, "Instructions");

            if (showRules)
            {
                GUILayout.TextArea("You have chosen CA");

            }

            if (!Selection.activeTransform)
            {
                showRules = false;
            }

            EditorGUILayout.EndFoldoutHeaderGroup();

            #endregion


            DFEditorUtil.SpacesUILayout(4);



            switch (mainScript.state)
            {
                case CellularAutomataMA.UI_STATE.MAIN_ALGO:

                    mainScript.allowedBack = false;

                    ranVal = EditorGUILayout.Slider(new GUIContent() { text = "", tooltip = "" }, ranVal, 0.3f, 0.7f);


                    if (GUILayout.Button(new GUIContent() { text = "Start CA", tooltip = "" }))
                    {
                        DFAlgoBank.SpawnRandomPointsOnTheGrid(mainScript.pcgManager.gridArr, ranVal);
                        mainScript.pcgManager.Plane.GetComponent<Renderer>().sharedMaterial.mainTexture = DFGeneralUtil.SetUpTextBiColShade(mainScript.pcgManager.gridArr, 0, 1, true);

                        started = true;
                        mainScript.allowedForward = true;
                    }

                    if (started)
                        DFEditorUtil.CellularAutomataEditorSection(mainScript.pcgManager, ref mainScript.neighboursNeeded);


                    break;

                case CellularAutomataMA.UI_STATE.ROOM_GEN:
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

                case CellularAutomataMA.UI_STATE.EXTRA_ROOM_GEN:
                    {
                        mainScript.allowedForward = true;
                        mainScript.allowedBack = false;

                        DFEditorUtil.ExtraRoomEditorSelection(mainScript.pcgManager, mainScript.rooms, ref radius, ref height, ref width);
                    }

                    break;

                case CellularAutomataMA.UI_STATE.PATHING:

                    #region corridor making region
                    mainScript.allowedBack = true;

                    DFEditorUtil.GenerateCorridorsEditorSection(mainScript.pcgManager, mainScript.rooms, ref mainScript.allowedForward, ref mainScript.allowedBack, ref corridorThickness
                        , ref selGridConnectionType, ref selGridPathGenType,ref useWeights, ref bezierOndulation, ref mainScript.pathType, ref randomAddCorr, ref deadEndAmount, ref deadEndCorridorThickness,ref deadEndOndulation,ref mainScript.edges );


                    #endregion

                    break;
                case CellularAutomataMA.UI_STATE.GENERATION:


                    mainScript.allowedBack = false;
                    mainScript.allowedForward = false;

                    DFEditorUtil.SaveGridDataToGenerate(mainScript.pcgManager, saveMapFileName, out saveMapFileName);



                    break;

                default:
                    break;
            }



            if (mainScript.state != CellularAutomataMA.UI_STATE.GENERATION)
            {
                DFEditorUtil.SpacesUILayout(4);

                EditorGUI.BeginDisabledGroup(mainScript.allowedBack == false);

                if (GUILayout.Button(new GUIContent() { text = "Go Back", tooltip = mainScript.allowedForward == true ? "Press this to go back one step" : "You cant go back" }))// gen something
                {
                    mainScript.pcgManager.ClearUndos();
                    mainScript.allowedBack = false;
                    mainScript.currStateIndex--;
                    mainScript.state = (CellularAutomataMA.UI_STATE)mainScript.currStateIndex;
                }

                EditorGUI.EndDisabledGroup();



                EditorGUI.BeginDisabledGroup(mainScript.allowedForward == false);

                if (GUILayout.Button(new GUIContent() { text = "Continue", tooltip = mainScript.allowedForward == true ? "Press this to continue to the next step" : "You need to finish this step to continue" }))// gen something
                {
                    mainScript.pcgManager.ClearUndos();
                    mainScript.allowedForward = false;
                    mainScript.currStateIndex++;
                    mainScript.state = (CellularAutomataMA.UI_STATE)mainScript.currStateIndex;

                }

                EditorGUI.EndDisabledGroup();
            }



        }
    }
}