
namespace DungeonForge.Editor
{
    using System.Collections.Generic;
    using UnityEditor;
    using UnityEngine;
    using DungeonForge.Utils;
    using DungeonForge.AlgoScript;
    [CustomEditor(typeof(DiffLimAggMA))]
    public class DiffLimAggEditor : Editor
    {
        bool showRules = false;

        int corridorThickness;

        int selStartRoomGenType = 0;
        GUIContent[] selStringStartRoomGenType = { new GUIContent() { text = "Circle room", tooltip = "Create a circular room in the middle of the canvas" }, new GUIContent() { text = "Square room", tooltip = "Create a square room in the middle of the canvas" }, new GUIContent() { text = "Random room", tooltip = "Given the maximum height and length a randomly created room will be generated" } };

        float percentageOfStick;

        float percOfSpawn;

        int sizeOfRoomSphere;

        int sizeOfRoomSphereWidth;
        int sizeOfRoomSphereHeight;

        string saveMapFileName = "";

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            DiffLimAggMA mainScript = (DiffLimAggMA)target;


            #region explanation

            showRules = EditorGUILayout.BeginFoldoutHeaderGroup(showRules, "Instructions");

            if (showRules)
            {
                GUILayout.TextArea("diff lim agg");
            }

            if (!Selection.activeTransform)
            {
                showRules = false;
            }

            EditorGUILayout.EndFoldoutHeaderGroup();

            #endregion

            DFEditorUtil.SpacesUILayout(4);


            if (!mainScript.generatedBool)
            {
                GUILayout.BeginVertical("Box");
                selStartRoomGenType = GUILayout.SelectionGrid(selStartRoomGenType, selStringStartRoomGenType, 1);
                GUILayout.EndVertical();

                switch (selStartRoomGenType)
                {
                    case 0:  //sphere

                        sizeOfRoomSphere = (int)EditorGUILayout.Slider(new GUIContent() { text = "size of room as sphere", tooltip = "" }, sizeOfRoomSphere, 4, 25);

                        break;

                    case 1: // room

                        sizeOfRoomSphereWidth = (int)EditorGUILayout.Slider(new GUIContent() { text = "width", tooltip = "" }, sizeOfRoomSphereWidth, 10, 30);
                        sizeOfRoomSphereHeight = (int)EditorGUILayout.Slider(new GUIContent() { text = "height", tooltip = "" }, sizeOfRoomSphereHeight, 10, 30);

                        break;

                    case 2: // random walk
                        sizeOfRoomSphereWidth = (int)EditorGUILayout.Slider(new GUIContent() { text = "width", tooltip = "" }, sizeOfRoomSphereWidth, 10, 30);
                        sizeOfRoomSphereHeight = (int)EditorGUILayout.Slider(new GUIContent() { text = "height", tooltip = "" }, sizeOfRoomSphereHeight, 10, 30);

                        break;


                    default:
                        break;

                }

                DFEditorUtil.SpacesUILayout(2);
                percentageOfStick = EditorGUILayout.Slider(new GUIContent() { text = "percentage of stick", tooltip = "" }, percentageOfStick, 0.05f, 1f);

                percOfSpawn = EditorGUILayout.Slider(new GUIContent() { text = "percentage of spawn", tooltip = "" }, percOfSpawn, 0.15f, 0.7f);

                DFEditorUtil.SpacesUILayout(2);

                corridorThickness = (int)EditorGUILayout.Slider(new GUIContent() { text = "thickness of corrs", tooltip = "" }, corridorThickness, 1, 5);

                DFEditorUtil.SpacesUILayout(2);

                if (GUILayout.Button("Generate Diff lim agg Randomisation"))// gen something
                {
                    var centerPoint = new Vector2Int(mainScript.pcgManager.gridArr.GetLength(0) / 2, mainScript.pcgManager.gridArr.GetLength(1) / 2);
                    DFGeneralUtil.RestartGrid(mainScript.pcgManager.gridArr);

                    bool allowedToContinue = false;

                    switch (selStartRoomGenType)
                    {
                        case 0:  //sphere
                            {
                                var sphereRoom = DFAlgoBank.CreateCircleRoom(mainScript.pcgManager.gridArr, centerPoint, sizeOfRoomSphere + 2);

                                if (sphereRoom != null)
                                {
                                    DFAlgoBank.CreateCircleRoom(mainScript.pcgManager.gridArr, centerPoint, sizeOfRoomSphere, actuallyDraw: true);
                                    mainScript.generatedBool = true;
                                    allowedToContinue = true;
                                }
                            }
                            break;

                        case 1: // room
                            {
                                var squareRoom = DFAlgoBank.CreateSquareRoom(sizeOfRoomSphereWidth, sizeOfRoomSphereHeight, centerPoint, mainScript.pcgManager.gridArr);

                                if (squareRoom != null)
                                {
                                    DFAlgoBank.CreateSquareRoom(sizeOfRoomSphereWidth, sizeOfRoomSphereHeight, centerPoint, mainScript.pcgManager.gridArr);
                                    mainScript.generatedBool = true;
                                    allowedToContinue = true;
                                }
                            }
                            break;

                        case 2: // random walk
                            {
                                var squareRoom = DFAlgoBank.CreateSquareRoom(sizeOfRoomSphereWidth, sizeOfRoomSphereHeight, centerPoint, mainScript.pcgManager.gridArr, true);

                                if (squareRoom != null)
                                {
                                    var roomBounds = new BoundsInt() { xMin = centerPoint.x - sizeOfRoomSphereWidth / 2, xMax = centerPoint.x + sizeOfRoomSphereWidth / 2, zMin = centerPoint.y - sizeOfRoomSphereHeight / 2, zMax = centerPoint.y - sizeOfRoomSphereHeight / 2 };

                                    var room = DFAlgoBank.CompartimentalisedCA(roomBounds);

                                    for (int y = 0; y < room.Length; y++)
                                    {
                                        for (int x = 0; x < room.GetLength(0); x++)
                                        {
                                            if (room[x, y].tileWeight == 1)
                                            {
                                                mainScript.pcgManager.gridArr[x + roomBounds.xMin, y + roomBounds.zMin].tileWeight = 1;
                                            }
                                            else
                                            {
                                                mainScript.pcgManager.gridArr[x + roomBounds.xMin, y + roomBounds.zMin].tileWeight = 0;
                                            }
                                        }
                                    }
                                    allowedToContinue = true;
                                    mainScript.generatedBool = true;
                                }
                            }
                            break;

                        default:
                            break;
                    }
                    if (!allowedToContinue)
                    {
                        EditorGUILayout.HelpBox("There was an issue with the size asked to generate the rooms please choose another size", MessageType.Error);
                        mainScript.pcgManager.Restart();
                        mainScript.generatedBool = false;
                    }
                    else
                    {
                        int size = mainScript.pcgManager.gridArr.GetLength(1) * mainScript.pcgManager.gridArr.GetLength(0);

                        DFAlgoBank.DiffLimAggregation(mainScript.pcgManager.gridArr, (int)(size * percOfSpawn), percentageOfStick);

                        DFAlgoBank.SetUpTileCorridorTypesUI(mainScript.pcgManager.gridArr, corridorThickness);

                        mainScript.pcgManager.Plane.GetComponent<Renderer>().sharedMaterial.mainTexture = DFGeneralUtil.SetUpTextBiColShade(mainScript.pcgManager.gridArr, 0, 1, true);
                    }
                }
            }
            else
            {
                if (GUILayout.Button("Restart"))// gen something
                {
                    mainScript.pcgManager.Restart();
                    mainScript.generatedBool = false;
                }

                DFEditorUtil.SpacesUILayout(2);

                DFEditorUtil.SaveGridDataToGenerateEditorSection(mainScript.pcgManager, saveMapFileName, out saveMapFileName);
            }
        }
    }
}