namespace DungeonForge.Editor
{
    using UnityEditor;
    using UnityEngine;
    using DungeonForge.Utils;
    using DungeonForge.AlgoScript;

    [CustomEditor(typeof(PerlinWormsMA))]
    public class PerlinWormsEditor : Editor
    {
        bool showRules = false;

        int wormLength = 40;
        float turnMultiplier = 0.5f;

        string saveMapFileName;

        int wormThickness = 2;

        int width = 10;
        int height = 10;
        int radius = 10;

        bool useWeights = false;

        int corridorThickness = 2;

        int selGridConnectionType = 0;
        int selGridPathGenType = 0;

        int randomAddCorr = 0;

        int bezierOndulation = 20;
        int deadEndOndulation = 20;

        int deadEndAmount = 0;

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            PerlinWormsMA mainScript = (PerlinWormsMA)target;

            #region explanation

            showRules = EditorGUILayout.BeginFoldoutHeaderGroup(showRules, "Introduction");

            if (showRules)
            {
                GUILayout.TextArea("Perlin Worms is an extension of the Perlin Noise algorithm, which uses \"worms\" that follow a Perlin Noise-generated path to carve out corridors within the dungeon space. This creates a network of winding tunnels and caverns, making it ideal for generating cave-like environments or dungeons with a labyrinthine structure.\n\nVisit the wiki for more informations: https://github.com/AlessandroBufalino3115/Dungeon-Forge/wiki/Using-the-Pack#4-perlin-worms");
            }

            if (!Selection.activeTransform)
            {
                showRules = false;
            }

            EditorGUILayout.EndFoldoutHeaderGroup();

            #endregion

            DFEditorUtil.SpacesUILayout(4);

            #region Main algo region

            switch (mainScript.currUiState)
            {
                case PerlinWormsMA.UI_STATE.WORM_CREATION:
                    {
                        mainScript.allowedBack = false;

                        wormLength = (int)EditorGUILayout.Slider(new GUIContent() { text = "Worm Maximum Length", tooltip = "" }, wormLength, 10, 500);
                        turnMultiplier = EditorGUILayout.Slider(new GUIContent() { text = "Turn Multiplier", tooltip = "The higher the number the more twisty the worm will look" }, turnMultiplier, 0.2f, 1.5f);

                        mainScript.offsetX = (int)EditorGUILayout.Slider(new GUIContent() { text = "Perlin Offset X", tooltip = "The X offset of the Perlin noise pattern" }, mainScript.offsetX, 0, 10000);
                        mainScript.offsetY = (int)EditorGUILayout.Slider(new GUIContent() { text = "Perlin Offset Y", tooltip = "The Y offset of the Perlin noise pattern" }, mainScript.offsetY, 0, 10000);
                        
                        mainScript.scale = EditorGUILayout.Slider(new GUIContent() { text = "Perlin Scale", tooltip = "The scale of the Perlin noise pattern" }, mainScript.scale, 3f, 35f);
                        mainScript.octaves = (int)EditorGUILayout.Slider(new GUIContent() { text = "Perlin Octaves", tooltip = "The number of layers of Perlin noise to combine" }, mainScript.octaves, 1, 8);
                        
                        mainScript.persistance = EditorGUILayout.Slider(new GUIContent() { text = "Perlin Persitance", tooltip = "The rate at which the amplitude decreases for each successive octave" }, mainScript.persistance, 0.1f, 0.9f);
                        mainScript.lacunarity = EditorGUILayout.Slider(new GUIContent() { text = "Perlin Lacunarity", tooltip = "The rate at which the frequency increases for each successive octave" }, mainScript.lacunarity, 0.5f, 10f);
                        
                        DFEditorUtil.SpacesUILayout(2);

                        if (GUILayout.Button("Generate One Worm"))
                        {
                            mainScript.numberOfWorms++;

                            mainScript.pcgManager.CreateBackUpGrid();

                            var worm = DFAlgoBank.PerlinWorms(mainScript.pcgManager.gridArr, mainScript.scale, mainScript.octaves, mainScript.persistance, mainScript.lacunarity, mainScript.offsetX, mainScript.offsetY, wormLength, turnMultiplier);

                            mainScript.wormsTiles.UnionWith(worm);

                            for (int y = 0; y < mainScript.pcgManager.gridArr.GetLength(1); y++)
                            {
                                for (int x = 0; x < mainScript.pcgManager.gridArr.GetLength(0); x++)
                                {
                                    if (mainScript.wormsTiles.Contains(mainScript.pcgManager.gridArr[x, y]))
                                    {
                                        mainScript.pcgManager.gridArr[x, y].tileWeight = 1;
                                        mainScript.pcgManager.gridArr[x, y].tileType = DFTile.TileType.FLOORCORRIDOR;
                                    }
                                    else
                                    {
                                        mainScript.pcgManager.gridArr[x, y].tileWeight = 0;
                                    }
                                }
                            }

                            mainScript.pcgManager.Plane.GetComponent<Renderer>().sharedMaterial.mainTexture = DFGeneralUtil.SetUpTextBiColAnchor(mainScript.pcgManager.gridArr);

                        }

                        if (GUILayout.Button("Restart"))
                        {
                            mainScript.pcgManager.Restart();
                            mainScript.numberOfWorms = 0;
                        }


                        DFEditorUtil.SpacesUILayout(2);


                        wormThickness = (int)EditorGUILayout.Slider(new GUIContent() { text = "Worm Thickness", tooltip = "" }, wormThickness, 2, 5);


                        if (mainScript.numberOfWorms == 0)
                        {
                            mainScript.allowedForward = false;
                        }
                        else
                        {
                            mainScript.allowedForward = true;
                        }
                    }
                    break;

                case PerlinWormsMA.UI_STATE.EXTRA_ROOM:
                    {
                        mainScript.allowedForward = true;
                        mainScript.allowedBack = false;

                        DFEditorUtil.ExtraRoomEditorSelection(mainScript.pcgManager, mainScript.rooms, ref radius, ref height, ref width);
                    }
                    break;
                case PerlinWormsMA.UI_STATE.PATHING:

                    #region corridor making region
                    mainScript.allowedBack = true;


                    DFEditorUtil.GenerateCorridorsEditorSection(mainScript.pcgManager, mainScript.rooms, ref mainScript.allowedForward, ref mainScript.allowedBack, ref corridorThickness
                     , ref selGridConnectionType, ref selGridPathGenType, ref useWeights, ref bezierOndulation, ref mainScript.pathType, ref randomAddCorr, ref deadEndAmount, ref deadEndOndulation, ref mainScript.edges);


                    //if (mainScript.rooms.Count == 1)
                    //{
                    //    mainScript.allowedForward = true;
                    //    GUILayout.Label("Only one room detected, Corridor making is not possible");



                    //}
                    //else if (mainScript.rooms.Count == 2)
                    //{
                    //    GUILayout.Label("Only two rooms detected, triangulation not possible");

                    //    GUILayout.Label("Choose the algorithm to create the corridor");

                    //    DFEditorUtil.SpacesUILayout(2);

                    //    GUILayout.BeginVertical("Box");
                    //    selGridPathGenType = GUILayout.SelectionGrid(selGridPathGenType, DFEditorUtil.selStringPathGenType, 1);
                    //    GUILayout.EndVertical();

                    //    DFEditorUtil.SpacesUILayout(2);

                    //    switch (selGridPathGenType)
                    //    {
                    //        case 0:   // A* pathfindind
                    //            mainScript.pathType = EditorGUILayout.Toggle(new GUIContent() { text = "Use Straight corridors", tooltip = "PathFinding will prioritize the creation of straight corridors" }, mainScript.pathType);
                    //            useWeights = EditorGUILayout.Toggle(new GUIContent() { text = "Use weights", tooltip = "" }, useWeights);
                    //            break;

                    //        case 1:   // djistra 
                    //            //DjAvoidWalls = EditorGUILayout.Toggle(new GUIContent() { text = "Avoid Walls", tooltip = "" });
                    //            break;
                    //        case 2:   // beizier 

                    //            bezierOndulation = (int)EditorGUILayout.Slider(new GUIContent() { text = "Curve Multiplier", tooltip = "beizeir curve thing to change" }, bezierOndulation, 10, 40);

                    //            DFEditorUtil.SpacesUILayout(1);

                    //            //mainScript.pathType = EditorGUILayout.Toggle(new GUIContent() { text = "Use Straight corridors", tooltip = "PathFinding will prioritize the creation of straight corridors" }, mainScript.pathType);

                    //            break;

                    //        default:
                    //            break;
                    //    }


                    //    EditorGUI.BeginDisabledGroup(mainScript.pcgManager.prevGridArray2D.Count == 1);

                    //    if (GUILayout.Button("Connect all the rooms"))// dfor the corridor making
                    //    {
                    //        mainScript.pcgManager.CreateBackUpGrid();

                    //        Vector2Int tileA = mainScript.rooms[0][Random.Range(0, mainScript.rooms[0].Count - 1)].position;
                    //        Vector2Int tileB = mainScript.rooms[1][Random.Range(0, mainScript.rooms[1].Count - 1)].position;


                    //        mainScript.allowedForward = true;

                    //        switch (selGridPathGenType)
                    //        {
                    //            case 0:   //A* pathfingin

                    //                var path = DFAlgoBank.A_StarPathfinding2D(mainScript.pcgManager.gridArr, new Vector2Int(tileA.x, tileA.y), new Vector2Int(tileB.x, tileB.y), !mainScript.pathType, useWeights: useWeights, arrWeights: mainScript.pcgManager.tileCosts);

                    //                DFAlgoBank.SetUpCorridorWithPath(path.Item1);

                    //                break;
                    //            case 1:  //dijistra

                    //                var pathD = DFAlgoBank.DijstraPathfinding(mainScript.pcgManager.gridArr, new Vector2Int(tileA.x, tileA.y), new Vector2Int(tileB.x, tileB.y));

                    //                DFAlgoBank.SetUpCorridorWithPath(pathD);


                    //                break;
                    //            case 2://  beizier curve

                    //                DFAlgoBank.BezierCurvePathing(new Vector2Int(tileA.x, tileA.y), new Vector2Int(tileB.x, tileB.y), bezierOndulation, mainScript.pcgManager.gridArr);

                    //                break;

                    //            default:
                    //                break;
                    //        }


                    //        DFAlgoBank.SetUpTileCorridorTypesUI(mainScript.pcgManager.gridArr, corridorThickness);

                    //        mainScript.pcgManager.Plane.GetComponent<Renderer>().sharedMaterial.mainTexture = DFGeneralUtil.SetUpTextBiColShade(mainScript.pcgManager.gridArr, 0, 1, true);
                    //    }

                    //    EditorGUI.EndDisabledGroup();


                    //    if (mainScript.pcgManager.prevGridArray2D.Count == 1)
                    //    {
                    //        mainScript.allowedForward = true;

                    //        mainScript.allowedBack = false;
                    //    }
                    //    else
                    //    {
                    //        mainScript.allowedForward = false;

                    //        mainScript.allowedBack = true;
                    //    }
                    //}
                    //else if (mainScript.rooms.Count > 2)
                    //{

                    //    GUILayout.Label("Choose how to order the connection of the rooms");

                    //    DFEditorUtil.SpacesUILayout(2);

                    //    GUILayout.BeginVertical("Box");
                    //    selGridConnectionType = GUILayout.SelectionGrid(selGridConnectionType, DFEditorUtil.selStringsConnectionType, 1);
                    //    GUILayout.EndVertical();

                    //    DFEditorUtil.SpacesUILayout(2);

                    //    GUILayout.Label("Choose the Thickness of the corridor");

                    //    corridorThickness = (int)EditorGUILayout.Slider(new GUIContent() { text = "Thickness of the corridor", tooltip = "How wide should the corridor be" }, corridorThickness, 2, 5);

                    //    DFEditorUtil.SpacesUILayout(3);


                    //    GUILayout.Label("Choose the algorithm to that creates the corridor");


                    //    DFEditorUtil.SpacesUILayout(2);

                    //    GUILayout.BeginVertical("Box");
                    //    selGridPathGenType = GUILayout.SelectionGrid(selGridPathGenType, DFEditorUtil.selStringPathGenType, 1);
                    //    GUILayout.EndVertical();

                    //    DFEditorUtil.SpacesUILayout(2);


                    //    switch (selGridPathGenType)
                    //    {
                    //        case 0:   // A* pathfindind
                    //            mainScript.pathType = EditorGUILayout.Toggle(new GUIContent() { text = "Use Straight corridors", tooltip = "PathFinding will prioritize the creation of straight corridors" }, mainScript.pathType);
                    //            useWeights = EditorGUILayout.Toggle(new GUIContent() { text = "Use weights", tooltip = "" }, useWeights);
                    //            break;

                    //        case 1:   // djistra 
                    //            //DjAvoidWalls = EditorGUILayout.Toggle(new GUIContent() { text = "Avoid Walls", tooltip = "" });
                    //            break;
                    //        case 2:   // beizier 

                    //            bezierOndulation = (int)EditorGUILayout.Slider(new GUIContent() { text = "Curve Multiplier", tooltip = "A higher multiplier is going to equal to a a more extreme curver" }, bezierOndulation, 10, 40);

                    //            break;

                    //        default:
                    //            break;
                    //    }


                    //    EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

                    //    DFEditorUtil.SpacesUILayout(3);

                    //    switch (selGridConnectionType)
                    //    {
                    //        case 0:   // prims ran

                    //            if (mainScript.rooms.Count >= 4)
                    //            {
                    //                randomAddCorr = (int)EditorGUILayout.Slider(new GUIContent() { text = "Additional random connections", tooltip = "Add another random connection. This number dictates how many times the script is going to TRY to add a new corridor" }, randomAddCorr, 0, mainScript.rooms.Count / 2);
                    //                DFEditorUtil.SpacesUILayout(2);
                    //            }
                    //            break;

                    //        case 2:

                    //            if (mainScript.rooms.Count >= 4)
                    //            {
                    //                randomAddCorr = (int)EditorGUILayout.Slider(new GUIContent() { text = "Additional random connections", tooltip = "Add another random connection. This number dictates how many times the script is going to TRY to add a new corridor" }, randomAddCorr, 0, mainScript.rooms.Count / 2);
                    //                DFEditorUtil.SpacesUILayout(2);
                    //            }
                    //            break;

                    //        default:
                    //            break;
                    //    }


                    //    DFEditorUtil.SpacesUILayout(1);
                    //    EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
                    //    DFEditorUtil.SpacesUILayout(1);


                    //    deadEndAmount = (int)EditorGUILayout.Slider(new GUIContent() { text = "Amount of dead end corridors", tooltip = "Dead end corridors start from somewhere in the dungeon and lead to nowhere" }, deadEndAmount, 0, 5);

                    //    deadEndCorridorThickness = (int)EditorGUILayout.Slider(new GUIContent() { text = "Thickness of the dead end corridor", tooltip = "How wide should the corridor be" }, deadEndCorridorThickness, 3, 6);

                    //    deadEndOndulation = (int)EditorGUILayout.Slider(new GUIContent() { text = "Curve Multiplier for dead end", tooltip = "A higher multiplier is going to equal to a a more extreme curver" }, deadEndOndulation, 10, 40);

                    //    DFEditorUtil.SpacesUILayout(2);


                    //    EditorGUI.BeginDisabledGroup(mainScript.pcgManager.prevGridArray2D.Count == 1);

                    //    if (GUILayout.Button("Connect all the rooms"))// dfor the corridor making
                    //    {
                    //        mainScript.pcgManager.CreateBackUpGrid();

                    //        mainScript.rooms = DFAlgoBank.GetAllRooms(mainScript.pcgManager.gridArr, true);
                    //        var centerPoints = new List<Vector2>();
                    //        var roomDict = new Dictionary<Vector2Int, List<DFTile>>();
                    //        foreach (var room in mainScript.rooms)
                    //        {
                    //            var centerPoint = DFGeneralUtil.FindMiddlePoint(room);

                    //            roomDict.Add(new Vector2Int(Mathf.FloorToInt(centerPoint.x), Mathf.FloorToInt(centerPoint.y)), room);
                    //            centerPoints.Add(new Vector2Int(Mathf.FloorToInt(centerPoint.x), Mathf.FloorToInt(centerPoint.y)));
                    //        }

                    //        switch (selGridConnectionType)
                    //        {
                    //            case 0:
                    //                mainScript.edges = DFAlgoBank.PrimAlgoNoDelu(centerPoints);
                    //                if (randomAddCorr > 0)
                    //                {
                    //                    int len = mainScript.edges.Count - 1;

                    //                    for (int i = 0; i < randomAddCorr; i++)
                    //                    {
                    //                        var pointA = mainScript.edges[Random.Range(0, len)].edge[0];
                    //                        var pointBEdgeCheck = mainScript.edges[Random.Range(0, len)];

                    //                        Vector3 pointB;

                    //                        if (pointA == pointBEdgeCheck.edge[0])
                    //                            pointB = pointBEdgeCheck.edge[1];
                    //                        else if (pointA == pointBEdgeCheck.edge[1])
                    //                            pointB = pointBEdgeCheck.edge[0];
                    //                        else
                    //                            pointB = pointBEdgeCheck.edge[1];


                    //                        Edge newEdge = new Edge(pointA, pointB);

                    //                        bool toAdd = true;

                    //                        foreach (var primEdge in mainScript.edges)
                    //                        {
                    //                            if (DFAlgoBank.LineIsEqual(primEdge, newEdge))
                    //                            {
                    //                                toAdd = false;
                    //                                break;
                    //                            }
                    //                        }


                    //                        if (toAdd)
                    //                        {
                    //                            mainScript.edges.Add(newEdge);
                    //                        }
                    //                    }
                    //                }
                    //                break;

                    //            case 1:
                    //                mainScript.edges = DFAlgoBank.DelaunayTriangulation(centerPoints).Item2;
                    //                break;

                    //            case 2://ran
                    //                {

                    //                    DFAlgoBank.ShuffleList(mainScript.rooms);

                    //                    foreach (var item in roomDict.Keys)
                    //                    {
                    //                        Debug.Log(item);
                    //                    }

                    //                    for (int i = 0; i < centerPoints.Count; i++)
                    //                    {
                    //                        if (i == centerPoints.Count - 1) { continue; }
                    //                        mainScript.edges.Add(new Edge(new Vector3(Mathf.FloorToInt(centerPoints[i].x), Mathf.FloorToInt(centerPoints[i].y), 0), new Vector3(Mathf.FloorToInt(centerPoints[i + 1].x), Mathf.FloorToInt(centerPoints[i + 1].y), 0)));
                    //                    }

                    //                    if (randomAddCorr > 0)
                    //                    {
                    //                        int len = mainScript.edges.Count - 1;

                    //                        for (int i = 0; i < randomAddCorr; i++)
                    //                        {
                    //                            int ranStarter = Random.Range(0, len);
                    //                            int ranEnder = Random.Range(0, len);

                    //                            if (ranStarter == ranEnder) { continue; }
                    //                            else if (Mathf.Abs(ranStarter - ranEnder) == 1) { continue; }
                    //                            else
                    //                            {
                    //                                mainScript.edges.Add(new Edge(new Vector3(centerPoints[ranStarter].x, centerPoints[ranStarter].y, 0), new Vector3(centerPoints[ranEnder].x, centerPoints[ranEnder].y, 0)));
                    //                            }
                    //                        }
                    //                    }
                    //                }
                    //                break;
                    //        }

                    //        //its a roudning error

                    //        switch (selGridPathGenType)
                    //        {
                    //            case 0:   //A* pathfingin

                    //                foreach (var edge in mainScript.edges)
                    //                {
                    //                    //use where so we get soemthing its not the wall but not necessary
                    //                    var tileA = roomDict[new Vector2Int(Mathf.FloorToInt(edge.edge[0].x), Mathf.FloorToInt(edge.edge[0].y))][Random.Range(0, roomDict[new Vector2Int(Mathf.FloorToInt(edge.edge[0].x), Mathf.FloorToInt(edge.edge[0].y))].Count)].position;
                    //                    var tileB = roomDict[new Vector2Int(Mathf.FloorToInt(edge.edge[1].x), Mathf.FloorToInt(edge.edge[1].y))][Random.Range(0, roomDict[new Vector2Int(Mathf.FloorToInt(edge.edge[1].x), Mathf.FloorToInt(edge.edge[1].y))].Count)].position;

                    //                    var path = DFAlgoBank.A_StarPathfinding2D(mainScript.pcgManager.gridArr, new Vector2Int(tileA.x, tileA.y), new Vector2Int(tileB.x, tileB.y), !mainScript.pathType, useWeights: useWeights, arrWeights: mainScript.pcgManager.tileCosts);

                    //                    DFAlgoBank.SetUpCorridorWithPath(path.Item1);
                    //                }

                    //                break;
                    //            case 1:  //dijistra
                    //                foreach (var edge in mainScript.edges)
                    //                {
                    //                    var tileA = roomDict[new Vector2Int(Mathf.FloorToInt(edge.edge[0].x), Mathf.FloorToInt(edge.edge[0].y))][Random.Range(0, roomDict[new Vector2Int(Mathf.FloorToInt(edge.edge[0].x), Mathf.FloorToInt(edge.edge[0].y))].Count)].position;
                    //                    var tileB = roomDict[new Vector2Int(Mathf.FloorToInt(edge.edge[1].x), Mathf.FloorToInt(edge.edge[1].y))][Random.Range(0, roomDict[new Vector2Int(Mathf.FloorToInt(edge.edge[1].x), Mathf.FloorToInt(edge.edge[1].y))].Count)].position;

                    //                    var path = DFAlgoBank.DijstraPathfinding(mainScript.pcgManager.gridArr, new Vector2Int(tileA.x, tileA.y), new Vector2Int(tileB.x, tileB.y));

                    //                    DFAlgoBank.SetUpCorridorWithPath(path);
                    //                }

                    //                break;

                    //            case 2://  beizier curve
                    //                foreach (var edge in mainScript.edges)
                    //                {
                    //                    //  Debug.Log(new Vector2Int(Mathf.FloorToInt(edge.edge[0].x), Mathf.FloorToInt(edge.edge[0].z));

                    //                    var tileA = roomDict[new Vector2Int(Mathf.FloorToInt(edge.edge[0].x), Mathf.FloorToInt(edge.edge[0].y))][Random.Range(0, roomDict[new Vector2Int(Mathf.FloorToInt(edge.edge[0].x), Mathf.FloorToInt(edge.edge[0].y))].Count)].position;
                    //                    var tileB = roomDict[new Vector2Int(Mathf.FloorToInt(edge.edge[1].x), Mathf.FloorToInt(edge.edge[1].y))][Random.Range(0, roomDict[new Vector2Int(Mathf.FloorToInt(edge.edge[1].x), Mathf.FloorToInt(edge.edge[1].y))].Count)].position;

                    //                    DFAlgoBank.BezierCurvePathing(new Vector2Int(tileA.x, tileA.y), new Vector2Int(tileB.x, tileB.y), bezierOndulation, mainScript.pcgManager.gridArr);
                    //                }
                    //                break;

                    //            default:
                    //                break;
                    //        }

                    //        for (int i = 0; i < deadEndAmount; i++)
                    //        {
                    //            var room = mainScript.rooms[DFGeneralUtil.ReturnRandomFromList(mainScript.rooms)];

                    //            var randomTileInRoom = room[DFGeneralUtil.ReturnRandomFromList(room)];

                    //            DFTile randomTileOutsideOfRoom;

                    //            while (true)
                    //            {
                    //                var tile = mainScript.pcgManager.gridArr[Random.Range(0, mainScript.pcgManager.gridArr.GetLength(0)), Random.Range(0, mainScript.pcgManager.gridArr.GetLength(1))];

                    //                if (tile.tileWeight == 0)
                    //                {
                    //                    randomTileOutsideOfRoom = tile;

                    //                    var tileA = randomTileOutsideOfRoom.position;
                    //                    var tileB = randomTileInRoom.position;

                    //                    DFAlgoBank.BezierCurvePathing(new Vector2Int(tileA.x, tileA.y), new Vector2Int(tileB.x, tileB.y), bezierOndulation, mainScript.pcgManager.gridArr);

                    //                    break;
                    //                }
                    //            }
                    //        }

                    //        DFAlgoBank.SetUpTileCorridorTypesUI(mainScript.pcgManager.gridArr, corridorThickness);

                    //        mainScript.pcgManager.Plane.GetComponent<Renderer>().sharedMaterial.mainTexture = DFGeneralUtil.SetUpTextBiColShade(mainScript.pcgManager.gridArr, 0, 1, true);
                    //    }

                    //    EditorGUI.EndDisabledGroup();


                    //    if (mainScript.pcgManager.prevGridArray2D.Count == 1)
                    //    {
                    //        mainScript.allowedForward = true;

                    //        mainScript.allowedBack = false;
                    //    }
                    //    else
                    //    {
                    //        mainScript.allowedForward = false;

                    //        mainScript.allowedBack = true;
                    //    }
                    //}

                    //else
                    //{
                    //    GUILayout.Label("To access the corridor making function you need to\nGenerate the rooms first");
                    //}


                    #endregion

                    break;
                case PerlinWormsMA.UI_STATE.GENERATE:

                    DFEditorUtil.SaveGridDataToGenerateEditorSection(mainScript.pcgManager, saveMapFileName, out saveMapFileName);
                    break;
                default:
                    break;
            }

            #endregion

            if (mainScript.currUiState != PerlinWormsMA.UI_STATE.GENERATE)
            {
                DFEditorUtil.SpacesUILayout(4);

                EditorGUI.BeginDisabledGroup(mainScript.allowedBack == false);

                if (GUILayout.Button(new GUIContent() { text = "Go Back", tooltip = mainScript.allowedForward == true ? "Press this to go back one step" : "You cant go back" }))// gen something
                {
                    mainScript.pcgManager.ClearUndos();
                    mainScript.allowedBack = false;
                    mainScript.currStateIndex--;
                    mainScript.currUiState = (PerlinWormsMA.UI_STATE)mainScript.currStateIndex;
                }

                EditorGUI.EndDisabledGroup();


                EditorGUI.BeginDisabledGroup(mainScript.allowedForward == false);

                if (GUILayout.Button(new GUIContent() { text = "Continue", tooltip = mainScript.allowedForward == true ? "Press this to continue to the next step" : "You need to finish this step to continue" }))// gen something
                {
                    if (mainScript.currUiState == PerlinWormsMA.UI_STATE.WORM_CREATION)
                    {
                        DFAlgoBank.SetUpTileCorridorTypesUI(mainScript.pcgManager.gridArr, wormThickness);
                        mainScript.rooms = DFAlgoBank.GetAllRooms(mainScript.pcgManager.gridArr, true, DFTile.TileType.FLOORCORRIDOR);
                    }

                    mainScript.pcgManager.ClearUndos();
                    mainScript.allowedForward = false;
                    mainScript.currStateIndex++;
                    mainScript.currUiState = (PerlinWormsMA.UI_STATE)mainScript.currStateIndex;

                }

                EditorGUI.EndDisabledGroup();
            }

        }
    }
}