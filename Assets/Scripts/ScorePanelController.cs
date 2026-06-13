using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace OilSafetyTrainer
{
    public sealed class ScorePanelController : MonoBehaviour
    {
        [SerializeField] private Text objectiveText;
        [SerializeField] private Text checklistText;
        [SerializeField] private Text promptText;
        [SerializeField] private Text messageText;
        [SerializeField] private Text scoreText;
        [SerializeField] private Text finalText;
        [SerializeField] private Text guideText;
        [SerializeField] private CanvasGroup finalPanel;
        [SerializeField] private CanvasGroup guidePanel;
        [SerializeField] private Button resetButton;
        [SerializeField] private Button quitButton;

        private Coroutine messageRoutine;

        public bool IsFinalVisible => finalPanel != null && finalPanel.alpha > 0.99f;
        public bool IsGuideVisible => guidePanel != null && guidePanel.alpha > 0.99f;
        public bool HasGuidePanel => guidePanel != null && guideText != null;
        public bool HasFinalActions => resetButton != null && quitButton != null;

        public void Bind(
            Text objective,
            Text checklist,
            Text prompt,
            Text message,
            Text score,
            Text final,
            Text guide,
            CanvasGroup finalGroup,
            CanvasGroup guideGroup,
            Button resetActionButton,
            Button quitActionButton)
        {
            objectiveText = objective;
            checklistText = checklist;
            promptText = prompt;
            messageText = message;
            scoreText = score;
            finalText = final;
            guideText = guide;
            finalPanel = finalGroup;
            guidePanel = guideGroup;
            resetButton = resetActionButton;
            quitButton = quitActionButton;
        }

        public void BindActions(UnityAction resetAction, UnityAction quitAction)
        {
            WireButton(resetButton, resetAction);
            WireButton(quitButton, quitAction);
        }

        public void SetObjective(string value)
        {
            if (objectiveText != null)
            {
                objectiveText.text = value;
            }
        }

        public void SetChecklist(string value)
        {
            if (checklistText != null)
            {
                checklistText.text = value;
            }
        }

        public void SetPrompt(string value)
        {
            if (promptText != null)
            {
                promptText.text = value;
            }
        }

        public void SetScore(string value)
        {
            if (scoreText != null)
            {
                scoreText.text = value;
            }
        }

        public void SetGuide(string value)
        {
            if (guideText != null)
            {
                guideText.text = value;
            }
        }

        public void ShowMessage(string value, float seconds = 4f)
        {
            if (messageText == null)
            {
                return;
            }

            if (messageRoutine != null)
            {
                StopCoroutine(messageRoutine);
            }

            messageText.text = value;
            messageRoutine = StartCoroutine(ClearMessageAfter(seconds));
        }

        public void ShowFinal(string value)
        {
            if (finalText != null)
            {
                finalText.text = value;
            }

            SetFinalPanelVisible(true);
        }

        public void HideFinal()
        {
            SetFinalPanelVisible(false);
        }

        public void ShowGuide()
        {
            SetGuidePanelVisible(true);
        }

        public void HideGuide()
        {
            SetGuidePanelVisible(false);
        }

        public void ToggleGuide()
        {
            SetGuidePanelVisible(!IsGuideVisible);
        }

        private IEnumerator ClearMessageAfter(float seconds)
        {
            yield return new WaitForSeconds(seconds);
            if (messageText != null)
            {
                messageText.text = string.Empty;
            }
        }

        private void SetFinalPanelVisible(bool visible)
        {
            if (finalPanel == null)
            {
                return;
            }

            finalPanel.alpha = visible ? 1f : 0f;
            finalPanel.blocksRaycasts = visible;
            finalPanel.interactable = visible;
        }

        private void SetGuidePanelVisible(bool visible)
        {
            if (guidePanel == null)
            {
                return;
            }

            guidePanel.alpha = visible ? 1f : 0f;
            guidePanel.blocksRaycasts = false;
            guidePanel.interactable = false;
        }

        private static void WireButton(Button button, UnityAction action)
        {
            if (button == null)
            {
                return;
            }

            button.onClick.RemoveAllListeners();
            if (action != null)
            {
                button.onClick.AddListener(action);
            }
        }
    }
}
