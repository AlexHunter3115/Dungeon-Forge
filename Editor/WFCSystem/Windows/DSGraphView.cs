using DS.Elements;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace DS.Windows
{
    using DS.Enumerations;
    using DS.Utilities;
    using System;
    using System.IO;
    using System.Linq;

    public class DSGraphView : GraphView
    {

        private DSSearchWindow searchWindow;

        private DSEditorWindow editorWindow;

        private GraphViewDataCont graphViewCont;

        private DSInfoNodeNode ruleNode;
        public IDictionary<int, string> ruleDict = new Dictionary<int, string>();


        public DSGraphView(DSEditorWindow dSEditorWindow)
        {
            editorWindow = dSEditorWindow;

            this.RegisterCallback<KeyDownEvent>(OnKeyDown, TrickleDown.TrickleDown);


            AddManipulators();

            AddSearchWindow();
            AddGridBackground();


            AddMiniMap();
            AddStyles();
        }

        void OnKeyDown(KeyDownEvent ev)
        {
            Vector2 mousePosition = Event.current.mousePosition;

            // Convert the mouse position to graph space
            Vector2 graphMousePosition = GetLocalMousePos(mousePosition);


            switch (ev.keyCode)
            {
                case KeyCode.A:
                    var ruleNodeA = (DSMultiChoiceNode)CreateNode(DSDialogueType.MultiChoice, graphMousePosition);
                    AddElement(ruleNodeA);

                    break;
                
                case KeyCode.D:
                    var ruleNodeD = (DSSingleChoiceNode)CreateNode(DSDialogueType.SingleChoice, graphMousePosition);
                    AddElement(ruleNodeD);
                    break;
               
                case KeyCode.S:
                    var ruleNodeS = (DSQuickRuleNode)CreateNode(DSDialogueType.QuickRule, graphMousePosition);
                    AddElement(ruleNodeS);
                    break;
                
                default:
                    break;
            }
        }

        private void AddMiniMap()
        {
            MiniMap miniMap = new MiniMap()
            {
                anchored = true,
            };

            miniMap.SetPosition(new Rect(15, 50, 200, 200));

            this.Add(miniMap);
        }
        private void AddSearchWindow()
        {
            if (searchWindow == null)
            {
                searchWindow = ScriptableObject.CreateInstance<DSSearchWindow>();

                searchWindow.Initilize(this);

            }

            nodeCreationRequest = context => SearchWindow.Open(new SearchWindowContext(context.screenMousePosition), searchWindow);
        }

        private void AddManipulators()
        {
            SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);

            this.AddManipulator(CreateNodeContextualMenu("Main Rule Node", DSDialogueType.SingleChoice));
            this.AddManipulator(CreateNodeContextualMenu("Sub Rule Node", DSDialogueType.MultiChoice));
            this.AddManipulator(CreateNodeContextualMenu("Quick Rule Node", DSDialogueType.QuickRule));
            this.AddManipulator(new ContentDragger());


            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());

            this.AddManipulator(CreateGroupContextualMenu());
        }
        private void AddGridBackground()
        {
            GridBackground gridBackground = new GridBackground();

            gridBackground.StretchToParentSize();

            Insert(0, gridBackground);
        }
        private void AddStyles()
        {
            StyleSheet styleSheet = (StyleSheet)EditorGUIUtility.Load("DialogueSystem/UIGraphViewStyle.uss");

            styleSheets.Add(styleSheet);

        }


        public void RefreshRules(string fileName)
        {
            ruleDict.Clear();

            var nodes = this.nodes.ToList();
            bool respawn = true;


            foreach (var GVnode in nodes)
            {
                DSNode iterNode = GVnode as DSNode;

                if (iterNode.dialogueType == DSDialogueType.InfoNode)
                {
                    respawn = false;
                    break;
                }
            }

            if (respawn)
            {
                ruleNode = (DSInfoNodeNode)CreateNode(DSDialogueType.InfoNode, Vector2.zero);
                this.AddElement(ruleNode);
            }

            if (fileName == null) { return; }

            List<string> fileNames = new List<string>();

            fileName = "Assets/Resources/" + fileName;

            var info = new DirectoryInfo(fileName);
            var fileInfo = info.GetFiles();

            foreach (var file in fileInfo)
            {
                if (file.Name.Contains("meta"))
                {
                    continue;
                }

                int index = file.Name.IndexOf(".");
                var manipString = file.Name.Substring(0, index);

                manipString = manipString.Replace("Variant", "");

                fileNames.Add(manipString);

                ruleDict.Add(fileNames.Count - 1, manipString);
            }

            AddRuleNode(fileNames);
        }
        private void AddRuleNode(List<string> TileSetNames)
        {
            ruleNode.inputContainer.Clear();
            ruleNode.outputContainer.Clear();

            Label tileNameDesc = new Label() { text = "Name of the tile:" };
            Label tileIndexDesc = new Label() { text = "Index of the tile:" };

            ruleNode.inputContainer.Add(tileNameDesc);
            ruleNode.outputContainer.Add(tileIndexDesc);

            ruleNode.RefreshExpandedState();

            for (int i = 0; i < TileSetNames.Count; i++)
            {
                Label tileName = new Label() { text = TileSetNames[i] };
                Label tileIndex = new Label() { text = "    " + i.ToString() };

                ruleNode.inputContainer.Add(tileName);
                ruleNode.outputContainer.Add(tileIndex);

                ruleNode.RefreshExpandedState();
            }
        }


        private IManipulator CreateGroupContextualMenu()
        {
            ContextualMenuManipulator contextualMenuManipulator = new ContextualMenuManipulator(
                menuEvent => menuEvent.menu.AppendAction("Add group", actionEvent => AddElement(CreateGroup("Dialouge group", GetLocalMousePos(actionEvent.eventInfo.localMousePosition))))
                );

            return contextualMenuManipulator;
        }
        public GraphElement CreateGroup(string title, Vector2 pos)
        {
            Group group = new Group()
            {
                title = title
            };

            group.SetPosition(new Rect(pos, Vector2.zero));

            return group;

        }
        private IManipulator CreateNodeContextualMenu(string actionTitle, DSDialogueType type)
        {
            ContextualMenuManipulator contextualMenuManipulator = new ContextualMenuManipulator(
                menuEvent => menuEvent.menu.AppendAction(actionTitle, actionEvent => AddElement(CreateNode(type, GetLocalMousePos(actionEvent.eventInfo.localMousePosition))))
                );

            return contextualMenuManipulator;
        }


        public DSNode CreateNode(DSDialogueType type, Vector2 pos)
        {
            Type nodeType = Type.GetType($"DS.Elements.DS{type}Node");

            DSNode node = (DSNode)Activator.CreateInstance(nodeType);

            node.Initialize(pos, this);
            node.Draw();

            return node;
        }
        public DSNode CreateNode(DSDialogueType type, Vector2 pos, string indexVal, string Guid)
        {
            Type nodeType = Type.GetType($"DS.Elements.DS{type}Node");

            DSNode node = (DSNode)Activator.CreateInstance(nodeType);

            node.Initialize(pos, this);
            node.indexVal = indexVal;
            node.nodeGuid = Guid;

            node.Draw();

            return node;
        }
        public DSNode CreateQuickNode(DSDialogueType type, Vector2 pos, string indexVal, string Guid, bool above, bool below, bool left, bool right)
        {
            Type nodeType = Type.GetType($"DS.Elements.DS{type}Node");

            DSQuickRuleNode node = (DSQuickRuleNode)Activator.CreateInstance(nodeType);

            node.Initialize(pos, this);
            node.indexVal = indexVal;
            node.nodeGuid = Guid;
            node.isOpenAboveBool = above;
            node.isOpenBelowBool = below;
            node.isOpenLeftBool = left;
            node.isOpenRightBool = right;
          
            node.Draw();

            return node;
        }

        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
        {
            List<Port> compatiblePorts = new List<Port>();

            ports.ForEach(port =>
            {
                if (startPort == port) { return; }

                if (startPort.node == port.node) { return; }

                if (startPort.direction == port.direction) { return; }

                compatiblePorts.Add(port);
            });

            return compatiblePorts;
        }
        public Vector2 GetLocalMousePos(Vector2 pos, bool isSearchWindow = false)
        {
            Vector2 worldMousePos = pos;

            if (searchWindow)
            {
                worldMousePos -= editorWindow.position.position;
            }

            Vector2 localMousePos = contentViewContainer.WorldToLocal(worldMousePos);

            return localMousePos;
        }



        public void LoadGraph(string filename)
        {
            graphViewCont = Resources.Load<GraphViewDataCont>("Resources_Algorithms/WFC_Rule_Sets/" + filename);

            if (graphViewCont == null) { return; }

            ClearGraph();
            GenNodes();
            GenQuickNodes();

            var arr = nodes.ToList().Cast<DSNode>().ToList();
            ConnectNodes(arr);

            RefreshRules(editorWindow._fileNameResources);
        }
        private void ClearGraph()
        {
            foreach (var edge in edges)
            {
                this.RemoveElement(edge);
            }

            foreach (var node in nodes)
            {
                this.RemoveElement(node);
            }
        }
        private void GenNodes()
        {
            var list = new List<DSNode>();

            foreach (var node in graphViewCont.nodeData)
            {
                var createdNode = CreateNode(node.dialogueType, node.position, node.IndexTile, node.nodeGuid);
                list.Add(createdNode);
                this.AddElement(createdNode);
            }
        }
        private void ConnectNodes(List<DSNode> arr)
        {
            var nodeConnections = graphViewCont.nodeLinkData;

            foreach (var nodeCon in nodeConnections)
            {

                DSNode inputNode = null;
                DSNode outputNode = null;

                foreach (var node in arr)
                {
                    if (node.nodeGuid == nodeCon.TargetNodeGuid)
                    {
                        inputNode = node;
                    }
                    else if (node.nodeGuid == nodeCon.BaseNodeGuid)
                    {
                        outputNode = node;
                    }
                }

                int idx = DSElementUtility.GetPortIdx(nodeCon.PortName);

                var tempEdge = new UnityEditor.Experimental.GraphView.Edge { output = (Port)outputNode.outputContainer[0], input = (Port)inputNode.inputContainer[idx] };

                tempEdge?.input.Connect(tempEdge);
                tempEdge?.output.Connect(tempEdge);

                this.Add(tempEdge);
            }
        }

        private void GenQuickNodes()
        {
            foreach (var quickNode in graphViewCont.quickNodeData)
            {
                var createdNode = CreateQuickNode(quickNode.dialogueType, quickNode.position, quickNode.IndexTile, quickNode.nodeGuid, 
                    quickNode.IsOpenAbove, quickNode.IsOpenBelow, quickNode.IsOpenLeft, quickNode.IsOpenRight);

                this.AddElement(createdNode);
            }
        }

        public void SaveGraph(string filename)
        {
            var edges = this.edges.ToList();
            var nodes = this.nodes.ToList();

            var GVcont = ScriptableObject.CreateInstance<GraphViewDataCont>();

            var connectedPorts = edges.Where(x => x.input.node != null).ToArray();

            for (int i = 0; i < connectedPorts.Length; i++)
            {
                var outputNode = connectedPorts[i].output.node as DSNode;
                var inputNode = connectedPorts[i].input.node as DSNode;

                GVcont.nodeLinkData.Add(new NodeLinkData() { BaseNodeGuid = outputNode.nodeGuid, PortName = connectedPorts[i].input.portName, TargetNodeGuid = inputNode.nodeGuid });
            }

            bool save = true;

            foreach (var GVnode in nodes)
            {
                DSNode iterNode = GVnode as DSNode;

                if (iterNode.dialogueType == DSDialogueType.InfoNode)
                { continue; }
                if (iterNode.allowed == false)
                {
                    save = false;
                    EditorUtility.DisplayDialog("Inavlid Index Given", "lase ensure all the index are within range", "OK!");
                    break;
                }

                if (iterNode.dialogueType == DSDialogueType.QuickRule)
                {

                    var refNode = GVnode as DSQuickRuleNode;

                    GVcont.quickNodeData.Add(new QuickNodeData()
                    {
                        position = iterNode.GetPosition().position,
                        nodeGuid = iterNode.nodeGuid,
                        dialogueType = iterNode.dialogueType,
                        IndexTile = iterNode.indexVal,
                        IsOpenAbove = refNode.isOpenAboveBool,
                        IsOpenBelow = refNode.isOpenBelowBool,
                        IsOpenLeft = refNode.isOpenLeftBool,
                        IsOpenRight = refNode.isOpenRightBool
                    });
                }
                else
                {
                    GVcont.nodeData.Add(new NodeData() { position = iterNode.GetPosition().position, nodeGuid = iterNode.nodeGuid, dialogueType = iterNode.dialogueType, IndexTile = iterNode.indexVal });
                }
            }

            if (save == false)
            {
                return;
            }

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

            AssetDatabase.CreateAsset(GVcont, $"Assets/Resources/Resources_Algorithms/Tile_Sets_Ruleset/{filename}.asset");
            AssetDatabase.SaveAssets();

        }
    }

}

