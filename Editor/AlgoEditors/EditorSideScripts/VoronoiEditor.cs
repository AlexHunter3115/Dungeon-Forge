

namespace DungeonForge.Editor
{
    using System.Collections.Generic;
    using UnityEditor;
    using UnityEngine;
    using DungeonForge.Utils;
    using DungeonForge.AlgoScript;

    [CustomEditor(typeof(VoronoiMA))]
    public class VoronoiEditor : Editor
    {
        bool showRules = false;

        int vorPoints = 5;
        int minNumOfRooms = 1;
        bool typeOfVoronoi = false;
        bool voronoiCalculation = false;

        int selGridPathGenType = 0;
        int selGridConnectionType = 0;

        int corridorThickness = 3;
        int randomAddCorr = 0;
        int deadEndAmount = 0;
        int deadEndCorridorThickness = 3;

        int width = 10;
        int height = 10;
        int radius = 10;

        bool useWeights = false;
        //bool DjAvoidWalls = false;

        int bezierOndulation = 5;
        int deadEndOndulation = 5;

        string saveMapFileName = "";

        //in the voronoi we need random connections

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            VoronoiMA mainScript = (VoronoiMA)target;


            #region explanation

            showRules = EditorGUILayout.BeginFoldoutHeaderGroup(showRules, "Descirption");

            if (showRules)
            {  //Calculates the distance between points in a space and uses it to partition the space into regions based on the closest point. It achieves efficient spatial analysis
                GUILayout.TextArea("You have choosen Voronoi algorithm as a starting algorithm.\nCalculates the distance between points in a space and uses it to partition the space into regions based on the closest point. It achieves efficient spatial analysis\n\nFor more information visit the Wiki!!");
            }

            if (!Selection.activeTransform)
            {
                showRules = false;
            }

            EditorGUILayout.EndFoldoutHeaderGroup();

            DFEditorUtil.SpacesUILayout(4);


            #endregion


            switch (mainScript.currUiState)
            {
                case DFEditorUtil.UI_STATE.MAIN_ALGO:
                    {
                        typeOfVoronoi = EditorGUILayout.Toggle(new GUIContent() { text = typeOfVoronoi == true ? "Room to Room is selected" : "Random deletion room is selected", tooltip = typeOfVoronoi == true ? $"Will create {vorPoints} number of rooms and connect the rooms with small to no corridors" : $"Random deletion room is selected, will generate {vorPoints} rooms and then randomly delete them untill only {minNumOfRooms} are left" }, typeOfVoronoi);

                        DFEditorUtil.SpacesUILayout(1);

                        vorPoints = (int)EditorGUILayout.Slider(new GUIContent() { text = "Number of voronoi divisions", tooltip = "" }, vorPoints, 8, 40);
                        voronoiCalculation = EditorGUILayout.Toggle(new GUIContent() { text = voronoiCalculation == true ? "Euclidian distance calculation selected" : "Manhattan distance calculation selected", tooltip = "" }, voronoiCalculation);


                        if (!typeOfVoronoi)
                            minNumOfRooms = (int)EditorGUILayout.Slider(new GUIContent() { text = "Number of rooms", tooltip = "" }, minNumOfRooms, 2, vorPoints - 2);


                        if (GUILayout.Button(new GUIContent() { text = "Generate Voronoi Fracture", tooltip = "" }))// gen something
                        {
                            mainScript.pcgManager.Restart();

                            DFAlgoBank.Voronoi2D(mainScript.pcgManager.gridArr, vorPoints, voronoiCalculation);

                            mainScript.rooms = DFAlgoBank.GetAllRooms(mainScript.pcgManager.gridArr);

                            if (!typeOfVoronoi)
                            {
                                while (mainScript.rooms.Count > minNumOfRooms)
                                {
                                    int roomCount = mainScript.rooms.Count - 1;
                                    int ranIndex = Random.Range(0, roomCount);

                                    for (int i = 0; i < mainScript.rooms[ranIndex].Count; i++)
                                    {
                                        mainScript.rooms[ranIndex][i].tileWeight = 0;
                                        mainScript.rooms[ranIndex][i].tileType = DFTile.TileType.VOID;
                                        mainScript.rooms[ranIndex][i].color = Color.white;
                                    }

                                    mainScript.rooms.RemoveAt(ranIndex);
                                }

                                mainScript.rooms = DFAlgoBank.GetAllRooms(mainScript.pcgManager.gridArr);

                            }

                            mainScript.pcgManager.Plane.GetComponent<Renderer>().sharedMaterial.mainTexture = DFGeneralUtil.SetUpTextBiColShade(mainScript.pcgManager.gridArr, 0, 1, true);
                        }


                        if (mainScript.rooms.Count > 1)
                        {
                            mainScript.allowedForward = true;
                        }

                        break;
                    }

                case DFEditorUtil.UI_STATE.CA:
                    {
                        mainScript.pcgManager.ClearUndos();
                        mainScript.allowedForward = false;
                        mainScript.currStateIndex++;
                        mainScript.currUiState = (DFEditorUtil.UI_STATE)mainScript.currStateIndex;

                        break;
                    }

                case DFEditorUtil.UI_STATE.ROOM_GEN:
                    {
                        mainScript.pcgManager.ClearUndos();
                        mainScript.allowedForward = false;
                        mainScript.currStateIndex++;
                        mainScript.currUiState = (DFEditorUtil.UI_STATE)mainScript.currStateIndex;

                        break;
                    }

                case DFEditorUtil.UI_STATE.EXTRA_ROOM_GEN:
                    {
                        if (!typeOfVoronoi)
                        {
                            mainScript.allowedForward = true;
                            mainScript.allowedBack = false;

                            DFEditorUtil.ExtraRoomEditorSelection(mainScript.pcgManager, mainScript.rooms, ref radius, ref height, ref width);
                        }
                        else
                        {
                            mainScript.pcgManager.ClearUndos();
                            mainScript.allowedForward = false;
                            mainScript.currStateIndex++;
                            mainScript.currUiState = (DFEditorUtil.UI_STATE)mainScript.currStateIndex;
                        }
                        break;
                    }

                case DFEditorUtil.UI_STATE.PATHING:
                    {
                        #region corridor making region

                        if (typeOfVoronoi)
                        {

                            EditorGUI.BeginDisabledGroup(mainScript.pcgManager.prevGridArray2D.Count == 1);

                            GUILayout.Label("Choose how to order the connection of the rooms");

                            DFEditorUtil.SpacesUILayout(2);

                            GUILayout.BeginVertical("Box");
                            selGridConnectionType = GUILayout.SelectionGrid(selGridConnectionType, DFEditorUtil.selStringsConnectionType, 1);
                            GUILayout.EndVertical();

                            randomAddCorr = (int)EditorGUILayout.Slider(new GUIContent() { text = "Additional random connections", tooltip = "Add another random connection. This number dictates how many times the script is going to TRY to add a new corridor" }, randomAddCorr, 0, mainScript.rooms.Count / 2);
                            DFEditorUtil.SpacesUILayout(2);

                            if (GUILayout.Button("Connect all the rooms"))// dfor the corridor making
                            {
                                mainScript.allowedForward = true;

                                mainScript.pcgManager.CreateBackUpGrid();
                                mainScript.rooms = DFAlgoBank.GetAllRooms(mainScript.pcgManager.gridArr);
                                var centerPoints = new List<Vector2>();
                                var roomDict = new Dictionary<Vector2, List<DFTile>>();
                                foreach (var room in mainScript.rooms)
                                {
                                    roomDict.Add(DFGeneralUtil.FindMiddlePoint(room), room);
                                    centerPoints.Add(DFGeneralUtil.FindMiddlePoint(room));
                                }

                                switch (selGridConnectionType)
                                {
                                    case 0:
                                        mainScript.edges = DFAlgoBank.PrimAlgoNoDelu(centerPoints);
                                        if (randomAddCorr > 0)
                                        {
                                            int len = mainScript.edges.Count - 1;

                                            for (int i = 0; i < randomAddCorr; i++)
                                            {
                                                var pointA = mainScript.edges[Random.Range(0, len)].edge[0];
                                                var pointBEdgeCheck = mainScript.edges[Random.Range(0, len)];

                                                Vector3 pointB;

                                                if (pointA == pointBEdgeCheck.edge[0])
                                                    pointB = pointBEdgeCheck.edge[1];
                                                else if (pointA == pointBEdgeCheck.edge[1])
                                                    pointB = pointBEdgeCheck.edge[0];
                                                else
                                                    pointB = pointBEdgeCheck.edge[1];


                                                Edge newEdge = new Edge(pointA, pointB);

                                                bool toAdd = true;

                                                foreach (var primEdge in mainScript.edges)
                                                {
                                                    if (DFAlgoBank.LineIsEqual(primEdge, newEdge))
                                                    {
                                                        toAdd = false;
                                                        break;
                                                    }
                                                }

                                                if (toAdd)
                                                {
                                                    mainScript.edges.Add(newEdge);
                                                }
                                            }
                                        }
                                        break;

                                    case 1:
                                        mainScript.edges = DFAlgoBank.DelaunayTriangulation(centerPoints).Item2;
                                        break;

                                    case 2://ran

                                        DFAlgoBank.ShuffleList(mainScript.rooms);

                                        centerPoints = new List<Vector2>();
                                        roomDict = new Dictionary<Vector2, List<DFTile>>();
                                        foreach (var room in mainScript.rooms)
                                        {
                                            roomDict.Add(DFGeneralUtil.FindMiddlePoint(room), room);
                                            centerPoints.Add(DFGeneralUtil.FindMiddlePoint(room));
                                        }

                                        for (int i = 0; i < centerPoints.Count; i++)
                                        {
                                            if (i == centerPoints.Count - 1) { continue; }
                                            mainScript.edges.Add(new Edge(new Vector3(centerPoints[i].x, centerPoints[i].y, 0), new Vector3(centerPoints[i + 1].x, centerPoints[i + 1].y, 0)));
                                        }

                                        if (randomAddCorr > 0)
                                        {
                                            int len = mainScript.edges.Count - 1;

                                            for (int i = 0; i < randomAddCorr; i++)
                                            {
                                                int ranStarter = Random.Range(0, len);
                                                int ranEnder = Random.Range(0, len);

                                                if (ranStarter == ranEnder) { continue; }
                                                else if (Mathf.Abs(ranStarter - ranEnder) == 1) { continue; }
                                                else
                                                {
                                                    mainScript.edges.Add(new Edge(new Vector3(centerPoints[ranStarter].x, centerPoints[ranStarter].y, 0), new Vector3(centerPoints[ranEnder].x, centerPoints[ranEnder].y, 0)));
                                                }
                                            }
                                        }

                                        break;
                                }


                                foreach (var edge in mainScript.edges)
                                {
                                    var tileA = roomDict[edge.edge[0]][Random.Range(0, roomDict[edge.edge[0]].Count)].position;
                                    var tileB = roomDict[edge.edge[1]][Random.Range(0, roomDict[edge.edge[1]].Count)].position;
                                    DFAlgoBank.BezierCurvePathing(new Vector2Int(tileA.x, tileA.y), new Vector2Int(tileB.x, tileB.y), 5, mainScript.pcgManager.gridArr);
                                }


                                DFAlgoBank.SetUpTileCorridorTypesUI(mainScript.pcgManager.gridArr, corridorThickness);

                                mainScript.pcgManager.Plane.GetComponent<Renderer>().sharedMaterial.mainTexture = DFGeneralUtil.SetUpTextBiColShade(mainScript.pcgManager.gridArr, 0, 1, true);
                            }

                            EditorGUI.EndDisabledGroup();

                            if (mainScript.pcgManager.prevGridArray2D.Count == 1)
                            {
                                GUILayout.Label(new GUIContent() { text = "You have generated your corridors, either click the undo button to try another algorithm or continue" });

                                mainScript.allowedBack = false;
                                mainScript.allowedForward = true;
                            }
                            else
                            {
                                mainScript.allowedBack = true;
                                mainScript.allowedForward = false;
                            }

                        }
                        else
                        {
                            mainScript.allowedBack = true;

                            DFEditorUtil.GenerateCorridorsEditorSection(mainScript.pcgManager, mainScript.rooms, ref mainScript.allowedForward, ref mainScript.allowedBack, ref corridorThickness
                            , ref selGridConnectionType, ref selGridPathGenType, ref useWeights, ref bezierOndulation, ref mainScript.pathType, ref randomAddCorr, ref deadEndAmount, ref deadEndCorridorThickness, ref deadEndOndulation, ref mainScript.edges);
                        }

                        #endregion
                        break;
                    }

                case DFEditorUtil.UI_STATE.GENERATION:
                    {
                        mainScript.allowedBack = false;

                        DFEditorUtil.SaveGridDataToGenerate(mainScript.pcgManager, saveMapFileName, out saveMapFileName);
                        break;
                    }


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