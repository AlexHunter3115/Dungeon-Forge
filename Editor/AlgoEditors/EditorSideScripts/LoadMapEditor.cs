

namespace DungeonForge.Editor
{
    using UnityEditor;
    using UnityEngine;
    using DungeonForge.Utils;
    using DungeonForge.AlgoScript;

    [CustomEditor(typeof(LoadMapMA))]
    public class LoadMapEditor : Editor
    {

        public bool showRules = false;

        public string fileName = "";

        int selGridGenType = 0;
        GUIContent[] selStringsGenType = { new GUIContent() { text = "Vertice Generation", tooltip = "Using the algorithm marching cubes create a mesh object which can be exported to other 3D softwares" }, new GUIContent() { text = "TileSet Generation", tooltip = "Generate the Dungeon using the tileset provided" } };

        bool succesfullLoading = false;
        bool blockGeneration = false;





        // this is where i wish the more design positioning thing oriented design goes but it will be tough


        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            LoadMapMA mainScript = (LoadMapMA)target;
            #region explanation

            showRules = EditorGUILayout.BeginFoldoutHeaderGroup(showRules, "Instructions");

            if (showRules)
            {
                GUILayout.TextArea("");
            }

            if (!Selection.activeTransform)
            {
                showRules = false;
            }

            EditorGUILayout.EndFoldoutHeaderGroup();

            #endregion


            DFGeneralUtil.SpacesUILayout(4);

            fileName = EditorGUILayout.TextField("Save file name: ", fileName);

            if (GUILayout.Button(new GUIContent() { text = "load Data" }))
            {
                succesfullLoading = false;
                var map = mainScript.LoadDataCall(fileName);


                if (map == null) { }
                else
                {
                    succesfullLoading = true;
                    mainScript.PcgManager.gridArr = map;
                    mainScript.PcgManager.height = mainScript.PcgManager.gridArr.GetLength(1);
                    mainScript.PcgManager.width = mainScript.PcgManager.gridArr.GetLength(0);

                    mainScript.PcgManager.CreatePlane();

                    mainScript.PcgManager.gridArr = map;

                    mainScript.rooms.Clear();

                    mainScript.rooms = DFAlgoBank.GetAllRooms(mainScript.PcgManager.gridArr, true);
                    Debug.Log(mainScript.rooms.Count);
                    mainScript.PcgManager.Plane.GetComponent<Renderer>().sharedMaterial.mainTexture = DFGeneralUtil.SetUpTextBiColShade(mainScript.PcgManager.gridArr, 0, 1, true);
                }

            }


            if (GUILayout.Button(new GUIContent() { text = "test poissant" }))
            {
                var poissant = DFAlgoBank.GeneratePossiantPoints(mainScript.PcgManager.gridArr.GetLength(0), mainScript.PcgManager.gridArr.GetLength(1), 4);
                var acceptedPointed = DFAlgoBank.RunPoissantCheckOnCurrentTileMap(poissant, mainScript.PcgManager.gridArr, 0.2f);

                for (int i = 0; i < acceptedPointed.Count; i++)
                {
                    Instantiate(mainScript.debris, new Vector3(acceptedPointed[i].x, 0, acceptedPointed[i].y), Quaternion.identity);
                }
            }


            DFGeneralUtil.SpacesUILayout(4);


            EditorGUI.BeginDisabledGroup(succesfullLoading == false);


            GUILayout.BeginVertical("Box");
            selGridGenType = GUILayout.SelectionGrid(selGridGenType, selStringsGenType, 1);
            GUILayout.EndVertical();

            DFGeneralUtil.SpacesUILayout(2);

            if (GUILayout.Button(new GUIContent() { text = "Generate YOUR Dungeon!" }))
            {
                switch (selGridGenType)
                {
                    case 0:

                        for (int y = 0; y < mainScript.PcgManager.gridArr.GetLength(1); y++)
                        {
                            for (int x = 0; x < mainScript.PcgManager.gridArr.GetLength(0); x++)
                            {
                                if (mainScript.PcgManager.gridArr[x, y].tileType == DFTile.TileType.WALLCORRIDOR)
                                {
                                    mainScript.PcgManager.gridArr[x, y].tileType = DFTile.TileType.FLOORCORRIDOR;
                                }
                            }
                        }

                        DFAlgoBank.SetUpTileTypesCorridor(mainScript.PcgManager.gridArr);

                        mainScript.PcgManager.FormObject(DFAlgoBank.MarchingCubesAlgo(DFAlgoBank.ExtrapolateMarchingCubes(mainScript.PcgManager.gridArr, mainScript.PcgManager.RoomHeight), false));
                        break;

                    case 1:

                        if (blockGeneration)
                            mainScript.PcgManager.DrawTileMapBlockType();
                        else
                            mainScript.PcgManager.DrawTileMapDirectionalWalls();

                        break;
                }
            }

            if (selGridGenType == 1)
            {
                blockGeneration = EditorGUILayout.Toggle(new GUIContent() { text = blockGeneration == true ? "Block gen selected" : "Wall directional gen selected", tooltip = "" }, blockGeneration);
                DFGeneralUtil.SpacesUILayout(1);
                mainScript.PcgManager.ChunkHeight = (int)EditorGUILayout.Slider(new GUIContent() { text = "This is for the chunk size", tooltip = "" }, mainScript.PcgManager.ChunkHeight, 10, 30);
                mainScript.PcgManager.ChunkWidth = mainScript.PcgManager.ChunkHeight;
            }

            EditorGUI.EndDisabledGroup();
        }
    }
}