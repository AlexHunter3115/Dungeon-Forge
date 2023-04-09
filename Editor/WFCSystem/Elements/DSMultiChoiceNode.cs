using DS.Enumerations;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace DS.Elements 
{
    using DS.Utilities;
    using DS.Windows;

    public class DSMultiChoiceNode : DSNode
    {
        public override void Initialize(Vector2 pos, DSGraphView graphView)
        {
            base.Initialize(pos, graphView);

            titleString = "\n Main Rule";
            dialogueType = DSDialogueType.MultiChoice;
        }

        public override void Draw()
        {
            base.Draw();

            Label dialogueText = new Label(titleString);

            titleContainer.Insert(0, dialogueText);


            var textFieldIndexRule = DSElementUtility.CreateTextField(indexVal);
            textFieldIndexRule.MarkDirtyRepaint();
            textFieldIndexRule.RegisterValueChangedCallback(
            evt => {
                indexVal = CheckExists(evt.newValue);
                titleString = allowed == true ? $"{titleString}" : $"<color=red>{titleString}</color>";
            }) ;   //indexVal = evt.newValue

            mainContainer.Insert(1,textFieldIndexRule);

            Port LeftPort = this.CreatePort("Left Side", Orientation.Horizontal, Direction.Input,Port.Capacity.Multi);
            inputContainer.Add(LeftPort);
            
            Port UpPort = this.CreatePort("Up Side", Orientation.Horizontal, Direction.Input, Port.Capacity.Multi);
            inputContainer.Add(UpPort);

            Port RightPort = this.CreatePort("Right Side", Orientation.Horizontal, Direction.Input, Port.Capacity.Multi);
            inputContainer.Add(RightPort);

            Port DownPort = this.CreatePort("Down Side", Orientation.Horizontal, Direction.Input, Port.Capacity.Multi);
            inputContainer.Add(DownPort);

            RefreshExpandedState();
        }
    }
}
