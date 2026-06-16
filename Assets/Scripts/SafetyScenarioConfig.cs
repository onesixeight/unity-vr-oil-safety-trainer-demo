using System;
using UnityEngine;

namespace OilSafetyTrainer
{
    [CreateAssetMenu(fileName = "OilSafetyTrainerScenario", menuName = "Oil Safety Trainer/Scenario Config")]
    public sealed class SafetyScenarioConfig : ScriptableObject
    {
        [Serializable]
        public struct PpeItem
        {
            public string id;
            public string label;
        }

        [Serializable]
        public struct HazardItem
        {
            public string id;
            public string label;
            [TextArea]
            public string recommendation;
        }

        [SerializeField] private PpeItem[] requiredPpe = Array.Empty<PpeItem>();
        [SerializeField] private HazardItem[] hazards = Array.Empty<HazardItem>();
        [SerializeField] private int baseScore = 100;
        [SerializeField] private int missingPpePenalty = 12;
        [SerializeField] private int uninspectedHazardPenalty = 8;
        [SerializeField] private string scenarioName = string.Empty;
        [SerializeField] private string objectiveText = string.Empty;
        [SerializeField] private string guideText = string.Empty;
        [SerializeField] private string startMessage = string.Empty;
        [SerializeField] private bool forceFullscreenInBuild = true;

        public PpeItem[] RequiredPpe => requiredPpe;
        public HazardItem[] Hazards => hazards;
        public int BaseScore => baseScore;
        public int MissingPpePenalty => missingPpePenalty;
        public int UninspectedHazardPenalty => uninspectedHazardPenalty;
        public string ScenarioName => string.IsNullOrWhiteSpace(scenarioName) ? "Обход нефтедобывающей площадки" : scenarioName;
        public string ObjectiveText => string.IsNullOrWhiteSpace(objectiveText) ? SafetyTrainerText.Objective : objectiveText;
        public string GuideText => string.IsNullOrWhiteSpace(guideText) ? SafetyTrainerText.GuideText : guideText;
        public string StartMessage => string.IsNullOrWhiteSpace(startMessage) ? SafetyTrainerText.StartMessage : startMessage;
        public bool ForceFullscreenInBuild => forceFullscreenInBuild;

        public void Configure(
            PpeItem[] ppeItems,
            HazardItem[] hazardItems,
            int configuredBaseScore,
            int configuredMissingPpePenalty,
            int configuredUninspectedHazardPenalty,
            string configuredScenarioName = null,
            string configuredObjectiveText = null,
            string configuredGuideText = null,
            string configuredStartMessage = null,
            bool configuredForceFullscreenInBuild = true)
        {
            requiredPpe = ppeItems ?? Array.Empty<PpeItem>();
            hazards = hazardItems ?? Array.Empty<HazardItem>();
            baseScore = Mathf.Max(0, configuredBaseScore);
            missingPpePenalty = Mathf.Max(0, configuredMissingPpePenalty);
            uninspectedHazardPenalty = Mathf.Max(0, configuredUninspectedHazardPenalty);
            if (configuredScenarioName != null)
            {
                scenarioName = configuredScenarioName;
            }

            if (configuredObjectiveText != null)
            {
                objectiveText = configuredObjectiveText;
            }

            if (configuredGuideText != null)
            {
                guideText = configuredGuideText;
            }

            if (configuredStartMessage != null)
            {
                startMessage = configuredStartMessage;
            }

            forceFullscreenInBuild = configuredForceFullscreenInBuild;
        }

        public bool EnsureInstructionText(string defaultScenarioName, string defaultObjectiveText, string defaultGuideText, string defaultStartMessage)
        {
            var changed = false;
            if (string.IsNullOrWhiteSpace(scenarioName))
            {
                scenarioName = defaultScenarioName;
                changed = true;
            }

            if (string.IsNullOrWhiteSpace(objectiveText))
            {
                objectiveText = defaultObjectiveText;
                changed = true;
            }

            if (string.IsNullOrWhiteSpace(guideText))
            {
                guideText = defaultGuideText;
                changed = true;
            }

            if (string.IsNullOrWhiteSpace(startMessage))
            {
                startMessage = defaultStartMessage;
                changed = true;
            }

            return changed;
        }
    }
}
