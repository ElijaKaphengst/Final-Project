using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Character.DialogueSystem
{
    /// <summary>
    /// Class of an answer button from the dialogue system to control what will happen when an answer will be selected.
    /// </summary>
    public class DialogueAnswerButton : MonoBehaviour, IPointerClickHandler
    {
        /// <summary> The text of an answer button. </summary>
        private TextMeshProUGUI text;
        /// <summary> The button component of the answer button. </summary>
        private Button button;
        /// <summary> The answer wich will be represented by the button. </summary>
        public AnswerClass answer;
        /// <summary> The option that will be triggered by selecting this answer. </summary>
        private TriggeredDialogueOption triggeredDialogueOption;

        private void OnEnable()
        {
            text = GetComponentInChildren<TextMeshProUGUI>();
            button = GetComponent<Button>();
        }

        /// <summary>
        /// Sets the answer of the button
        /// </summary>
        /// <param name="answerText"> The text of the answer that will be displayed insided of the button. </param>
        /// <param name="clickEvent"> The events that will happen by selecting this button. </param>
        /// <param name="answerClass"> The answer of this button. </param>
        /// <param name="triggeredOption"> The option that will be triggered by selecting this answer. </param>
        public void SetAnswer(string answerText, Button.ButtonClickedEvent clickEvent, AnswerClass answerClass, TriggeredDialogueOption triggeredOption)
        {
            text.text = answerText;
            button.onClick = clickEvent;
            answer = answerClass;
            triggeredDialogueOption = triggeredOption;
        }

        /// <summary>
        /// Unity method that was implemented by the IPointerClickHandler interface and wich will be called when the button was pressed.
        /// </summary>
        /// <param name="eventData"></param>
        public void OnPointerClick(PointerEventData eventData)
        {
            //Check the triggeredDialogueOption of the answer
            if (triggeredDialogueOption == TriggeredDialogueOption.NextDialogueLine)
            {
                //If it's 'NextLine' then add the lines to skip index to the current dialogue index.
                DialogueManager.instance.CurrentDialogueInd += answer.SkipedLines;
            }

            //Call the 'SelectNextLineAnswer' to write the answer as a new line.
            DialogueManager.instance.SelectNextLineAnswer(answer);
            //Sets the current selected answer of the dialogue manager to this answer.
            DialogueManager.instance.CurrentAnswer = answer;
            //The the Cursor visibility to false.
            UiManager.instance.SetCursorDisplayMode(false);
        }

        void OnDisable()
        {
            //Remove the onClick events when the button gets disabled.
            button.onClick.RemoveAllListeners();
        }
    }
}
