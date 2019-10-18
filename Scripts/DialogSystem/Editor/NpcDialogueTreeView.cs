using Character.NPC;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace Character.DialogueSystem.Editor
{
    /// <summary> The TreeView wich displays the NPC wich the player can talk to and all of there dialogues. </summary>
    public class NpcDialogueTreeView : TreeView
    {
        /// <summary> reference of the dialogue window wich contains this tree view. </summary>
        DialogueWindow dialogueWindow;
        /// <summary> The Storage wich contains all dialogues of all npc wich the player can talk with. </summary>
        DialogueStorage storage;
        /// <summary> List of all NpcDialogue classes that are inside of the 'storage'. </summary>
        List<NpcDialogue> npcDialogues;
        /// <summary> The id of the rows that are added to the tree view. </summary>
        int id = 1;

        /// <summary> Initialize the tree view and give him the storage of all dialogues, the window, the treeViewState of the tree view and the cultiComlumnHeader of the tree view. </summary>
        /// <param name="treeViewState"> The TreeViewState of the tree view. </param>
        /// <param name="multiColumnHeader"> The multiColumnHeader that arragne the tree view. </param>
        /// <param name="_storage"> The Storage of all dialogue that are visible inside the tree view. </param>
        /// <param name="window"> The window wich contains the tree view. </param>
        public NpcDialogueTreeView (TreeViewState treeViewState, MultiColumnHeader multiColumnHeader, DialogueStorage _storage, DialogueWindow window) : base(treeViewState, multiColumnHeader)
        {
            storage = _storage;
            dialogueWindow = window;

            //Set the height of a row
            rowHeight = 20;
            //Set the visibility of the border to true
            showBorder = true;
            customFoldoutYOffset = (20 - EditorGUIUtility.singleLineHeight) * 0.5f;
            //Let the multi column header resize to the size of the tree view.
            multiColumnHeader.ResizeToFit();

            //Reloads the tree view so that the changes are visible.
            Reload();
        }

        #region Build Rows/Root

        /// <summary> Builds the root row of the tree view. </summary>
        /// <returns> Rettuns the root item of the tree view. </returns>
        protected override TreeViewItem BuildRoot()
        {
            //Returns an new TreeViewItem wich has an id of 0 and a depth of - 1 so it isn't visible and alle other items inside the tree view can have a minimum depth of 0 and are so a child item of the root item.
            return new TreeViewItem { id = 0, depth = -1 };
        }

        /// <summary> Builds the rows for all items that should be inside of the tree view. </summary>
        /// <param name="root"> The root item of the tree view. </param>
        /// <returns> Retuns a IList of all rows that are inside the tree view. </returns>
        protected override IList<TreeViewItem> BuildRows(TreeViewItem root)
        {
            //Set the rows to the current rows if it's not null, otherwise create a new list of TreeViewItems.
            var rows = GetRows() ?? new List<TreeViewItem>(200);
            //Set the id to 1 because the root item has the id 0.
            id = 1;
            //Calls the method to add the items to the tree view.
            DisplayRows(root, rows);
            return rows;
        }

        /// <summary> Displays all NpcDialogues of the storage in the tree view and his dialogues. </summary>
        /// <param name="root"> The root item of the tree view. </param>
        /// <param name="rows"> List of all item in the tree view. </param>
        void DisplayRows(TreeViewItem root, IList<TreeViewItem> rows)
        {
            //Clears the list with all items.
            rows.Clear();
            //Set the list of all NpcDialogues that hast to be in the tree view.
            npcDialogues = new List<NpcDialogue>(storage.DialogueList);
            //Create an index for the npc wich is the item representing.
            int npcInd = 0;

            //Loops through all 
            for (int i = 0; i < npcDialogues.Count; i++)
            {
                //Create a new DialoguePartnerElement item wich stores the informations of the NpcDialouge and will be shown in the tree view.
                var element = new DialoguePartnerElement(id, 0, npcDialogues[i].Name, npcDialogues[i].ScriptableNpc, new List<DialogueElement>(npcDialogues[i].Dialogues.Length), npcInd);
                //Increase the id and the npcInd so no 'element' item has the same id / npcInd.
                id++;
                npcInd++;

                //Add the 'element' item as child to the root item and to the rows.
                root.AddChild(element);
                rows.Add(element);

                //Checks if the NpcDialogue has dialogues.
                if(npcDialogues[i].Dialogues.Length > 0)
                {
                    //Create a index for the dialogue the item will represent.
                    int dialogueInd = 0;
                    //Loops through all dialogues of and NpcDialogue.
                    for (int a = 0; a < npcDialogues[i].Dialogues.Length; a++)
                    {
                        //Create a new DialogueElement item wich stores all informations of the Dialogue that will be added to the 'element' item as child.
                        var childItem = new DialogueElement(id, 1, "Dialogue " + (a + 1), npcDialogues[i].Dialogues[a].DialogueDescription, npcDialogues[i].Dialogues[a].DialogueLines.Length, dialogueInd);
                        //Increase the id and the dialogueInd so no 'childItem' item has the same id / dialogueInd.
                        id++;
                        dialogueInd++;
                        //Set the parent item of the 'childItem' item to the 'element' item and adds it to the list of all DialogueElements of the 'element' item.
                        childItem.ParentElement = element;
                        element.dialogueElements.Add(childItem);
                    }

                    //Check if the 'element' item is expanded and if it is the add the children to the rows, otherwise set the children of the 'element' to a dummy list created by 'CreateChildListForCollapsedParent'
                    if (IsExpanded(element.id))
                        AddChildren(npcDialogues, element, rows);
                    else
                        element.children = CreateChildListForCollapsedParent();
                }
            }
            SetupParentsAndChildrenFromDepths(root, rows);
        }

        /// <summary> Adds the child item to an DialoguePartnerElemnt inside the tree view. </summary>
        /// <param name="npcDialogues"> List of all NpcDialogues. </param>
        /// <param name="item"> The DialoguePartnerElement item that where the child items should be added. </param>
        /// <param name="rows"> List of all rows that are inside the tree view. </param>
        void AddChildren(List<NpcDialogue> npcDialogues, DialoguePartnerElement item, IList<TreeViewItem>rows)
        {
            int childCount = npcDialogues[item.npcInd].Dialogues.Length;
            item.children = new List<TreeViewItem>(childCount);

            for(int i = 0; i < childCount; ++i)
            {
                item.AddChild(item.dialogueElements[i]);
                rows.Add(item.dialogueElements[i]);
            }
        }

        protected override IList<int> GetAncestors(int id)
        {
            var item = FindItem(id, rootItem);
            List<int> ancestors = new List<int>();

            while(item.parent != null)
            {
                ancestors.Add(item.parent.id);
                item = item.parent;
            }
            return ancestors;
        }

        protected override IList<int> GetDescendantsThatHaveChildren(int id)
        {
            Stack<TreeViewItem> stack = new Stack<TreeViewItem>();

            var start = FindItem(id, rootItem);
            stack.Push(start);

            var parents = new List<int>();
            while (stack.Count > 0)
            {
                TreeViewItem current = stack.Pop();
                parents.Add(current.id);

                for (int i = 0; i < current.children.Count; i++)
                {
                    if (current.children.Count > 0)
                        stack.Push(current.children[i]);
                }
            }
            return parents;
        }

        public override void OnGUI(Rect rect)
        {
            // Draws the Background of the tree view.
            if (Event.current.type == EventType.Repaint)
                DefaultStyles.backgroundOdd.Draw(rect, false, false, false, false);

            // Draws the TreeView.
            base.OnGUI(rect);
        }

        #endregion

        #region Rows/Cells GUI

        /// <summary> Methods that overrides how the rows in the tree view should look like. Called one time for every tree view item. </summary>
        /// <param name="args"> the arguments of the row that is called. </param>
        protected override void RowGUI(RowGUIArgs args)
        {
            //Loops through all Header Columns that are visible.
            for (int i = 0; i < args.GetNumVisibleColumns(); ++i)
            {
                //Check if the item of the row is a DialoguePartnerElement or a DialogueElement
                if (args.item.GetType() == typeof(DialoguePartnerElement))
                {
                    //Creates a DialogueParterElement wich stores the item of the row and call the 'DialoguePartnerCellGUI' method to draw anything inside the row that's needed.
                    var partnerItem = (DialoguePartnerElement)args.item;
                    DialoguePartnerCellGUI(args.GetCellRect(i), partnerItem, (MyColumns)args.GetColumn(i));
                }
                else if (args.item.GetType() == typeof(DialogueElement))
                {
                    //Creates a DialogueElement wich stores the item of the row and call the 'DialogueElementCellGUI' method to draw anything inside the row that's needed. 
                    var dialogueItem = (DialogueElement)args.item;
                    DialogueElementCellGUI(args.GetCellRect(i), dialogueItem, (MyColumns)args.GetColumn(i));
                }
            }
        }

        /// <summary> Sets the UI of a cell inside of a row for a DialoguePartnerElement. </summary>
        /// <param name="cellRect"> The rect of the cell. </param>
        /// <param name="item"> The DialoguePartnerElement item inside of the row. </param>
        /// <param name="column"> The column in wich the UI will create. </param>
        void DialoguePartnerCellGUI(Rect cellRect, DialoguePartnerElement item, MyColumns column)
        {
            CenterRectUsingSingleLineHeight(ref cellRect);

            var dialogueList = new List<Dialogue>(storage.DialogueList[item.npcInd].Dialogues);

            switch (column)
            {
                case MyColumns.Name:
                    {
                        if (!item.rename)
                            GUI.Label(new Rect(cellRect.x + (foldoutWidth * 1.1f), cellRect.y, cellRect.width - (foldoutWidth * 1.1f), cellRect.height), item.Name);
                    }
                    break;
                case MyColumns.ScriptableNpc:
                    {
                        if (item.ScriptableNpc == null)
                        {
                            item.ScriptableNpc = (SpecialScriptableNpc)EditorGUI.ObjectField(new Rect(cellRect.x, cellRect.y, cellRect.width * .5f, cellRect.height), item.ScriptableNpc, typeof(SpecialScriptableNpc), false);

                            if (GUI.Button(new Rect(cellRect.x + (cellRect.width * .5f), cellRect.y, cellRect.width * .5f, cellRect.height), new GUIContent("Create/Find Scriptable Npc", "Click to find the 'SpecialScripableNpc' that represents the npc ingame. If there's no 'SpecialScripableNpc', a new one will be created.")))
                            {
                                SpecialScriptableNpc npc;

                                if (item.Name != "")
                                {
                                    npc = DialogueWindow.CreateScriptableNpc(item.Name);
                                    Debug.Log(npc);
                                }
                                else
                                    npc = DialogueWindow.CreateScriptableNpc("new NPC" + item.npcInd.ToString());

                                item.ScriptableNpc = npc;
                                storage.DialogueList[item.npcInd].ScriptableNpc = item.ScriptableNpc;
                            }
                        }
                        else
                            item.ScriptableNpc = (SpecialScriptableNpc)EditorGUI.ObjectField(cellRect, item.ScriptableNpc, typeof(SpecialScriptableNpc), false);
                    }
                    break;
                case MyColumns.DialogueCount:
                    {
                        if (dialogueList.Count > 0)
                        {
                            if (GUI.Button(new Rect(cellRect.x + (cellRect.width * .1f), cellRect.y, cellRect.width * .2f, cellRect.height), new GUIContent("-", "Click to decrease the number of dialogues that the npc have.")))
                            {
                                dialogueList.RemoveAt(dialogueList.Count - 1);
                                storage.DialogueList[item.npcInd].Dialogues = dialogueList.ToArray();
                                Reload();
                                break;
                            }
                        }

                        GUI.Label(new Rect((cellRect.x  - 5)+ (cellRect.width * .5f), cellRect.y, cellRect.width * .25f, cellRect.height), dialogueList.Count.ToString());

                        if (GUI.Button(new Rect(cellRect.x + (cellRect.width * .7f), cellRect.y, cellRect.width * .2f, cellRect.height), new GUIContent("+", "Click to increase the number of dialogues that the npc have.")))
                        {
                            dialogueList.Add(new Dialogue("This is a Dialogue", 0));                            
                            storage.DialogueList[item.npcInd].Dialogues = dialogueList.ToArray();
                            Reload();
                            break;
                        }
                    }
                    break;
                case MyColumns.Remove:
                    {
                        if (GUI.Button(new Rect(cellRect.x + (cellRect.width * .1f), cellRect.y, cellRect.width * .8f, cellRect.height), new GUIContent("X", "Click to remove the dialogue partner and all dialouges of him.")))
                        {
                            RemoveDialoguePartner(item.npcInd);
                            Reload();
                            break;
                        }
                    }
                    break;
            }
        }

        /// <summary> Sets the UI of a cell inside of a row for a DialogueElement. </summary>
        /// <param name="cellRect"> The rect of the cell. </param>
        /// <param name="item"> The DialoguePartnerElement item inside of the row. </param>
        /// <param name="column"> The column in wich the UI will create. </param>
        void DialogueElementCellGUI(Rect cellRect, DialogueElement item, MyColumns column)
        {
            CenterRectUsingSingleLineHeight(ref cellRect);
            var lineList = new List<DialogueLine>(storage.DialogueList[item.ParentElement.npcInd].Dialogues[item.dialogueInd].DialogueLines);

            switch (column)
            {
                case MyColumns.Name:
                    {
                        Rect rect = new Rect((cellRect.x + 15) + (cellRect.width * .1f), cellRect.y, (cellRect.width - 15) - (cellRect.width * .1f), cellRect.height);
                        EditorGUI.LabelField(rect, item.Name);
                    }
                    break;
                case MyColumns.ScriptableNpc:
                    {
                        if (!item.rename)
                        {
                            Rect rect = new Rect(cellRect.x + 5, cellRect.y, cellRect.width, cellRect.height);
                            EditorGUI.LabelField(rect, item.displayName);
                        }
                    }
                    break;
                case MyColumns.DialogueCount:
                    {
                        if (item.lineElements.Capacity > 0)
                        {
                            if (GUI.Button(new Rect(cellRect.x + (cellRect.width * .1f), cellRect.y, cellRect.width * .2f, cellRect.height), new GUIContent("-", "Click to decrease the number of lines the dialogue have.")))
                            {
                                lineList.RemoveAt(lineList.Count - 1);
                                storage.DialogueList[item.ParentElement.npcInd].Dialogues[item.dialogueInd].DialogueLines = lineList.ToArray();

                                dialogueWindow.InitDialogueTreeView();

                                Reload();
                                break;
                            }
                        }

                        GUI.Label(new Rect((cellRect.x - 5) + (cellRect.width * .5f), cellRect.y, cellRect.width * .25f, cellRect.height), item.lineElements.Capacity.ToString());

                        if (GUI.Button(new Rect(cellRect.x + (cellRect.width * .7f), cellRect.y, cellRect.width * .2f, cellRect.height), new GUIContent("+", "Click to increase the number of lines that the dialogue have.")))
                        {
                            lineList.Add(new DialogueLine(new SentenceClass(), 0));
                            storage.DialogueList[item.ParentElement.npcInd].Dialogues[item.dialogueInd].DialogueLines = lineList.ToArray();

                            dialogueWindow.InitDialogueTreeView();

                            Reload();
                            break;
                        }
                    }
                    break;
                case MyColumns.Remove:
                    {
                        if (GUI.Button(new Rect(cellRect.x + (cellRect.width * .1f), cellRect.y, cellRect.width * .8f, cellRect.height), new GUIContent("X", "Click to remove the dialogue.")))
                        {
                            List<Dialogue> dialogues = new List<Dialogue>(storage.DialogueList[item.ParentElement.npcInd].Dialogues);
                            dialogues.RemoveAt(item.dialogueInd);
                            storage.DialogueList[item.ParentElement.npcInd].Dialogues = dialogues.ToArray();
                            Reload();
                        }
                    }
                    break;
            }
        }

        #endregion

        #region HandleClicks

        /// <summary> Method that will be called when an item was double clicked. </summary>
        /// <param name="id"> The ID of the item that was double clicked </param>
        protected override void DoubleClickedItem(int id)
        {
            TreeViewItem item = FindItem(GetSelection()[0], rootItem);

            if (item.GetType() == typeof(DialoguePartnerElement))
            {
                DialoguePartnerElement element = (DialoguePartnerElement)item;
                element.rename = true;
                BeginRename(element);
            }
            else if(item.GetType() == typeof(DialogueElement))
            {
                DialogueElement element = (DialogueElement)item;
                element.rename = true;
                BeginRename(element);
            }
        }

        /// <summary> Method that will be called when the selection of items has changed. </summary>
        /// <param name="selectedIds"> List of the item that are selected. </param>
        protected override void SelectionChanged(IList<int> selectedIds)
        {
            if (FindItem(selectedIds[0], rootItem).GetType() == typeof(DialogueElement))
            {
                DialogueElement item = (DialogueElement)FindItem(selectedIds[0], rootItem);
                dialogueWindow.npcInd = item.ParentElement.npcInd;
                dialogueWindow.dialogueInd = item.dialogueInd;
                dialogueWindow.dialogueSelected = true;
                dialogueWindow.InitDialogueTreeView();
            }
            else
            {

                dialogueWindow.dialogueSelected = false;
            }
        }

        #endregion

        #region Rename

        protected override bool CanRename(TreeViewItem item)
        {
            Rect renameRect = GetRenameRect(treeViewRect, 0, item);
            return renameRect.width > 30;
        }

        protected override void RenameEnded(RenameEndedArgs args)
        {
            if (args.acceptedRename)
            {
                TreeViewItem item = FindItem(GetSelection()[0], rootItem);

                if (item.GetType() == typeof(DialoguePartnerElement))
                {
                    DialoguePartnerElement element = (DialoguePartnerElement)item;
                    element.rename = false;
                    storage.DialogueList[element.npcInd].Name = args.newName;
                    BuildRows(rootItem);
                }
                else if(item.GetType() == typeof(DialogueElement))
                {
                    DialogueElement element = (DialogueElement)item;
                    element.rename = false;
                    storage.DialogueList[element.ParentElement.npcInd].Dialogues[element.dialogueInd].DialogueDescription = args.newName;
                    BuildRows(rootItem);
                }
            }
        }

        protected override Rect GetRenameRect(Rect rowRect, int row, TreeViewItem item)
        {
            Rect cellRect = rowRect;

            if(item.GetType() == typeof(DialoguePartnerElement))
            {
                cellRect = GetCellRectForTreeFoldouts(rowRect);
                cellRect.x += foldoutWidth;
                cellRect.width -= foldoutWidth;
            }
            else if(item.GetType() == typeof(DialogueElement))
            {
                cellRect = new Rect(multiColumnHeader.GetColumnRect(1).x + 2 , rowRect.y, multiColumnHeader.GetColumnRect(1).width - 2, rowHeight);
            }
            CenterRectUsingSingleLineHeight(ref cellRect);

            return cellRect;
        }

        #endregion

        #region ColumnHeader

        /// <summary> Creates the Multi Column Header of the tree view. </summary>
        /// <param name="treeViewWith"> the width og the tree view. </param>
        /// <returns></returns>
        public static MultiColumnHeaderState CreateMultiColumnHeaderState(float treeViewWith)
        {
            //Array of all Columns inside of the Multi Column Header.
            var columns = new[]
            {
                //Column for the name of an NPC or dialogue.
                new MultiColumnHeaderState.Column
                {
                    headerContent = new GUIContent("Name", "The Name of the NPC."),
                    headerTextAlignment = TextAlignment.Left,
                    width = 60,
                    minWidth = 40,
                    autoResize = true,
                    allowToggleVisibility = false,
                    canSort = false
                },  //Column for the scriptableNpc of the NpcDialogue or description of the dialogue.
                new MultiColumnHeaderState.Column
                {
                    headerContent = new GUIContent("Scriptable NPC | Description", "The Scriptable NPC Object to find the right NPC. The Description of the Dialogue"),
                    headerTextAlignment = TextAlignment.Center,
                    sortedAscending = true,
                    width = 110,
                    minWidth = 94,
                    autoResize = true,
                    allowToggleVisibility = false,
                    canSort = false,
                },  //Column for the size of dialogues of an NpcDialogue or the size of line a diagoue has.
                new MultiColumnHeaderState.Column
                {
                    headerContent = new GUIContent("Dialogue/Line count", "The Number of Dialogue that a NPC have."),
                    headerTextAlignment = TextAlignment.Center,
                    sortedAscending = false,
                    width = 110,
                    minWidth = 95,
                    maxWidth = 140,
                    autoResize = true,
                    canSort = false,
                    allowToggleVisibility = false
                },  //Coloumn for the delete button
                new MultiColumnHeaderState.Column
                {
                    headerContent = new GUIContent("X", "Removes this Dialoge Element."),
                    headerTextAlignment = TextAlignment.Center,
                    sortedAscending = false,
                    width = 30,
                    minWidth = 27.5f,
                    maxWidth = 30,
                    autoResize = true,
                    canSort = false,
                    allowToggleVisibility = false
                }
            };

            var state = new MultiColumnHeaderState(columns);
            return state;
        }

        /// <summary> Enum of all Columns  </summary>
        enum MyColumns
        {
            /// <summary> Column for the name of an NPC or dialogue. </summary>
            Name,
            /// <summary> Column for the scriptableNpc of the NpcDialogue or description of the dialogue. </summary>
            ScriptableNpc,
            /// <summary> Column for the size of dialogues of an NpcDialogue or the size of line a diagoue has. </summary>
            DialogueCount,
            /// <summary> Coloumn for the delete button </summary>
            Remove
        }

        #endregion

        /// <summary> Use to get the storage of all NpcDialogues and his dialogues, but not the lines of the dialogues. </summary>
        /// <returns> Returns the DialogueStorage. </returns>
        public DialogueStorage GetStorage()
        {
            return storage;
        }

        /// <summary> Removes an NpcDialogue from the strorage. </summary>
        /// <param name="ind"> The index of the NpcDialogue that should be removed. </param>
        void RemoveDialoguePartner(int ind)
        {
            //Convert the NpcDialogues array from the storage into a list.
            List<NpcDialogue> npcs = new List<NpcDialogue>(storage.DialogueList);
            //Remove the NpcDialogue at the right index.
            npcs.RemoveAt(ind);
            //Set the NpcDialogues array from the storage to the list.
            storage.DialogueList = npcs.ToArray();
        }

        /// <summary> If the user is able to select multiple items at once. </summary>
        /// <param name="item"> The item that will be set by this method. </param>
        /// <returns> Returns false </returns>
        protected override bool CanMultiSelect(TreeViewItem item)
        {
            //Returns false because the user should be able to just select one item at one.
            return false;
        }
    }
}
