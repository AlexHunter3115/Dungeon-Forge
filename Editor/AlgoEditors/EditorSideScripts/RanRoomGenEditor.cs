
namespace DungeonForge.Editor
{
    using System.Collections.Generic;
    using UnityEditor;
    using UnityEngine;
    using DungeonForge.Utils;
    using DungeonForge.AlgoScript;

    [CustomEditor(typeof(RanRoomGenMA))]
    public class RanRoomGenEditor : Editor
    {
        bool specialAlgo;
        bool CAselected;

        int neighboursNeeded;

        int minSize;

        bool showRules = false;

        int selDungGenType = 0;
        GUIContent[] selStringsDungGenType = { new GUIContent() { text = "BPS", tooltip = "" }, new GUIContent() { text = "Random", tooltip = "" }, new GUIContent() { text = "Room to Room", tooltip = "" } };

        int selGridGenType = 0;

        bool useWeights = false;
        bool DjAvoidWalls = false;

        int corridorThickness = 2;

        int selGridConnectionType = 0;

        int selGridPathGenType = 0;

        int bezierOndulation = 20;
        int deadEndOndulation = 20;

        string saveMapFileName = "";

        int radius = 10;
        int width = 10;
        int height = 10;

        int randomAddCorr = 0;

        int deadEndAmount = 0;
        int deadEndCorridorThickness = 0;

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            RanRoomGenMA mainScript = (RanRoomGenMA)target;

            #region explanation

            showRules = EditorGUILayout.BeginFoldoutHeaderGroup(showRules, "Instructions");

            if (showRules)
            {
                GUILayout.TextArea("You have choosen random room allocation");

            }

            if (!Selection.activeTransform)
            {
                showRules = false;
            }

            EditorGUILayout.EndFoldoutHeaderGroup();


            DFGeneralUtil.SpacesUILayout(4);

            #endregion

            switch (mainScript.state)
            {
                case RanRoomGenMA.UI_STATE.MAIN_ALGO:
                    {
                        #region main Algo

                        mainScript.allowedBack = false;
                        GUILayout.BeginVertical("Box");
                        selDungGenType = GUILayout.SelectionGrid(selDungGenType, selStringsDungGenType, 1);
                        GUILayout.EndVertical();

                        DFGeneralUtil.SpacesUILayout(3);


                        switch (selDungGenType)
                        {
                            case 0:   //bps

                                mainScript.MinHeight = (int)EditorGUILayout.Slider(new GUIContent() { text = "Min Height of the room", tooltip = "" }, mainScript.MinHeight, 10, 75);

                                mainScript.MinWidth = (int)EditorGUILayout.Slider(new GUIContent() { text = "Min Width of the room", tooltip = "" }, mainScript.MinWidth, 10, 75);

                                mainScript.NumOfRoom = (int)EditorGUILayout.Slider(new GUIContent() { text = "Maximum number of rooms to spawn", tooltip = "" }, mainScript.NumOfRoom, 3, 30);

                                specialAlgo = EditorGUILayout.Toggle("special algo toggle", specialAlgo);
                                if (specialAlgo)
                                {
                                    CAselected = EditorGUILayout.Toggle(CAselected == true ? "CA selected for gen" : "drunk walk selected", CAselected);
                                    if (CAselected)
                                    {

                                        neighboursNeeded = (int)EditorGUILayout.Slider(neighboursNeeded, 3, 5);
                                    }
                                }

                                if (GUILayout.Button("Use BPS algo"))// gen something
                                {
                                    mainScript.RoomList.Clear();

                                    mainScript.rooms.Clear();

                                    DFGeneralUtil.RestartGrid(mainScript.PcgManager.gridArr);

                                    mainScript.BPSRoomGen(mainScript.PcgManager.gridArr);
                                    mainScript.SetUpWeights(mainScript.PcgManager.gridArr);

                                    if (specialAlgo)
                                        mainScript.RanRoom(mainScript.BPSg, CAselected);

                                    mainScript.PcgManager.Plane.GetComponent<Renderer>().sharedMaterial.mainTexture = DFGeneralUtil.SetUpTextBiColAnchor(mainScript.PcgManager.gridArr, true);
                                    mainScript.allowedForward = true;
                                }


                                break;

                            case 1:  //random

                                mainScript.MaxHeight = (int)EditorGUILayout.Slider(new GUIContent() { text = "Max height of the room", tooltip = "" }, mainScript.MaxHeight, 15, 125);

                                if (mainScript.MaxHeight <= mainScript.MinHeight)
                                {
                                    mainScript.MaxHeight = mainScript.MinHeight + 1;
                                }
                                mainScript.MinHeight = (int)EditorGUILayout.Slider(new GUIContent() { text = "Min height of the room", tooltip = "" }, mainScript.MinHeight, 10, 75);


                                mainScript.MaxWidth = (int)EditorGUILayout.Slider(new GUIContent() { text = "Max width of the room", tooltip = "" }, mainScript.MaxWidth, 15, 125);
                                if (mainScript.MaxWidth <= mainScript.MinWidth)
                                {
                                    mainScript.MaxWidth = mainScript.MinWidth + 1;
                                }

                                mainScript.MinWidth = (int)EditorGUILayout.Slider(new GUIContent() { text = "Min width of the room", tooltip = "" }, mainScript.MinWidth, 10, 75);
                                mainScript.NumOfRoom = (int)EditorGUILayout.Slider(new GUIContent() { text = "Maximum number of rooms to spawn", tooltip = "" }, mainScript.NumOfRoom, 3, 30);

                                specialAlgo = EditorGUILayout.Toggle("special algo toggle", specialAlgo);
                                if (specialAlgo)
                                {
                                    CAselected = EditorGUILayout.Toggle(CAselected == true ? "CA selected for gen" : "drunk walk selected", CAselected);
                                    if (CAselected)
                                    {
                                        neighboursNeeded = (int)EditorGUILayout.Slider(neighboursNeeded, 3, 5);
                                    }
                                }

                                if (GUILayout.Button("Use random Room gen"))// gen something
                                {
                                    mainScript.roomList.Clear();
                                    mainScript.rooms.Clear();

                                    DFGeneralUtil.RestartGrid(mainScript.PcgManager.gridArr);

                                    mainScript.RandomRoomGen(mainScript.PcgManager.gridArr);
                                    mainScript.SetUpWeights(mainScript.PcgManager.gridArr);

                                    if (specialAlgo)
                                        mainScript.RanRoom(mainScript.BPSg, CAselected);

                                    mainScript.PcgManager.Plane.GetComponent<Renderer>().sharedMaterial.mainTexture = DFGeneralUtil.SetUpTextBiColAnchor(mainScript.PcgManager.gridArr, true);
                                    mainScript.allowedForward = true;
                                }

                                break;


                            case 2:  //cor cor

                                mainScript.MaxHeight = (int)EditorGUILayout.Slider(new GUIContent() { text = "Max height of the room", tooltip = "" }, mainScript.MaxHeight, 15, 125);

                                if (mainScript.MaxHeight <= mainScript.MinHeight)
                                {
                                    mainScript.MaxHeight = mainScript.MinHeight + 1;
                                }
                                mainScript.MinHeight = (int)EditorGUILayout.Slider(new GUIContent() { text = "Min height of the room", tooltip = "" }, mainScript.MinHeight, 10, 75);


                                mainScript.MaxWidth = (int)EditorGUILayout.Slider(new GUIContent() { text = "Max width of the room", tooltip = "" }, mainScript.MaxWidth, 15, 125);
                                if (mainScript.MaxWidth <= mainScript.MinWidth)
                                {
                                    mainScript.MaxWidth = mainScript.MinWidth + 1;
                                }

                                mainScript.MinWidth = (int)EditorGUILayout.Slider(new GUIContent() { text = "Min width of the room", tooltip = "" }, mainScript.MinWidth, 10, 75);

                                mainScript.NumOfRoom = (int)EditorGUILayout.Slider(new GUIContent() { text = "Maximum number of rooms to spawn", tooltip = "" }, mainScript.NumOfRoom, 3, 30);

                                if (GUILayout.Button("create attached rooms"))// gen something
                                {
                                    mainScript.roomList.Clear();
                                    mainScript.rooms.Clear();

                                    DFGeneralUtil.RestartGrid(mainScript.PcgManager.gridArr);

                                    var currentHeadRoomGenerator = new Vector2Int((int)mainScript.PcgManager.gridArr.GetLength(0) / 2, (int)mainScript.PcgManager.gridArr.GetLength(1) / 2);

                                    mainScript.roomList.Add(new RoomsClass() { minY = currentHeadRoomGenerator.y, maxX = currentHeadRoomGenerator.x + Random.Range(mainScript.MinWidth, mainScript.MaxWidth), maxY = currentHeadRoomGenerator.y + Random.Range(mainScript.MinHeight, mainScript.MaxHeight), minX = currentHeadRoomGenerator.x });
                                    mainScript.WorkoutRegion(mainScript.roomList[0]);


                                    for (int i = 0; i < mainScript.NumOfRoom * 2; i++)
                                    {
                                        if (mainScript.roomList.Count == mainScript.NumOfRoom) { break; }

                                        int actualWidth = Random.Range(mainScript.MinWidth, mainScript.MaxWidth);
                                        int actualHeight = Random.Range(mainScript.MinHeight, mainScript.MaxHeight);

                                        var decidedRoom = mainScript.roomList[mainScript.roomList.Count == 0 ? 0 : Random.Range(0, mainScript.roomList.Count)];

                                        float ranVal = Random.value;

                                        var newRoom = new RoomsClass();

                                        if (ranVal < 0.25f)
                                        {
                                            newRoom.minX = decidedRoom.maxX + 3;
                                            newRoom.maxX = newRoom.minX + actualWidth;

                                            newRoom.maxY = decidedRoom.maxY;
                                            newRoom.minY = decidedRoom.minY;
                                        }

                                        else if (ranVal < 0.5f)
                                        {
                                            newRoom.maxX = decidedRoom.minX - 3;
                                            newRoom.minX = newRoom.maxX - actualWidth;

                                            newRoom.maxY = decidedRoom.maxY;
                                            newRoom.minY = decidedRoom.minY;
                                        }

                                        else if (ranVal < 0.75f)
                                        {
                                            newRoom.maxY = decidedRoom.minY - 3;
                                            newRoom.minY = newRoom.maxY - actualHeight;

                                            newRoom.maxX = decidedRoom.maxX;
                                            newRoom.minX = decidedRoom.minX;
                                        }

                                        else
                                        {
                                            newRoom.minY = decidedRoom.maxY + 3;
                                            newRoom.maxY = newRoom.minY + actualHeight;

                                            newRoom.maxX = decidedRoom.maxX;
                                            newRoom.minX = decidedRoom.minX;
                                        }

                                        bool occupied = false;

                                        if (newRoom.minX < 0 || newRoom.minX > mainScript.PcgManager.gridArr.GetLength(0)
                                            || newRoom.minY < 0 || newRoom.minY > mainScript.PcgManager.gridArr.GetLength(0)
                                            || newRoom.maxY < 0 || newRoom.maxY > mainScript.PcgManager.gridArr.GetLength(0)
                                            || newRoom.maxX < 0 || newRoom.maxX > mainScript.PcgManager.gridArr.GetLength(0)) { occupied = true; }

                                        if (!occupied)
                                        {
                                            foreach (var otherRoom in mainScript.roomList)
                                            {
                                                if (mainScript.AABBCol(newRoom, otherRoom))
                                                {
                                                    occupied = true;
                                                    break;
                                                }
                                            }
                                        }

                                        if (!occupied)
                                        {
                                            mainScript.WorkoutRegion(newRoom);
                                            decidedRoom.childRooms.Add(newRoom);
                                            mainScript.roomList.Add(newRoom);
                                        }

                                    }

                                    mainScript.SetUpWeights(mainScript.PcgManager.gridArr);

                                    foreach (var room in mainScript.roomList)
                                    {
                                        var mainRoomCent = Vector2.Lerp(new Vector2Int(room.minX, room.minY), new Vector2Int(room.maxX, room.maxY), 0.5f);

                                        foreach (var childRoom in room.childRooms)
                                        {
                                            var childRoomCent = Vector2.Lerp(new Vector2Int(childRoom.minX, childRoom.minY), new Vector2Int(childRoom.maxX, childRoom.maxY), 0.5f);

                                            var path = DFAlgoBank.DijstraPathfinding(mainScript.PcgManager.gridArr, new Vector2Int((int)childRoomCent.x, (int)childRoomCent.y), new Vector2Int((int)mainRoomCent.x, (int)mainRoomCent.y));

                                            foreach (var tile in path)
                                            {
                                                if (tile.tileType != DFTile.TileType.FLOORROOM)
                                                    tile.tileType = DFTile.TileType.FLOORCORRIDOR;

                                                tile.tileWeight = 0.75f;
                                            }
                                        }
                                    }

                                    DFAlgoBank.SetUpTileCorridorTypesUI(mainScript.PcgManager.gridArr, 2);

                                    mainScript.PcgManager.Plane.GetComponent<Renderer>().sharedMaterial.mainTexture = DFGeneralUtil.SetUpTextBiColShade(mainScript.PcgManager.gridArr, 0, 1, true);

                                    mainScript.allowedForward = true;
                                }

                                break;

                            default:
                                break;
                        }

                        #endregion
                    }

                    break;
                case RanRoomGenMA.UI_STATE.CA:
                    {
                        if (selDungGenType == 2)
                        {
                            mainScript.PcgManager.ClearUndos();
                            mainScript.allowedForward = false;
                            mainScript.currStateIndex++;
                            mainScript.state = (RanRoomGenMA.UI_STATE)mainScript.currStateIndex;
                        }
                        else
                        {
                            mainScript.allowedBack = true;
                            mainScript.allowedForward = true;
                            DFGeneralUtil.CellularAutomataEditorSection(mainScript.PcgManager, neighboursNeeded, out neighboursNeeded);
                        }

                    }


                    break;
                case RanRoomGenMA.UI_STATE.ROOM_GEN:
                    {

                        if (selDungGenType == 2)
                        {
                            mainScript.PcgManager.ClearUndos();
                            mainScript.allowedForward = false;
                            mainScript.currStateIndex++;
                            mainScript.state = (RanRoomGenMA.UI_STATE)mainScript.currStateIndex;
                        }
                        else
                        {
                            mainScript.allowedBack = true;
                            List<List<DFTile>> rooms;
                            if (DFGeneralUtil.CalculateRoomsEditorSection(mainScript.PcgManager, minSize, out rooms, out minSize))
                            {
                                mainScript.allowedForward = true;
                            }

                            if (rooms != null)
                            {
                                mainScript.rooms = rooms;
                            }
                        }
                    }
                    break;
                case RanRoomGenMA.UI_STATE.EXTRA_ROOM_GEN:
                    {
                        mainScript.allowedBack = false;
                        mainScript.allowedForward = true;

                        if (selDungGenType == 2)
                        {
                            mainScript.PcgManager.ClearUndos();
                            mainScript.allowedForward = false;
                            mainScript.currStateIndex++;
                            mainScript.state = (RanRoomGenMA.UI_STATE)mainScript.currStateIndex;
                        }
                        else
                        {
                            radius = (int)EditorGUILayout.Slider(new GUIContent() { text = "Radius of the arena", tooltip = "Creates a circular room in a random position on the canvas. The code will try to fit it, if nothing spawns try again or lower the size" }, radius, 10, 40);

                            if (GUILayout.Button(new GUIContent() { text = "Spawn one Arena" }))
                            {
                                bool success = false;

                                for (int i = 0; i < 5; i++)
                                {
                                    var randomPoint = new Vector2Int(Random.Range(0 + radius + 3, mainScript.PcgManager.gridArr.GetLength(0) - radius - 3), Random.Range(0 + radius + 3, mainScript.PcgManager.gridArr.GetLength(1) - radius - 3));

                                    var room = DFAlgoBank.CreateCircleRoom(mainScript.PcgManager.gridArr, randomPoint, radius + 2);

                                    if (room != null)
                                    {
                                        mainScript.PcgManager.CreateBackUpGrid();
                                        room = DFAlgoBank.CreateCircleRoom(mainScript.PcgManager.gridArr, randomPoint, radius, actuallyDraw: true);

                                        mainScript.PcgManager.Plane.GetComponent<Renderer>().sharedMaterial.mainTexture = DFGeneralUtil.SetUpTextBiColShade(mainScript.PcgManager.gridArr, 0, 1, true);

                                        mainScript.rooms.Add(room);

                                        success = true;

                                        break;
                                    }
                                }

                                if (!success)
                                    Debug.Log($"<color=red>I tried to spawn the Room as requested 5 times but couldnt find any free space either try again or lower the size</color>");

                            }


                            DFGeneralUtil.SpacesUILayout(2);

                            height = (int)EditorGUILayout.Slider(new GUIContent() { text = "Height", tooltip = "" }, height, 10, 40);
                            width = (int)EditorGUILayout.Slider(new GUIContent() { text = "Widht", tooltip = "" }, width, 10, 40);

                            if (GUILayout.Button(new GUIContent() { text = "gen Room" }))
                            {

                                bool success = false;
                                for (int i = 0; i < 5; i++)
                                {
                                    var randomPoint = new Vector2Int(Random.Range(0 + radius + 3, mainScript.PcgManager.gridArr.GetLength(0) - radius - 3), Random.Range(0 + radius + 3, mainScript.PcgManager.gridArr.GetLength(1) - radius - 3));

                                    var squareRoom = DFAlgoBank.CreateSquareRoom(width, height, randomPoint, mainScript.PcgManager.gridArr, true);

                                    if (squareRoom != null)
                                    {
                                        mainScript.PcgManager.CreateBackUpGrid();
                                        squareRoom = DFAlgoBank.CreateSquareRoom(width, height, randomPoint, mainScript.PcgManager.gridArr);

                                        mainScript.PcgManager.Plane.GetComponent<Renderer>().sharedMaterial.mainTexture = DFGeneralUtil.SetUpTextBiColShade(mainScript.PcgManager.gridArr, 0, 1, true);

                                        mainScript.rooms.Add(squareRoom);

                                        success = true;
                                        break;
                                    }
                                }

                                if (!success)
                                    Debug.Log($"<color=red>I tried to spawn the Room as requested 5 times but couldnt find any free space either try again or lower the size</color>");

                            }

                        }




                    }
                    break;
                case RanRoomGenMA.UI_STATE.CORRIDOR_MAKING:
                    {
                        if (selDungGenType == 2)
                        {
                            mainScript.PcgManager.ClearUndos();
                            mainScript.allowedForward = false;
                            mainScript.currStateIndex++;
                            mainScript.state = (RanRoomGenMA.UI_STATE)mainScript.currStateIndex;
                        }
                        else
                        {
                            #region corridor making region


                            mainScript.allowedBack = true;

                            if (mainScript.rooms.Count == 1)
                            {
                                mainScript.allowedForward = true;
                                GUILayout.Label("Only one room detected, Corridor making is not possible");
                            }
                            else if (mainScript.rooms.Count == 2)
                            {
                                GUILayout.Label("Only two rooms detected, triangulation not possible");

                                GUILayout.Label("Choose the algorithm to create the corridor");

                                DFGeneralUtil.SpacesUILayout(2);

                                GUILayout.BeginVertical("Box");
                                selGridPathGenType = GUILayout.SelectionGrid(selGridPathGenType, DFGeneralUtil.selStringPathGenType, 1);
                                GUILayout.EndVertical();

                                DFGeneralUtil.SpacesUILayout(2);

                                switch (selGridPathGenType)
                                {
                                    case 0:   // A* pathfindind
                                        mainScript.PathType = EditorGUILayout.Toggle(new GUIContent() { text = "Use Straight corridors", tooltip = "PathFinding will prioritize the creation of straight corridors" }, mainScript.PathType);
                                        useWeights = EditorGUILayout.Toggle(new GUIContent() { text = "Use weights", tooltip = "" }, useWeights);
                                        break;

                                    case 1:   // djistra 
                                        DjAvoidWalls = EditorGUILayout.Toggle(new GUIContent() { text = "Avoid Walls", tooltip = "" }, DjAvoidWalls);
                                        break;
                                    case 2:   // beizier 

                                        bezierOndulation = (int)EditorGUILayout.Slider(new GUIContent() { text = "Curve Multiplier", tooltip = "beizeir curve thing to change" }, bezierOndulation, 10, 40);

                                        DFGeneralUtil.SpacesUILayout(1);

                                        mainScript.PathType = EditorGUILayout.Toggle(new GUIContent() { text = "Use Straight corridors", tooltip = "PathFinding will prioritize the creation of straight corridors" }, mainScript.PathType);

                                        break;

                                    default:
                                        break;
                                }

                                if (GUILayout.Button("Connect all the rooms"))// dfor the corridor making
                                {
                                    mainScript.PcgManager.CreateBackUpGrid();

                                    Vector2Int tileA = mainScript.rooms[0][Random.Range(0, mainScript.rooms[0].Count - 1)].position;
                                    Vector2Int tileB = mainScript.rooms[1][Random.Range(0, mainScript.rooms[1].Count - 1)].position;


                                    mainScript.allowedForward = true;

                                    switch (selGridPathGenType)
                                    {
                                        case 0:   //A* pathfingin

                                            var path = DFAlgoBank.A_StarPathfinding2D(mainScript.PcgManager.gridArr, new Vector2Int(tileA.x, tileA.y), new Vector2Int(tileB.x, tileB.y), !mainScript.PathType, useWeights: useWeights, arrWeights: mainScript.PcgManager.tileCosts);
                                            DFAlgoBank.SetUpCorridorWithPath(path.Item1);

                                            break;
                                        case 1:  //dijistra

                                            var pathD = DFAlgoBank.DijstraPathfinding(mainScript.PcgManager.gridArr, new Vector2Int(tileA.x, tileA.y), new Vector2Int(tileB.x, tileB.y), DjAvoidWalls);
                                            DFAlgoBank.SetUpCorridorWithPath(pathD);

                                            break;
                                        case 2://  beizier curve

                                            DFAlgoBank.BezierCurvePathing(new Vector2Int(tileA.x, tileA.y), new Vector2Int(tileB.x, tileB.y), bezierOndulation, mainScript.PcgManager.gridArr, !mainScript.PathType);

                                            break;

                                        default:
                                            break;
                                    }

                                    DFAlgoBank.SetUpTileCorridorTypesUI(mainScript.PcgManager.gridArr, corridorThickness);

                                    mainScript.PcgManager.Plane.GetComponent<Renderer>().sharedMaterial.mainTexture = DFGeneralUtil.SetUpTextBiColShade(mainScript.PcgManager.gridArr, 0, 1, true);
                                }

                            }
                            else if (mainScript.rooms.Count > 2)
                            {

                                GUILayout.Label("Choose how to order the connection of the rooms");

                                DFGeneralUtil.SpacesUILayout(2);

                                GUILayout.BeginVertical("Box");
                                selGridConnectionType = GUILayout.SelectionGrid(selGridConnectionType, DFGeneralUtil.selStringsConnectionType, 1);
                                GUILayout.EndVertical();

                                DFGeneralUtil.SpacesUILayout(2);

                                GUILayout.Label("Choose the Thickness of the corridor");

                                corridorThickness = (int)EditorGUILayout.Slider(new GUIContent() { text = "Thickness of the corridor", tooltip = "How wide should the corridor be" }, corridorThickness, 2, 5);

                                DFGeneralUtil.SpacesUILayout(3);


                                GUILayout.Label("Choose the algorithm to that creates the corridor");


                                DFGeneralUtil.SpacesUILayout(2);

                                GUILayout.BeginVertical("Box");
                                selGridPathGenType = GUILayout.SelectionGrid(selGridPathGenType, DFGeneralUtil.selStringPathGenType, 1);
                                GUILayout.EndVertical();

                                DFGeneralUtil.SpacesUILayout(2);


                                switch (selGridPathGenType)
                                {
                                    case 0:   // A* pathfindind
                                        mainScript.PathType = EditorGUILayout.Toggle(new GUIContent() { text = "Use Straight corridors", tooltip = "PathFinding will prioritize the creation of straight corridors" }, mainScript.PathType);
                                        useWeights = EditorGUILayout.Toggle(new GUIContent() { text = "Use weights", tooltip = "" }, useWeights);
                                        break;

                                    case 1:   // djistra 
                                        DjAvoidWalls = EditorGUILayout.Toggle(new GUIContent() { text = "Avoid Walls", tooltip = "" }, DjAvoidWalls);
                                        break;
                                    case 2:   // beizier 

                                        bezierOndulation = (int)EditorGUILayout.Slider(new GUIContent() { text = "Curve Multiplier", tooltip = "A higher multiplier is going to equal to a a more extreme curver" }, bezierOndulation, 10, 40);

                                        DFGeneralUtil.SpacesUILayout(1);
                                        mainScript.PathType = EditorGUILayout.Toggle(new GUIContent() { text = "Use Straight corridors", tooltip = "Pathfinding will prioritize the creation of straight corridors" }, mainScript.PathType);

                                        break;

                                    default:
                                        break;
                                }


                                EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

                                DFGeneralUtil.SpacesUILayout(3);

                                switch (selGridConnectionType)
                                {
                                    case 0:   // prims ran

                                        if (mainScript.rooms.Count >= 4)
                                        {
                                            randomAddCorr = (int)EditorGUILayout.Slider(new GUIContent() { text = "Additional random connections", tooltip = "Add another random connection. This number dictates how many times the script is going to TRY to add a new corridor" }, randomAddCorr, 0, mainScript.rooms.Count / 2);
                                            DFGeneralUtil.SpacesUILayout(2);
                                        }
                                        break;

                                    case 2:

                                        if (mainScript.rooms.Count >= 4)
                                        {
                                            randomAddCorr = (int)EditorGUILayout.Slider(new GUIContent() { text = "Additional random connections", tooltip = "Add another random connection. This number dictates how many times the script is going to TRY to add a new corridor" }, randomAddCorr, 0, mainScript.rooms.Count / 2);
                                            DFGeneralUtil.SpacesUILayout(2);
                                        }
                                        break;

                                    default:
                                        break;
                                }


                                DFGeneralUtil.SpacesUILayout(1);
                                EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
                                DFGeneralUtil.SpacesUILayout(1);


                                deadEndAmount = (int)EditorGUILayout.Slider(new GUIContent() { text = "Amount of dead end corridors", tooltip = "Dead end corridors start from somewhere in the dungeon and lead to nowhere" }, deadEndAmount, 0, 5);

                                deadEndCorridorThickness = (int)EditorGUILayout.Slider(new GUIContent() { text = "Thickness of the dead end corridor", tooltip = "How wide should the corridor be" }, deadEndCorridorThickness, 3, 6);

                                deadEndOndulation = (int)EditorGUILayout.Slider(new GUIContent() { text = "Curve Multiplier for dead end", tooltip = "A higher multiplier is going to equal to a a more extreme curver" }, deadEndOndulation, 10, 40);

                                DFGeneralUtil.SpacesUILayout(2);
                                if (GUILayout.Button("Connect all the rooms"))// dfor the corridor making
                                {
                                    mainScript.allowedForward = true;

                                    mainScript.PcgManager.CreateBackUpGrid();

                                    mainScript.rooms = DFAlgoBank.GetAllRooms(mainScript.PcgManager.gridArr);
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
                                            {
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
                                            }
                                            break;
                                    }

                                    switch (selGridPathGenType)
                                    {
                                        case 0:   //A* pathfingin

                                            foreach (var edge in mainScript.edges)
                                            {
                                                //use where so we get soemthing its not the wall but not necessary
                                                var tileA = roomDict[edge.edge[0]][Random.Range(0, roomDict[edge.edge[0]].Count)].position;
                                                var tileB = roomDict[edge.edge[1]][Random.Range(0, roomDict[edge.edge[1]].Count)].position;

                                                var path = DFAlgoBank.A_StarPathfinding2D(mainScript.PcgManager.gridArr, new Vector2Int(tileA.x, tileA.y), new Vector2Int(tileB.x, tileB.y), !mainScript.PathType, useWeights: useWeights, arrWeights: mainScript.PcgManager.tileCosts);

                                                DFAlgoBank.SetUpCorridorWithPath(path.Item1);
                                            }

                                            break;
                                        case 1:  //dijistra
                                            foreach (var edge in mainScript.edges)
                                            {
                                                var tileA = roomDict[edge.edge[0]][Random.Range(0, roomDict[edge.edge[0]].Count)].position;
                                                var tileB = roomDict[edge.edge[1]][Random.Range(0, roomDict[edge.edge[1]].Count)].position;

                                                var path = DFAlgoBank.DijstraPathfinding(mainScript.PcgManager.gridArr, new Vector2Int(tileA.x, tileA.y), new Vector2Int(tileB.x, tileB.y), DjAvoidWalls);

                                                DFAlgoBank.SetUpCorridorWithPath(path);
                                            }

                                            break;
                                        case 2://  beizier curve
                                            foreach (var edge in mainScript.edges)
                                            {
                                                var tileA = roomDict[edge.edge[0]][Random.Range(0, roomDict[edge.edge[0]].Count)].position;
                                                var tileB = roomDict[edge.edge[1]][Random.Range(0, roomDict[edge.edge[1]].Count)].position;

                                                DFAlgoBank.BezierCurvePathing(new Vector2Int(tileA.x, tileA.y), new Vector2Int(tileB.x, tileB.y), bezierOndulation, mainScript.PcgManager.gridArr, !mainScript.PathType);
                                            }
                                            break;

                                        default:
                                            break;
                                    }

                                    for (int i = 0; i < deadEndAmount; i++)
                                    {
                                        var room = mainScript.rooms[DFGeneralUtil.ReturnRandomFromList(mainScript.rooms)];

                                        var randomTileInRoom = room[DFGeneralUtil.ReturnRandomFromList(room)];

                                        DFTile randomTileOutsideOfRoom;

                                        while (true)
                                        {
                                            var tile = mainScript.PcgManager.gridArr[Random.Range(0, mainScript.PcgManager.gridArr.GetLength(0)), Random.Range(0, mainScript.PcgManager.gridArr.GetLength(1))];

                                            if (tile.tileWeight == 0)
                                            {

                                                randomTileOutsideOfRoom = tile;

                                                var tileA = randomTileOutsideOfRoom.position;
                                                var tileB = randomTileInRoom.position;

                                                DFAlgoBank.BezierCurvePathing(new Vector2Int(tileA.x, tileA.y), new Vector2Int(tileB.x, tileB.y), bezierOndulation, mainScript.PcgManager.gridArr);

                                                break;
                                            }
                                        }
                                    }

                                    DFAlgoBank.SetUpTileCorridorTypesUI(mainScript.PcgManager.gridArr, corridorThickness);

                                    mainScript.PcgManager.Plane.GetComponent<Renderer>().sharedMaterial.mainTexture = DFGeneralUtil.SetUpTextBiColShade(mainScript.PcgManager.gridArr, 0, 1, true);
                                }
                            }
                            else
                            {
                                GUILayout.Label("To access the corridor making function you need to\nGenerate the rooms first");
                            }


                            #endregion
                        }
                    }
                    break;
                case RanRoomGenMA.UI_STATE.GENERATE:
                    {

                        DFGeneralUtil.SaveGridDataToGenerate(mainScript.PcgManager, saveMapFileName, out saveMapFileName);
                    }
                    break;
                default:
                    break;
            }


            if (mainScript.state != RanRoomGenMA.UI_STATE.GENERATE || selGridGenType == 2)
            {
                DFGeneralUtil.SpacesUILayout(4);

                EditorGUI.BeginDisabledGroup(mainScript.allowedBack == false);

                if (GUILayout.Button(new GUIContent() { text = "Go Back", tooltip = mainScript.allowedForward == true ? "Press this to go back one step" : "You cant go back" }))// gen something
                {
                    mainScript.PcgManager.ClearUndos();
                    mainScript.allowedBack = false;
                    mainScript.currStateIndex--;
                    mainScript.state = (RanRoomGenMA.UI_STATE)mainScript.currStateIndex;
                }

                EditorGUI.EndDisabledGroup();

                EditorGUI.BeginDisabledGroup(mainScript.allowedForward == false);

                if (GUILayout.Button(new GUIContent() { text = "Continue", tooltip = mainScript.allowedForward == true ? "Press this to continue to the next step" : "You need to finish this step to continue" }))// gen something
                {
                    mainScript.PcgManager.ClearUndos();
                    mainScript.allowedForward = false;
                    mainScript.currStateIndex++;
                    mainScript.state = (RanRoomGenMA.UI_STATE)mainScript.currStateIndex;
                }

                EditorGUI.EndDisabledGroup();
            }
        }
    }
}
