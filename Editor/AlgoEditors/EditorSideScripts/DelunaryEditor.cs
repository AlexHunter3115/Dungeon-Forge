namespace DungeonForge.Editor
{
    using System.Collections.Generic;
    using UnityEditor;
    using UnityEngine;
    using DungeonForge.Utils;
    using DungeonForge.AlgoScript;


    [CustomEditor(typeof(DelunaryMA))]
    public class DelunaryEditor : Editor
    {
        bool showRules = false;

        int selStartRoomGenType = 0;
        GUIContent[] selStringStartRoomGenType = { new GUIContent() { text = "Circle room", tooltip = "" }, new GUIContent() { text = "square room", tooltip = "" }, new GUIContent() { text = "Random Room Layout", tooltip = "Spawns a square rooms and then performs the Cellular automata algorithm on the room for a more interesting layout" } };

        int selOtherRoomGenType = 0;
        GUIContent[] selStringOtherRoomGenType = 
            { new GUIContent() { text = "None", tooltip = "" }, 
            new GUIContent() { text = "Circle", tooltip = "" }, 
            new GUIContent() { text = "Square Room", tooltip = "" }, 
            new GUIContent() { text = "Random Room Layout", tooltip = "Spawns a square rooms and then performs the Cellular automata algorithm on the room for a more interesting layout" }, 
            new GUIContent() { text = "Mix of rooms", tooltip = "A mix of all" } };

        int widthFrom=5;
        int widthTo=10;

        int heightFrom=5;
        int heightTo=10;

        string saveMapFileName = "";

        int width;
        int height;

        int numbersOfVertices;
        int ondulation;

        int corridorWidth;

        //first need to gen the main room then everything
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            DelunaryMA mainScript = (DelunaryMA)target;


            #region explanation

            showRules = EditorGUILayout.BeginFoldoutHeaderGroup(showRules, "Introduction");

            if (showRules)
            {
                GUILayout.TextArea("Delaunay Triangulation first generates random points around the map and then creates a set of non-overlapping triangles by connecting these points. Following triangulation, the algorithm connects the triangle vertices using Bezier curves, resulting in a maze-like dungeon layout. This method is suitable for generating dungeons with interconnected paths and a complex, maze-like structure.\n\n Visit the wiki for more information: https://github.com/AlessandroBufalino3115/Dungeon-Forge/wiki/Using-the-Pack#10-delaunay-triangulation ");

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
                case DelunaryMA.UI_STATE.STAGE_1:
                    { 
                        EditorGUI.BeginDisabledGroup(mainScript.rooms.Count >= 1);

                        GUILayout.BeginVertical("Box");
                        selStartRoomGenType = GUILayout.SelectionGrid(selStartRoomGenType, selStringStartRoomGenType, 1);
                        GUILayout.EndVertical();

                        switch (selStartRoomGenType)
                        {
                            case 0:  //sphere

                                width = (int)EditorGUILayout.Slider(new GUIContent() { text = "Radius of circle", tooltip = "" }, width, 10, 50);

                                break;

                            case 1: // room

                                height = (int)EditorGUILayout.Slider(new GUIContent() { text = "Height", tooltip = "" }, height, 10, 50);
                                width = (int)EditorGUILayout.Slider(new GUIContent() { text = "Width", tooltip = "" }, width, 10, 50);

                                break;

                            case 2: // random walk

                                height = (int)EditorGUILayout.Slider(new GUIContent() { text = "Height", tooltip = "" }, height, 10, 50);
                                width = (int)EditorGUILayout.Slider(new GUIContent() { text = "Width", tooltip = "" }, width, 10, 50);
                                break;

                            default:
                                break;
                        }


                        if (GUILayout.Button(new GUIContent() { text = "Generate starting room", tooltip = "" }))
                        {
                            var centerPoint = new Vector2Int(mainScript.pcgManager.gridArr.GetLength(0) / 2, mainScript.pcgManager.gridArr.GetLength(1) / 2);

                            switch (selStartRoomGenType)
                            {
                                case 0:  //sphere
                                    {
                                        var sphereRoom = DFAlgoBank.CreateCircleRoom(mainScript.pcgManager.gridArr, centerPoint, width + 2);

                                        if (sphereRoom != null)
                                        {
                                            mainScript.pcgManager.CreateBackUpGrid();
                                            sphereRoom = DFAlgoBank.CreateCircleRoom(mainScript.pcgManager.gridArr, centerPoint, width, actuallyDraw: true);

                                            mainScript.pcgManager.Plane.GetComponent<Renderer>().sharedMaterial.mainTexture = DFGeneralUtil.SetUpTextBiColShade(mainScript.pcgManager.gridArr, 0, 1, true);

                                            mainScript.rooms.Add(sphereRoom);
                                        }
                                    }

                                    break;

                                case 1: // room
                                    {
                                        var squareRoom = DFAlgoBank.CreateSquareRoom(width, height, centerPoint, mainScript.pcgManager.gridArr, false);

                                        if (squareRoom != null)
                                        {
                                            mainScript.pcgManager.CreateBackUpGrid();
                                            squareRoom = DFAlgoBank.CreateSquareRoom(width, height, centerPoint, mainScript.pcgManager.gridArr,true);

                                            mainScript.pcgManager.Plane.GetComponent<Renderer>().sharedMaterial.mainTexture = DFGeneralUtil.SetUpTextBiColShade(mainScript.pcgManager.gridArr, 0, 1, true);

                                            mainScript.rooms.Add(squareRoom);
                                        }
                                    }

                                    break;

                                case 2: // random walk
                                    {
                                        var squareRoom = DFAlgoBank.CreateSquareRoom(width, height, centerPoint, mainScript.pcgManager.gridArr, false);

                                        if (squareRoom != null)
                                        {
                                            var roomBounds = new BoundsInt() { xMin = centerPoint.x - width / 2, xMax = centerPoint.x + width / 2, zMin = centerPoint.y - height / 2, zMax = centerPoint.y + height / 2 };

                                            var room = DFAlgoBank.CompartimentalisedCA(roomBounds);

                                            for (int y = 0; y < room.GetLength(1); y++)
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

                                            mainScript.pcgManager.Plane.GetComponent<Renderer>().sharedMaterial.mainTexture = DFGeneralUtil.SetUpTextBiColShade(mainScript.pcgManager.gridArr, 0, 1, true);

                                            DFAlgoBank.GetAllRooms(mainScript.pcgManager.gridArr);
                                        }
                                    }

                                    break;

                                default:
                                    break;
                            }
                        }

                        EditorGUI.EndDisabledGroup();

                        if (mainScript.rooms.Count > 0)
                            mainScript.allowedForward = true;
                        else
                            mainScript.allowedForward = false;
                    }

                    break;

                case DelunaryMA.UI_STATE.STAGE_2:
                    {
                        EditorGUI.BeginDisabledGroup(mainScript.generatedCorridors == true);

                        numbersOfVertices = (int)EditorGUILayout.Slider(new GUIContent() { text = "Number of vertices", tooltip = "number of points where the paths are going to meet" }, numbersOfVertices, 5, 50);
                        ondulation = (int)EditorGUILayout.Slider(new GUIContent() { text = "Ondulation", tooltip = "The higher the number the more curved the corridors will be" }, ondulation, 5, 40);

                        corridorWidth = (int)EditorGUILayout.Slider(new GUIContent() { text = "Corridor thickness", tooltip = "" }, corridorWidth, 3, 6);

                        DFEditorUtil.SpacesUILayout(2);

                        GUILayout.BeginVertical("Box");
                        selOtherRoomGenType = GUILayout.SelectionGrid(selOtherRoomGenType, selStringOtherRoomGenType, 1);
                        GUILayout.EndVertical();



                        switch (selOtherRoomGenType)
                        {
                            case 0:  //none

                                break;

                            case 1: // circle

                                widthFrom = (int)EditorGUILayout.Slider(new GUIContent() { text = "Minimum radius of the rooms", tooltip = "" }, widthFrom, 5, 50);
                                widthTo = (int)EditorGUILayout.Slider(new GUIContent() { text = "Minimum radius of the rooms", tooltip = "" }, widthTo, 10, 55);

                                if (widthFrom > widthTo)
                                    widthTo = widthFrom + 1;

                                break;

                            case 2: // square

                                widthFrom = (int)EditorGUILayout.Slider(new GUIContent() { text = "Minimum width of the rooms", tooltip = "" }, widthFrom, 5, 50);
                                widthTo = (int)EditorGUILayout.Slider(new GUIContent() { text = "Maximum width of the rooms", tooltip = "" }, widthTo, 10, 55);


                                heightFrom = (int)EditorGUILayout.Slider(new GUIContent() { text = "Minimum width of the rooms", tooltip = "" }, heightFrom, 5, 50);
                                heightTo = (int)EditorGUILayout.Slider(new GUIContent() { text = "Maximum width of the rooms", tooltip = "" }, heightTo, 10, 55);

                                if (widthFrom > widthTo)
                                    widthTo = widthFrom + 1;

                                if (heightFrom > heightTo)
                                    heightTo = heightFrom + 1;

                                break;

                            case 3:  //random room gen

                                widthFrom = (int)EditorGUILayout.Slider(new GUIContent() { text = "Minimum width of the rooms", tooltip = "" }, widthFrom, 5, 50);
                                widthTo = (int)EditorGUILayout.Slider(new GUIContent() { text = "Maximum width of the rooms", tooltip = "" }, widthTo, 10, 55);


                                heightFrom = (int)EditorGUILayout.Slider(new GUIContent() { text = "Minimum width of the rooms", tooltip = "" }, heightFrom, 5, 50);
                                heightTo = (int)EditorGUILayout.Slider(new GUIContent() { text = "Maximum width of the rooms", tooltip = "" }, heightTo, 10, 55);

                                if (widthFrom > widthTo)
                                    widthTo = widthFrom + 1;

                                if (heightFrom > heightTo)
                                    heightTo = heightFrom + 1;

                                break;

                            case 4:  //mix

                                widthFrom = (int)EditorGUILayout.Slider(new GUIContent() { text = "Minimum width/diameter of the rooms", tooltip = "This acts as the diameter for the cirlce room too" }, widthFrom, 5, 50);
                                widthTo = (int)EditorGUILayout.Slider(new GUIContent() { text = "Maximum width/diameter of the rooms", tooltip = "This acts as the diameter for the cirlce room too" }, widthTo, 10, 55);


                                heightFrom = (int)EditorGUILayout.Slider(new GUIContent() { text = "Minimum width of the rooms", tooltip = "" }, heightFrom, 5, 50);
                                heightTo = (int)EditorGUILayout.Slider(new GUIContent() { text = "Maximum width of the rooms", tooltip = "" }, heightTo, 10, 55);

                                if (widthFrom > widthTo)
                                    widthTo = widthFrom + 1;

                                if (heightFrom > heightTo)
                                    heightTo = heightFrom + 1;


                                break;

                            default:
                                break;
                        }


                        DFEditorUtil.SpacesUILayout(2);


                        if (GUILayout.Button(new GUIContent() { text = "Generate the path", tooltip = "" }))
                        {
                            mainScript.generatedCorridors = true;

                            mainScript.pcgManager.CreateBackUpGrid();
                            for (int i = 0; i < numbersOfVertices; i++)
                            {
                                int startXMin = 0;
                                int startYMin = 0;

                                int confirmedWidth = Random.Range(widthFrom, widthTo);
                                int confirmedHeight = Random.Range(heightFrom, heightTo);

                                int startXMax = mainScript.pcgManager.gridArr.GetLength(0) - confirmedWidth;
                                int startYMax = mainScript.pcgManager.gridArr.GetLength(1) - confirmedHeight;

                                int startX = Random.Range(startXMin, startXMax + 1);
                                int startY = Random.Range(startYMin, startYMax + 1);

                                var randomPoint = new Vector2Int(startX, startY);
                               

                                switch (selOtherRoomGenType)
                                {
                                    case 0:  //none

                                        var sphereRoom = DFAlgoBank.CreateCircleRoom(mainScript.pcgManager.gridArr, randomPoint, 2);

                                        if (sphereRoom != null)
                                        {
                                            sphereRoom = DFAlgoBank.CreateCircleRoom(mainScript.pcgManager.gridArr, randomPoint, 2, actuallyDraw: true);

                                            mainScript.rooms.Add(sphereRoom);
                                        }

                                        break;

                                    case 1: // circle
                                        {
                                            var room = DFAlgoBank.CreateCircleRoom(mainScript.pcgManager.gridArr, randomPoint, confirmedWidth, actuallyDraw: true);

                                            if (room == null)
                                                Debug.Log($"There is an issue");
                                            else
                                                mainScript.rooms.Add(room);
                                        }
                                        break;

                                    case 2: // square
                                        {
                                            var room = DFAlgoBank.CreateSquareRoom(confirmedWidth,confirmedHeight,randomPoint,mainScript.pcgManager.gridArr,true);
                                           
                                            if (room == null)
                                                Debug.Log($"There is an issue");
                                            else
                                                mainScript.rooms.Add(room);
                                        }
                                        break;

                                    case 3:  //random room gen
                                        {
                                            var roomBounds = new BoundsInt() { xMin = randomPoint.x - confirmedWidth / 2, xMax = randomPoint.x + confirmedWidth / 2, zMin = randomPoint.y - confirmedHeight / 2, zMax = randomPoint.y + confirmedHeight / 2 };

                                            var room = DFAlgoBank.CompartimentalisedCA(roomBounds);

                                            for (int y = 0; y < room.GetLength(1); y++)
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
                                        }
                                        break;

                                    case 4:  //mix
                                        {

                                            var ranNum = Random.value;

                                            if (ranNum <0.33f)
                                            {
                                                var room = DFAlgoBank.CreateCircleRoom(mainScript.pcgManager.gridArr, randomPoint, confirmedWidth, actuallyDraw: true);

                                                if (room == null)
                                                    Debug.Log($"There is an issue");
                                                else
                                                    mainScript.rooms.Add(room);
                                            }
                                            else if (ranNum >= 0.33f   &&   ranNum <= 0.66f) 
                                            {
                                                var room = DFAlgoBank.CreateSquareRoom(confirmedWidth, confirmedHeight, randomPoint, mainScript.pcgManager.gridArr, true);

                                                if (room == null)
                                                    Debug.Log($"There is an issue");
                                                else
                                                    mainScript.rooms.Add(room);
                                            }
                                            else 
                                            {
                                                var roomBounds = new BoundsInt() { xMin = randomPoint.x - confirmedWidth / 2, xMax = randomPoint.x + confirmedWidth / 2, zMin = randomPoint.y - confirmedHeight / 2, zMax = randomPoint.y + confirmedHeight / 2 };

                                                var room = DFAlgoBank.CompartimentalisedCA(roomBounds);

                                                for (int y = 0; y < room.GetLength(1); y++)
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
                                            }
                                          
                                        }
                                        break;

                                    default:
                                        break;
                                }

                            }


                            mainScript.pcgManager.Plane.GetComponent<Renderer>().sharedMaterial.mainTexture = DFGeneralUtil.SetUpTextBiColShade(mainScript.pcgManager.gridArr, 0, 1, true);

                            mainScript.rooms = DFAlgoBank.GetAllRooms(mainScript.pcgManager.gridArr);

                            Debug.Log(mainScript.rooms.Count);

                            var centerPoints = new List<Vector2>();
                            var roomDict = new Dictionary<Vector2, List<DFTile>>();
                            foreach (var room in mainScript.rooms)
                            {
                                roomDict.Add(DFGeneralUtil.FindMiddlePoint(room), room);
                                centerPoints.Add(DFGeneralUtil.FindMiddlePoint(room));
                            }


                            var edges = DFAlgoBank.DelaunayTriangulation(centerPoints).Item2;

                            foreach (var edge in edges)
                            {
                                var tileA = roomDict[edge.edge[0]][Random.Range(0, roomDict[edge.edge[0]].Count)].position;
                                var tileB = roomDict[edge.edge[1]][Random.Range(0, roomDict[edge.edge[1]].Count)].position;

                                DFAlgoBank.BezierCurvePathing(new Vector2Int(tileA.x, tileA.y), new Vector2Int(tileB.x, tileB.y), ondulation, mainScript.pcgManager.gridArr);
                            }

                            DFAlgoBank.SetUpTileCorridorTypesUI(mainScript.pcgManager.gridArr, corridorWidth);

                            mainScript.pcgManager.Plane.GetComponent<Renderer>().sharedMaterial.mainTexture = DFGeneralUtil.SetUpTextBiColShade(mainScript.pcgManager.gridArr, 0, 1, true);
                        }


                        EditorGUI.EndDisabledGroup();

                        if (mainScript.generatedCorridors)
                            mainScript.allowedForward = true;
                        else
                            mainScript.allowedForward = false;

                    }

                    break;

                case DelunaryMA.UI_STATE.GENERATION:

                    mainScript.allowedBack = true;

                    DFEditorUtil.SaveGridDataToGenerateEditorSection(mainScript.pcgManager, saveMapFileName, out saveMapFileName);

                    break;

                default:
                    break;
            }


            if (mainScript.state != DelunaryMA.UI_STATE.GENERATION)
            {
                DFEditorUtil.SpacesUILayout(4);

                EditorGUI.BeginDisabledGroup(mainScript.allowedBack == false);

                if (GUILayout.Button(new GUIContent() { text = "Go Back", tooltip = mainScript.allowedForward == true ? "Press this to go back one step" : "You can't go back" }))// gen something
                {
                    mainScript.pcgManager.ClearUndos();
                    mainScript.allowedBack = false;
                    mainScript.currStateIndex--;
                    mainScript.state = (DelunaryMA.UI_STATE)mainScript.currStateIndex;
                }

                EditorGUI.EndDisabledGroup();

                EditorGUI.BeginDisabledGroup(mainScript.allowedForward == false);

                if (GUILayout.Button(new GUIContent() { text = "Continue", tooltip = mainScript.allowedForward == true ? "Press this to continue to the next step" : "You need to finish this step to continue" }))// gen something
                {
                    mainScript.pcgManager.ClearUndos();
                    mainScript.allowedForward = false;
                    mainScript.currStateIndex++;
                    mainScript.state = (DelunaryMA.UI_STATE)mainScript.currStateIndex;
                }

                EditorGUI.EndDisabledGroup();
            }
        }
    }
}