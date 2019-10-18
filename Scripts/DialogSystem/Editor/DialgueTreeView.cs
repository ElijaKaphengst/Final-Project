using System.Collections.Generic;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace Character.DialogueSystem.Editor
{
    /// <summary> The TreeView wich displays a complete dialogue of an NPC. </summary>
    public class DialogueTreeView : TreeView
    {
        /// <summary> Stores GUIStyles for the rows. </summary>
        static class Styles
        {
            /// <summary> Style for the background. </summary>
            public static GUIStyle background = "RL Background";
        }

        /// <summary> reference of the dialogue window wich contains this tree view. </summary>
        DialogueWindow dialogueWindow;
        /// <summary> GUIStyle for a label that has its anchor set to the right site. </summary>
        GUIStyle rightAnchoredLabel = new GUIStyle(GUIStyle.none);
        /// <summary> Reference of the dialogue that is currently shown in the tree view. </summary>
        Dialogue dialogue;
        /// <summary> The id of the rows that are added to the tree view. </summary>
        int id = 1;

        /// <summary> Initialize the tree view and give him the dialouge, window and treeViewState of the tree view. </summary>
        /// <param name="treeViewState"> The TreeViewState of the tree view. </param>
        /// <param name="_dialogue"> The dialogue that should be displayed inside the tree view. </param>
        /// <param name="window"> The window wich contains the tree view. </param>
        public DialogueTreeView(TreeViewState treeViewState, Dialogue _dialogue, DialogueWindow window) : base (treeViewState)
        {
            dialogueWindow = window;
            dialogue = _dialogue;

            //Let the tree view shows a border around it and set the offset of the foldouts.
            showBorder = true;
            customFoldoutYOffset = 3f;

            //Reloads the tree view so that the changes are visible.
            Reload();
        }

        /// <summary> Sets the height of the a rows. </summary>
        /// <param name="row"> the id of the row. </param>
        /// <param name="item"> Item that's inside if the row. </param>
        /// <returns></returns>
        protected override float GetCustomRowHeight(int row, TreeViewItem item)
        {
            return 95f;
        }

        #region Build Root/Rows

        /// <summary> Builds the root row of the tree view. </summary>
        /// <returns> Rettuns the root item of the tree view. </returns>
        protected override TreeViewItem BuildRoot()
        {
            //Set the right anchored label GUIStyle.
            rightAnchoredLabel.alignment = TextAnchor.MiddleRight;
            //Returns an new TreeViewItem wich has an id of 0 and a depth of - 1 so it isn't visible and alle other items inside the tree view can have a minimum depth of 0 and are so a child item of the root item.
            return new TreeViewItem { id = 0, depth = -1, };
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

        /// <summary> Display all Lines of the current selected dialogue. </summary>
        /// <param name="root"> The root item of the tree view. </param>
        /// <param name="rows"> List of all item in the tree view. </param>
        void DisplayRows(TreeViewItem root, IList<TreeViewItem> rows)
        {
            //Clears the list with all items.
            rows.Clear();
            int lineInd = 0;

            for (int i = 0; i < dialogue.DialogueLines.Length; i++)
            {
                var sentence = new SentenceElement(dialogue.DialogueLines[i].Sentence.Text, (TriggeredDialogueOption)dialogue.DialogueLines[i].Sentence.TriggeredDialogueOption, dialogue.DialogueLines[i].Sentence.SkipedLines);
                var bgColor = new Color(dialogue.DialogueLines[i].backgroundColor[0], dialogue.DialogueLines[i].backgroundColor[1], dialogue.DialogueLines[i].backgroundColor[2], dialogue.DialogueLines[i].backgroundColor[3]);

                var element = new DialogueLineElement(id, 0, "Line " + (lineInd + 1), lineInd, sentence, new List<AnswerElement>(dialogue.DialogueLines[i].Answers.Length), (Narrator)dialogue.DialogueLines[i].Narrator, bgColor);
                id++;

                root.AddChild(element);
                rows.Add(element);

                if(dialogue.DialogueLines[i].Answers.Length > 0)
                {
                    for (int a = 0; a < dialogue.DialogueLines[i].Answers.Length; a++)
                    {
                        var bgAnswerColor = new Color(dialogue.DialogueLines[i].Answers[a].backgroundColor[0], dialogue.DialogueLines[i].Answers[a].backgroundColor[1], dialogue.DialogueLines[i].Answers[a].backgroundColor[2], dialogue.DialogueLines[i].Answers[a].backgroundColor[3]);

                        var childItem = new AnswerElement(id, 1, "Answer " + (a + 1), dialogue.DialogueLines[lineInd].Answers[a].Text, dialogue.DialogueLines[lineInd].Answers[a].SkipedLines, a, (TriggeredDialogueOption)dialogue.DialogueLines[lineInd].Answers[a].TriggeredDialogueOption, dialogue.DialogueLines[lineInd].Answers[a].Description, bgAnswerColor);
                        id++;
                        element.answerElements.Add(childItem);
                        childItem.parentElement = element;
                    }

                    if (IsExpanded(element.id))
                        AddAnswerChild(element, rows);
                    else
                        element.children = CreateChildListForCollapsedParent();
                }
                lineInd++;
            }

            SetupParentsAndChildrenFromDepths(root, rows);
        }

        /// <summary> Adds the child items from an DialogueLineElement item to it. </summary>
        /// <param name="item"> The Item where the children should be added. </param>
        /// <param name="rows"> List of all items that are currently in the tree view. </param>
        void AddAnswerChild(DialogueLineElement item, IList<TreeViewItem> rows)
        {
            int childCount = dialogue.DialogueLines[item.lineInd].Answers.Length;
            item.children = new List<TreeViewItem>(childCount);

            for (int i = 0; i < childCount; ++i)
            {
                item.AddChild(item.answerElements[i]);
                rows.Add(item.answerElements[i]);
            }
        }

        protected override IList<int> GetAncestors(int id)
        {
            var item = FindItem(id, rootItem);
            List<int> ancestors = new List<int>();

            while (item.parent != null)
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
            //Check the type of the tree view item inside of the row.
            if(args.item.GetType() == typeof(DialogueLineElement))
            {
                //If it's a DialogueLineElement store it in a new variable and call the 'DisplayLineRow' method.
                var item = (DialogueLineElement)args.item;
                DisplayLineRow(item, args.rowRect);
            }
            else if(args.item.GetType() == typeof(AnswerElement))
            {
                //If it's a AnwerElement store it in a new variable and call the 'DisplayAnswerRow' method.
                var item = (AnswerElement)args.item;
                DisplayAnswerRow(item, args.rowRect);
            }
        }

        #region DisplayLine

        /// <summary> Draws the Line Row </summary>
        /// <param name="item"></param>
        /// <param name="rowRect"></param>
        void DisplayLineRow(DialogueLineElement item, Rect rowRect)
        {
            var contentIndent = GetContentIndent(item);

            //Draw Brackground
            var bgRect = rowRect;
            bgRect.x = contentIndent;
            bgRect.width = Mathf.Max(rowRect.width - contentIndent) - 5f;
            bgRect.yMin += 2f;
            bgRect.yMax -= 2f;
            DrawItemBackground(bgRect, item.backgroundColor);
            //Draw Row
            var controllRect = bgRect;
            controllRect.xMin += 5f;
            controllRect.xMax += 5f;
            DrawLineGUI(item, controllRect);
        }

        /// <summary> Set's the GUI for the line GUI </summary>
        /// <param name="item"></param>
        /// <param name="rowRect"></param>
        void DrawLineGUI(DialogueLineElement item, Rect rowRect)
        {
            //!! Always set the rect where GUI element should drawn befor it get's drawn.
            //Sets the rect of an normal line.
            var rect = rowRect;
            rect.y += 3f;
            rect.height = EditorGUIUtility.singleLineHeight;

            //Sets the rect of the first line.
            var lineRect = rect;

            //Draw the label of the line.
            lineRect.width = 42;
            EditorGUI.LabelField(rect, item.displayName, EditorStyles.boldLabel);

            //Draw a label that says sentence.
            lineRect.x = lineRect.xMax;
            lineRect.width = 47;
            EditorGUI.LabelField(lineRect, "Sentece", EditorStyles.miniBoldLabel);

            //Draw a label for the Narrator enum popup.
            lineRect.x = lineRect.xMax + 10;
            lineRect.width = 52;
            EditorGUI.LabelField(lineRect, "Narrator:");

            //Draw an enum popup for the Narractor and save it to the dialgoue.
            lineRect.x = lineRect.xMax + 2;
            item.narrator = (Narrator)EditorGUI.EnumPopup(lineRect, new GUIContent("", "The Narrator of the Line"), item.narrator);
            dialogue.DialogueLines[item.lineInd].Narrator = (int)item.narrator;

            //Draw am  enum popup for the triggered dialogue option and save it to the dialogue.
            lineRect.x = rect.width * .83f;
            lineRect.xMax = rect.xMax - 10;
            item.sentenceElement.dialogueOption = (TriggeredDialogueOption)EditorGUI.EnumPopup(lineRect, item.sentenceElement.dialogueOption);
            dialogue.DialogueLines[item.lineInd].Sentence.TriggeredDialogueOption = (int)item.sentenceElement.dialogueOption;

            //Draw the label for the triggered dialogue option enum popup in the front of it
            lineRect.xMax = lineRect.xMin;
            lineRect.xMin = rect.x;
            EditorGUI.LabelField(lineRect, "Triggered Dialogue Option:", rightAnchoredLabel);

            //Sets the rect of the second line.
            var secLineRect = rect;
            secLineRect.y += secLineRect.height + EditorGUIUtility.standardVerticalSpacing;
            secLineRect.width = 20;
            secLineRect.height = 20;

            //Button for remove the line and if it get's pressed remove the line and reload the tree view.
            if(GUI.Button(secLineRect, new GUIContent("X", "Click to remove the line")))
            {
                var dialogueList = new List<DialogueLine>(dialogue.DialogueLines);
                dialogueList.RemoveAt(item.lineInd);
                dialogue.DialogueLines = dialogueList.ToArray();
                dialogueWindow.InitNpcDialogueTreeView();
                Reload();
                return;
            }

            //Create a button to clear the text
            if (GUI.Button(new Rect(secLineRect.x, secLineRect.y + 24, secLineRect.width, secLineRect.height), new GUIContent("C", "Click to clear the sentence text")))
            {
                item.sentenceElement.sentence = "";
                dialogue.DialogueLines[item.lineInd].Sentence.Text = item.sentenceElement.sentence;
            }

            //Draw a text area in wich the Text of the sentence could be modified.
            secLineRect.x += secLineRect.width + EditorGUIUtility.standardVerticalSpacing;
            secLineRect.width = rect.width - secLineRect.width - (secLineRect.width * .6f);
            secLineRect.height = rect.height * 2.75f;
            item.sentenceElement.sentence = EditorGUI.TextArea(secLineRect, item.sentenceElement.sentence);
            dialogue.DialogueLines[item.lineInd].Sentence.Text = item.sentenceElement.sentence;

            //Sets the rect of the third line
            var thirdLineRect = rect;
            thirdLineRect.y += (secLineRect.height * 1.1f) + thirdLineRect.height + EditorGUIUtility.standardVerticalSpacing;

            //Draw Colorfield for the background Color
            thirdLineRect.width = rect.width * .04f;
            item.backgroundColor = EditorGUI.ColorField(thirdLineRect, new GUIContent("","The background color of this line"), item.backgroundColor, false, true, false);
            dialogue.DialogueLines[item.lineInd].backgroundColor = new float[4] { item.backgroundColor.r, item.backgroundColor.g, item.backgroundColor.b, item.backgroundColor.a };

            //Check if the line end the dialogue
            if (item.sentenceElement.dialogueOption != TriggeredDialogueOption.CloseDialogueWithIncreaseProgress || item.sentenceElement.dialogueOption != TriggeredDialogueOption.CloseDialogueWithoutIncreaseProgress)
            {
                //If there are enough dialogue lines you can skip a few
                if (dialogue.DialogueLines.Length > 2 && item.lineInd < dialogue.DialogueLines.Length - 2)
                {
                    var skipLineRect = thirdLineRect;
                    skipLineRect.x += rect.width * .05f;
                    //Creating the Rect for the third line
                    skipLineRect.width = 80;
                    //Label for the Skiped Lines Field
                    EditorGUI.LabelField(skipLineRect, "Lines to Skip");

                    skipLineRect.x = skipLineRect.xMax;
                    skipLineRect.width = skipLineRect.height + 2;

                    //If skiped lines is bigger than 0 create a button to decrease the int
                    if (item.sentenceElement.skipedLines > 0)
                    {
                        if (GUI.Button(skipLineRect, "-"))
                        {
                            item.sentenceElement.skipedLines--;
                            dialogue.DialogueLines[item.lineInd].Sentence.SkipedLines = item.sentenceElement.skipedLines;
                        }
                    }

                    //label field to display the skiped Lines ind
                    if (item.sentenceElement.skipedLines > 9)
                        EditorGUI.LabelField(new Rect(skipLineRect.xMax + 5, skipLineRect.y, skipLineRect.width, skipLineRect.height), item.sentenceElement.skipedLines.ToString());
                    else
                        EditorGUI.LabelField(new Rect(skipLineRect.xMax + 9, skipLineRect.y, skipLineRect.width, skipLineRect.height), item.sentenceElement.skipedLines.ToString());

                    //if there are enough dialogues to skip create a button to increase the int
                    if (item.sentenceElement.skipedLines < dialogue.DialogueLines.Length - 2)
                    {
                        skipLineRect.x += skipLineRect.width * 2.5f;

                        if (GUI.Button(skipLineRect, "+"))
                        {
                            item.sentenceElement.skipedLines++;
                            dialogue.DialogueLines[item.lineInd].Sentence.SkipedLines = item.sentenceElement.skipedLines;
                        }
                    }
                }

                thirdLineRect.x = rect.x + 200;
                thirdLineRect.width = rect.width * .15f;
                //Create a button to add an answer
                if (GUI.Button(thirdLineRect, new GUIContent("+ Answer", "Click to add a new Answer to the line.")))
                {
                    var answers = new List<AnswerClass>(dialogue.DialogueLines[item.lineInd].Answers);
                    answers.Add(new AnswerClass());
                    dialogue.DialogueLines[item.lineInd].Answers = answers.ToArray();
                    Reload();
                }

                //Check if the dialogueLine has answers 
                if (item.hasChildren)
                {
                    thirdLineRect.x += rect.width * .16f;

                    //Create a button to remove an answer
                    if (GUI.Button(thirdLineRect, new GUIContent("- Answer", "Click to remove the last Answer.")))
                    {
                        var answers = new List<AnswerClass>(dialogue.DialogueLines[item.lineInd].Answers);

                        if (answers.Count > 0)
                            answers.RemoveAt(answers.Count - 1);

                        dialogue.DialogueLines[item.lineInd].Answers = answers.ToArray();
                        Reload();
                    }

                    //Check if the dialogueLine has more than 1 answer
                    if (item.answerElements.Count > 1)
                    {
                        thirdLineRect.x += rect.width * .16f;

                        //Button to delete all answer.
                        if (GUI.Button(thirdLineRect, new GUIContent("- all", "Click to remove all answers of the line.")))
                        {
                            var answers = new List<AnswerClass>(dialogue.DialogueLines[item.lineInd].Answers);
                            answers = new List<AnswerClass>();
                            dialogue.DialogueLines[item.lineInd].Answers = answers.ToArray();
                            Reload();
                        }
                    }
                }
            }
        }

        #endregion

        #region DispalyAnswer

        /// <summary> Draws the Answer row </summary>
        /// <param name="item"></param>
        /// <param name="rowRect"></param>
        void DisplayAnswerRow(AnswerElement item, Rect rowRect)
        {
            var contentIndent = GetContentIndent(item);

            //Draw Brackground
            var bgRect = rowRect;
            bgRect.x = contentIndent;
            bgRect.width = Mathf.Max(rowRect.width - contentIndent) - 5f;
            bgRect.yMin += 2f;
            bgRect.yMax -= 2f;
            DrawItemBackground(bgRect, item.backgroundColor);
            //Draw Row
            var controllRect = bgRect;
            controllRect.xMin += 5f;
            controllRect.xMax += 5f;
            DrawAnswerGUI(item, controllRect);
        }

        /// <summary> Set's the GUI for the Answer GUI </summary>
        /// <param name="item"></param>
        /// <param name="rowRect"></param>
        void DrawAnswerGUI(AnswerElement item, Rect rowRect)
        {
            //!! Always set the rect where GUI element should drawn befor it get's drawn.
            //Sets the rect of an normal line.
            var rect = rowRect;
            rect.y += 3f;
            rect.height = EditorGUIUtility.singleLineHeight;

            //Sets the rect of the Fist Line 
            var firstLineRect = rect;
            //Create the Label how the Answer should be named. Default: Answer + ID
            EditorGUI.LabelField(firstLineRect, item.displayName, EditorStyles.boldLabel);

            //Create a Label of the description text field
            firstLineRect.x += 75;
            firstLineRect.xMax = firstLineRect.x + 63;
            EditorGUI.LabelField(firstLineRect, new GUIContent("Decription:", "A short description of the answer wich is later displayed in game for choosing an answer if you decide to."));

            //Create an Textfield for the short version of the Answer
            firstLineRect.x = firstLineRect.xMax;
            firstLineRect.xMax = rect.xMax - 10;
            item.description = EditorGUI.TextField(firstLineRect, item.description);
            //Set the description text equals the answer description
            dialogue.DialogueLines[item.parentElement.lineInd].Answers[item.answerInd].Description = item.description;

            //Sets the rect of the second Second Line
            var secLineRect = rect;
            secLineRect.y += secLineRect.height + EditorGUIUtility.standardVerticalSpacing;
            secLineRect.width = 20;
            secLineRect.height = 20;

            //Create the remove button
            if (GUI.Button(secLineRect, new GUIContent("X", "Click to remove the line.")))
            {
                var answerList = new List<AnswerClass>(dialogue.DialogueLines[item.parentElement.lineInd].Answers);
                answerList.RemoveAt(item.answerInd);
                dialogue.DialogueLines[item.parentElement.lineInd].Answers = answerList.ToArray();
                Reload();
                return;
            }

            //If there is an answer text create a button to clear the text
            if (GUI.Button(new Rect(secLineRect.x, secLineRect.y + 24, secLineRect.width, secLineRect.height), new GUIContent("C", "Click to clear the answer text.")))
            {
                item.answer = "";
                dialogue.DialogueLines[item.parentElement.lineInd].Answers[item.answerInd].Text = item.answer;
            }

            secLineRect.x += secLineRect.width + EditorGUIUtility.standardVerticalSpacing;
            secLineRect.width = rect.width - secLineRect.width - (secLineRect.width * .6f);
            secLineRect.height = rect.height * 2.75f;

            //Create the text area for the answer text
            item.answer = EditorGUI.TextArea(secLineRect, item.answer);
            dialogue.DialogueLines[item.parentElement.lineInd].Answers[item.answerInd].Text = item.answer;

            //Sets the line of the third line.
            var thirdLineRect = rect;
            thirdLineRect.y += (secLineRect.height * 1.1f) + thirdLineRect.height + EditorGUIUtility.standardVerticalSpacing;

            //Creat a color field for the background color
            thirdLineRect.width = rect.width * .04f;
            item.backgroundColor = EditorGUI.ColorField(thirdLineRect, new GUIContent("", "The background color of this Answer"), item.backgroundColor, false, true, false);
            dialogue.DialogueLines[item.parentElement.lineInd].Answers[item.answerInd].backgroundColor = new float[4] { item.backgroundColor.r, item.backgroundColor.g, item.backgroundColor.b, item.backgroundColor.a };

            //If there are enough dialogue lines you can skip a few
            if (dialogue.DialogueLines.Length > 2 && item.parentElement.lineInd < dialogue.DialogueLines.Length - 2 && item.dialogueOption != TriggeredDialogueOption.CloseDialogueWithoutIncreaseProgress || dialogue.DialogueLines.Length > 2 && item.parentElement.lineInd < dialogue.DialogueLines.Length - 2 && item.dialogueOption != TriggeredDialogueOption.CloseDialogueWithIncreaseProgress)
            {
                //Creating the Rect for the third line
                thirdLineRect.x += rect.width * .05f;
                thirdLineRect.width = 80;
                //Label for the Skiped Lines Field
                EditorGUI.LabelField(thirdLineRect, "Lines to Skip");

                thirdLineRect.x = thirdLineRect.xMax;
                thirdLineRect.width = thirdLineRect.height + 2;

                //If skiped lines is bigger than 0 create a button to decrease the int
                if (item.skipedLines > 0)
                {
                    if (GUI.Button(thirdLineRect, "-"))
                    {
                        item.skipedLines--;
                        dialogue.DialogueLines[item.parentElement.lineInd].Answers[item.answerInd].SkipedLines = item.skipedLines;
                    }
                }


                //label field to display the skiped Lines ind
                if (item.skipedLines > 9)
                    EditorGUI.LabelField(new Rect(thirdLineRect.xMax + 5, thirdLineRect.y, thirdLineRect.width, thirdLineRect.height), item.skipedLines.ToString());
                else
                    EditorGUI.LabelField(new Rect(thirdLineRect.xMax + 9, thirdLineRect.y, thirdLineRect.width, thirdLineRect.height), item.skipedLines.ToString());

                //if there are enough dialogues to skip create a button to increase the int
                if (item.skipedLines + item.parentElement.lineInd < dialogue.DialogueLines.Length - 2)
                {
                    thirdLineRect.x += thirdLineRect.width * 2.5f;

                    if (GUI.Button(thirdLineRect, "+"))
                    {
                        item.skipedLines++;
                        dialogue.DialogueLines[item.parentElement.lineInd].Answers[item.answerInd].SkipedLines = item.skipedLines;
                    }
                }
            }

            thirdLineRect.x = rect.width * .85f;
            thirdLineRect.xMax = rect.xMax - 10;
            //Create the enum popup for the triggered dialogue option
            item.dialogueOption = (TriggeredDialogueOption)EditorGUI.EnumPopup(thirdLineRect, item.dialogueOption);
            //saves the enum to the dialogue
            dialogue.DialogueLines[item.parentElement.lineInd].Answers[item.answerInd].TriggeredDialogueOption = (int)item.dialogueOption;

            //Create the Label for the triggered dialogue option enum popup
            thirdLineRect.xMax = thirdLineRect.xMin;
            thirdLineRect.xMin = rect.x;
            EditorGUI.LabelField(thirdLineRect, "Triggered Dialogue Option:", rightAnchoredLabel);
        }

        #endregion

        /// <summary> Draws the Background Color </summary>
        /// <param name="bgRect"> The rect of the background. </param>
        void DrawItemBackground(Rect bgRect, Color bgColor)
        {
            if(Event.current.type == EventType.Repaint)
            {
                Styles.background.normal.background = CreateColorTex(Mathf.RoundToInt(bgRect.width), Mathf.RoundToInt(bgRect.height), bgColor);
                Styles.background.Draw(bgRect, false, false, false, false);
            }
        }

        /// <summary> Creates a new Texture wich is used for the background color of a tree view item. </summary>
        /// <param name="width"> The width of the texture. </param>
        /// <param name="height"> The height of the texture. </param>
        /// <param name="col"> The color of the texture</param>
        /// <returns></returns>
        private Texture2D CreateColorTex(int width, int height, Color col)
        {
            //Create array of colors for every pixel in the row.
            Color[] pix = new Color[width * height];

            //Loop through all pixels and set the color of the pixel to the target color.
            for (int i = 0; i < pix.Length; i++)
                pix[i] = col;

            //Create the result texture and set the width and height of the texture to the needed width and height.
            Texture2D result = new Texture2D(width, height);
            //Set the color of the pixels of the texture to the above created 'pix' array.
            result.SetPixels(pix);
            //Saves the changes made to the texture.
            result.Apply();
            //return the 'result' texure.
            return result;
        }

        #endregion

        /// <summary> Get the dialogue for saving or whatever you wanna do with it </summary>
        /// <returns></returns>
        public Dialogue GetDialogue()
        {
            return dialogue;
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
