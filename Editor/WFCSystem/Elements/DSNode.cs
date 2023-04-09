using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace DS.Elements 
{
    using DS.Enumerations;
    using DS.Windows;
    using UnityEditor;

    public class DSNode : Node
    {

        public bool allowed = true;
        public DSDialogueType dialogueType { get; set; }
        public string indexVal = "";
        public string nodeGuid = "";
        private DSGraphView _graphView;
        public string titleString;
        public string tileNameString;

        public virtual void Initialize(Vector2 pos, DSGraphView graphView) 
        {
            this._graphView = graphView;
            nodeGuid = GUID.Generate().ToString();

            SetPosition(new Rect(pos, Vector2.zero));
            RefreshPorts();
            RefreshExpandedState();
        }

        public virtual void Draw() 
        { }

        public string CheckExists(string newVal)
        {
            bool isNumber = int.TryParse(newVal, out int idx);

            if (isNumber) // is it a string 
            {
                if (_graphView.ruleDict.ContainsKey(idx)) 
                {

                    allowed = true;
                }
                else 
                {
                    allowed = false; 
                }

            }
            else 
            {
                allowed = false;
            }


            //if (!allowed)
            //    titleString = $"<color=red>{titleString}</color>";
            //else
            //    titleString = $"{titleString}";

            return newVal;
        }

        private void UpdateMainCoint() 
        {
            
        }

    }
}

