using DungeonForge.AlgoScript;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEditor;
using UnityEngine;

namespace DungeonForge.Utils
{
    public static class DFEditorUtil
    {
        public static void SpacesUILayout(int spaceNum)
        {
            for (int i = 0; i < spaceNum; i++)
            {
                EditorGUILayout.Space();
            }
        }

        public static void CellularAutomataEditorSection(PCGManager pcgManager,ref int neighbours)
        {
            DFTile[,] gridArr = pcgManager.gridArr;

            neighbours = (int)EditorGUILayout.Slider(new GUIContent() { text = "Neighbours Needed", tooltip = "To run the CA algortihm a set number of neighbours needs to be given as a rule" }, neighbours, 3, 5);

            if (GUILayout.Button(new GUIContent() { text = "Clean Up using CA", tooltip = "Run half of the CA algortihm to only take out tiles, to help slim down the result" }))
            {
                pcgManager.CreateBackUpGrid();

                DFAlgoBank.CleanUp2dCA(gridArr, neighbours);

                pcgManager.Plane.GetComponent<Renderer>().sharedMaterial.mainTexture = DFGeneralUtil.SetUpTextBiColAnchor(gridArr);
            }
            if (GUILayout.Button(new GUIContent() { text = "Use CA algorithm", tooltip = "Run the full CA algorithm on the current iteration of the grid" }))
            {
                pcgManager.CreateBackUpGrid();

                DFAlgoBank.RunCaIteration2D(gridArr, neighbours);
                pcgManager.Plane.GetComponent<Renderer>().sharedMaterial.mainTexture = DFGeneralUtil.SetUpTextBiColAnchor(gridArr);
            }
        }

        public static void ExtraRoomEditorSelection(PCGManager pcgManager,List<List<DFTile>> rooms,ref int radius, ref int height, ref int width)
        {
            radius = (int)EditorGUILayout.Slider(new GUIContent() { text = "Radius of the room" }, radius, 10, 40);

            if (GUILayout.Button(new GUIContent() { text = "Spawn circular room", tooltip = "Creates a circular room in a random position on the canvas. The code will try to fit it, if nothing spawns try again or lower the size" }))
            {
                bool success = false;

                for (int i = 0; i < 5; i++)
                {
                    var randomPoint = new Vector2Int(Random.Range(0 + radius + 3, pcgManager.gridArr.GetLength(0) - radius - 3), Random.Range(0 + radius + 3, pcgManager.gridArr.GetLength(1) - radius - 3));

                    var room = DFAlgoBank.CreateCircleRoom(pcgManager.gridArr, randomPoint, radius + 2, checkForFitting: true);

                    if (room != null)
                    {
                        pcgManager.CreateBackUpGrid();
                        room = DFAlgoBank.CreateCircleRoom(pcgManager.gridArr, randomPoint, radius, actuallyDraw: true);

                        pcgManager.Plane.GetComponent<Renderer>().sharedMaterial.mainTexture = DFGeneralUtil.SetUpTextBiColShade(pcgManager.gridArr, 0, 1, true);

                        rooms.Add(room);

                        success = true;

                        break;
                    }
                }

                if (!success)
                    Debug.Log($"<color=red>I tried to spawn the Room as requested 5 times but couldnt find any free space either try again or lower the size</color>");
            }

            SpacesUILayout(2);

            height = (int)EditorGUILayout.Slider(new GUIContent() { text = "Height", tooltip = "" }, height, 10, 40);
            width = (int)EditorGUILayout.Slider(new GUIContent() { text = "Widht", tooltip = "" }, width, 10, 40);

            if (GUILayout.Button(new GUIContent() { text = "Generate random rectangular room", tooltip = "Creates a rectangular room in a random position on the canvas. The code will try to fit it, if nothing spawns try again or lower the size" }))
            {
                bool success = false;
                for (int i = 0; i < 5; i++)
                {
                    var randomPoint = new Vector2Int(Random.Range(0 + radius + 3, pcgManager.gridArr.GetLength(0) - radius - 3), Random.Range(0 + radius + 3, pcgManager.gridArr.GetLength(1) - radius - 3));

                    var squareRoom = DFAlgoBank.CreateSquareRoom(width, height, randomPoint, pcgManager.gridArr, checkForFitting: true);

                    if (squareRoom != null)
                    {
                        pcgManager.CreateBackUpGrid();
                        squareRoom = DFAlgoBank.CreateSquareRoom(width, height, randomPoint, pcgManager.gridArr,actuallyDraw: true);

                        pcgManager.Plane.GetComponent<Renderer>().sharedMaterial.mainTexture = DFGeneralUtil.SetUpTextBiColShade(pcgManager.gridArr, 0, 1, true);

                        rooms.Add(squareRoom);

                        success = true;
                        break;
                    }
                }

                if (!success)
                    Debug.Log($"<color=red>I tried to spawn the Room as requested 5 times but couldnt find any free space either try again or lower the size</color>");

            }
        }

        public static bool CalculateRoomsEditorSection(PCGManager pcgManager,ref int minSize, out List<List<DFTile>> rooms)
        {
            minSize = (int)EditorGUILayout.Slider(new GUIContent() { text = "Minimum size of room to delete", tooltip = "Any room with a lower number of tiles will be deleted" }, minSize, 0, 200);

            if (GUILayout.Button("Generate rooms"))
            {
                rooms = DFAlgoBank.GetAllRooms(pcgManager.gridArr);

                pcgManager.CreateBackUpGrid();

                if (minSize > 0)
                {
                    for (int i = rooms.Count; i-- > 0;)
                    {
                        if (rooms[i].Count <= minSize)
                        {
                            foreach (var tile in rooms[i])
                            {
                                tile.tileWeight = 0;
                                tile.tileType = DFTile.TileType.VOID;
                            }

                            rooms.RemoveAt(i);
                        }
                    }
                }
                //mainScript.allowedForward = true;
                pcgManager.Plane.GetComponent<Renderer>().sharedMaterial.mainTexture = DFGeneralUtil.SetUpTextBiColShade(pcgManager.gridArr, 0, 1, true);


                return true;
            }

            rooms = null;
            return false;
        }

        public static void GenerateCorridorsEditorSection(PCGManager pcgManager, List<List<DFTile>> rooms, ref bool allowedForward,ref bool allowedBack,
            ref int corridorThickness,ref int selGridConnectionType, ref int selGridPathGenType,
            ref bool useWeights, ref int bezierOndulation, ref bool pathType,
            ref int randomAddCorr,ref int deadEndAmount, ref int deadEndCorridorThickness, ref int deadEndOndulation, 
            ref List<Edge> edges) 
        {

            if (rooms.Count == 1)
            {
                allowedForward = true;
                GUILayout.Label("Only one room detected, Corridor making is not possible");

                SpacesUILayout(1);

                deadEndAmount = (int)EditorGUILayout.Slider(new GUIContent() { text = "Amount of dead end corridors", tooltip = "Dead end corridors start from somewhere in the dungeon and lead to nowhere" }, deadEndAmount, 0, 5);

                deadEndCorridorThickness = (int)EditorGUILayout.Slider(new GUIContent() { text = "Thickness of the dead end corridor", tooltip = "How wide should the corridor be" }, deadEndCorridorThickness, 3, 6);

                deadEndOndulation = (int)EditorGUILayout.Slider(new GUIContent() { text = "Curve Multiplier for dead end", tooltip = "A higher multiplier is going to equal to a more extreme curver"}, deadEndOndulation, 10, 40);

                if (GUILayout.Button("Connect all the rooms"))// dfor the corridor making
                {
                    pcgManager.CreateBackUpGrid();

                    for (int i = 0; i < deadEndAmount; i++)
                    {
                        var room = rooms[DFGeneralUtil.ReturnRandomFromList(rooms)];

                        var randomTileInRoom = room[DFGeneralUtil.ReturnRandomFromList(room)];

                        DFTile randomTileOutsideOfRoom;

                        while (true)
                        {
                            var tile = pcgManager.gridArr[Random.Range(0, pcgManager.gridArr.GetLength(0)), Random.Range(0, pcgManager.gridArr.GetLength(1))];

                            if (tile.tileWeight == 0)
                            {
                                randomTileOutsideOfRoom = tile;

                                var tileAd = randomTileOutsideOfRoom.position;
                                var tileBd = randomTileInRoom.position;

                                DFAlgoBank.BezierCurvePathing(new Vector2Int(tileAd.x, tileAd.y), new Vector2Int(tileBd.x, tileBd.y), bezierOndulation, pcgManager.gridArr);

                                break;
                            }
                        }
                    }

                    DFAlgoBank.SetUpTileCorridorTypesUI(pcgManager.gridArr, corridorThickness);

                    pcgManager.Plane.GetComponent<Renderer>().sharedMaterial.mainTexture = DFGeneralUtil.SetUpTextBiColShade(pcgManager.gridArr, 0, 1, true);
                }

            }
            else if (rooms.Count == 2)
            {
                GUILayout.Label("Only two rooms detected, triangulation not possible");

                GUILayout.Label("Choose the algorithm to create the corridor");

                SpacesUILayout(2);

                GUILayout.BeginVertical("Box");
                selGridPathGenType = GUILayout.SelectionGrid(selGridPathGenType, selStringPathGenType, 1);
                GUILayout.EndVertical();

                SpacesUILayout(2);

                switch (selGridPathGenType)
                {
                    case 0:   // A* pathfinding
                        pathType = EditorGUILayout.Toggle(new GUIContent() { text = "Use Straight corridors", tooltip = "PathFinding will prioritize the creation of straight corridors" }, pathType);
                      
                        break;

                    case 2:   // beizier 

                        bezierOndulation = (int)EditorGUILayout.Slider(new GUIContent() { text = "Curve Multiplier", tooltip = "beizeir curve thing to change" }, bezierOndulation, 10, 40);
                        SpacesUILayout(1);

                        break;
                    default:
                        break;
                }

                SpacesUILayout(1);

                deadEndAmount = (int)EditorGUILayout.Slider(new GUIContent() { text = "Amount of dead end corridors", tooltip = "Dead end corridors start from somewhere in the dungeon and lead to nowhere" }, deadEndAmount, 0, 5);

                deadEndCorridorThickness = (int)EditorGUILayout.Slider(new GUIContent() { text = "Thickness of the dead end corridor", tooltip = "How wide should the corridor be" }, deadEndCorridorThickness, 3, 6);

                deadEndOndulation = (int)EditorGUILayout.Slider(new GUIContent() { text = "Curve Multiplier for dead end", tooltip = "A higher multiplier is going to equal to a more extreme curver" }, deadEndOndulation, 10, 40);

                SpacesUILayout(2);

                EditorGUI.BeginDisabledGroup(pcgManager.prevGridArray2D.Count == 1);

                if (GUILayout.Button("Connect all the rooms"))// dfor the corridor making
                {

                    pcgManager.CreateBackUpGrid();

                    Vector2Int tileA = rooms[0][Random.Range(0, rooms[0].Count - 1)].position;
                    Vector2Int tileB = rooms[1][Random.Range(0, rooms[1].Count - 1)].position;

                    allowedForward = true;

                    switch (selGridPathGenType)
                    {
                        case 0:   //A* pathfingin

                            var path = DFAlgoBank.A_StarPathfinding2D(pcgManager.gridArr, new Vector2Int(tileA.x, tileA.y), new Vector2Int(tileB.x, tileB.y), !pathType, useWeights: false, arrWeights: pcgManager.tileCosts);
                            DFAlgoBank.SetUpCorridorWithPath(path.Item1);

                            break;
                        case 1:  //dijistra

                            var pathD = DFAlgoBank.DijstraPathfinding(pcgManager.gridArr, new Vector2Int(tileA.x, tileA.y), new Vector2Int(tileB.x, tileB.y));
                            DFAlgoBank.SetUpCorridorWithPath(pathD);

                            break;
                        case 2://  beizier curve

                            DFAlgoBank.BezierCurvePathing(new Vector2Int(tileA.x, tileA.y), new Vector2Int(tileB.x, tileB.y), bezierOndulation, pcgManager.gridArr);

                            break;

                        default:
                            break;
                    }

                    for (int i = 0; i < deadEndAmount; i++)
                    {
                        var room = rooms[DFGeneralUtil.ReturnRandomFromList(rooms)];

                        var randomTileInRoom = room[DFGeneralUtil.ReturnRandomFromList(room)];

                        DFTile randomTileOutsideOfRoom;

                        while (true)
                        {
                            var tile = pcgManager.gridArr[Random.Range(0, pcgManager.gridArr.GetLength(0)), Random.Range(0, pcgManager.gridArr.GetLength(1))];

                            if (tile.tileWeight == 0)
                            {
                                randomTileOutsideOfRoom = tile;

                                var tileAd = randomTileOutsideOfRoom.position;
                                var tileBd = randomTileInRoom.position;

                                DFAlgoBank.BezierCurvePathing(new Vector2Int(tileAd.x, tileAd.y), new Vector2Int(tileBd.x, tileBd.y), bezierOndulation, pcgManager.gridArr);

                                break;
                            }
                        }
                    }

                    DFAlgoBank.SetUpTileCorridorTypesUI(pcgManager.gridArr, corridorThickness);

                    pcgManager.Plane.GetComponent<Renderer>().sharedMaterial.mainTexture = DFGeneralUtil.SetUpTextBiColShade(pcgManager.gridArr, 0, 1, true);
                }

                EditorGUI.EndDisabledGroup();

                if (pcgManager.prevGridArray2D.Count == 1)
                {
                    allowedForward = true;
                    allowedBack = false;
                }
                else
                {
                    allowedForward = false;
                    allowedBack = true;
                }
            }
            else if (rooms.Count > 2)
            {

                GUILayout.Label("Choose how to order the connection of the rooms");

                SpacesUILayout(2);

                GUILayout.BeginVertical("Box");
                selGridConnectionType = GUILayout.SelectionGrid(selGridConnectionType, DFEditorUtil.selStringsConnectionType, 1);
                GUILayout.EndVertical();

                SpacesUILayout(2);

                GUILayout.Label("Choose the Thickness of the corridor");

                corridorThickness = (int)EditorGUILayout.Slider(new GUIContent() { text = "Thickness of the corridor", tooltip = "How wide should the corridor be" }, corridorThickness, 2, 5);

                SpacesUILayout(3);

                GUILayout.Label("Choose the algorithm to that creates the corridor");

                SpacesUILayout(2);

                GUILayout.BeginVertical("Box");
                selGridPathGenType = GUILayout.SelectionGrid(selGridPathGenType, DFEditorUtil.selStringPathGenType, 1);
                GUILayout.EndVertical();

                SpacesUILayout(2);

                switch (selGridPathGenType)
                {
                    case 0:   // A* pathfindind
                        pathType = EditorGUILayout.Toggle(new GUIContent() { text = "Use Straight corridors", tooltip = "PathFinding will prioritize the creation of straight corridors" }, pathType);
                        useWeights = EditorGUILayout.Toggle(new GUIContent() { text = "Use weights", tooltip = "" }, useWeights);
                        break;

                    case 1:   // djistra 
                              //DjAvoidWalls = EditorGUILayout.Toggle(new GUIContent() { text = "Avoid Walls", tooltip = "" }, DjAvoidWalls);
                        break;
                    case 2:   // beizier 

                        bezierOndulation = (int)EditorGUILayout.Slider(new GUIContent() { text = "Curve Multiplier", tooltip = "A higher multiplier is going to equal to a a more extreme curver" }, bezierOndulation, 10, 40);

                        break;

                    default:
                        break;
                }

                EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

                SpacesUILayout(3);

                switch (selGridConnectionType)
                {
                    case 0:   // prims ran

                        if (rooms.Count >= 4)
                        {
                            randomAddCorr = (int)EditorGUILayout.Slider(new GUIContent() { text = "Additional random connections", tooltip = "Add another random connection. This number dictates how many times the script is going to TRY to add a new corridor" }, randomAddCorr, 0, rooms.Count / 2);
                            SpacesUILayout(2);
                        }
                        break;

                    case 2:

                        if (rooms.Count >= 4)
                        {
                            randomAddCorr = (int)EditorGUILayout.Slider(new GUIContent() { text = "Additional random connections", tooltip = "Add another random connection. This number dictates how many times the script is going to TRY to add a new corridor" }, randomAddCorr, 0, rooms.Count / 2);
                            SpacesUILayout(2);
                        }
                        break;

                    default:
                        break;
                }

                SpacesUILayout(1);
                EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
                SpacesUILayout(1);

                deadEndAmount = (int)EditorGUILayout.Slider(new GUIContent() { text = "Amount of dead end corridors", tooltip = "Dead end corridors start from somewhere in the dungeon and lead to nowhere" }, deadEndAmount, 0, 5);

                deadEndCorridorThickness = (int)EditorGUILayout.Slider(new GUIContent() { text = "Thickness of the dead end corridor", tooltip = "How wide should the corridor be" }, deadEndCorridorThickness, 3, 6);

                deadEndOndulation = (int)EditorGUILayout.Slider(new GUIContent() { text = "Curve Multiplier for dead end", tooltip = "A higher multiplier is going to equal to a a more extreme curver" }, deadEndOndulation, 10, 40);

                SpacesUILayout(2);

                EditorGUI.BeginDisabledGroup(pcgManager.prevGridArray2D.Count == 1);

                if (GUILayout.Button("Connect all the rooms"))// dfor the corridor making
                {
                    if (useWeights && pcgManager.tileCosts.Length != 6)
                    {
                        EditorUtility.DisplayDialog("Invalid Weights given", "The given weights are not accepted", "OK!");
                    }
                    else
                    {
                        pcgManager.CreateBackUpGrid();

                        rooms = DFAlgoBank.GetAllRooms(pcgManager.gridArr, true);
                        var centerPoints = new List<Vector2>();
                        var roomDict = new Dictionary<Vector2Int, List<DFTile>>();
                        foreach (var room in rooms)
                        {
                            var centerPoint = DFGeneralUtil.FindMiddlePoint(room);

                            roomDict.Add(new Vector2Int(Mathf.FloorToInt(centerPoint.x), Mathf.FloorToInt(centerPoint.y)), room);
                            centerPoints.Add(new Vector2Int(Mathf.FloorToInt(centerPoint.x), Mathf.FloorToInt(centerPoint.y)));
                        }

                        switch (selGridConnectionType)
                        {
                            case 0:
                                edges = DFAlgoBank.PrimAlgoNoDelu(centerPoints);
                                if (randomAddCorr > 0)
                                {
                                    int len = edges.Count - 1;

                                    for (int i = 0; i < randomAddCorr; i++)
                                    {
                                        var pointA = edges[Random.Range(0, len)].edge[0];
                                        var pointBEdgeCheck = edges[Random.Range(0, len)];

                                        Vector3 pointB;

                                        if (pointA == pointBEdgeCheck.edge[0])
                                            pointB = pointBEdgeCheck.edge[1];
                                        else if (pointA == pointBEdgeCheck.edge[1])
                                            pointB = pointBEdgeCheck.edge[0];
                                        else
                                            pointB = pointBEdgeCheck.edge[1];


                                        Edge newEdge = new Edge(pointA, pointB);

                                        bool toAdd = true;

                                        foreach (var primEdge in edges)
                                        {
                                            if (DFAlgoBank.LineIsEqual(primEdge, newEdge))
                                            {
                                                toAdd = false;
                                                break;
                                            }
                                        }


                                        if (toAdd)
                                        {
                                            edges.Add(newEdge);
                                        }
                                    }
                                }
                                break;

                            case 1:
                                edges = DFAlgoBank.DelaunayTriangulation(centerPoints).Item2;
                                break;

                            case 2:
                                {

                                    DFAlgoBank.ShuffleList(rooms);

                                    foreach (var item in roomDict.Keys)
                                    {
                                        Debug.Log(item);
                                    }

                                    for (int i = 0; i < centerPoints.Count; i++)
                                    {
                                        if (i == centerPoints.Count - 1) { continue; }
                                        edges.Add(new Edge(new Vector3(Mathf.FloorToInt(centerPoints[i].x), Mathf.FloorToInt(centerPoints[i].y), 0), new Vector3(Mathf.FloorToInt(centerPoints[i + 1].x), Mathf.FloorToInt(centerPoints[i + 1].y), 0)));
                                    }

                                    if (randomAddCorr > 0)
                                    {
                                        int len = edges.Count - 1;

                                        for (int i = 0; i < randomAddCorr; i++)
                                        {
                                            int ranStarter = Random.Range(0, len);
                                            int ranEnder = Random.Range(0, len);

                                            if (ranStarter == ranEnder) { continue; }
                                            else if (Mathf.Abs(ranStarter - ranEnder) == 1) { continue; }
                                            else
                                            {
                                                edges.Add(new Edge(new Vector3(centerPoints[ranStarter].x, centerPoints[ranStarter].y, 0), new Vector3(centerPoints[ranEnder].x, centerPoints[ranEnder].y, 0)));
                                            }
                                        }
                                    }
                                }
                                break;
                        }

                        switch (selGridPathGenType)
                        {
                            case 0:   //A* pathfingin

                                foreach (var edge in edges)
                                {
                                    var tileA = roomDict[new Vector2Int(Mathf.FloorToInt(edge.edge[0].x), Mathf.FloorToInt(edge.edge[0].y))][Random.Range(0, roomDict[new Vector2Int(Mathf.FloorToInt(edge.edge[0].x), Mathf.FloorToInt(edge.edge[0].y))].Count)].position;
                                    var tileB = roomDict[new Vector2Int(Mathf.FloorToInt(edge.edge[1].x), Mathf.FloorToInt(edge.edge[1].y))][Random.Range(0, roomDict[new Vector2Int(Mathf.FloorToInt(edge.edge[1].x), Mathf.FloorToInt(edge.edge[1].y))].Count)].position;

                                    var path = DFAlgoBank.A_StarPathfinding2D(pcgManager.gridArr, new Vector2Int(tileA.x, tileA.y), new Vector2Int(tileB.x, tileB.y), !pathType, useWeights: useWeights, arrWeights: pcgManager.tileCosts);

                                    DFAlgoBank.SetUpCorridorWithPath(path.Item1);
                                }

                                break;
                            case 1:  //dijistra
                                foreach (var edge in edges)
                                {
                                    var tileA = roomDict[new Vector2Int(Mathf.FloorToInt(edge.edge[0].x), Mathf.FloorToInt(edge.edge[0].y))][Random.Range(0, roomDict[new Vector2Int(Mathf.FloorToInt(edge.edge[0].x), Mathf.FloorToInt(edge.edge[0].y))].Count)].position;
                                    var tileB = roomDict[new Vector2Int(Mathf.FloorToInt(edge.edge[1].x), Mathf.FloorToInt(edge.edge[1].y))][Random.Range(0, roomDict[new Vector2Int(Mathf.FloorToInt(edge.edge[1].x), Mathf.FloorToInt(edge.edge[1].y))].Count)].position;

                                    var path = DFAlgoBank.DijstraPathfinding(pcgManager.gridArr, new Vector2Int(tileA.x, tileA.y), new Vector2Int(tileB.x, tileB.y));

                                    DFAlgoBank.SetUpCorridorWithPath(path);
                                }

                                break;

                            case 2://  beizier curve
                                foreach (var edge in edges)
                                {

                                    var tileA = roomDict[new Vector2Int(Mathf.FloorToInt(edge.edge[0].x), Mathf.FloorToInt(edge.edge[0].y))][Random.Range(0, roomDict[new Vector2Int(Mathf.FloorToInt(edge.edge[0].x), Mathf.FloorToInt(edge.edge[0].y))].Count)].position;
                                    var tileB = roomDict[new Vector2Int(Mathf.FloorToInt(edge.edge[1].x), Mathf.FloorToInt(edge.edge[1].y))][Random.Range(0, roomDict[new Vector2Int(Mathf.FloorToInt(edge.edge[1].x), Mathf.FloorToInt(edge.edge[1].y))].Count)].position;

                                    DFAlgoBank.BezierCurvePathing(new Vector2Int(tileA.x, tileA.y), new Vector2Int(tileB.x, tileB.y), bezierOndulation, pcgManager.gridArr);
                                }
                                break;

                            default:
                                break;
                        }

                        for (int i = 0; i < deadEndAmount; i++)
                        {
                            var room = rooms[DFGeneralUtil.ReturnRandomFromList(rooms)];

                            var randomTileInRoom = room[DFGeneralUtil.ReturnRandomFromList(room)];

                            DFTile randomTileOutsideOfRoom;

                            while (true)
                            {
                                var tile = pcgManager.gridArr[Random.Range(0, pcgManager.gridArr.GetLength(0)), Random.Range(0, pcgManager.gridArr.GetLength(1))];

                                if (tile.tileWeight == 0)
                                {
                                    randomTileOutsideOfRoom = tile;

                                    var tileA = randomTileOutsideOfRoom.position;
                                    var tileB = randomTileInRoom.position;

                                    DFAlgoBank.BezierCurvePathing(new Vector2Int(tileA.x, tileA.y), new Vector2Int(tileB.x, tileB.y), bezierOndulation, pcgManager.gridArr);

                                    break;
                                }
                            }
                        }

                        DFAlgoBank.SetUpTileCorridorTypesUI(pcgManager.gridArr, corridorThickness);

                        pcgManager.Plane.GetComponent<Renderer>().sharedMaterial.mainTexture = DFGeneralUtil.SetUpTextBiColShade(pcgManager.gridArr, 0, 1, true);
                    }

                }

                EditorGUI.EndDisabledGroup();


                if (pcgManager.prevGridArray2D.Count == 1)
                {
                    allowedForward = true;

                    allowedBack = false;
                }
                else
                {
                    allowedForward = false;

                    allowedBack = true;
                }
            }
            else
            {
                GUILayout.Label("To access the corridor making function you need to\nGenerate the rooms first");
            }
        }

        public static void SaveGridDataToGenerateEditorSection(PCGManager pcgManager, string inSaveMapFileName, out string saveMapFileName)
        {
            saveMapFileName = EditorGUILayout.TextField("Save file name: ", inSaveMapFileName);
            if (GUILayout.Button("save"))
            {
                DFAlgoBank.SaveTileArrayData(pcgManager.gridArr, inSaveMapFileName);
            }
            SpacesUILayout(2);

            GUILayout.Label("Once saved you can access this data later on.\nTo Generate your dungeon switch to the Generate component (in the main algo selection) and give this file name");
        }


        public enum UI_STATE
        {
            MAIN_ALGO,
            CA,
            ROOM_GEN,
            EXTRA_ROOM_GEN,
            PATHING,
            GENERATION
        }

        public enum PathFindingType
        {
            A_STAR,
            DJISTRA,
            BFS,
            DFS
        }


        public static GUIContent[] selStringsConnectionType = { new GUIContent() { text = "Prims's algo", tooltip = "Create a singualar path that traverses the whole dungeon" }, new GUIContent() { text = "Delunary trig", tooltip = "One rooms can have many corridors" }, new GUIContent() { text = "Random", tooltip = "Completly random allocation of corridor connections" } };

        public static GUIContent[] selStringsGenType = { new GUIContent() { text = "Vertice Generation", tooltip = "Using the algorithm marching cubes create a mesh object which can be exported to other 3D softwares" }, new GUIContent() { text = "TileSet Generation", tooltip = "Generate the Dungeon using the tileset provided" } };

        public static GUIContent[] selStringPathGenType = { new GUIContent() { text = "A* pathfinding", tooltip = "Weights are available for this algortihm, remember to create the ruleSet" }, new GUIContent() { text = "Dijistra", tooltip = "" }, new GUIContent() { text = "Beizier Curve", tooltip = "Create curved corridors" } };
    }
}