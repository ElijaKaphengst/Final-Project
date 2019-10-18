using Character.NPC;
using Managers;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Character.DialogueSystem
{
    /// <summary> Manages the dialogues. Used for open and close an dialogue. </summary>
    public class DialogueManager : MonoBehaviour
    {
        #region Singleton

        /// <summary> The Instance of the DialogueManager wich is accessable from other classes. </summary>
        public static DialogueManager instance;

        private void Awake()
        {
            if (instance == null)
                instance = this;
            else
                Destroy(gameObject);

        }

        #endregion

        #region Variables/Base Methods

        /// <summary> The index of the current dialogue line wich is used. </summary>
        public int CurrentDialogueInd { get; set; }
        /// <summary> The last selected answer of the dialogue. </summary>
        public AnswerClass CurrentAnswer { get; set; }
        /// <summary> The 'DialogueStorage' wich contains all dialogues that are accessable </summary>
        public DialogueStorage Storage { get { return dialogueStorage; } }

        /// <summary> How fast the line will be drawn when the 'drawType' is 'charByChar'. </summary>
        public float lineSpeed;
        /// <summary> True when the answer buttons are displayed and the player have to select one. </summary>
        public bool isWaitingForAnswer;
        /// <summary> How the Text of the Lines will be drawn. </summary>
        public DrawType drawType;
        /// <summary> The window wich contains the dialogue UI. </summary>
        public GameObject dialogueWindow;
        /// <summary> The prefab of an answer button that will be intantiated when answer are available. </summary>
        public GameObject decisionButton;
        /// <summary> The GameObject in wich the lines will be drawn. </summary>
        public GameObject sentenceObj;
        /// <summary> The Text wich will display the lines. </summary>
        public TextMeshProUGUI lineText;
        /// <summary> The Text that will display the current narrator of the line. </summary>
        public TextMeshProUGUI narratorText;
        /// <summary> The parent where the answer button will be instantiated. </summary>
        public Transform decisionButtonParent;
        /// <summary> The event that will happen when the triggeredDialogueOption of a line is 'NextLine'. </summary>
        public Button.ButtonClickedEvent TriggeredDialogueOption_NextLine;
        /// <summary> The event that will happen when the triggeredDialogueOption of a line is 'CloseDialogueWithIncreaseProgress' of CloseDialogueWichoutIncreaseProgress'. </summary>
        public Button.ButtonClickedEvent TriggeredDialogueOption_CloseDialogue;

        /// <summary> The index of lines that will be skiped. </summary>
        private int skipedLines;
        /// <summary> True if the line have answers. </summary>
        private bool hasAnswer = false;
        /// <summary> The current dialogue of an NPC wich is used. </summary>
        private Dialogue curDialogue;
        /// <summary> The 'ScriptalbeNpc' of the NPC who the player talk to. </summary>
        private ScriptableNpc curNpc;
        /// <summary> The storage of all dialogues. </summary>
        private DialogueStorage dialogueStorage;
        /// <summary> The controller of the player to set his 'canMove' bool to false. </summary>
        private Character controller;
        /// <summary> The ineractions of the player to set his 'isInteracting' bool to true. </summary>
        private CharacterInteractions interactions;
        /// <summary> The 'DialoguePartner' component of the NPC who the player talk to. </summary>
        private DialoguePartner curDialoguePartner;
        /// <summary> The camera controller reference. </summary>
        private CameraController camControll;
        /// <summary> List of all the answer buttons that are accessable. </summary>
        private List <GameObject> answerButtons = new List<GameObject>();

        private void Start()
        {
            //Reads the Dialogue.json file and set it to the 'dialogueStorage'.
            JsonReader.ReadFromJson(FilePaths.StreamingAssetsPath + FilePaths.DialoguePath, ref dialogueStorage);
            camControll = FindObjectOfType<CameraController>();
        }

        private void Update()
        {
            //Check if there are answer buttons accessable and if there are some the set 'isWaitingForAnswer to true, otherwise set it to false.
            if (answerButtons.Count > 0)
                isWaitingForAnswer = true;
            else
                isWaitingForAnswer = false;

            //Check if the dialogue window is active.
            if(dialogueWindow.activeInHierarchy)
            {
                //Check if the skip line key was pressed and if it was then call 'SkipLine'
                if (Input.GetKeyDown(KeyCode.Space))
                    SkipLine(); //Check if the pause Key was pressed and if it was then close the dialogue.
                else if (Input.GetKeyDown(KeyCode.Escape))
                    CloseDialogue();
            }
        }

        #endregion

        #region MainMethods
        
        /// <summary> Starts a Dialogue with an NPC. </summary>
        /// <param name="npc"> The NPC the player will talk to. </param>
        /// <param name="playerController"> The controller of the player to set his movement off. </param>
        /// <param name="playerInteractions"> The interactions of the player to say it, that the player is now interacting </param>
        /// <param name="partner"> The 'DialoguePartner' component of the NPC. </param>
        public void StartDialogue(ScriptableNpc npc, Character playerController, CharacterInteractions playerInteractions, DialoguePartner partner)
        {
            //Set the progress of the current dialogue to 0.
            CurrentDialogueInd = 0;
            //Set 'hasAnswer' to false.
            hasAnswer = false;
            //Set the current dialogue partner to the 'partner' parameter.
            curDialoguePartner = partner;
            //Calls the 'GetCharacters' method.
            GetCharacters(playerController, playerInteractions);
            //Get the dialogue of the NPC that will be open.
            curDialogue = GetDialogue(npc, curDialoguePartner.dialogueProgress);
            //Don't allow the player to move.
            controller.canMove = false;
            //Don't alloe the camera to rotate.
            camControll.canRotate = false;
            //Set the 'isInteracting' bool from the 'playerInteractions' to false.
            interactions.isInteracting = true;
            //Set the NPC with wich the player will talk.
            curNpc = npc;
            //Draws the first line of the dialogue.
            NextLine(curDialogue, npc);
            //Activate the dialogue window.
            dialogueWindow.SetActive(true);
        }

        /// <summary> Closes the Dialogue </summary>
        public void CloseDialogue()
        {
            //Delete all anser buttons and let the player move, the camera rotate and so one.
            DeleteAnswerButtons();
            controller.canMove = true;
            camControll.canRotate = true;
            interactions.isInteracting = false;
            dialogueWindow.SetActive(false);
            GetCharacters(null, null);
            curDialoguePartner = null;
        }

        /// <summary> Calls the methods to draw a new line. Can instantiat answer buttons so call this after a sentence. </summary>
        /// <param name="dialogue"> The current dialogue wich is used. </param>
        /// <param name="npc"> The NPC the player talk to. </param>
        public void NextLine(Dialogue dialogue, ScriptableNpc npc)
        {
            //Check if there're lines left to draw.
            if (CurrentDialogueInd <= dialogue.DialogueLines.Length - 1)
            {
                //Check who is the narrator if the line and set the 'narractorText' to the narrators name.
                if (dialogue.DialogueLines[CurrentDialogueInd].Narrator == (int)Narrator.NPC && !hasAnswer)
                    narratorText.text = npc.npcName;
                else if (dialogue.DialogueLines[CurrentDialogueInd].Narrator == (int)Narrator.Player || hasAnswer)
                    narratorText.text = "Adam";

                //Check if the previous line has an answer and it it has then instantiat them by calling the 'AddAllAnswerButtons'  method.
                if (hasAnswer)
                {
                    AddAllAnswerButtons(dialogue.DialogueLines[CurrentDialogueInd - 1].Answers);
                }   //If its hasn't an answer then draw the line.
                else if (dialogue.DialogueLines[CurrentDialogueInd].Sentence != null && !isWaitingForAnswer)
                    DrawSentence(dialogue.DialogueLines[CurrentDialogueInd].Sentence, dialogue.DialogueLines[CurrentDialogueInd].Answers);
            }
        }

        /// <summary> Calls the methods to draw a new line if the line is an answer. </summary>
        /// <param name="dialogue"> The current dialogue wich is used. </param>
        /// <param name="npc"> The NPC the player talk to. </param>
        /// <param name="line"> The Line that should be drawn. </param>
        public void NextLine(Dialogue dialogue, ScriptableNpc npc, LineClass line)
        {
            //Check if there're lines left to draw. 
            if (CurrentDialogueInd <= dialogue.DialogueLines.Length)
            {
                //Check who is the narrator if the line and set the 'narractorText' to the narrators name.
                if (line.GetType() == typeof(SentenceClass))
                    narratorText.text = npc.npcName;
                else if (line.GetType() == typeof(AnswerClass))
                    narratorText.text = "Adam";

                //Set the GameObject true in wich the line text will be drawn.
                sentenceObj.SetActive(true);

                //Check how the line should be drawn.
                switch (drawType)
                {
                    //If the line should be char by char drawn then start the 'DrawLine' coroutine.
                    case DrawType.CharByChar:
                        StartCoroutine(DrawLine(line));
                        break;
                    //If the line should be drawn sudden the start the 'SuddenLine' coroutine.
                    case DrawType.Sudden:
                        StartCoroutine(SuddenLine(line));
                        break;
                }
            }
        }

        /// <summary> Skips the current line. </summary>
        void SkipLine()
        {
            //Check if there are answers to select and if there some then return.
            if (isWaitingForAnswer)
                return;

            //Stops all coroutines.
            StopAllCoroutines();

            //Check if the current line is an answer.
            if (CurrentAnswer != null)
            {
                //Check the triggered dialogue option of the answer.
                if (CurrentAnswer.TriggeredDialogueOption == (int)TriggeredDialogueOption.CloseDialogueWithIncreaseProgress)
                {
                    //Increase the dialogue progress and close the dialogue if the triggrerd dialogue option is 'CloseDialogueWithIncreaseProgress'.
                    IncreaseDialogueProgress(curDialoguePartner.npcDialogue, curDialoguePartner);
                    CloseDialogue();
                }
                else if (CurrentAnswer.TriggeredDialogueOption == (int)TriggeredDialogueOption.CloseDialogueWithoutIncreaseProgress)
                {
                    //Close the dialogue if the triggrerd dialogue option is 'CloseDialogueWithoutIncreaseProgress'.
                    CloseDialogue();
                }
                else if (CurrentAnswer.TriggeredDialogueOption == (int)TriggeredDialogueOption.NextDialogueLine)
                {
                    //Draw the next line if the triggered dialogue option is 'NextDialogueLine.
                    NextLine(curDialogue, curNpc);
                }
            }   //Check the triggered dialogue option of the line that is currently drawn
            else if (curDialogue.DialogueLines[CurrentDialogueInd - (1 + skipedLines)].Sentence.TriggeredDialogueOption == (int)TriggeredDialogueOption.CloseDialogueWithIncreaseProgress)
            {
                //Increase the dialogue progress and close the dialogue if the triggrerd dialogue option is 'CloseDialogueWithIncreaseProgress'.
                IncreaseDialogueProgress(curDialoguePartner.npcDialogue, curDialoguePartner);
                CloseDialogue();
            }   //Check the triggered dialogue option of the line that is currently drawn
            else if (curDialogue.DialogueLines[CurrentDialogueInd - (1 + skipedLines)].Sentence.TriggeredDialogueOption == (int)TriggeredDialogueOption.CloseDialogueWithoutIncreaseProgress)
            {
                //Close the dialogue if the triggrerd dialogue option is 'CloseDialogueWithoutIncreaseProgress'.
                CloseDialogue();
            }
            else    //Draw the next line if triggerd dialogue option is not set to close and if there's no answer.
                NextLine(curDialogue, curNpc);

            //Set the current answer to null so the next time a line will skiped it uses the SentenceClass if there isn't a new answer.
            CurrentAnswer = null;
        }

        #endregion

        #region SentenceDraw

        /// <summary> Class the methods to draw a sentence of a dialogue. </summary>
        /// <param name="sentence"> The sentece that should be drawn. </param>
        /// <param name="answers"> The answer the sentence have. </param>
        void DrawSentence(SentenceClass sentence, AnswerClass[] answers)
        {
            //Activate the sentence object.
            sentenceObj.SetActive(true);
            //Set the lines to skip value.
            skipedLines = sentence.SkipedLines;
            //Increase the curent dialogue index.
            CurrentDialogueInd += 1 + sentence.SkipedLines;

            //Check how the sentence should be drawn.
            switch (drawType)
            {
                //If the sentence should be char by char drawn then start the 'DrawLine' coroutine.
                case DrawType.CharByChar:
                    StartCoroutine(DrawLine(sentence));
                    break;
                //If the sentence should be drawn sudden the start the 'SuddenLine' coroutine.
                case DrawType.Sudden:
                    StartCoroutine(SuddenLine(sentence));
                    break;
            }

            //Check if the sentence has answers and set the 'hasAnswer' bool.
            if (answers.Length > 0)
                hasAnswer = true;
            else
                hasAnswer = false;
        }

        #endregion

        #region AnswerDraw

        /// <summary> Add all needed answer buttons. </summary>
        /// <param name="answers"> Array of the needed answers. </param>
        void AddAllAnswerButtons(AnswerClass[] answers)
        {
            //Deactivate the sentence GameObject.
            sentenceObj.SetActive(false);
            //Set 'isWaitingForAnswer' to true so that the answer buttons aren't skipable.
            isWaitingForAnswer = true;
            //Set the cursor visibility to true.
            UiManager.instance.SetCursorDisplayMode(true);
            //Instantiate a new button for every answer.
            for (int i = 0; i < answers.Length; i++)
                AddAnswerButton(answers[i].Description, (TriggeredDialogueOption)answers[i].TriggeredDialogueOption, answers[i]);
        }

        /// <summary> Adds an answer button to the dialogue window </summary>
        /// <param name="answerText"> the that's displayed in the button. </param>
        /// <param name="triggeredOption"> What happens when the answer will be selected. </param>
        /// <param name="answerClass"> The answer class wich will be given to the 'DialogueAnswerButton' component of the button object. </param>
        void AddAnswerButton(string answerText, TriggeredDialogueOption triggeredOption, AnswerClass answerClass)
        {
            //Instantiate the GameObject wich contains the button.
            GameObject buttonObj = Instantiate(decisionButton, decisionButtonParent) as GameObject;
            //Gets the 'DialogueAnswerButton' to set the answer the what will happen when select it.
            var answerButton = buttonObj.GetComponent<DialogueAnswerButton>();
            //Adds the button to the 'answerButtons' list.
            answerButtons.Add(buttonObj);

            //Check the triggered dialogue option of the answer and set the answer for each diffred triggered option.
            switch (triggeredOption)
            {
                case TriggeredDialogueOption.NextDialogueLine:
                    answerButton.SetAnswer(answerText, TriggeredDialogueOption_NextLine, answerClass, triggeredOption);
                    break;
                case TriggeredDialogueOption.CloseDialogueWithoutIncreaseProgress:
                    answerButton.SetAnswer(answerText, TriggeredDialogueOption_CloseDialogue, answerClass, triggeredOption);
                    break;
                case TriggeredDialogueOption.CloseDialogueWithIncreaseProgress:
                    answerButton.SetAnswer(answerText, TriggeredDialogueOption_CloseDialogue, answerClass, triggeredOption);
                    break;
            }
        }

        /// <summary> Get's called when an answer was selected </summary>
        /// <param name="answer"></param>
        public void SelectNextLineAnswer(AnswerClass answer)
        {
            //Check the triggrerd dialogue option of the answer.
            switch(answer.TriggeredDialogueOption)
            {
                case (int)TriggeredDialogueOption.NextDialogueLine:
                    {
                        //Delete all answer buttons and draw the next line wich contains the text of the answer.
                        DeleteAnswerButtons();
                        isWaitingForAnswer = false;
                        NextLine(curDialogue, curNpc, answer);
                    }
                    break;
                case (int)TriggeredDialogueOption.CloseDialogueWithIncreaseProgress:
                    {
                        //Draws the next line so that the answer is drawn and then the dialogue will close.
                        NextLine(curDialogue, curNpc, answer);
                        IncreaseDialogueProgress(curDialoguePartner.npcDialogue, curDialoguePartner);
                    }
                    break;
                case (int)TriggeredDialogueOption.CloseDialogueWithoutIncreaseProgress:
                    {
                        //Draws the next line so that the answer is drawn and then the dialogue will close.
                        NextLine(curDialogue, curNpc, answer);
                    }
                    break;
            }
        }

        /// <summary> Deletes all Answerbuttons. </summary>
        public void DeleteAnswerButtons()
        {
            //Loops through all answer buttons and delete them.
            foreach (GameObject obj in answerButtons)
                Destroy(obj);
            //Set the 'hasAnswer' bool to false.
            hasAnswer = false;
            //Clear the 'answerButtons' list.
            answerButtons.Clear();
        }

        #endregion

        #region LineDraw

        /// <summary> Let's the whole sentence pop up. </summary>
        /// <param name="line"> The line that will be drawn. </param>
        IEnumerator SuddenLine(LineClass line)
        {
            //Sets the 'lineText' of the dialogue window to the text of the drawn line.
            lineText.text = line.Text;
            //wait for the second how long the line is devided by 10 + .5f seconds so the player have enough timer to read it.
            yield return new WaitForSeconds((line.Text.ToCharArray().Length / 10) + .5f);

            //Check the triggered dialogue option of the line.
            if (line.TriggeredDialogueOption == (int)TriggeredDialogueOption.NextDialogueLine)
            {
                //Draw the next line.
                NextLine(curDialogue, curNpc);
            }
            else if (line.TriggeredDialogueOption == (int)TriggeredDialogueOption.CloseDialogueWithoutIncreaseProgress)
            {
                //Close the dialogue.
                CloseDialogue();
            }
            else if(line.TriggeredDialogueOption == (int)TriggeredDialogueOption.CloseDialogueWithIncreaseProgress)
            {
                //Increase the dialogue progress and close the dialogue.
                IncreaseDialogueProgress(curDialoguePartner.npcDialogue, curDialoguePartner);
                CloseDialogue();
            }
        }

        /// <summary> Draws the Senctence char by char. </summary>
        /// <param name="line"> The line that will be drawn. </param>
        IEnumerator DrawLine(LineClass line)
        {
            //Clear the 'lineText' of the dialogue window.
            lineText.text = null;

            //Creates a char array out of the line that should be displayed.
            char[] lineChars = line.Text.ToCharArray();
            //Set the empty char.
            char empty = ' ';

            //Loops through all chars.
            for (int i = 0; i < lineChars.Length; i++)
            {
                //Add the char to the line text.
                lineText.text += lineChars[i];
                //If the char doesn't is an empty char then wait for the line speed value.
                if (lineChars[i] != empty)
                    yield return new WaitForSeconds(lineSpeed);
            }

            //Waot .5 seconds so the player have time to read the text.
            yield return new WaitForSeconds(.5f);

            //Check the triggered dialogue option of the line
            if (line.TriggeredDialogueOption == (int)TriggeredDialogueOption.NextDialogueLine)
            {
                //Draw the next line.
                NextLine(curDialogue, curNpc);
            }
            else if (line.TriggeredDialogueOption == (int)TriggeredDialogueOption.CloseDialogueWithoutIncreaseProgress)
            {
                //Close the line.
                CloseDialogue();
            }
            else if (line.TriggeredDialogueOption == (int)TriggeredDialogueOption.CloseDialogueWithIncreaseProgress)
            {
                //Increase the dialogue progress and close the dialogue
                IncreaseDialogueProgress(curDialoguePartner.npcDialogue, curDialoguePartner);
                CloseDialogue();
            }
        }

        #endregion

        /// <summary> Used for search for the right dialogue of an NPC. </summary>
        /// <param name="npc"> The NPC wich dialogue is needed. </param>
        /// <param name="dialogueInd"> The index at which the dialog is located within the array of all dialogs. </param>
        /// <returns> Retuns the needed dialogue of the an NPC. </returns>
        Dialogue GetDialogue(ScriptableNpc npc, int dialogueInd)
        {
            //Loops through all NpcDialogues.
            for(int i = 0; i < dialogueStorage.DialogueList.Length; i++)
            {
                //Check if the Dialogues NPC name is equel to the NPC name of the needed NPC
                if(dialogueStorage.DialogueList[i].Name == npc.npcName)
                {
                    //Loops through all dialogues of the NPC
                    for (int a = 0; a < dialogueStorage.DialogueList[i].Dialogues.Length; a++)
                    {
                        //Checks if the Dialogue is the wanted dialogue and if it's, return it. 
                        if (a == dialogueInd)
                            return dialogueStorage.DialogueList[i].Dialogues[a];
                    }
                }
            }
            //when there's no dialogue, return null.
            return null;
        }

        /// <summary> Sets the 'controller' and the 'interactions'. </summary>
        /// <param name="playerController"> The controller that will be set to 'controller'. </param>
        /// <param name="playerInteractions"> The interactions controller that will be set to 'interactions' </param>
        void GetCharacters(Character playerController, CharacterInteractions playerInteractions)
        {
            controller = playerController;
            interactions = playerInteractions;
        }

        /// <summary> Increases the dialogue progess value of the dialoguePartner wich the player talk to. </summary>
        /// <param name="dialogue"> The dialogue of the NPC who gets increased his progress. </param>
        /// <param name="partner"> The 'DialoguePartner' component wich contains the 'dialogueProgress' value. </param>
        public void IncreaseDialogueProgress(NpcDialogue dialogue, DialoguePartner partner)
        {
            //Check if there are enough dialogues left to increase the progress.
            if (dialogue.Dialogues.Length > partner.dialogueProgress + 1)
                partner.dialogueProgress++;
        }
    }

    /// <summary> How the lines should be drawn. </summary>
    public enum DrawType
    {
        /// <summary> Draw each character of the line one by one. </summary>
        CharByChar,
        /// <summary> Lets the whole line popup at once. </summary>
        Sudden
    }
}
