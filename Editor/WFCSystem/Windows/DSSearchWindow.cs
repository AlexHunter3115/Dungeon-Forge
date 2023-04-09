using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace DS.Windows
{
    using DS.Elements;
    using Enumerations;

    public class DSSearchWindow : ScriptableObject, ISearchWindowProvider
    {
        private DSGraphView _graphView;

        private Texture2D indentationText;
        public void Initilize(DSGraphView dsGraphView) 
        {
            _graphView = dsGraphView;

            indentationText = new Texture2D(1, 1);
            indentationText.SetPixel(0, 0, Color.clear);
            indentationText.Apply();
        }


        public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context)
        {
            List<SearchTreeEntry> searchTreeEntries = new List<SearchTreeEntry>()
           {
               new SearchTreeGroupEntry(new GUIContent("Create element")),
               new SearchTreeGroupEntry(new GUIContent("Rule Node Types"),1),
               new SearchTreeEntry(new GUIContent("Sub Tile Rule Node",indentationText))
               {
                level = 2,
                userData = DSDialogueType.SingleChoice
               },

               new SearchTreeEntry(new GUIContent("Main Tile Rule Node",indentationText))
               {
                level = 2,
                userData = DSDialogueType.MultiChoice
               },
                new SearchTreeEntry(new GUIContent("Quick Tile Rule Node",indentationText))
               {
                level = 2,
                userData = DSDialogueType.QuickRule
               },

           };

            return searchTreeEntries;
        }

        public bool OnSelectEntry(SearchTreeEntry SearchTreeEntry, SearchWindowContext context)
        {

            Vector2 localMousePos = _graphView.GetLocalMousePos(context.screenMousePosition,true);


            switch (SearchTreeEntry.userData)
            {

                case DSDialogueType.SingleChoice:
                    DSSingleChoiceNode createdNodeS = (DSSingleChoiceNode) _graphView.CreateNode(DSDialogueType.SingleChoice, localMousePos);
                    _graphView.AddElement(createdNodeS);
                    return true;
                

                case DSDialogueType.MultiChoice:
                    DSMultiChoiceNode createdNodeM = (DSMultiChoiceNode)_graphView.CreateNode(DSDialogueType.MultiChoice, localMousePos);
                    _graphView.AddElement(createdNodeM);
                    return true;

                case DSDialogueType.QuickRule:
                    DSQuickRuleNode createdNodeQ = (DSQuickRuleNode)_graphView.CreateNode(DSDialogueType.QuickRule, localMousePos);
                    _graphView.AddElement(createdNodeQ);
                    return true;



                case Group _:
                    Group group = (Group)_graphView.CreateGroup("dialogue groupd", context.screenMousePosition);
                    _graphView.AddElement(group);

                    return true;

            }


            return false;
        }
    }



}

