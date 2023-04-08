using System.Collections;
using System.Collections.Generic;
using UnityEngine;




namespace DS.Elements 
{
    using DS.Utilities;
    using DS.Windows;
    using Enumerations;
    using System;
    using UnityEditor;
    using UnityEditor.Experimental.GraphView;
    using UnityEngine.UIElements;

    public class DSSingleChoiceNode : DSNode
    {

        public override void Initialize(Vector2 pos, DSGraphView graphView)
        {

            base.Initialize(pos, graphView);

            titleString = "\n Sub Rule Node";

            dialogueType = DSDialogueType.SingleChoice;

            //Choices.Add("Next Dialogue");
        }


        public override void Draw()
        {
            base.Draw();

            Label dialogueTextField;

                 dialogueTextField = new Label(titleString);

            titleContainer.Insert(0, dialogueTextField);


            Port DownPort = this.CreatePort("Output Port", Orientation.Horizontal, Direction.Output, Port.Capacity.Multi);
            outputContainer.Add(DownPort);


            var textFieldIndexRule = DSElementUtility.CreateTextField(indexVal);
            textFieldIndexRule.MarkDirtyRepaint();
            textFieldIndexRule.RegisterValueChangedCallback(evt => indexVal = CheckExists(evt.newValue));

            mainContainer.Insert(1, textFieldIndexRule);
           
            RefreshPorts();
            RefreshExpandedState();
        }


    }





}

