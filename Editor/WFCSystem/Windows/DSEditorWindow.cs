using DS.Utilities;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace DS.Windows 
{
    public class DSEditorWindow : EditorWindow
    {
        private string _fileName = "WFCFile";
        public string _fileNameResources = "";
        private DSGraphView _graphView;

        [MenuItem("PCG Algorithms/WFC RuleBuilder", priority = 12)]
        public static void ShowExample()
        {
            DSEditorWindow wnd = GetWindow<DSEditorWindow>("WFC RuleBuilder");
        }

        private void CreateGUI()
        {
            AddGraphView();
            AddToolbar();
            AddStyles();
        }

        private void AddStyles() 
        {
            StyleSheet styleSheet = (StyleSheet)EditorGUIUtility.Load("DialogueSystem/GraphViewVariables.uss");

           rootVisualElement.styleSheets.Add(styleSheet);
        }

        private void AddGraphView() 
        {
            _graphView = new DSGraphView(this);

            _graphView.StretchToParentSize();

            rootVisualElement.Add(_graphView);
        }

        private void AddToolbar()
        {
            var toolbar = new Toolbar();
            Label labFN = new Label() { text = " Save File Name" }; 
            Label labFolderName = new Label() { text = " Name Of Folder With TileSet" }; 

            var textFieldFileName = DSElementUtility.CreateTextField(_fileName);
            textFieldFileName.MarkDirtyRepaint();
            textFieldFileName.RegisterValueChangedCallback(evt => _fileName = evt.newValue);

            var textFieldResourcesName = DSElementUtility.CreateTextField(_fileNameResources);
            textFieldResourcesName.MarkDirtyRepaint();
            textFieldResourcesName.RegisterValueChangedCallback(evt => _fileNameResources = evt.newValue);

            var loadButton = DSElementUtility.CreateButton("Load RuleSet", () => _graphView.LoadGraph(_fileName));
            var saveButton = DSElementUtility.CreateButton("Save RuleSet", () => _graphView.SaveGraph(_fileName));

            var refreshRules = DSElementUtility.CreateButton("Refresh Rules", () => _graphView.RefreshRules(_fileNameResources));

            toolbar.Add(labFN);
            toolbar.Add(textFieldFileName);
            toolbar.Add(loadButton);
            toolbar.Add(saveButton);
            toolbar.Add(labFolderName);
            toolbar.Add(textFieldResourcesName);
            toolbar.Add(refreshRules);

            Label rulesGV = new Label() { text = " Use the SpaceBar  --  A S D  to spawn the Nodes" };

            toolbar.Add(rulesGV);

            rootVisualElement.Add(toolbar);
        }
    }
}


