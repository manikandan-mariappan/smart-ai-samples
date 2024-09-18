﻿using Syncfusion.Blazor.Diagram;
using Syncfusion.Blazor.Spinner;
using System.Collections.ObjectModel;
using System.Text.Json;
using shapes = Syncfusion.Blazor.Diagram.NodeShapes;

namespace TextToMindMapDiagram
{
    public partial class TextToMindMap
    {
#pragma warning disable CS8618
        public SfDiagramComponent Diagram;
        public SfSpinner SpinnerRef;
        public DiagramOpenAI DiagramOpenAIRef;
        public DiagramShortCutKey DiagramShortCutKeyRef;
        public DiagramMenuBar MenubarRef;
        public DiagramToolBar Toolbar;
        public SnapConstraints SnapConstraint = SnapConstraints.ShowLines;
        public double CurrentZoom { get; set; } = 1;
        public bool IsGeneratingFromAI = false;
        /// <summary>
        /// Collection of keyboard commands for the diagram.
        /// </summary>
        DiagramObjectCollection<KeyboardCommand> commands = new DiagramObjectCollection<KeyboardCommand>();
        /// <summary>
        /// The minimum zoom level allowed for the diagram.
        /// </summary>
        private double minZoom { get; set; } = 0.25;
        /// <summary>
        /// The maximum zoom level allowed for the diagram.
        /// </summary>
        private double maxZoom { get; set; } = 30;
        /// <summary>
        /// Specifies whether the undo functionality is enabled in the diagram.
        /// </summary>
        public bool IsUndo = false;
        /// <summary>
        /// Specifies whether the redo functionality is enabled in the diagram.
        /// </summary>
        public bool IsRedo = false;
        /// <summary>
        /// Specifies whether the diagram is selected.
        /// </summary>
        public bool diagramSelected = false;
        /// <summary>
        /// Represents an array of fill colors.
        /// </summary>
        private static string[] fillColorCode = { "#C4F2E8", "#F7E0B3", "#E5FEE4", "#E9D4F1", "#D4EFED", "#DEE2FF" };
        /// <summary>
        /// Represents an array of border colors.
        /// </summary>
        private static string[] borderColorCode = { "#8BC1B7", "#E2C180", "#ACCBAA", "#D1AFDF", "#90C8C2", "#BBBFD6" };
        /// <summary>
        /// Represents the last fill index, an integer.
        /// </summary>
        private static int lastFillIndex = 0;
        LayoutType layoutType = LayoutType.MindMap;
        public string height = "700px";
        public string width = "100%";
        public DiagramObjectCollection<Node> nodes = new DiagramObjectCollection<Node>();
        public DiagramObjectCollection<Connector> connectors = new DiagramObjectCollection<Connector>();
        ScrollLimitMode scrollLimit { get; set; } = ScrollLimitMode.Diagram;
        DiagramInteractions interactionController = DiagramInteractions.SingleSelect;
        int VerticalSpacing = 20;
        int HorizontalSpacing = 80;
        DiagramSelectionSettings selectionSettings = new DiagramSelectionSettings();
        DiagramObjectCollection<UserHandle> handles = new DiagramObjectCollection<UserHandle>();
#pragma warning restore CS8618
        public List<MindMapDetails> MindmapData = new List<MindMapDetails>()
        {
            new MindMapDetails(){Id="node1",Label="Business Planning",ParentId ="",Branch= BranchType.Root, Fill="#D0ECFF", Level = 0 },
            new MindMapDetails(){Id="node2",Label= "Expectation",ParentId = "node1",Branch= BranchType.Left,Fill= "#C4F2E8", Level = 1  },
            new MindMapDetails(){Id="node3",Label= "Requirements", ParentId="node1",Branch= BranchType.Right,Fill= "#F7E0B3", Level = 1  },
            new MindMapDetails(){Id="node4",Label= "Marketing", ParentId="node1",Branch= BranchType.Left,Fill= "#E5FEE4", Level = 1  },
            new MindMapDetails(){Id="node5",Label= "Budgets",ParentId= "node1",Branch= BranchType.Right,Fill= "#E9D4F1", Level = 1  },
            new MindMapDetails(){ Id="node6", Label="Situation in Market", ParentId= "node1", Branch = BranchType.Left, Fill= "#D4EFED", Level = 1  },
            new MindMapDetails(){ Id="node7", Label="Product Sales", ParentId= "node2", Branch = BranchType.SubLeft, Fill= "#C4F2E8", Level = 2  },
            new MindMapDetails() { Id = "node8", Label= "Strategy", ParentId="node2", Branch = BranchType.SubLeft, Fill="#C4F2E8", Level = 2  },
            new MindMapDetails() { Id = "node9", Label="Contacts", ParentId="node2", Branch = BranchType.SubLeft, Fill="#C4F2E8", Level = 2  },
            new MindMapDetails() { Id = "node10", Label="Customer Groups", ParentId= "node4", Branch = BranchType.SubLeft,Fill= "#E5FEE4", Level = 2  },
            new MindMapDetails() { Id = "node11", Label= "Branding", ParentId= "node4", Branch = BranchType.SubLeft, Fill= "#E5FEE4", Level = 2  },
            new MindMapDetails() { Id = "node12", Label= "Advertising", ParentId= "node4", Branch = BranchType.SubLeft, Fill= "#E5FEE4", Level = 2  },
            new MindMapDetails() { Id = "node13", Label= "Competitors", ParentId= "node6", Branch = BranchType.SubLeft, Fill="#D4EFED", Level = 2  },
            new MindMapDetails() { Id = "node14", Label="Location", ParentId="node6", Branch = BranchType.SubLeft, Fill= "#D4EFED", Level = 2  },
            new MindMapDetails() { Id = "node15", Label= "Director", ParentId= "node3", Branch = BranchType.SubRight, Fill="#F7E0B3", Level = 2  },
            new MindMapDetails() { Id = "node16", Label="Accounts Department", ParentId= "node3", Branch = BranchType.SubRight, Fill= "#F7E0B3", Level = 2  },
            new MindMapDetails() { Id = "node17", Label="Administration", ParentId= "node3", Branch = BranchType.SubRight, Fill="#F7E0B3", Level = 2  },
            new MindMapDetails() { Id = "node18", Label= "Development", ParentId="node3", Branch = BranchType.SubRight, Fill= "#F7E0B3", Level = 2  },
            new MindMapDetails() { Id = "node19", Label= "Estimation", ParentId= "node5", Branch = BranchType.SubRight, Fill="#E9D4F1", Level = 2  },
            new MindMapDetails() { Id = "node20", Label= "Profit", ParentId= "node5", Branch = BranchType.SubRight, Fill= "#E9D4F1", Level = 2  },
            new MindMapDetails(){ Id="node21", Label="Funds", ParentId= "node5", Branch = BranchType.SubRight, Fill= "#E9D4F1", Level = 2  }
        };

        public void StateChanged()
        {
            StateHasChanged();
        }

        private void CollectionChanging(CollectionChangingEventArgs args)
        {
            if (args.Action == CollectionChangedAction.Add && IsGeneratingFromAI)
            {
                Connector connector = args.Element as Connector;
                if (connector != null)
                {
                    UpdateMermaidNodeInfo(connector);
                }
            }
        }
        BranchType CurrentBranch = BranchType.Left;
        private void UpdateMermaidNodeInfo(Connector connector)
        {
            Node sourceNode = Diagram.GetObject(connector.SourceID) as Node;
            Node targetNode = Diagram.GetObject(connector.TargetID) as Node;
            if (connector.ID == Diagram.Connectors[0].ID)
            {
                CurrentBranch = BranchType.Left;
                sourceNode.AdditionalInfo["ParentId"] = "";
                sourceNode.AdditionalInfo["Orientation"] = BranchType.Root;
                sourceNode.AdditionalInfo["Level"] = 0;
                sourceNode.Style.Fill = "#D0ECFF";
                sourceNode.Style.StrokeColor = "#80BFEA";
            }
            if (sourceNode != null && (BranchType)sourceNode.AdditionalInfo["Orientation"] == BranchType.Root)
            {
                targetNode.AdditionalInfo["ParentId"] = sourceNode.ID;
                targetNode.AdditionalInfo["Orientation"] = CurrentBranch;
                targetNode.AdditionalInfo["Level"] = 1;
                CurrentBranch = (CurrentBranch == BranchType.Left) ? BranchType.Right : BranchType.Left;
            }
            else
            {
                BranchType sourceNodeBranch = (BranchType)sourceNode.AdditionalInfo["Orientation"];
                targetNode.AdditionalInfo["ParentId"] = sourceNode.ID;
                targetNode.AdditionalInfo["Orientation"] = (sourceNodeBranch == BranchType.Left || sourceNodeBranch == BranchType.SubLeft) ? BranchType.SubLeft : BranchType.SubRight;
                targetNode.AdditionalInfo["Level"] = Convert.ToDouble(sourceNode.AdditionalInfo["Level"]) + 1;
            }

            UpdateNodeStyles(targetNode, sourceNode);
            BranchType targetNodeBranch = (BranchType)targetNode.AdditionalInfo["Orientation"];
            if (targetNodeBranch == BranchType.Right || targetNodeBranch == BranchType.SubRight)
            {
                connector.SourcePortID = sourceNode.Ports[0].ID;
                connector.TargetPortID = targetNode.Ports[1].ID;
            }
            else if (targetNodeBranch == BranchType.Left || targetNodeBranch == BranchType.SubLeft)
            {
                connector.SourcePortID = sourceNode.Ports[1].ID;
                connector.TargetPortID = targetNode.Ports[0].ID;
            }
        }
        /// <summary>
        /// This Method to execute the custom command.
        /// </summary>
        public async Task ExecuteCommand(CommandKeyArgs obj)
        {

            if (obj.Name == "leftChild")
            {
                if (Diagram.SelectionSettings != null && Diagram.SelectionSettings.Nodes.Count > 0)
                {
                    Diagram.StartGroupAction();
                    BranchType type = (BranchType)Diagram.SelectionSettings.Nodes[0].AdditionalInfo["Orientation"];
                    if (type == BranchType.SubRight || type == BranchType.Right)
                    {
                        await TextToMindMap.AddLeftChild(Diagram);
                    }
                    else if (type == BranchType.SubLeft || type == BranchType.Left || type == BranchType.Root)
                    {
                        await TextToMindMap.AddRightChild(Diagram);
                    }
                    Diagram.ClearSelection();
                    Diagram.Select(new ObservableCollection<IDiagramObject>() { Diagram.Nodes[Diagram.Nodes.Count - 1] });
                    Diagram.StartTextEdit(Diagram.Nodes[Diagram.Nodes.Count - 1]);
                    Diagram.EndGroupAction();
                }
            }
            if (obj.Name == "rightChild")
            {
                if (Diagram.SelectionSettings != null && Diagram.SelectionSettings.Nodes.Count > 0)
                {
                    Diagram.StartGroupAction();
                    BranchType type = (BranchType)Diagram.SelectionSettings.Nodes[0].AdditionalInfo["Orientation"];
                    if (type == BranchType.SubLeft || type == BranchType.Left)
                    {
                        await TextToMindMap.AddRightChild(Diagram);
                    }
                    else if (type == BranchType.SubRight || type == BranchType.Right || type == BranchType.Root)
                    {
                        await TextToMindMap.AddLeftChild(Diagram);
                    }
                    Diagram.ClearSelection();
                    Diagram.Select(new ObservableCollection<IDiagramObject>() { Diagram.Nodes[Diagram.Nodes.Count - 1] });
                    Diagram.StartTextEdit(Diagram.Nodes[Diagram.Nodes.Count - 1]);
                    Diagram.EndGroupAction();
                }
            }
            if (obj.Name == "sibilingChildTop")
            {
                Node rootNode = Diagram.Nodes.Where(node => node.InEdges.Count == 0).ToList()[0];
                if (rootNode.ID != Diagram.SelectionSettings.Nodes[0].ID)
                {
                    Diagram.StartGroupAction();
                    string nodeParent = Convert.ToString(Diagram.SelectionSettings.Nodes[0].AdditionalInfo["ParentId"]);
                    string parentID = nodeParent;
                    Node parentNode = Diagram.GetObject(parentID) as Node;
                    BranchType branch = (BranchType)(parentNode.AdditionalInfo["Orientation"]);
                    BranchType nodeBranch = (BranchType)(Diagram.SelectionSettings.Nodes[0].AdditionalInfo["Orientation"]);
                    if (branch == BranchType.SubRight || branch == BranchType.Right || (branch == BranchType.Root && nodeBranch == BranchType.Right))
                    {
                        await TextToMindMap.AddLeftChild(Diagram, true);
                    }
                    else
                    {
                        await TextToMindMap.AddRightChild(Diagram, true);
                    }
                    Diagram.ClearSelection();
                    Diagram.Select(new ObservableCollection<IDiagramObject>() { Diagram.Nodes[Diagram.Nodes.Count - 1] });
                    Diagram.StartTextEdit(Diagram.Nodes[Diagram.Nodes.Count - 1]);
                    Diagram.EndGroupAction();
                }
            }
            if (obj.Name == "navigationDown")
            {
                NavigateChild("Bottom");
            }
            if (obj.Name == "navigationUp")
            {
                NavigateChild("Top");
            }
            if (obj.Name == "navigationLeft")
            {
                NavigateChild("Right");
            }
            if (obj.Name == "navigationRight")
            {
                NavigateChild("Left");
            }
            if (obj.Name == "deleteChid" || obj.Name == "delete" || obj.Name == "backspace")
            {
                Diagram.BeginUpdate();
                RemoveData(Diagram.SelectionSettings.Nodes[0], Diagram);
                _ = Diagram.EndUpdate();
                await Diagram.DoLayout();
            }
            if (obj.Name == "fitPage")
            {
                FitOptions fitoption = new FitOptions()
                {
                    Mode = FitMode.Both,
                    Region = DiagramRegion.PageSettings,
                };
                Diagram.FitToPage(fitoption);
            }
            if (obj.Name == "showShortCut")
            {
                ShowHideShortcutKey();
            }
            if (obj.Name == "duplicate")
            {
                MenubarRef.IsDuplicate = true;
                Diagram.Copy();
                Diagram.Paste(); MenubarRef.IsDuplicate = false;
            }
            if (obj.Name == "fileNew")
            {
                Diagram.Clear();
                MenubarRef.enablePasteButten = false;
                Diagram.BeginUpdate();
                MenubarRef.ViewMenuItems[7].IconCss = "sf-icon-blank";
                MenubarRef.WindowMenuItems[1].IconCss = "sf-icon-Selection";
                DiagramShortCutKeyRef.ShowShortCutKey = "block";
                Toolbar.PointerItemCssClass = "tb-item-middle tb-item-selected tb-item-pointer";
                Toolbar.PanItemCssClass = "tb-item-start tb-item-pan";
                await Toolbar.HideElements("hide-toolbar", true);
                MenubarRef.WindowMenuItems[0].IconCss = "sf-icon-Selection";
                Toolbar.StateChanged();
                DiagramShortCutKeyRef.RefreshShortcutKeyPanel();
                MenubarRef.ItemSelection();
                StateHasChanged();
            }
            if (obj.Name == "fileOpen")
            {
                await MenubarRef.OpenUploadBox(true, ".json");
            }
            if (obj.Name == "fileSave")
            {
                string fileName = "diagram";
                await MenubarRef.Download(fileName);
            }
        }

        /// <summary>
        /// This method is used to navigate between the nodes
        /// </summary>
        public void NavigateChild(string direction)
        {
            SfDiagramComponent diagram = Diagram;
            Node? node = null;
            List<Node> sameLevelNodes = new List<Node>();
            if (direction == "Top" || direction == "Bottom")
            {
                sameLevelNodes = GetSameLevelNodes();
                int index = sameLevelNodes.IndexOf(diagram.SelectionSettings.Nodes[0]);
                node = direction == "Top" ? sameLevelNodes[index == 0 ? 0 : index - 1] : sameLevelNodes[index == (sameLevelNodes.Count - 1) ? index : index + 1];
            }
            else
                node = GetMinDistanceNode(diagram, direction);
            if (node != null)
            {
                diagram.Select(new ObservableCollection<IDiagramObject>() { node });
            }

        }

        /// <summary>
        /// This method is used to return a minimum distance node whie navigating between left and right
        /// </summary>
        private Node GetMinDistanceNode(SfDiagramComponent diagram, string direction)
        {
            Node node = diagram.SelectionSettings.Nodes[0];
            double? nodeWidth = (node.Width == null) ? node.MinWidth : node.Width;
            DiagramRect parentBounds = new DiagramRect((node.OffsetX - (nodeWidth / 2)), node.OffsetY - (node.Height / 2), nodeWidth, node.Height);
            DiagramRect childBounds = new DiagramRect();
            double oldChildBoundsTop = 0;
            Node? childNode = null;
            Node? lastChildNode = null;
            Node? leftOrientationFirstChild = null;
            Node? rightOrientationFirstChild = null;
            Node rootNode = diagram.Nodes.Where(node => node.InEdges.Count == 0).ToList()[0];
            if (node.ID == rootNode.ID)
            {
                List<string> edges = node.OutEdges;
                for (int i = 0; i < edges.Count; i++)
                {
                    Connector connector = GetConnector(diagram.Connectors, edges[i]);
                    childNode = GetNode(diagram.Nodes, connector.TargetID);
                    if (Convert.ToString((BranchType)childNode.AdditionalInfo["Orientation"]) == direction)
                    {
                        if (direction == "Left" && leftOrientationFirstChild == null)
                            leftOrientationFirstChild = childNode;
                        if (direction == "Right" && rightOrientationFirstChild == null)
                            rightOrientationFirstChild = childNode;
                        double? childNodeWidth = (childNode.Width == null) ? childNode.MinWidth : childNode.Width;
                        childBounds = new DiagramRect((childNode.OffsetX - (childNodeWidth / 2)), childNode.OffsetY - (childNode.Height / 2), childNodeWidth, childNode.Height);
                        if (parentBounds.Top >= childBounds.Top && (childBounds.Top >= oldChildBoundsTop || oldChildBoundsTop == 0))
                        {
                            oldChildBoundsTop = childBounds.Top;
                            lastChildNode = childNode;
                        }
                    }
                }
                if (lastChildNode != null)
                    lastChildNode = direction == "Left" ? leftOrientationFirstChild : rightOrientationFirstChild;
            }
            else
            {
                List<string> edges = new List<string>();
                string selectType = string.Empty;
                string orientation = ((BranchType)node.AdditionalInfo["Orientation"]).ToString();
                if (orientation == "Left" || orientation == "SubLeft")
                {
                    edges = direction == "Left" ? node.OutEdges : node.InEdges;
                    selectType = direction == "Left" ? "Target" : "Source";
                }
                else
                {
                    edges = direction == "Right" ? node.OutEdges : node.InEdges;
                    selectType = direction == "Right" ? "Target" : "Source";
                }
                for (int i = 0; i < edges.Count; i++)
                {
                    Connector connector = GetConnector(diagram.Connectors, edges[i]);
                    childNode = GetNode(diagram.Nodes, selectType == "Target" ? connector.TargetID : connector.SourceID);
                    if (childNode.ID == rootNode.ID)
                        lastChildNode = childNode;
                    else
                    {
                        double? childNodeWidth = (childNode.Width == null) ? childNode.MinWidth : childNode.Width;
                        childBounds = new DiagramRect((childNode.OffsetX - (childNodeWidth / 2)), childNode.OffsetY - (childNode.Height / 2), childNodeWidth, childNode.Height);
                        if (selectType == "Target")
                        {
                            if (parentBounds.Top >= childBounds.Top && (childBounds.Top >= oldChildBoundsTop || oldChildBoundsTop == 0))
                            {
                                oldChildBoundsTop = childBounds.Top;
                                lastChildNode = childNode;
                            }
                        }
                        else
                            lastChildNode = childNode;
                    }
                }
            }
            return lastChildNode;
        }

        /// <summary>
        /// This method is used to return a same level nodes
        /// </summary>

        private List<Node> GetSameLevelNodes()
        {
            List<Node> sameLevelNodes = new List<Node>();
            SfDiagramComponent diagram = Diagram;
            if (diagram.SelectionSettings.Nodes.Count > 0)
            {
                Node node = diagram.SelectionSettings.Nodes[0];
                string orientation = ((BranchType)node.AdditionalInfo["Orientation"]).ToString();
                Connector connector = GetConnector(diagram.Connectors, node.InEdges[0]);
                Node parentNode = GetNode(diagram.Nodes, connector.SourceID);
                for (int i = 0; i < parentNode.OutEdges.Count; i++)
                {
                    connector = GetConnector(diagram.Connectors, parentNode.OutEdges[i]);
                    Node childNode = GetNode(diagram.Nodes, connector.TargetID);
                    if (childNode != null)
                    {
                        string childOrientation = Convert.ToString((BranchType)childNode.AdditionalInfo["Orientation"]);
                        if (orientation == childOrientation)
                        {
                            sameLevelNodes.Add(childNode);
                        }
                    }
                }
            }
            return sameLevelNodes;
        }

        /// <summary>
        /// This method is used to get the Nodes by connectors sourceID and targetID.
        /// </summary>
        public Node GetNode(DiagramObjectCollection<Node> diagramNodes, string name)
        {
            for (int i = 0; i < diagramNodes.Count; i++)
            {
                if (diagramNodes[i].ID == name)
                {
                    return diagramNodes[i];
                }
            }
            return null;
        }
        /// <summary>
        /// This method is used to get the connectors by node's inedges and outedges
        /// </summary>
        public Connector GetConnector(DiagramObjectCollection<Connector> diagramConnectors, string name)
        {
            for (int i = 0; i < diagramConnectors.Count; i++)
            {
                if (diagramConnectors[i].ID == name)
                {
                    return diagramConnectors[i];
                }
            }
            return null;
        }
        /// <summary>
        /// This method to determine whether this command can execute or not.
        /// </summary>
        public void CanExecute(CommandKeyArgs args)
        {
            args.CanExecute = true;
        }
        private BranchType getbranch(IDiagramObject obj)
        {
            Node node = obj as Node;
            BranchType Branch = (BranchType)node.AdditionalInfo["Orientation"];

            return Branch;
        }

        private void OnCreated()
        {
            Diagram.Select(new ObservableCollection<IDiagramObject>() { Diagram.Nodes[0] });
        }

        /// <summary>
        /// This method is triggered when select or deselect any objects from the diagram. .
        /// </summary>
        private void SelectionChanged(Syncfusion.Blazor.Diagram.SelectionChangedEventArgs args)
        {
            Toolbar.EnableToolbarItems(args.NewValue, "selectionchange");
            int ObjectsLength = Diagram.SelectionSettings.Nodes.Count + Diagram.SelectionSettings.Connectors.Count;
            if (ObjectsLength > 1 && (Diagram.SelectionSettings.Nodes.Count > 0 || (Diagram.SelectionSettings.Connectors.Count > 0)))
            {
                diagramSelected = false;
                this.MultipleSelectionSettings(args.NewValue);
            }
            else if (ObjectsLength == 1 && (Diagram.SelectionSettings.Nodes.Count == 1 || Diagram.SelectionSettings.Connectors.Count == 1))
            {
                Toolbar.SingleSelectionToolbarItems();
            }
            else
            {
                diagramSelected = true;
                Toolbar.DiagramSelectionToolbarItems();
            }
        }

        /// <summary>
        /// This method is used to enable the toolbar items in diagram interaction.
        /// </summary>
        private void HistoryChange(HistoryChangedEventArgs args)
        {
            Toolbar.EnableToolbarItems(new object() { }, "historychange");
        }
        /// <summary>
        /// This method is used to enable the tool bar items in multiple selection.
        /// </summary>
        private void MultipleSelectionSettings(ObservableCollection<IDiagramObject> SelectedItems)
        {
            Toolbar.MutipleSelectionToolbarItems();
        }
        /// <summary>
        /// This method is used show or hide the shortcut key.
        /// </summary>
        public void ShowHideShortcutKey()
        {
            DiagramShortCutKeyRef.ShowShortCutKey = DiagramShortCutKeyRef.ShowShortCutKey == "none" ? "block" : "none";
            int shortcutIndex = MenubarRef.WindowMenuItems.FindIndex(item => item.Text == "Show Shortcuts");
            MenubarRef.WindowMenuItems[shortcutIndex].IconCss = MenubarRef.WindowMenuItems[shortcutIndex].IconCss == "sf-icon-blank" ? "sf-icon-Selection" : "sf-icon-blank";
            MenubarRef.StateChanged();
            DiagramShortCutKeyRef.RefreshShortcutKeyPanel();
        }
        private void ScrollChanged()
        {
            if (CurrentZoom >= 0.25 && CurrentZoom <= 30)
            {
                Toolbar.ZoomItemDropdownContent = FormattableString.Invariant($"{Math.Round(CurrentZoom * 100)}") + "%";
                Toolbar.StateChanged();
            }
        }
        // Method to customize the tool
        public InteractionControllerBase GetCustomTool(DiagramElementAction action, string id)
        {
            InteractionControllerBase tool = null;
            if (id == "AddLeft")
            {
                tool = new AddRightTool(Diagram);
            }
            else if (id == "AddRight")
            {
                tool = new AddLeftTool(Diagram);
            }
            else
            {
                tool = new DeleteTool(Diagram);
            }
            return tool;
        }

        public static async Task AddRightChild(SfDiagramComponent diagram, bool isSibling = false)
        {
            string newChildID = RandomId();
            string newchildColor = ""; BranchType type = BranchType.Left; Node parentNode = null;
            string parentId = Convert.ToString(diagram.SelectionSettings.Nodes[0].AdditionalInfo["ParentId"]);
            BranchType nodeBranch = (BranchType)diagram.SelectionSettings.Nodes[0].AdditionalInfo["Orientation"];
            double currentLevel = Convert.ToDouble(diagram.SelectionSettings.Nodes[0].AdditionalInfo["Level"]);
            double parentLevel = 0;
            if (!string.IsNullOrEmpty(parentId))
            {
                parentNode = diagram.GetObject(parentId) as Node;
                BranchType parentNodeBranch = (BranchType)parentNode.AdditionalInfo["Orientation"];
                type = isSibling ? parentNodeBranch : nodeBranch;
            }
            else
            {
                type = nodeBranch;
            }
            BranchType childType = BranchType.Left;
            if (parentNode != null) parentLevel = Convert.ToDouble(parentNode.AdditionalInfo["Level"]);
            switch (type.ToString())
            {
                case "Root":
                    childType = BranchType.Left;
                    break;
                case "Left":
                    childType = BranchType.SubLeft;
                    break;
                case "SubLeft":
                    childType = BranchType.SubLeft;
                    break;
            }

            double level = isSibling ? parentLevel : currentLevel;
            if (level == 0)
            {
                int index = Convert.ToInt32(GetFillColorIndex(level));
                newchildColor = fillColorCode[index];
            }
            else
            {
                newchildColor = diagram.SelectionSettings.Nodes[0].Style.Fill;
            }

            MindMapDetails childNode = new MindMapDetails()
            {
                Id = newChildID.ToString(),
                ParentId = isSibling ? parentId : diagram.SelectionSettings.Nodes[0].ID,
                Fill = newchildColor,
                Branch = childType,
                Label = "New Child",
                Level = isSibling ? parentLevel + 1 : currentLevel + 1
            };
            diagram.BeginUpdate();
            await UpdatePortConnection(childNode, diagram, isSibling);
            await diagram.EndUpdate();
        }
        // Custom tool to add the node.
        public class AddLeftTool : InteractionControllerBase
        {
            SfDiagramComponent diagram;
            public AddLeftTool(SfDiagramComponent Diagram) : base(Diagram)
            {
                diagram = Diagram;
            }
            public override async void OnMouseDown(DiagramMouseEventArgs args)
            {
                await AddRightChild(diagram);
                diagram.ClearSelection();
                base.OnMouseDown(args);
                diagram.Select(new ObservableCollection<IDiagramObject>() { diagram.Nodes[diagram.Nodes.Count - 1] });
                diagram.StartTextEdit(diagram.Nodes[diagram.Nodes.Count - 1]);
                this.InAction = true;
            }
        }
        private static async Task UpdatePortConnection(MindMapDetails childNode, SfDiagramComponent diagram, bool isSibling)
        {
            Node node = new Node()
            {
                ID = "node" + childNode.Id,
                Height = 50,
                Width = 100,
                Annotations = new DiagramObjectCollection<ShapeAnnotation>()
            {
                new ShapeAnnotation()
                {
                    Content = childNode.Label,
                    Style=new TextStyle(){FontSize = 12,FontFamily="Segoe UI"},
                    Offset=new DiagramPoint(){X=0.5,Y=0.5}
                }
            },
                Style = new ShapeStyle() { Fill = childNode.Fill, StrokeColor = childNode.Fill },
                AdditionalInfo = new Dictionary<string, object>()
            {
                {"Orientation", childNode.Branch},
                {"ParentId", childNode.ParentId},
                {"Level", childNode.Level},
            }
            };
            Connector connector = new Connector()
            {
                TargetID = node.ID,
                SourceID = isSibling ? childNode.ParentId : diagram.SelectionSettings.Nodes[0].ID
            };
            await diagram.AddDiagramElements(new DiagramObjectCollection<NodeBase>() { node, connector });
            Node sourceNode = diagram.GetObject((connector as Connector).SourceID) as Node;
            Node targetNode = diagram.GetObject((connector as Connector).TargetID) as Node;
            if (targetNode != null && targetNode.AdditionalInfo.Count > 0)
            {
                BranchType nodeBranch = (BranchType)targetNode.AdditionalInfo["Orientation"];
                if (nodeBranch == BranchType.Right || nodeBranch == BranchType.SubRight)
                {
                    (connector as Connector).SourcePortID = sourceNode.Ports[0].ID;
                    (connector as Connector).TargetPortID = targetNode.Ports[1].ID;
                }
                else if (nodeBranch == BranchType.Left || nodeBranch == BranchType.SubLeft)
                {
                    (connector as Connector).SourcePortID = sourceNode.Ports[1].ID;
                    (connector as Connector).TargetPortID = targetNode.Ports[0].ID;
                }
            }
            await diagram.DoLayout();
        }
        public void ZoomTo(ZoomOptions options)
        {
            double factor = options.ZoomFactor != 0 ? options.ZoomFactor : 0.2;
            factor = options.Type == "ZoomOut" ? 1 / (1 + factor) : (1 + factor);
            Diagram.Zoom(factor, null);
        }

        /// <summary>
        /// This method is used to allows users to pan the diagram.
        /// </summary>
        public void UpdateTool()
        {
            interactionController = DiagramInteractions.ZoomPan;
            StateHasChanged();
        }
        /// <summary>
        /// This method is used to allows users to perform selection in the diagram.
        /// </summary>
        public void UpdatePointerTool()
        {
            interactionController = DiagramInteractions.SingleSelect;
            StateHasChanged();
        }

        /// <summary>
        /// Represents the zoom option in a diagram.
        /// </summary>
        public class ZoomOptions
        {
            public double ZoomFactor { get; set; }
            public string Type { get; set; }
        }

        public static async Task AddLeftChild(SfDiagramComponent diagram, bool isSibling = false)
        {
            string newChildID = RandomId();
            string newchildColor = ""; BranchType type = BranchType.Left; Node parentNode = null;
            string parentId = Convert.ToString(diagram.SelectionSettings.Nodes[0].AdditionalInfo["ParentId"]);
            BranchType nodeBranch = (BranchType)diagram.SelectionSettings.Nodes[0].AdditionalInfo["Orientation"];
            double currentLevel = Convert.ToDouble(diagram.SelectionSettings.Nodes[0].AdditionalInfo["Level"]);
            double parentLevel = 0;
            if (!string.IsNullOrEmpty(parentId))
            {
                parentNode = diagram.GetObject(parentId) as Node;
                BranchType parentNodeBranch = (BranchType)parentNode.AdditionalInfo["Orientation"];
                type = isSibling ? parentNodeBranch : nodeBranch;
            }
            else
            {
                type = nodeBranch;
            }

            BranchType childType = BranchType.Left;
            if (parentNode != null) parentLevel = Convert.ToDouble(parentNode.AdditionalInfo["Level"]);
            switch (type.ToString())
            {
                case "Root":
                    childType = BranchType.Right;
                    break;
                case "Right":
                    childType = BranchType.SubRight;
                    break;
                case "SubRight":
                    childType = BranchType.SubRight;
                    break;
            }
            double level = isSibling ? parentLevel : currentLevel;
            if (level == 0)
            {
                int index = Convert.ToInt32(GetFillColorIndex(level));
                newchildColor = fillColorCode[index];
            }
            else
            {
                newchildColor = diagram.SelectionSettings.Nodes[0].Style.Fill;
            }
            MindMapDetails childNode = new MindMapDetails()
            {
                Id = newChildID.ToString(),
                ParentId = isSibling ? parentId : diagram.SelectionSettings.Nodes[0].ID,
                Fill = newchildColor,
                Branch = childType,
                Label = "New Child",
                Level = isSibling ? parentLevel + 1 : currentLevel + 1
            };
            diagram.BeginUpdate();
            await UpdatePortConnection(childNode, diagram, isSibling);
            await diagram.EndUpdate();
        }
        // Custom tool to add the node.
        public class AddRightTool : InteractionControllerBase
        {
            SfDiagramComponent diagram;
            public AddRightTool(SfDiagramComponent Diagram) : base(Diagram)
            {
                diagram = Diagram;
            }
            public override async void OnMouseDown(DiagramMouseEventArgs args)
            {
                await AddLeftChild(diagram);
                diagram.ClearSelection();
                base.OnMouseDown(args);
                diagram.Select(new ObservableCollection<IDiagramObject>() { diagram.Nodes[diagram.Nodes.Count - 1] });
                diagram.StartTextEdit(diagram.Nodes[diagram.Nodes.Count - 1]);
                this.InAction = true;
            }
        }

        public class DeleteTool : InteractionControllerBase
        {
            SfDiagramComponent sfDiagram;
            Node deleteObject = null;
            public DeleteTool(SfDiagramComponent Diagram) : base(Diagram)
            {
                sfDiagram = Diagram;
            }
            public override void OnMouseDown(DiagramMouseEventArgs args)
            {
                deleteObject = (sfDiagram.SelectionSettings.Nodes[0]) as Node;
            }
            public override async void OnMouseUp(DiagramMouseEventArgs args)
            {
                if (deleteObject != null)
                {
                    sfDiagram.BeginUpdate();
                    RemoveData(deleteObject, sfDiagram);
                    _ = sfDiagram.EndUpdate();
                    await sfDiagram.DoLayout();
                }
                base.OnMouseUp(args);
                this.InAction = true;
            }
        }

        private static void RemoveData(Node node, SfDiagramComponent diagram)
        {
            if (node.OutEdges.Count > 0)
            {
                List<string> outEdges = new List<string>();
                node.OutEdges.ForEach(edges => outEdges.Add(edges));
                for (int i = 0; i < outEdges.Count; i++)
                {
                    Connector connector = diagram.GetObject(outEdges[i]) as Connector;
                    Node targetnode = diagram.GetObject(connector.TargetID) as Node;
                    if (targetnode.OutEdges.Count > 0)
                    {
                        RemoveData(targetnode, diagram);
                    }
                    else
                    {
                        diagram.Delete(new DiagramObjectCollection<NodeBase>() { targetnode });
                    }
                }
                diagram.Delete(new DiagramObjectCollection<NodeBase>() { node });
            }
            else
            {
                diagram.Delete(new DiagramObjectCollection<NodeBase>() { node });
            }
        }

        private void OnSelectionChanging(SelectionChangingEventArgs args)
        {
            if (args.NewValue.Count > 0)
            {
                if (args.NewValue[0] is Node && (args.NewValue[0] as Node).AdditionalInfo.Count > 0)
                {
                    BranchType type = (BranchType)((args.NewValue[0] as Node).AdditionalInfo["Orientation"]);
                    if (type == BranchType.Root)
                    {
                        selectionSettings.UserHandles[0].Visible = false;
                        selectionSettings.UserHandles[1].Visible = false;
                        selectionSettings.UserHandles[2].Visible = true;
                        selectionSettings.UserHandles[3].Visible = true;
                    }
                    else if (type == BranchType.Left || type == BranchType.SubLeft)
                    {
                        selectionSettings.UserHandles[0].Visible = false;
                        selectionSettings.UserHandles[1].Visible = true;
                        selectionSettings.UserHandles[2].Visible = true;
                        selectionSettings.UserHandles[3].Visible = false;
                    }
                    else if (type == BranchType.Right || type == BranchType.SubRight)
                    {
                        selectionSettings.UserHandles[0].Visible = true;
                        selectionSettings.UserHandles[1].Visible = false;
                        selectionSettings.UserHandles[2].Visible = false;
                        selectionSettings.UserHandles[3].Visible = true;
                    }
                }
            }
        }

        private void NodeCreating(IDiagramObject obj)
        {
            Node node = obj as Node;

            node.Height = 50;
            node.Width = 100;
            node.Shape = new BasicShape() { Type = shapes.Basic, Shape = NodeBasicShapes.Ellipse };
            PointPort port21 = new PointPort()
            {
                ID = "left",
                Offset = new DiagramPoint() { X = 0, Y = 0.5 },
                Height = 10,
                Width = 10,
            };

            PointPort port22 = new PointPort()
            {
                ID = "right",
                Offset = new DiagramPoint() { X = 1, Y = 0.5 },
                Height = 10,
                Width = 10,
            };

            node.Ports = new DiagramObjectCollection<PointPort>()
            {
                port21,port22
            };
            if (MenubarRef.IsJsonLoading)
            {
                if (node.AdditionalInfo["Level"] is JsonElement level)
                {
                    double levelString = level.GetDouble();
                    node.AdditionalInfo["Level"] = levelString;
                }
                if (node.AdditionalInfo["Orientation"] is JsonElement orientation)
                {
                    int orientationValue = orientation.GetInt32();
                    BranchType branch = (BranchType)orientationValue;
                    node.AdditionalInfo["Orientation"] = branch;
                }
                if (node.AdditionalInfo["ParentId"] is JsonElement parentId)
                {
                    string parent = parentId.GetString();
                    node.AdditionalInfo["ParentId"] = parent;
                }
            }
            node.Constraints &= ~NodeConstraints.Rotate;
        }

        private void ConnectorCreating(IDiagramObject obj)
        {
            Connector connector = obj as Connector;
            connector.Type = ConnectorSegmentType.Bezier;
            connector.BezierConnectorSettings = new BezierConnectorSettings() { AllowSegmentsReset = false };
            connector.Constraints = ConnectorConstraints.Default & ~ConnectorConstraints.Select;
            connector.Style = new ShapeStyle() { StrokeColor = "#4f4f4f", StrokeWidth = 1 };
            connector.TargetDecorator = new DecoratorSettings() { Shape = DecoratorShape.None };
            connector.SourceDecorator.Shape = DecoratorShape.None;
            Node sourceNode = Diagram.GetObject((connector as Connector).SourceID) as Node;
            Node targetNode = Diagram.GetObject((connector as Connector).TargetID) as Node;
            if (targetNode != null && targetNode.AdditionalInfo.Count > 0)
            {
                BranchType nodeBranch = (BranchType)targetNode.AdditionalInfo["Orientation"];
                if (nodeBranch == BranchType.Right || nodeBranch == BranchType.SubRight)
                {
                    (connector as Connector).SourcePortID = sourceNode.Ports[0].ID;
                    (connector as Connector).TargetPortID = targetNode.Ports[1].ID;
                }
                else if (nodeBranch == BranchType.Left || nodeBranch == BranchType.SubLeft)
                {
                    (connector as Connector).SourcePortID = sourceNode.Ports[1].ID;
                    (connector as Connector).TargetPortID = targetNode.Ports[0].ID;
                }
            }
        }
        public void UpdateNodeStyles(Node node, Node parentNode)
        {
            if (node != null && parentNode != null)
            {
                double levelValue = Convert.ToDouble(node.AdditionalInfo["Level"]);
                if (levelValue == 1)
                {
                    node.Style.Fill = fillColorCode[lastFillIndex];
                    node.Style.StrokeColor = borderColorCode[lastFillIndex];
                    node.Style.StrokeWidth = 2;
                    if (lastFillIndex + 1 >= fillColorCode.Length)
                        lastFillIndex = 0;
                    else
                        lastFillIndex++;
                }
                else
                    node.Style.StrokeColor = node.Style.Fill = parentNode.Style.Fill;
            }
        }

        private static double GetFillColorIndex(double level)
        {
            double index = lastFillIndex;
            if (lastFillIndex + 1 >= fillColorCode.Length)
                lastFillIndex = 0;
            else
                lastFillIndex++;
            return index;
        }
        private void UpdateHandle()
        {
            UserHandle deleteLeftHandle = AddHandle("DeleteRight", "delete", Direction.Right);

            UserHandle addRightHandle = AddHandle("AddLeft", "add", Direction.Left);

            UserHandle addLeftHandle = AddHandle("AddRight", "add", Direction.Right);

            UserHandle deleteRightHandle = AddHandle("DeleteLeft", "delete", Direction.Left);

            handles.Add(deleteLeftHandle);
            handles.Add(deleteRightHandle);
            handles.Add(addLeftHandle);
            handles.Add(addRightHandle);

            selectionSettings.UserHandles = handles;
        }

        private UserHandle AddHandle(string name, string path, Direction direction)
        {
            UserHandle handle = new UserHandle()
            {
                Name = name,
                Visible = true,
                Offset = 0.5,
                Side = direction,
                Margin = new DiagramThickness() { Top = 0, Bottom = 0, Left = 0, Right = 0 }
            };
            if (path == "delete")
            {
                handle.PathData = "M1.0000023,3 L7.0000024,3 7.0000024,8.75 C7.0000024,9.4399996 6.4400025,10 5.7500024,10 L2.2500024,10 C1.5600024,10 1.0000023,9.4399996 1.0000023,8.75 z M2.0699998,0 L5.9300004,0 6.3420029,0.99999994 8.0000001,0.99999994 8.0000001,2 0,2 0,0.99999994 1.6580048,0.99999994 z";
            }
            else
            {
                handle.PathData = "M4.0000001,0 L6,0 6,4.0000033 10,4.0000033 10,6.0000033 6,6.0000033 6,10 4.0000001,10 4.0000001,6.0000033 0,6.0000033 0,4.0000033 4.0000001,4.0000033 z";
            }
            return handle;
        }

        public class MindMapDetails
        {
            public string Id { get; set; }
            public string Label { get; set; }
            public string ParentId { get; set; }
            public BranchType Branch { get; set; }
            public string Fill { get; set; }
            public double Level { get; set; }
        }

        protected override void OnInitialized()
        {
            selectionSettings.Constraints &= ~(SelectorConstraints.ResizeAll | SelectorConstraints.Rotate);
            commands = new DiagramObjectCollection<KeyboardCommand>()
        {
            new KeyboardCommand() { Name = "showShortCut", Gesture = new KeyGesture(){ Key = DiagramKeys.F1, Modifiers = ModifierKeys.None } },
            new KeyboardCommand() { Name = "leftChild", Gesture = new KeyGesture(){ Key = DiagramKeys.Tab, Modifiers = ModifierKeys.Shift } },
            new KeyboardCommand() { Name = "rightChild", Gesture = new KeyGesture(){ Key = DiagramKeys.Tab,Modifiers = ModifierKeys.None } },
            new KeyboardCommand() { Name = "sibilingChildTop", Gesture = new KeyGesture(){ Key = DiagramKeys.Enter,Modifiers = ModifierKeys.None } },
            new KeyboardCommand() { Name = "fitPage", Gesture = new KeyGesture(){ Key = DiagramKeys.F8, Modifiers = ModifierKeys.None } },
            new KeyboardCommand() { Name = "duplicate", Gesture = new KeyGesture(){ Key = DiagramKeys.D, Modifiers = ModifierKeys.Control } },

            new KeyboardCommand() { Name = "navigationUp", Gesture = new KeyGesture(){ Key = DiagramKeys.ArrowUp, Modifiers = ModifierKeys.None } },
            new KeyboardCommand() { Name = "navigationDown", Gesture = new KeyGesture(){ Key = DiagramKeys.ArrowDown, Modifiers = ModifierKeys.None } },
            new KeyboardCommand() { Name = "navigationLeft", Gesture = new KeyGesture(){ Key = DiagramKeys.ArrowLeft, Modifiers = ModifierKeys.None } },
            new KeyboardCommand() { Name = "navigationRight", Gesture = new KeyGesture(){ Key = DiagramKeys.ArrowRight,Modifiers = ModifierKeys.None } },

            new KeyboardCommand() { Name = "fileNew", Gesture = new KeyGesture(){ Key = DiagramKeys.N, Modifiers = ModifierKeys.Shift } },
            new KeyboardCommand() { Name = "fileOpen", Gesture = new KeyGesture(){ Key = DiagramKeys.O, Modifiers = ModifierKeys.Control } },
            new KeyboardCommand() { Name = "fileSave", Gesture = new KeyGesture(){ Key = DiagramKeys.S, Modifiers = ModifierKeys.Control } },
            new KeyboardCommand() { Name = "delete", Gesture = new KeyGesture(){ Key = DiagramKeys.Delete, Modifiers = ModifierKeys.None  } },
            new KeyboardCommand() { Name = "backspace", Gesture = new KeyGesture(){ Key = DiagramKeys.BackSpace, Modifiers = ModifierKeys.None  } },
        };
            MindMapDetails rootNodeData = MindmapData[0];
            CreateNode(rootNodeData.Id, rootNodeData.ParentId, rootNodeData.Label, rootNodeData.Fill, rootNodeData.Branch, rootNodeData.Level);
            for (int i = 1; i < MindmapData.Count; i++)
            {
                MindMapDetails nodeData = MindmapData[i];
                string sourcePortID = string.Empty;
                string targetPortID = string.Empty;
                CreateNode(nodeData.Id, nodeData.ParentId, nodeData.Label, nodeData.Fill, nodeData.Branch, nodeData.Level);
                if (nodeData.Branch == BranchType.Right || nodeData.Branch == BranchType.SubRight)
                {
                    sourcePortID = "left";
                    targetPortID = "right";
                }
                else if (nodeData.Branch == BranchType.Left || nodeData.Branch == BranchType.SubLeft)
                {
                    sourcePortID = "right";
                    targetPortID = "left";
                }
                Connector connector = new Connector() { ID = "connector" + i, SourceID = nodeData.ParentId, TargetID = nodeData.Id, SourcePortID = sourcePortID, TargetPortID = targetPortID };
                connectors.Add(connector);
            }
        }

        private void CreateNode(string id, string parentId, string annotationContent, string fillColor, BranchType branch, double level)
        {
            Node node = new Node()
            {
                ID = id,
                Annotations = new DiagramObjectCollection<ShapeAnnotation>()
            {
                new ShapeAnnotation()
                {
                    Content = annotationContent,
                    Style=new TextStyle(){FontSize = 12,FontFamily="Segoe UI"},
                    Offset=new DiagramPoint(){X=0.5,Y=0.5}, Width = 80
                }
            },
                Style = new ShapeStyle() { Fill = fillColor, StrokeColor = fillColor },
                AdditionalInfo = new Dictionary<string, object>()
            {
                {"Orientation", branch},
                {"ParentId", parentId},
                {"Level", level},
            }
            };
            nodes.Add(node);
        }
        internal static string RandomId()
        {
            Random random = new Random();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXTZabcdefghiklmnopqrstuvwxyz";
            return new string(Enumerable.Repeat(chars, 5)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);
            if (firstRender)
            {
                if (MenubarRef != null)
                    MenubarRef.Parent = this;
                if (Toolbar != null)
                    Toolbar.Parent = this;
                if (DiagramOpenAIRef != null)
                    DiagramOpenAIRef.Parent = this;
                if (DiagramShortCutKeyRef != null)
                    DiagramShortCutKeyRef.Parent = this;
                UpdateHandle();
            }
        }
    }
}
