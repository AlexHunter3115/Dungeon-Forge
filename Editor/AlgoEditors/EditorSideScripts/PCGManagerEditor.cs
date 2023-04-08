


namespace DungeonForge.Editor
{
    using UnityEditor;
    using UnityEngine;
    using DungeonForge.AlgoScript;
    using DungeonForge.Utils;

    [CustomEditor(typeof(PCGManager))]
    public class PCGManagerEditor : Editor
    {
        SerializedProperty tileCostSerialized;

        //SerializedProperty wallListGameObj;
        //SerializedProperty wallListOccurance;


        public void OnEnable()
        {
            tileCostSerialized = serializedObject.FindProperty("tileCosts");

            //wallListGameObj = serializedObject.FindProperty("WallsTilesGameobject");
            //wallListOccurance = serializedObject.FindProperty("WallsTilesOccurance");
        }

        public override void OnInspectorGUI()
        {
            GUILayout.TextArea("Welcome to the PCG tool, Use the sliders to set the canvas from which the dungeon will rise from\n\n" +
                "Before starting, you can load the tiles that will be used for the generation by creating a new rule and loading that rule\n\n" +
                "Then choose the starting main algorithm which will shape your dungeon");


            DFEditorUtil.SpacesUILayout(4);

            base.OnInspectorGUI();

            PCGManager mainScript = (PCGManager)target;

            DFEditorUtil.SpacesUILayout(4);

            if (GUILayout.Button(new GUIContent() { text = mainScript.Plane == null ? "Generate Plane" : "Refresh Plane", tooltip = mainScript.Plane == null ? "Generate The canvas where the PCG will be reinprinted" : "Restart the Canvas" }))
            {

                if (mainScript.mainAlgorithm == PCGManager.MainAlgo.WAVE_FUNCTION_COLLAPSE || mainScript.mainAlgorithm == PCGManager.MainAlgo.GENERATE_YOUR_DUNGEON)
                {
                }
                else
                {
                    mainScript.CreatePlane();
                    mainScript.Restart();
                }
            }

            if (mainScript.Plane != null)
            {
                if (GUILayout.Button("Delete Plane"))
                {
                    mainScript.DeletePlane();
                    mainScript.DelPrevAlgo();
                }
            }

            if (mainScript.Plane != null || mainScript.mainAlgorithm == PCGManager.MainAlgo.WAVE_FUNCTION_COLLAPSE || mainScript.mainAlgorithm == PCGManager.MainAlgo.GENERATE_YOUR_DUNGEON)
            {
                if (GUILayout.Button(new GUIContent() { text = mainScript.CurrMainAlgoIDX == (int)mainScript.mainAlgorithm ? "Refresh Main Algorithm Component" : "Load New Algorithm Component", tooltip = mainScript.CurrMainAlgoIDX == (int)mainScript.mainAlgorithm ? "Refresh the algorithm component" : "Load the choosen algorithm component to start to use it" }))
                {
                    if (mainScript.mainAlgorithm == PCGManager.MainAlgo.WAVE_FUNCTION_COLLAPSE || mainScript.mainAlgorithm == PCGManager.MainAlgo.GENERATE_YOUR_DUNGEON)
                    {
                        if (mainScript.Plane != null)
                        {
                            DestroyImmediate(mainScript.Plane);
                        }
                        mainScript.LoadMainAlgo();
                    }
                    else
                    {
                        mainScript.Restart();
                        mainScript.LoadMainAlgo();
                    }
                }
            }


            DFEditorUtil.SpacesUILayout(2);

            mainScript.loadSectionOpen = EditorGUILayout.BeginFoldoutHeaderGroup(mainScript.loadSectionOpen, "Loading Assets Section");

            if (mainScript.loadSectionOpen)
            {

                DFEditorUtil.SpacesUILayout(4);

                GUIContent tileSetRuleLabel = new GUIContent("Tile Set Scriptable File Name", "This is where the name of the scriptable object containing the tilesets objects should go with their occurance, to be used when using tile Generation");
                mainScript.TileSetRuleFileName = EditorGUILayout.TextField(tileSetRuleLabel,mainScript.TileSetRuleFileName);

                DFEditorUtil.SpacesUILayout(1);

                if (GUILayout.Button(new GUIContent() { text = "New tileSet rule", tooltip = "create a new scriptable object for the rules of the tiles that you want to use" }))
                {
                    var asset = CreateInstance<TilesRuleSet>();

                    if (!AssetDatabase.IsValidFolder("Assets/Resources"))
                    {
                        AssetDatabase.CreateFolder("Assets", "Resources");
                        AssetDatabase.Refresh();
                    }

                    if (!AssetDatabase.IsValidFolder("Assets/Resources/Resources_Algorithms"))
                    {
                        AssetDatabase.CreateFolder("Assets/Resources", "Resources_Algorithms");
                        AssetDatabase.Refresh();
                    }

                    if (!AssetDatabase.IsValidFolder("Assets/Resources/Resources_Algorithms/Tile_Sets_Ruleset"))
                    {
                        AssetDatabase.CreateFolder("Assets/Resources/Resources_Algorithms", "Tile_Sets_Ruleset");
                        AssetDatabase.Refresh();
                    }

                    AssetDatabase.CreateAsset(asset, $"Assets/Resources/Resources_Algorithms/Tile_Sets_Ruleset/NewTileSetRuleSet.asset");
                    AssetDatabase.SaveAssets();

                }

                if (GUILayout.Button(new GUIContent() { text = "Load tileSet rule", tooltip = "Remember to give the filename" }))
                {

                    if (string.IsNullOrEmpty(mainScript.TileSetRuleFileName))
                    {
                        EditorUtility.DisplayDialog("Error", "The tileSet rule file name is invalid", "OK");
                        return;
                    }

                    var tileRules = Resources.Load<TilesRuleSet>("Resources_Algorithms/Tile_Sets_Ruleset/" + mainScript.TileSetRuleFileName);

                    if (tileRules == null)
                    {
                        EditorUtility.DisplayDialog("Error", "The tileSet rule file name is invalid", "OK");
                        return;
                    }
                    else
                    {
                        mainScript.WallsTiles.Clear();
                        mainScript.FloorTiles.Clear();
                        mainScript.CeilingTiles.Clear();

                        //mainScript.WallsTilesGameobject.Clear();
                        //mainScript.FloorTilesGameobject.Clear();
                        //mainScript.CeilingTilesGameObject.Clear();

                        //mainScript.WallsTilesOccurance.Clear();
                        //mainScript.FloorTilesOccurance.Clear();
                        //mainScript.CeilingTilesOccurance.Clear();

                        foreach (var item in tileRules.WallsTiles)
                        {
                            mainScript.WallsTiles.Add(new PCGManager.TileRuleSetPCG() { occurance = item.occurance, objectPrefab = item.Tile });
                            //mainScript.WallsTilesOccurance.Add(item.occurance);
                            //mainScript.WallsTilesGameobject.Add(item.Tile);
                        }

                        foreach (var item in tileRules.FloorTiles)
                        {
                            mainScript.FloorTiles.Add(new PCGManager.TileRuleSetPCG() { occurance = item.occurance, objectPrefab = item.Tile });
                            //mainScript.FloorTilesOccurance.Add(item.occurance);
                            //mainScript.FloorTilesGameobject.Add(item.Tile);
                        }

                        foreach (var item in tileRules.CeilingTiles)
                        {
                            mainScript.CeilingTiles.Add(new PCGManager.TileRuleSetPCG() { occurance = item.occurance, objectPrefab = item.Tile });
                            //    mainScript.CeilingTilesOccurance.Add(item.occurance);
                            //    mainScript.CeilingTilesGameObject.Add(item.Tile);
                        }
                    }
                }


                DFEditorUtil.SpacesUILayout(4);

                GUIContent weightRuleLabel = new GUIContent("Pathing Weight Rule Scriptable File Name", "This is where the name of the scriptable object containing the paths weight should go to be loaded and used when creating corridors");
                mainScript.WeightRuleFileName = EditorGUILayout.TextField(weightRuleLabel,mainScript.WeightRuleFileName);
                DFEditorUtil.SpacesUILayout(1);
                if (GUILayout.Button(new GUIContent() { text = "New Weight RuleSet", tooltip = "create a new weightRule Set" }))
                {

                    var asset = ScriptableObject.CreateInstance<WeightRuleSet>();

                    if (!AssetDatabase.IsValidFolder("Assets/Resources"))
                    {
                        AssetDatabase.CreateFolder("Assets", "Resources");
                        AssetDatabase.Refresh();
                    }

                    if (!AssetDatabase.IsValidFolder("Assets/Resources/Resources_Algorithms"))
                    {
                        AssetDatabase.CreateFolder("Assets/Resources", "Resources_Algorithms");
                        AssetDatabase.Refresh();
                    }

                    if (!AssetDatabase.IsValidFolder("Assets/Resources/Resources_Algorithms/Weight_Pathfinding_RuleSet"))
                    {
                        AssetDatabase.CreateFolder("Assets/Resources/Resources_Algorithms", "Weight_Pathfinding_RuleSet");
                        AssetDatabase.Refresh();
                    }

                    AssetDatabase.CreateAsset(asset, $"Assets/Resources/Resources_Algorithms/Weight_Pathfinding_RuleSet/NewWeightRuleSet.asset");
                    AssetDatabase.SaveAssets();

                }

                if (GUILayout.Button(new GUIContent() { text = "Load Weight RuleSet", tooltip = "Remember to give the filename" }))
                {
                    if (string.IsNullOrEmpty(mainScript.WeightRuleFileName))
                    {
                        EditorUtility.DisplayDialog("Error", "The weight rule file name is invalid", "OK");
                        return;
                    }
                    var tileRules = Resources.Load<WeightRuleSet>("Resources_Algorithms/Weight_Pathfinding_RuleSet/" + mainScript.WeightRuleFileName);


                    if (tileRules == null)
                    {
                        EditorUtility.DisplayDialog("Error", "The weight rule file name is invalid", "OK");
                        return;
                    }
                    else
                    {
                        mainScript.tileCosts = new float[6];

                        mainScript.tileCosts[0] = tileRules.VOID;
                        mainScript.tileCosts[1] = tileRules.FLOORROOM;
                        mainScript.tileCosts[2] = tileRules.WALL;
                        mainScript.tileCosts[3] = tileRules.ROOF;
                        mainScript.tileCosts[4] = tileRules.FLOORCORRIDOR;
                        mainScript.tileCosts[5] = tileRules.AVOID;
                    }

                }
            }

            if (!Selection.activeTransform)
            {
                mainScript.loadSectionOpen = false;
            }

            EditorGUILayout.EndFoldoutHeaderGroup();


            if (mainScript.loadSectionOpen) 
            {
                serializedObject.Update();

                

                //for (int i = 0; i < wallListGameObj.arraySize; i++)
                //{
                //    EditorGUILayout.PropertyField(wallListGameObj.GetArrayElementAtIndex(i));
                //    EditorGUILayout.PropertyField(wallListOccurance.GetArrayElementAtIndex(i));
                //}




                DFEditorUtil.SpacesUILayout(1);

                EditorGUILayout.PropertyField(tileCostSerialized, true);

                serializedObject.ApplyModifiedProperties();
            }

            DFEditorUtil.SpacesUILayout(4);

            EditorGUI.BeginDisabledGroup(mainScript.prevGridArray2D.Count == 0 ? true : false);

            GUILayout.Label(new GUIContent() { text = $"You have {mainScript.prevGridArray2D.Count} undos left", tooltip = "" });
            if (GUILayout.Button(new GUIContent() { text = "Undo Step", tooltip = "" }))
            {
                mainScript.LoadBackUpGrid();
            }

            EditorGUI.EndDisabledGroup();

        }
    }




    /*

     https://docs.unity3d.com/ScriptReference/EditorGUILayout.Space.html
    https://docs.unity3d.com/ScriptReference/EditorGUI.ProgressBar.html
    https://docs.unity3d.com/ScriptReference/TooltipAttribute.html

     https://docs.unity3d.com/ScriptReference/HeaderAttribute.html


    https://docs.unity3d.com/Manual/editor-CustomEditors.html
    https://answers.unity.com/questions/1567638/how-can-i-change-the-variables-order-in-inspector.html
     */
}