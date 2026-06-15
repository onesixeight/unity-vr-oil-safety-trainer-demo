using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace OilSafetyTrainer
{
    public sealed class SafetyScenarioManager : MonoBehaviour
    {
        [Serializable]
        public struct PpeRequirement
        {
            public string id;
            public string label;
        }

        [Serializable]
        public struct HazardRequirement
        {
            public string id;
            public string label;
        }

        public static SafetyScenarioManager Instance { get; private set; }
        public static Action QuitRequestHandler = DefaultQuitRequestHandler;

        [Header("Scenario")]
        public PpeRequirement[] requiredPpe = Array.Empty<PpeRequirement>();
        public HazardRequirement[] hazards = Array.Empty<HazardRequirement>();
        public int baseScore = 100;
        public int missingPpePenalty = 12;
        public int uninspectedHazardPenalty = 8;

        [Header("Scene")]
        public ScorePanelController scorePanel;
        public Transform playerStart;

        private SafetyScenarioState state;
        private Dictionary<string, string> ppeLabels;
        private Dictionary<string, string> hazardLabels;
        private bool workZoneEntered;

        public SafetyScenarioState State => state;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            BuildState();
            if (scorePanel == null)
            {
                Debug.LogError("SafetyScenarioManager: scorePanel is not assigned. UI will not render.", this);
            }
        }

        private void OnDestroy()
        {
            if (Instance == this)
            {
                Instance = null;
            }
        }

        private void Start()
        {
            EnforceFullscreenForDemoBuild();
            scorePanel?.HideFinal();
            scorePanel?.SetGuide(SafetyTrainerText.GuideText);
            scorePanel?.ShowGuide();
            scorePanel?.BindActions(ResetScenario, QuitDemo);
            SetInteractionPrompt(SafetyTrainerText.DefaultPrompt);
            UpdateHud();
            scorePanel?.ShowMessage("Цель: наденьте СИЗ, пройдите КПП и найдите опасности на площадке.", 6f);
        }

        public bool EquipPpe(string ppeId, string label)
        {
            var changed = state.EquipPpe(ppeId);
            scorePanel?.ShowMessage(changed ? $"СИЗ выбрано: {label}" : $"СИЗ уже выбрано: {label}");
            UpdateHud();
            return changed;
        }

        public bool InspectHazard(string hazardId, string label, string recommendation)
        {
            var changed = state.InspectHazard(hazardId);
            var message = changed
                ? $"Опасность выявлена: {label}. {recommendation}"
                : $"Опасность уже внесена в чеклист: {label}.";
            scorePanel?.ShowMessage(message, 6f);
            UpdateHud();
            return changed;
        }

        public bool TryEnterWorkZone()
        {
            if (state.HasAllRequiredPpe)
            {
                if (!workZoneEntered)
                {
                    workZoneEntered = true;
                    scorePanel?.ShowMessage("Доступ разрешён. Выполните обход оборудования и отметьте все опасности.");
                }

                UpdateHud();
                return true;
            }

            state.RegisterMissingPpeAttempt();
            var missing = string.Join(", ", state.GetMissingPpe().Select(GetPpeLabel));
            scorePanel?.ShowMessage($"Вход запрещён: не хватает СИЗ ({missing}). Начислен штраф.", 6f);
            UpdateHud();
            return false;
        }

        public void CompleteScenario()
        {
            var score = state.CalculateScore(includeRemainingHazardPenalty: true);
            var result = new StringBuilder();
            result.AppendLine("Итог тренажёра");
            result.AppendLine($"Оценка: {score}/100");
            result.AppendLine($"СИЗ: {state.EquippedPpe.Count}/{state.RequiredPpe.Count}");
            result.AppendLine($"Опасности: {state.InspectedHazardCount}/{state.HazardIds.Count}");

            var missing = state.GetMissingPpe();
            if (missing.Length > 0)
            {
                result.AppendLine($"Не хватает СИЗ: {string.Join(", ", missing.Select(GetPpeLabel))}");
            }

            if (state.RemainingHazardCount > 0)
            {
                var remainingHazards = state.HazardIds
                    .Where(id => !state.InspectedHazards.Contains(id))
                    .Select(GetHazardLabel);
                result.AppendLine($"Не найдены опасности: {string.Join(", ", remainingHazards)}");
            }

            result.AppendLine();
            result.AppendLine(score >= 85 ? "Результат: зачёт." : "Результат: повторите инструктаж.");
            result.AppendLine("R - начать заново | Q - выйти | мышью можно нажать кнопки ниже");
            scorePanel?.ShowFinal(result.ToString());
            scorePanel?.ShowMessage("Сценарий завершён. Выберите повторный запуск или выход из тренажёра.", 6f);
            SetInteractionPrompt("Сценарий завершён | R - заново | Q - выход | H - памятка");
            SetPlayerPaused(true);
            UpdateHud();
        }

        public void ResetScenario()
        {
            state.Reset();
            workZoneEntered = false;

            foreach (var station in FindObjectsByType<PpeStation>(FindObjectsInactive.Exclude))
            {
                station.SetEquipped(false);
            }

            foreach (var hazard in FindObjectsByType<HazardInspectionPoint>(FindObjectsInactive.Exclude))
            {
                hazard.SetInspected(false);
            }

            var player = FindAnyObjectByType<PlayerRig>();
            if (player != null && playerStart != null)
            {
                player.TeleportTo(playerStart.position, playerStart.rotation);
            }

            scorePanel?.HideFinal();
            scorePanel?.ShowGuide();
            SetPlayerPaused(false);
            SetInteractionPrompt(SafetyTrainerText.DefaultPrompt);
            scorePanel?.ShowMessage("Сценарий сброшен. Начните с выбора СИЗ.", 4f);
            UpdateHud();
        }

        public void SetInteractionPrompt(string prompt)
        {
            scorePanel?.SetPrompt(prompt);
        }

        public void ToggleGuide()
        {
            scorePanel?.ToggleGuide();
            scorePanel?.ShowMessage(scorePanel != null && scorePanel.IsGuideVisible ? "Памятка показана." : "Памятка скрыта.", 2f);
        }

        public void QuitDemo()
        {
            QuitRequestHandler();
        }

        private void BuildState()
        {
            ppeLabels = requiredPpe
                .Where(item => !string.IsNullOrWhiteSpace(item.id))
                .GroupBy(item => item.id, StringComparer.OrdinalIgnoreCase)
                .ToDictionary(group => group.Key, group => group.First().label, StringComparer.OrdinalIgnoreCase);
            hazardLabels = hazards
                .Where(item => !string.IsNullOrWhiteSpace(item.id))
                .GroupBy(item => item.id, StringComparer.OrdinalIgnoreCase)
                .ToDictionary(group => group.Key, group => group.First().label, StringComparer.OrdinalIgnoreCase);
            state = new SafetyScenarioState(
                requiredPpe.Select(item => item.id),
                hazards.Select(item => item.id),
                baseScore,
                missingPpePenalty,
                uninspectedHazardPenalty);
        }

        private void UpdateHud()
        {
            if (scorePanel == null || state == null)
            {
                return;
            }

            scorePanel.SetObjective(SafetyTrainerText.Objective);
            scorePanel.SetChecklist(BuildChecklistText());
            scorePanel.SetScore($"Текущая оценка: {state.CalculateScore(includeRemainingHazardPenalty: false)}/100");
        }

        private string BuildChecklistText()
        {
            var builder = new StringBuilder();
            builder.AppendLine("СИЗ перед входом:");
            foreach (var item in requiredPpe)
            {
                var mark = state.EquippedPpe.Contains(item.id) ? "[x]" : "[ ]";
                builder.AppendLine($"{mark} {item.label}");
            }

            builder.AppendLine();
            builder.AppendLine("Опасности на площадке:");
            foreach (var item in hazards)
            {
                var mark = state.InspectedHazards.Contains(item.id) ? "[x]" : "[ ]";
                builder.AppendLine($"{mark} {item.label}");
            }

            return builder.ToString();
        }

        private string GetPpeLabel(string id)
        {
            return ppeLabels.TryGetValue(id, out var label) ? label : id;
        }

        private string GetHazardLabel(string id)
        {
            return hazardLabels.TryGetValue(id, out var label) ? label : id;
        }

        private void SetPlayerPaused(bool value)
        {
            var playerController = FindAnyObjectByType<DesktopPlayerController>();
            if (playerController != null)
            {
                playerController.SetPaused(value);
            }
        }

        private static void EnforceFullscreenForDemoBuild()
        {
#if !UNITY_EDITOR
            Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
            Screen.fullScreen = true;

            var display = Display.main;
            if (display != null && display.systemWidth > 0 && display.systemHeight > 0)
            {
                Screen.SetResolution(display.systemWidth, display.systemHeight, FullScreenMode.FullScreenWindow);
            }
#endif
        }

        private static void DefaultQuitRequestHandler()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}
