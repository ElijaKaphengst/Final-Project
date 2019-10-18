using Character.NPC;
using System;

namespace Character.DialogueSystem
{
    /// <summary> Stores all dialogues of all NPCs. </summary>
    [Serializable]
    public class DialogueStorage
    {
        /// <summary> The Array of all NPCs wich have an dialogue and all of its dialogues </summary>
        public NpcDialogue[] DialogueList;

        /// <summary> Initialize a new 'DialogueStorage'. </summary>
        /// <param name="arraySize"> The size of the array that contains all 'NpcDialogue'. </param>
        public DialogueStorage(int arraySize)
        {
            DialogueList = new NpcDialogue[arraySize];
        }
    }

    /// <summary> Stores all dialogues of an NPC. </summary>
    [Serializable]
    public class NpcDialogue
    {
        /// <summary> The name of the NPC. </summary>
        public string Name;
        /// <summary> The 'SpecialScriptableNpc' reference of the NPC. </summary>
        public SpecialScriptableNpc ScriptableNpc;
        /// <summary> Array of all dialogues that the NPC have. </summary>
        public Dialogue[] Dialogues;

        /// <summary> Initialize a new 'NpcDialogue' without specifying parameters. </summary>
        public NpcDialogue() { }

        /// <summary> Initialize a new 'NpcDialogue' with specifying the name and the size of the dialogues array. </summary>
        /// <param name="name"> The name of the NPC. </param>
        /// <param name="dialogueSize"> The size of the array wich contains all dialouges </param>
        public NpcDialogue(string name, int dialogueSize)
        {
            Name = name;
            Dialogues = new Dialogue[dialogueSize];
        }

        /// <summary> Initialize a new 'NpcDialogue' with specifying the name and the complete array of dialogues. </summary>
        /// <param name="name"> The name of the NPC. </param>
        /// <param name="dialogues"> The complete array of dialogues that the NPC have. Can be set as single object, seperate them with a comma. </param>
        public NpcDialogue(string name, params Dialogue[] dialogues)
        {
            Name = name;
            Dialogues =  dialogues;
        }

        /// <summary> Initialize a new 'NpcDialogue' with specifying the name and the scriptableNpc of the NPC. </summary>
        /// <param name="name"> The name of the NPC. </param>
        /// <param name="npc"> The 'SpecialScriptableNpc' of the NPC who uses this class. </param>
        public NpcDialogue(string name, SpecialScriptableNpc npc)
        {
            Name = name;
            ScriptableNpc = npc;
        }

        /// <summary> Initialize a new 'NpcDialogue' with specifying the name, the scriptableNpc of the NPC and the size of the dialogues array. </summary>
        /// <param name="name"> The name of the NPC. </param>
        /// <param name="npc"> The 'SpecialScriptableNpc' of the NPC who uses this class. </param>
        /// <param name="dialogueSize"> The size of the array wich contains all dialouges </param>
        public NpcDialogue(string name, SpecialScriptableNpc npc, int dialogueSize)
        {
            Name = name;
            ScriptableNpc = npc;
            Dialogues = new Dialogue[dialogueSize];
        }

        /// <summary> Initialize a new 'NpcDialogue' with specifying the name, the scriptableNpc of the NPC and the complete array of dialogues. </summary>
        /// <param name="name"> The name of the NPC. </param>
        /// <param name="npc"> The 'SpecialScriptableNpc' of the NPC who uses this class. </param>
        /// <param name="dialogues"> The complete array of dialogues that the NPC have. Can be set as single object, seperate them with a comma. </param>
        public NpcDialogue(string name, SpecialScriptableNpc npc, params Dialogue[] dialogues)
        {
            Name = name;
            ScriptableNpc = npc;
            Dialogues = dialogues;
        }
    }

    /// <summary> Stores a complete dialogue of an NPC. </summary>
    [Serializable]
    public class Dialogue
    {
        /// <summary> The description of the dialogue. </summary>
        public string DialogueDescription = "This is a Dialogue.";
        /// <summary> Array of all dialogue lines wich are inside of the dialogue. </summary>
        public DialogueLine[] DialogueLines;

        /// <summary> Initialize a new 'Dialogue' class without specifying any parameter. </summary>
        public Dialogue() { }

        /// <summary> Initialize a new 'Dialogue' class with specifying the description and the how many DialogueLines it contains. </summary>
        /// <param name="description"> The descciption of the dialogue. </param>
        /// <param name="lineCount"> The size of the 'DialogueLines' array. </param>
        public Dialogue(string description, int lineCount)
        {
            DialogueDescription = description;
            DialogueLines = new DialogueLine[lineCount];
        }

        /// <summary> Initialize a new 'Dialogue' class with specifying the description and the how many DialogueLines it contains. </summary>
        /// <param name="description"> The descciption of the dialogue. </param>
        /// <param name="lines"> The complete array of DialogueLines the dialogue has. Can be set as single object, seperate them with a comma. </param>
        public Dialogue(string description, params DialogueLine[] lines)
        {
            DialogueDescription = description;
            DialogueLines = lines;
        }
    }

    /// <summary> Stores a Line of a dialogue. </summary>
    [Serializable]
    public class DialogueLine
    {
        /// <summary> The Sentence wich every line has. </summary>
        public SentenceClass Sentence;
        /// <summary> An array of answers that can be used if the line should have answers. </summary>
        public AnswerClass[] Answers;
        /// <summary> Index of the 'Narrator' enum. </summary>
        public int Narrator;
        /// <summary> The backgound color if the line inside of the Dialogue Editor window. </summary>
        public float[] backgroundColor = new float[4] { 190, 190, 190, 255} ;

        /// <summary> Initialize a new 'DialogueLine' class with specifying the sentence and how many answer the line should have. </summary>
        /// <param name="sentence"> The sentence of the line. </param>
        /// <param name="answerCapacity"> The size of the 'Answers' array. </param>
        public DialogueLine(SentenceClass sentence, int answerCapacity)
        {
            Sentence = sentence;
            Answers = new AnswerClass[answerCapacity];
        }
    }

    /// <summary> The Parent class for a line class. </summary>
    public class LineClass
    {
        /// <summary> The text of the line. </summary>
        public string Text;
        /// <summary> The index of the 'TriggeredDialogueOption' that will be triggered after the line. </summary>
        public int TriggeredDialogueOption;
        /// <summary> The value of lines that should skip after this line. </summary>
        public int SkipedLines;
    }

    /// <summary> Stores a Sentence line. </summary>
    [Serializable]
    public class SentenceClass : LineClass
    {
        /// <summary> Initialize a new 'SentenceClass' class with Setting the text to "". </summary>
        public SentenceClass()
        {
            Text = "";
        }
    }

    /// <summary> Stores an Answer line. </summary>
    [Serializable]
    public class AnswerClass : LineClass
    {
        /// <summary> The description of the answer that will be shown in the button ingame. </summary>
        public string Description;
        /// <summary> The background color of the answer inside of the Dialogue Editor window. </summary>
        public float[] backgroundColor = new float[4] { 190, 190, 190, 255 };

        /// <summary> Initialize a new 'AnswerClass' class with Setting the text to "". </summary>
        public AnswerClass()
        {
            Text = "";
        }
    }

    /// <summary> Sets what will happen after a line was drawn. </summary>
    public enum TriggeredDialogueOption
    {
        /// <summary> Continue the dialogue. </summary>
        NextDialogueLine,
        /// <summary> Closes the dialogue and the next time you talk with the npc, the same dialogue starts again. </summary>
        CloseDialogueWithoutIncreaseProgress,
        /// <summary> Closes the dialogue and the next time you talk with the npc, the next dialogue starts. </summary>
        CloseDialogueWithIncreaseProgress
    }

    /// <summary> Sets who will talk a line. </summary>
    public enum Narrator
    {
        /// <summary> Lets the NPC talk the line. </summary>
        NPC,
        /// <summary> Lets the player talk the line. </summary>
        Player
    }
}