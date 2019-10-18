using Character.NPC;
using System;
using System.Collections.Generic;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace Character.DialogueSystem.Editor
{
    /// <summary> Class of a TreeViewItem for an NpcDialogue. </summary>
    [Serializable]
    internal class DialoguePartnerElement : TreeViewItem
    {
        /// <summary> The name of the NPC. </summary>
        public string Name;
        /// <summary> The 'specialScripableNpc' that represents the NPC. </summary>
        public SpecialScriptableNpc ScriptableNpc;
        /// <summary> True if the user can rename the 'Name' of the NPC. </summary>
        public bool rename = false;
        /// <summary> List of all DialogueElements that the NPC has. </summary>
        public List<DialogueElement> dialogueElements;
        /// <summary> The index of the NPC inside the DialogueStorage. </summary>
        public int npcInd;

        /// <summary> Initialize a new 'DialoguePartnerElement' class without specifying any parameter. </summary>
        public DialoguePartnerElement() { }

        /// <summary> Initialize a new 'DialoguePartnerElement' class with specifying the id, depth and name. </summary>
        /// <param name="id"> The id of the TreeViewItem. </param>
        /// <param name="depth"> The depth of the TreeViewItem. </param>
        /// <param name="name"> The name of the NPC and the TreeViewItem. </param>
        public DialoguePartnerElement(int id, int depth, string name) : base(id, depth, name)
        {
            Name = name;
        }

        /// <summary> Initialize a new 'DialoguePartnerElement' class with specifying the id, depth, name and the scriptableNpc. </summary>
        /// <param name="id"> The id of the TreeViewItem. </param>
        /// <param name="depth"> The depth of the TreeViewItem. </param>
        /// <param name="name"> The name of the NPC and the TreeViewItem. </param>
        /// <param name="scriptableNpc"> The SpecialScriptableNpc wich represents the NPC. </param>
        public DialoguePartnerElement(int id, int depth, string name, SpecialScriptableNpc scriptableNpc) : base(id, depth, name)
        {
            Name = name;
            ScriptableNpc = scriptableNpc;
        }

        /// <summary> Initialize a new 'DialoguePartnerElement' class with specifying the id, depth, name, the scriptableNpc and the list of all dialogues. </summary>
        /// <param name="id"> The id of the TreeViewItem. </param>
        /// <param name="depth"> The depth of the TreeViewItem. </param>
        /// <param name="name"> The name of the NPC and the TreeViewItem. </param>
        /// <param name="scriptableNpc"> The SpecialScriptableNpc wich represents the NPC. </param>
        /// <param name="dialogues"> List of all Dialogues that the NPC have. </param>
        public DialoguePartnerElement(int id, int depth, string name, SpecialScriptableNpc scriptableNpc, List<DialogueElement> dialogues) : base(id, depth, name)
        {
            Name = name;
            ScriptableNpc = scriptableNpc;
            dialogueElements = dialogues;
        }

        /// <summary> Initialize a new 'DialoguePartnerElement' class with specifying the id, depth, name, the scriptableNpc, a list of all dialogues and the index of the NPC. </summary>
        /// <param name="id"> The id of the TreeViewItem. </param>
        /// <param name="depth"> The depth of the TreeViewItem. </param>
        /// <param name="name"> The name of the NPC and the TreeViewItem. </param>
        /// <param name="scriptableNpc"> The SpecialScriptableNpc wich represents the NPC. </param>
        /// <param name="dialogues"> List of all Dialogues that the NPC have. </param>
        /// <param name="npcId">  The Index of the NPC in the DialogueStorage. </param>
        public DialoguePartnerElement(int id, int depth, string name, SpecialScriptableNpc scriptableNpc, List<DialogueElement> dialogues, int npcId) : base(id, depth, name)
        {
            Name = name;
            ScriptableNpc = scriptableNpc;
            dialogueElements = dialogues;
            npcInd = npcId;
        }
    }

    /// <summary> Class of a TreeViewItem for a Dialogue. </summary>
    [Serializable]
    internal class DialogueElement : TreeViewItem
    {
        /// <summary> The Name of the Dialogue. </summary>
        public string Name;
        /// <summary> The Description of the Dialogue. </summary>
        public string Description;
        /// <summary> List of all DialogueLines that the Dialogue has. </summary>
        public List<DialogueLineElement> lineElements;
        /// <summary> True if the user can rename the 'Description' of the player. </summary>
        public bool rename;
        /// <summary> The index of the Dialogue inside of its NpcDialogue. </summary>
        public int dialogueInd;
        /// <summary> The NpcDialogue that uses this dialogue. </summary>
        public DialoguePartnerElement ParentElement;

        /// <summary> Initialize a new 'DialogueElement' class without specifying any parameter. </summary>
        public DialogueElement() { }

        /// <summary> Initialize a new 'DialogueElement' class with specifying the id, depth and name. </summary>
        /// <param name="id"> The id of the TreeViewItem. </param>
        /// <param name="depth"> The depth of the TreeViewItem. </param>
        /// <param name="name"> The name of the 'Name' of the dialogue and the TreeViewItem. </param>
        public DialogueElement(int id, int depth, string name) : base(id, depth, name)
        {
            Name = name;
        }

        /// <summary> Initialize a new 'DialogueElement' class with specifying the id, depth, name and the full list of lines attached to the diallogue. </summary>
        /// <param name="id"> The id of the TreeViewItem. </param>
        /// <param name="depth"> The depth of the TreeViewItem. </param>
        /// <param name="name"> The name of the 'Name' of the dialogue and the TreeViewItem. </param>
        /// <param name="lineItem"> List of all DialogueLines that the Dialogue contrains. </param>
        public DialogueElement(int id, int depth, string name, List<DialogueLineElement> lineItem) : base(id, depth, name)
        {
            Name = name;
            lineElements = lineItem;
        }

        /// <summary> Initialize a new 'DialogueElement' class with specifying the id, depth, name and the description. </summary>
        /// <param name="id"> The id of the TreeViewItem. </param>
        /// <param name="depth"> The depth of the TreeViewItem. </param>
        /// <param name="name"> The 'Name' of the dialogue. </param>
        /// <param name="description"> The 'Descritption' of the dialogue and the name of the TreeViewItem. </param>
        public DialogueElement(int id, int depth, string name, string description) : base(id, depth, description)
        {
            Name = name;
            Description = description;
        }

        /// <summary> Initialize a new 'DialogueElement' class with specifying the id, depth, name, the description and the size of the lines list. </summary>
        /// <param name="id"> The id of the TreeViewItem. </param>
        /// <param name="depth"> The depth of the TreeViewItem. </param>
        /// <param name="name"> The 'Name' of the dialogue. </param>
        /// <param name="description"> The 'Descritption' of the dialogue and the name of the TreeViewItem. </param>
        /// <param name="lineElementsCount"> The size of the List wich contains the Lines that the Dialogue has. </param>
        public DialogueElement(int id, int depth, string name, string description, int lineElementsCount) : base(id, depth, description)
        {
            Name = name;
            Description = description;
            lineElements = new List<DialogueLineElement>(lineElementsCount);
        }

        /// <summary> Initialize a new 'DialogueElement' class with specifying the id, depth, name, the description, the size of the lines list and the dialogue index. </summary>
        /// <param name="id"> The id of the TreeViewItem. </param>
        /// <param name="depth"> The depth of the TreeViewItem. </param>
        /// <param name="name"> The 'Name' of the dialogue. </param>
        /// <param name="description"> The 'Descritption' of the dialogue and the name of the TreeViewItem. </param>
        /// <param name="lineElementsCount"> The size of the List wich contains the Lines that the Dialogue has. </param>
        /// <param name="ind"> The index of the Dialogue inside of the NpcDialogue. </param>
        public DialogueElement(int id, int depth, string name, string description, int lineElementsCount, int ind) : base(id, depth, description)
        {
            Name = name;
            Description = description;
            lineElements = new List<DialogueLineElement>(lineElementsCount);
            dialogueInd = ind;
        }
    }

    /// <summary> Class of a TreeViewItem for a DialogueLine. </summary>
    [Serializable]
    internal class DialogueLineElement : TreeViewItem
    {
        /// <summary> The index of the DialogueLine inside of the Dialoge class. </summary>
        public int lineInd;
        /// <summary> The Sentence wich has to be in the line. </summary>
        public SentenceElement sentenceElement;
        /// <summary> List of answer that can be set if the sentence should have answer possibilities. </summary>
        public List<AnswerElement> answerElements;
        /// <summary> Sets who will talk the line. </summary>
        public Narrator narrator;
        /// <summary> The background color of the TreeViewItem in the tree view </summary>
        public Color backgroundColor;

        /// <summary> Initialize a new 'DialogueLineElement' class with specifying the id, depth and name. </summary>
        /// <param name="id"> The id of the TreeViewItem. </param>
        /// <param name="depth"> The depth of the TreeViewItem. </param>
        /// <param name="name"> The name of the TreeViewItem. </param>
        public DialogueLineElement(int id, int depth, string name) : base(id, depth, name)
        { }

        /// <summary> Initialize a new 'DialogueLineElement' class with specifying the id, depth, name and the line index. </summary>
        /// <param name="id"> The id of the TreeViewItem. </param>
        /// <param name="depth"> The depth of the TreeViewItem. </param>
        /// <param name="name"> The name of the TreeViewItem. </param>
        /// <param name="Ind"> The index of the DialogueLine inside of the Dialoge class. </param>
        public DialogueLineElement(int id, int depth, string name, int Ind) : base (id, depth, name)
        {
            lineInd = Ind;
        }

        /// <summary> Initialize a new 'DialogueLineElement' class with specifying the id, depth, name, the line index and the answers. </summary>
        /// <param name="id"> The id of the TreeViewItem. </param>
        /// <param name="depth"> The depth of the TreeViewItem. </param>
        /// <param name="name"> The name of the TreeViewItem. </param>
        /// <param name="Ind"> The index of the DialogueLine inside of the Dialoge class. </param>
        /// <param name="answers"> List of answer that can be set if the sentence should have answer possibilities. </param>
        public DialogueLineElement(int id, int depth, string name, int Ind, List<AnswerElement> answers) : base(id, depth, name)
        {
            lineInd = Ind;
            answerElements = answers;
        }

        /// <summary> Initialize a new 'DialogueLineElement' class with specifying the id, depth, name, the line index and the sentence. </summary>
        /// <param name="id"> The id of the TreeViewItem. </param>
        /// <param name="depth"> The depth of the TreeViewItem. </param>
        /// <param name="name"> The name of the TreeViewItem. </param>
        /// <param name="Ind"> The index of the DialogueLine inside of the Dialoge class. </param>
        /// <param name="sentence"> The Sentence wich has to be in the line. </param>
        public DialogueLineElement(int id, int depth, string name, int Ind, SentenceElement sentence) : base(id, depth, name)
        {
            lineInd = Ind;
            sentenceElement = sentence;
        }

        /// <summary> Initialize a new 'DialogueLineElement' class with specifying the id, depth, name, the line index, the sentence and the answers. </summary>
        /// <param name="id"> The id of the TreeViewItem. </param>
        /// <param name="depth"> The depth of the TreeViewItem. </param>
        /// <param name="name"> The name of the TreeViewItem. </param>
        /// <param name="Ind"> The index of the DialogueLine inside of the Dialoge class. </param>
        /// <param name="sentence"> The Sentence wich has to be in the line. </param>
        /// <param name="answers"> List of answer that can be set if the sentence should have answer possibilities. </param>
        public DialogueLineElement(int id, int depth, string name, int Ind, SentenceElement sentence, List<AnswerElement> answers) : base(id, depth, name)
        {
            lineInd = Ind;
            sentenceElement = sentence;
            answerElements = answers;
        }

        /// <summary> Initialize a new 'DialogueLineElement' class with specifying the id, depth, name, the line index, the sentence, the answers and the narrator. </summary>
        /// <param name="id"> The id of the TreeViewItem. </param>
        /// <param name="depth"> The depth of the TreeViewItem. </param>
        /// <param name="name"> The name of the TreeViewItem. </param>
        /// <param name="Ind"> The index of the DialogueLine inside of the Dialoge class. </param>
        /// <param name="sentence"> The Sentence wich has to be in the line. </param>
        /// <param name="answers"> List of answer that can be set if the sentence should have answer possibilities. </param>
        /// <param name="narrat"> Sets who will talk the line. </param>
        public DialogueLineElement(int id, int depth, string name, int Ind, SentenceElement sentence, List<AnswerElement> answers, Narrator narrat) : base(id, depth, name)
        {
            lineInd = Ind;
            sentenceElement = sentence;
            answerElements = answers;
            narrator = narrat;
        }

        /// <summary> Initialize a new 'DialogueLineElement' class with specifying the id, depth, name, the line index, the sentence, the answers, the narrator and the background color. </summary>
        /// <param name="id"> The id of the TreeViewItem. </param>
        /// <param name="depth"> The depth of the TreeViewItem. </param>
        /// <param name="name"> The name of the TreeViewItem. </param>
        /// <param name="Ind"> The index of the DialogueLine inside of the Dialoge class. </param>
        /// <param name="sentence"> The Sentence wich has to be in the line. </param>
        /// <param name="answers"> List of answer that can be set if the sentence should have answer possibilities. </param>
        /// <param name="narrat"> Sets who will talk the line. </param>
        /// <param name="bgColor"> The background color of the TreeViewItem in the tree view </param>
        public DialogueLineElement(int id, int depth, string name, int Ind, SentenceElement sentence, List<AnswerElement> answers, Narrator narrat, Color bgColor) : base(id, depth, name)
        {
            lineInd = Ind;
            sentenceElement = sentence;
            answerElements = answers;
            narrator = narrat;
            backgroundColor = bgColor;
        }
    }

    /// <summary> Class for a TreeViewItem that references the SenteceClass. </summary>
    [Serializable]
    internal class SentenceElement
    {
        /// <summary> The text of the sentence. </summary>
        public string sentence;
        /// <summary> The the dialogue option that will trigger whenn the sentence was drawn. </summary>
        public TriggeredDialogueOption dialogueOption;
        /// <summary> The lines that will skip after this sentence was drawn. </summary>
        public int skipedLines;

        /// <summary> Initialize a new 'SentenceElement' class without specifying any parameter. </summary>
        public SentenceElement() { }

        /// <summary> Initialize a new 'SentenceElement' class with specifying the text of the sentence. </summary>
        /// <param name="line"> The text of the sentence. </param>
        public SentenceElement(string line)
        {
            sentence = line;
        }

        /// <summary> Initialize a new 'SentenceElement' class with specifying the text of the sentence and the triggered dialogue option. </summary>
        /// <param name="line"> The text of the sentence. </param>
        /// <param name="triggeredDialogueOption"> The the dialogue option that will trigger whenn the sentence was drawn. </param>
        public SentenceElement(string line, TriggeredDialogueOption triggeredDialogueOption)
        {
            sentence = line;
            dialogueOption = triggeredDialogueOption;
        }

        /// <summary> Initialize a new 'SentenceElement' class with specifying the text of the sentence, the triggered dialogue option and the skipe lines value. </summary>
        /// <param name="line"> The text of the sentence. </param>
        /// <param name="triggeredDialogueOption"> The the dialogue option that will trigger whenn the sentence was drawn. </param>
        /// <param name="skipLines"> The lines that will skip after this sentence was drawn. </param>
        public SentenceElement(string line, TriggeredDialogueOption triggeredDialogueOption, int skipLines)
        {
            sentence = line;
            dialogueOption = triggeredDialogueOption;
            skipedLines = skipLines;
        }
    }

    /// <summary> Class of a TreeViewItem for an AnswerClass. </summary>
    [Serializable]
    internal class AnswerElement : TreeViewItem
    {
        /// <summary> The Index of the answer inside of the DialogueLine class. </summary>
        public int answerInd;
        /// <summary> The lines that will be skiped after this answer. </summary>
        public int skipedLines;
        /// <summary> The text of the answer. </summary>
        public string answer;
        /// <summary> The description of the answer wich is displayed on the answer buttons. </summary>
        public string description;
        /// <summary> The TriggeredDialogueOption of the answer. </summary>
        public TriggeredDialogueOption dialogueOption;
        /// <summary> The DialogueLine that uses this answer. </summary>
        public DialogueLineElement parentElement;
        /// <summary> The background color of the TreeViewItem in the tree view. </summary>
        public Color backgroundColor;

        /// <summary> Initialize a new 'AnswerElement' class with specifying the id, depth, name and the text of the answer. </summary>
        /// <param name="id"> The id of the TreeViewItem. </param>
        /// <param name="depth"> The depth of the TreeViewItem. </param>
        /// <param name="name"> The name of the TreeViewItem. </param>
        /// <param name="line"> The line that the player will say when selecting the answer. </param>
        public AnswerElement(int id, int depth, string name, string line) : base(id, depth, name)
        {
            answer = line;
        }

        /// <summary> Initialize a new 'AnswerElement' class with specifying the id, depth, name, the text of the answer and the skiped lines value. </summary>
        /// <param name="id"> The id of the TreeViewItem. </param>
        /// <param name="depth"> The depth of the TreeViewItem. </param>
        /// <param name="name"> The name of the TreeViewItem. </param>
        /// <param name="line"> The line that the player will say when selecting the answer. </param>
        /// <param name="skipLines"> The lines that will be skiped after that answer. </param>
        public AnswerElement(int id, int depth, string name, string line, int skipLines) : base(id, depth, name)
        {
            answer = line;
            skipedLines = skipLines;
        }

        /// <summary> Initialize a new 'AnswerElement' class with specifying the id, depth, name, the text of the answer, the skiped lines value and the answer index. </summary>
        /// <param name="id"> The id of the TreeViewItem. </param>
        /// <param name="depth"> The depth of the TreeViewItem. </param>
        /// <param name="name"> The name of the TreeViewItem. </param>
        /// <param name="line"> The line that the player will say when selecting the answer. </param>
        /// <param name="skipLines"> The lines that will be skiped after that answer. </param>
        /// <param name="ind"> The index of the answer in the DialogueLine class. </param>
        public AnswerElement(int id, int depth, string name, string line, int skipLines, int ind) : base(id, depth, name)
        {
            answer = line;
            skipedLines = skipLines;
            answerInd = ind;
        }

        /// <summary> Initialize a new 'AnswerElement' class with specifying the id, depth, name, the text of the answer, the skiped lines value, the answer index and the triggered dialogue option. </summary>
        /// <param name="id"> The id of the TreeViewItem. </param>
        /// <param name="depth"> The depth of the TreeViewItem. </param>
        /// <param name="name"> The name of the TreeViewItem. </param>
        /// <param name="line"> The line that the player will say when selecting the answer. </param>
        /// <param name="skipLines"> The lines that will be skiped after that answer. </param>
        /// <param name="ind"> The index of the answer in the DialogueLine class. </param>
        /// <param name="triggeredDialogueOption"> The dialogue option that will be triggered by the answer. </param>
        public AnswerElement(int id, int depth, string name, string line, int skipLines, int ind, TriggeredDialogueOption triggeredDialogueOption) : base(id, depth, name)
        {
            answer = line;
            skipedLines = skipLines;
            answerInd = ind;
            dialogueOption = triggeredDialogueOption;
        }

        /// <summary> Initialize a new 'AnswerElement' class with specifying the id, depth, name, the text of the answer, skiped lines value, answer index, the triggered dialogue option and the description. </summary>
        /// <param name="id"> The id of the TreeViewItem. </param>
        /// <param name="depth"> The depth of the TreeViewItem. </param>
        /// <param name="name"> The name of the TreeViewItem. </param>
        /// <param name="line"> The line that the player will say when selecting the answer. </param>
        /// <param name="skipLines"> The lines that will be skiped after that answer. </param>
        /// <param name="ind"> The index of the answer in the DialogueLine class. </param>
        /// <param name="triggeredDialogueOption"> The dialogue option that will be triggered by the answer. </param>
        /// <param name="descript"> The description of the answer. </param>
        public AnswerElement(int id, int depth, string name, string line, int skipLines, int ind, TriggeredDialogueOption triggeredDialogueOption, string descript) : base(id, depth, name)
        {
            answer = line;
            skipedLines = skipLines;
            answerInd = ind;
            dialogueOption = triggeredDialogueOption;
            description = descript;
        }

        /// <summary> Initialize a new 'AnswerElement' class with specifying the id, depth, name, the text of the answer, skiped lines value, answer index, the triggered dialogue option, the description and the background color. </summary>
        /// <param name="id"> The id of the TreeViewItem. </param>
        /// <param name="depth"> The depth of the TreeViewItem. </param>
        /// <param name="name"> The name of the TreeViewItem. </param>
        /// <param name="line"> The line that the player will say when selecting the answer. </param>
        /// <param name="skipLines"> The lines that will be skiped after that answer. </param>
        /// <param name="ind"> The index of the answer in the DialogueLine class. </param>
        /// <param name="triggeredDialogueOption"> The dialogue option that will be triggered by the answer. </param>
        /// <param name="descript"> The description of the answer. </param>
        /// <param name="bgColor"> The background color of the TreeViewItem. </param>
        public AnswerElement(int id, int depth, string name, string line, int skipLines, int ind, TriggeredDialogueOption triggeredDialogueOption, string descript, Color bgColor) : base(id, depth, name)
        {
            answer = line;
            skipedLines = skipLines;
            answerInd = ind;
            dialogueOption = triggeredDialogueOption;
            description = descript;
            backgroundColor = bgColor;
        }
    }
}
