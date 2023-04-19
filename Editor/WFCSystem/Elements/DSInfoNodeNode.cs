using DS.Enumerations;
using DS.Windows;
using UnityEngine;
using UnityEngine.UIElements;

namespace DS.Elements 
{
    public class DSInfoNodeNode : DSNode
    {
        public override void Initialize(Vector2 pos, DSGraphView graphView)
        {
            base.Initialize(pos, graphView);

            dialogueType = DSDialogueType.InfoNode;
        }

        public override void Draw()
        {
            base.Draw();

            Label dialogueTextArea = new Label("\n Rule Legend");

            titleContainer.Insert(0, dialogueTextArea);

            Label ExplanationTextArea = new Label("\n Welcome to the Wave Function Collapse (WFC) ruleBuilder" +
                "\n\n Enter the folder name (in repsect to the Resources folder) where all the models that will be used for the generation are" +
                "\n Use the index of the tile to reference the needed tile for each node" +
                "\n\n You can add new nodes using the 'right-mouse button' | 'spacebar' | 'A  /  S  /  D'" +
                "\n\n There are 3 types of nodes" +
                "\n Quick Node: allows you to give it the index of a tile and just say which parts can be connected to and what cant" +
                "\n Main Rule Node: in case you want to be more specific on how the tiles interact you can make the rule yourself" +
                "\n and use the Sub rule node to add it to the preferred location");

            mainContainer.Add(ExplanationTextArea);

            RefreshExpandedState();
        }
    }
}

