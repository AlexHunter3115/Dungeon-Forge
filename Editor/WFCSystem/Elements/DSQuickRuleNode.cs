

namespace DS.Elements
{
    using DS.Enumerations;
    using DS.Utilities;
    using DS.Windows;
    using UnityEngine;
    using UnityEngine.UIElements;

    public class DSQuickRuleNode : DSNode
    {

        public bool isOpenLeftBool;
        public bool isOpenRightBool;
        public bool isOpenAboveBool;
        public bool isOpenBelowBool;

        public override void Initialize(Vector2 pos, DSGraphView graphView)
        {
            isOpenBelowBool = false;
            isOpenAboveBool = false;
            isOpenLeftBool = false;
            isOpenRightBool = false;

            base.Initialize(pos, graphView);

            dialogueType = DSDialogueType.QuickRule;
        }

        public override void Draw()
        {
            base.Draw();

            Label dialogueTextField = new Label("\n Quick Rule Node");


            titleContainer.Insert(0, dialogueTextField);

            var textFieldIndexRule = DSElementUtility.CreateTextField(indexVal);
            textFieldIndexRule.MarkDirtyRepaint();
            textFieldIndexRule.RegisterValueChangedCallback(
            evt => {
                indexVal = CheckExists(evt.newValue);
                titleString = allowed == true ? $"{titleString}" : $"<color=red>{titleString}</color>";
            });   //indexVal = evt.newValue




            mainContainer.Insert(1, textFieldIndexRule);



            Toggle isOpenRight = new Toggle() { text = "Right", value = isOpenRightBool };
            isOpenRight.MarkDirtyRepaint();
            isOpenRight.RegisterValueChangedCallback(evt => { isOpenRightBool = evt.newValue;});

            mainContainer.Add(isOpenRight);

            Toggle isOpenLeft = new Toggle() { text = "Left", value = isOpenLeftBool};
            isOpenLeft.MarkDirtyRepaint();
            isOpenLeft.RegisterValueChangedCallback(evt => { isOpenLeftBool = evt.newValue; });

            mainContainer.Add(isOpenLeft);

            Toggle isOpenAbove = new Toggle() { text = "Above", value = isOpenAboveBool};
            isOpenAbove.MarkDirtyRepaint();
            isOpenAbove.RegisterValueChangedCallback(evt => { isOpenAboveBool = evt.newValue; });

            mainContainer.Add(isOpenAbove);

            Toggle isOpenBelow = new Toggle() { text = "Below", value = isOpenBelowBool};
            isOpenBelow.MarkDirtyRepaint();
            isOpenBelow.RegisterValueChangedCallback(evt => { isOpenBelowBool = evt.newValue; });

            mainContainer.Add(isOpenBelow);


            RefreshExpandedState();
        }
        




    }



}
