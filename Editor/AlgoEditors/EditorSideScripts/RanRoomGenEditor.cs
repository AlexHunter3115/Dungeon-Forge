
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
        GUIContent[] selStringsDungGenType = { new GUIContent() { text = "BPS", tooltip = "Binary Partition System" }, new GUIContent() { text = "Random", tooltip = "Randomly places the rooms on the canvas, some can get connectd" }, new GUIContent() { text = "Room to Room", tooltip = "Room attached to room very small corridos" } };

        int selGridGenType = 0;

        bool useWeights = false;

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

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            RanRoomGenMA mainScript = (RanRoomGenMA)target;

            #region explanation

            showRules = EditorGUILayout.BeginFoldoutHeaderGroup(showRules, "Introduction");

            if (showRules)
            {
                GUILayout.TextArea("Generate a dungeon where rooms of a choosen size are the primary object to build upon.\n\nVisit the wiki for more informations: https://github.com/AlessandroBufalino3115/Dungeon-Forge/wiki/Using-the-Pack#8-random-room-generation");
            }

            if (!Selection.activeTransform)
            {
                showRules = false;
            }

            EditorGUILayout.EndFoldoutHeaderGroup();


            DFEditorUtil.SpacesUILayout(4);

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

                        DFEditorUtil.SpacesUILayout(3);


                        switch (selDungGenType)
                        {
                            case 0:   //bps

                                mainScript.MinHeight = (int)EditorGUILayout.Slider(new GUIContent() { text = "Minimum Height of the room", tooltip = "" }, mainScript.MinHeight, 10, 75);

                                mainScript.MinWidth = (int)EditorGUILayout.Slider(new GUIContent() { text = "Minimum Width of the room", tooltip = "" }, mainScript.MinWidth, 10, 75);

                                mainScript.NumOfRoom = (int)EditorGUILayout.Slider(new GUIContent() { text = "Maximum number of rooms to spawn", tooltip = "" }, mainScript.NumOfRoom, 3, 30);

                                specialAlgo = EditorGUILayout.Toggle(new GUIContent() { text = "Room layout randomiser", tooltip = "Make the rooms not square but instead use of the algorithms avaialble to randomise the layout" }, specialAlgo);
                                if (specialAlgo)
                                {
                                    CAselected = EditorGUILayout.Toggle(CAselected == true ? "Cellular automata selected for gen" : "Random walk selected", CAselected);
                                    if (CAselected)
                                    {
                                        neighboursNeeded = (int)EditorGUILayout.Slider(neighboursNeeded, 3, 5);
                                    }
                                }

                                if (GUILayout.Button("Use Binary Space Partitioning algorithm"))// gen something
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

                                mainScript.MaxHeight = (int)EditorGUILayout.Slider(new GUIContent() { text = "Maximum height of the room", tooltip = "" }, mainScript.MaxHeight, 15, 125);

                                if (mainScript.MaxHeight <= mainScript.MinHeight)
                                {
                                    mainScript.MaxHeight = mainScript.MinHeight + 1;
                                }
                                mainScript.MinHeight = (int)EditorGUILayout.Slider(new GUIContent() { text = "Minimum height of the room", tooltip = "" }, mainScript.MinHeight, 10, 75);


                                mainScript.MaxWidth = (int)EditorGUILayout.Slider(new GUIContent() { text = "Maximum width of the room", tooltip = "" }, mainScript.MaxWidth, 15, 125);
                                if (mainScript.MaxWidth <= mainScript.MinWidth)
                                {
                                    mainScript.MaxWidth = mainScript.MinWidth + 1;
                                }

                                mainScript.MinWidth = (int)EditorGUILayout.Slider(new GUIContent() { text = "Minimum width of the room", tooltip = "" }, mainScript.MinWidth, 10, 75);
                                mainScript.NumOfRoom = (int)EditorGUILayout.Slider(new GUIContent() { text = "Maximum number of rooms to spawn", tooltip = "" }, mainScript.NumOfRoom, 3, 30);

                                specialAlgo = EditorGUILayout.Toggle(new GUIContent() { text = "Room layout randomiser", tooltip = "Make the rooms not square but instead use of the algorithms avaialble to randomise the layout" }, specialAlgo);
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

                                mainScript.MaxHeight = (int)EditorGUILayout.Slider(new GUIContent() { text = "Maximum height of the room", tooltip = "" }, mainScript.MaxHeight, 15, 125);

                                if (mainScript.MaxHeight <= mainScript.MinHeight)
                                {
                                    mainScript.MaxHeight = mainScript.MinHeight + 1;
                                }
                                mainScript.MinHeight = (int)EditorGUILayout.Slider(new GUIContent() { text = "Minimum height of the room", tooltip = "" }, mainScript.MinHeight, 10, 75);


                                mainScript.MaxWidth = (int)EditorGUILayout.Slider(new GUIContent() { text = "Maximum width of the room", tooltip = "" }, mainScript.MaxWidth, 15, 125);
                                if (mainScript.MaxWidth <= mainScript.MinWidth)
                                {
                                    mainScript.MaxWidth = mainScript.MinWidth + 1;
                                }

                                mainScript.MinWidth = (int)EditorGUILayout.Slider(new GUIContent() { text = "Minimum width of the room", tooltip = "" }, mainScript.MinWidth, 10, 75);

                                mainScript.NumOfRoom = (int)EditorGUILayout.Slider(new GUIContent() { text = "Maximum number of rooms to spawn", tooltip = "" }, mainScript.NumOfRoom, 3, 30);

                                if (GUILayout.Button("Create attached rooms"))// gen something
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
                            DFEditorUtil.CellularAutomataEditorSection(mainScript.PcgManager,ref neighboursNeeded);
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
                            if (DFEditorUtil.CalculateRoomsEditorSection(mainScript.PcgManager,ref minSize, out rooms))
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
                            radius = (int)EditorGUILayout.Slider(new GUIContent() { text = "Radius of the room", tooltip = "Creates a circular room in a random position on the canvas. The code will try to fit it, if nothing spawns try again or lower the size" }, radius, 10, 40);

                            if (GUILayout.Button(new GUIContent() { text = "Spawn circular room", tooltip = "Creates a circular room in a random position on the canvas. The code will try to fit it, if nothing spawns try again or lower the size" }))
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
                                    Debug.Log($"<color=red>I tried to spawn the Room as requested 5 times but couldn't find any free space either try again or lower the size</color>");

                            }


                            DFEditorUtil.SpacesUILayout(2);

                            height = (int)EditorGUILayout.Slider(new GUIContent() { text = "Height", tooltip = "" }, height, 10, 40);
                            width = (int)EditorGUILayout.Slider(new GUIContent() { text = "Widht", tooltip = "" }, width, 10, 40);

                            if (GUILayout.Button(new GUIContent() { text = "Generate random rectangular room", tooltip = "Creates a rectangular room in a random position on the canvas. The code will try to fit it, if nothing spawns try again or lower the size" }))
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

                            DFEditorUtil.GenerateCorridorsEditorSection(mainScript.PcgManager, mainScript.rooms, ref mainScript.allowedForward, ref mainScript.allowedBack, ref corridorThickness
                            , ref selGridConnectionType, ref selGridPathGenType, ref useWeights, ref bezierOndulation, ref mainScript.pathType, ref randomAddCorr, ref deadEndAmount,  ref deadEndOndulation, ref mainScript.edges);

                            #endregion
                        }
                    }
                    break;
                case RanRoomGenMA.UI_STATE.GENERATE:
                    {
                        DFEditorUtil.SaveGridDataToGenerateEditorSection(mainScript.PcgManager, saveMapFileName, out saveMapFileName);
                    }
                    break;
                default:
                    break;
            }


            if (mainScript.state != RanRoomGenMA.UI_STATE.GENERATE || selGridGenType == 2)
            {
                DFEditorUtil.SpacesUILayout(4);

                EditorGUI.BeginDisabledGroup(mainScript.allowedBack == false);

                if (GUILayout.Button(new GUIContent() { text = "Go Back", tooltip = mainScript.allowedForward == true ? "Press this to go back one step" : "You can't go back" }))// gen something
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
