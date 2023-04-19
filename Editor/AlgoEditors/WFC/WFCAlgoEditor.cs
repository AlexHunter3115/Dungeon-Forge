namespace DungeonForge.Editor
{
    using DungeonForge.Utils;
    using UnityEditor;
    using UnityEngine;
    using DungeonForge.AlgoScript;

    [CustomEditor(typeof(NewWFCAlog))]
    public class WFCAlgoEditor : Editor
    {

        bool showRules = false;

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            NewWFCAlog mainScript = (NewWFCAlog)target;

            DFEditorUtil.SpacesUILayout(4);

            #region explanation

            showRules = EditorGUILayout.BeginFoldoutHeaderGroup(showRules, "Introduction");

            if (showRules)
            {
                GUILayout.TextArea("The Wave Function Collapse algorithm uses a set of predefined patterns to generate dungeons that obey certain constraints. By defining a set of input patterns, the algorithm can create a dungeon with a specific aesthetic and structural coherence. This algorithm is particularly useful for generating dungeons with a consistent style or theme.\n\n Visit the wiki for more informations: https://github.com/AlessandroBufalino3115/Dungeon-Forge/wiki/Using-the-Pack#wave-function-collapse-wfc");
            }

            if (!Selection.activeTransform)
            {
                showRules = false;
            }

            EditorGUILayout.EndFoldoutHeaderGroup();


            DFEditorUtil.SpacesUILayout(4);


            #endregion


            if (GUILayout.Button("Run WFC Algo"))
            {
                mainScript.RunWFCAlgo();
            }

            if (GUILayout.Button("Delete previous run"))
            {
                mainScript.DestroyKids();
            }
        }
    }
}