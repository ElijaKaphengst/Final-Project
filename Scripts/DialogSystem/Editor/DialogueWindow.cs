using Character.NPC;
using Managers;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using UnityEditor.AnimatedValues;

namespace Character.DialogueSystem.Editor
{
    public class DialogueWindow : EditorWindow
    {
        #region Variables

        [SerializeField] DialogueStorage storage = new DialogueStorage(0);
        [SerializeField] List<NpcDialogue> npcDialogues = new List<NpcDialogue>();

        private string storagePath;
        public int npcInd, dialogueInd;
        public bool dialogueSelected;

        [SerializeField] TreeViewState npcDialogueViewState;
        [SerializeField] TreeViewState dialogueViewSate;
        [SerializeField] MultiColumnHeaderState columnHeaderState;
        NpcDialogueTreeView npcDialogueTreeView;
        DialogueTreeView dialogueTreeView;

        #endregion

        #region BaseMathods

        [MenuItem("Window/Dialogue Editor")]
        public static DialogueWindow ShowWindow()
        {
            var window = GetWindow<DialogueWindow>();
            window.titleContent = new GUIContent("Dialogue Editor");
            window.minSize = new Vector2(400, 150);
            window.Show();
            return window;
        }

        void SetWindowSize(Vector2 size)
        {
            var window = GetWindow<DialogueWindow>("Dialogue Editor", false);
            window.minSize = size;
        }

        private void Awake()
        {
            storagePath = Application.streamingAssetsPath + FilePaths.DialoguePath;
            npcDialogues = new List<NpcDialogue>(storage.DialogueList);
        }

        private void OnEnable()
        {
            InitNpcDialogueTreeView();

            if (dialogueSelected)
                InitDialogueTreeView();
        }

        public void InitNpcDialogueTreeView()
        {
            if (!dialogueSelected)
                SetWindowSize(new Vector2(400, 150));

            if (npcDialogueViewState == null)
                npcDialogueViewState = new TreeViewState();

            var headerState = NpcDialogueTreeView.CreateMultiColumnHeaderState(position.width * .5f);
            columnHeaderState = headerState;

            var multiColumnHeader = new MultiColumnHeader(headerState);
            npcDialogueTreeView = new NpcDialogueTreeView(npcDialogueViewState, multiColumnHeader, storage, this);

        }

        public void InitDialogueTreeView()
        {
            if (storage.DialogueList[npcInd].Dialogues[dialogueInd].DialogueLines.Length > 0)
                dialogueSelected = true;
            else
            {
                dialogueSelected = false;
                return;
            }

            SetWindowSize(new Vector2(900, 150));

            if (dialogueViewSate == null)
                dialogueViewSate = new TreeViewState();

            npcDialogueTreeView.multiColumnHeader.ResizeToFit();
            dialogueTreeView = new DialogueTreeView(dialogueViewSate, storage.DialogueList[npcInd].Dialogues[dialogueInd], this);
        }

        #endregion

        #region Display Methods

        private void OnGUI()
        {
            GUILayout.BeginArea(new Rect(0, 0, position.width, 25));
            DisplayButtons();
            GUILayout.EndArea();

            if (!dialogueSelected)
                DisplayNpcDialogueTree();
            else
            {
                DisplayNpcDialogueTree();
                DisplayDialogueTree();
            }
        }

        void DisplayNpcDialogueTree()
        {
            if(!dialogueSelected)
            {
                GUILayout.BeginArea(new Rect(5, 25, position.width - 10, position.height - 30));
                npcDialogueTreeView.OnGUI(new Rect(0, 5, position.width - 10, position.height - 35));
                GUILayout.EndArea();
            }
            else
            {
                GUILayout.BeginArea(new Rect(5, 25, position.width * .4f, position.height - 30));
                npcDialogueTreeView.OnGUI(new Rect(0, 5, (position.width - 10) * .4f, position.height - 35));
                GUILayout.EndArea();
            }
        }

        void DisplayDialogueTree()
        {
            Rect area = new Rect((position.width * .4f) + 5, 25, (position.width - 20) * .6f, position.height - 30);
            GUILayout.BeginArea(area);
            DialogueTreeViewToolbar(new Rect(0, 5, (position.width - 20) * .6f, 20));
            dialogueTreeView.OnGUI(new Rect(0, 21, (position.width - 20) * .6f, position.height - 52));
            GUILayout.EndArea();
        }

        #region Toolbar

        void DisplayButtons()
        {
            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);

            if (GUILayout.Button("Add Dialogue", EditorStyles.toolbarButton))
            {
                storage = npcDialogueTreeView.GetStorage();

                npcDialogues = new List<NpcDialogue>(storage.DialogueList);
                npcDialogues.Add(new NpcDialogue("new NPC", 0));

                storage.DialogueList = npcDialogues.ToArray();

                InitNpcDialogueTreeView();
            }
            else if (GUILayout.Button("Load JSON", EditorStyles.toolbarButton))
            {
                JsonReader.ReadFromJson(storagePath, ref storage);
                npcDialogues = new List<NpcDialogue>(storage.DialogueList);

                for (int i = 0; i < npcDialogues.Count; i++)
                    storage.DialogueList[i].ScriptableNpc = FindScriptableNpc(npcDialogues[i].Name);

                InitNpcDialogueTreeView();
                if (dialogueSelected)
                    InitDialogueTreeView();
            }
            else if (GUILayout.Button("Save JSON", EditorStyles.toolbarButton))
            {
                storage = npcDialogueTreeView.GetStorage();
                if (dialogueSelected)
                    storage.DialogueList[npcInd].Dialogues[dialogueInd] = dialogueTreeView.GetDialogue();
                JsonReader.WriteToJson(storagePath, storage);
            }
            else if (GUILayout.Button("Clear Storage", EditorStyles.toolbarButton))
            {
                storage = new DialogueStorage(0);
                npcDialogues = new List<NpcDialogue>(storage.DialogueList);
                dialogueSelected = false;
                npcDialogueTreeView.SetSelection(new List<int>(0));
                InitNpcDialogueTreeView();
            }

            EditorGUILayout.LabelField(new GUIContent("Rename Key", "Double click or use shortcut to rename the selected dialogue partner. Keyboard shortcuts: Windows/Linux: F2; Mac OS X: Return"));

            EditorGUILayout.EndHorizontal();
        }

        void DialogueTreeViewToolbar(Rect toolbarRect)
        {
            GUILayout.BeginArea(toolbarRect);
            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar, GUILayout.Height(toolbarRect.height));

            if(GUILayout.Button("Add Line", EditorStyles.toolbarButton, GUILayout.Height(toolbarRect.height)))
            {
                storage.DialogueList[npcInd].Dialogues[dialogueInd] = dialogueTreeView.GetDialogue();

                List<DialogueLine> lines = new List<DialogueLine>(storage.DialogueList[npcInd].Dialogues[dialogueInd].DialogueLines);
                lines.Add(new DialogueLine(new SentenceClass(), 0));

                storage.DialogueList[npcInd].Dialogues[dialogueInd].DialogueLines = lines.ToArray();

                InitNpcDialogueTreeView();
                InitDialogueTreeView();
            }
            else if(GUILayout.Button("Remove last Line", EditorStyles.toolbarButton, GUILayout.Height(toolbarRect.height)))
            {
                List<DialogueLine> lines = new List<DialogueLine>(storage.DialogueList[npcInd].Dialogues[dialogueInd].DialogueLines);
                lines.RemoveAt(lines.Count - 1);
                storage.DialogueList[npcInd].Dialogues[dialogueInd].DialogueLines = lines.ToArray();

                InitNpcDialogueTreeView();
                InitDialogueTreeView();
            }
            else if(GUILayout.Button("Remove all Lines", EditorStyles.toolbarButton, GUILayout.Height(toolbarRect.height)))
            {
                List<DialogueLine> lines = new List<DialogueLine>(storage.DialogueList[npcInd].Dialogues[dialogueInd].DialogueLines);
                lines.RemoveAll(item => item.GetType() == typeof(DialogueLine));
                storage.DialogueList[npcInd].Dialogues[dialogueInd].DialogueLines = lines.ToArray();

                InitNpcDialogueTreeView();
                InitDialogueTreeView();
            }

            EditorGUILayout.EndHorizontal();
            GUILayout.EndArea();
        }

        #endregion

        #endregion

        #region ScriptableObjects

        static SpecialScriptableNpc FindScriptableNpc(string name)
        {
            SpecialScriptableNpc scriptableNpc;

            if (File.Exists(FilePaths.ScriptableFolderPath + "/" + name + ".asset"))
            {
                scriptableNpc = (SpecialScriptableNpc)AssetDatabase.LoadAssetAtPath(FilePaths.ScriptableFolderPath + "/" + name + ".asset", typeof(SpecialScriptableNpc));
                return scriptableNpc;
            }
            else
                return null;
        }


        /// <summary>
        /// Try to find the specialScriptableNpc and if it doesn't find it, then creates a new one.
        /// </summary>
        /// <param name="name"> How the specialScriptableNpc asset should be named. </param>
        /// <returns></returns>
        public static SpecialScriptableNpc CreateScriptableNpc(string name)
        {
            SpecialScriptableNpc scriptableNpc;

            if (File.Exists(FilePaths.ScriptableFolderPath + "/" + name + ".asset"))
            {
                scriptableNpc = (SpecialScriptableNpc)AssetDatabase.LoadAssetAtPath(FilePaths.ScriptableFolderPath + "/" + name + ".asset", typeof(SpecialScriptableNpc));
                return scriptableNpc;
            }

            scriptableNpc = CreateInstance<SpecialScriptableNpc>();

            if (!AssetDatabase.IsValidFolder(FilePaths.ScriptableFolderPath))
                AssetDatabase.CreateFolder("Assets", "ScriptableObjects");

            AssetDatabase.CreateAsset(scriptableNpc, FilePaths.ScriptableFolderPath + "/" + name + ".asset");
            scriptableNpc.npcName = name;
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            EditorUtility.FocusProjectWindow();
            Selection.activeObject = scriptableNpc;
            return scriptableNpc;
        }

        #endregion
    }
}

