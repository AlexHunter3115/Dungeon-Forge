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

            showRules = EditorGUILayout.BeginFoldoutHeaderGroup(showRules, "Instructions");

            if (showRules)
            {
                GUILayout.TextArea("You have choosen wfc");
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