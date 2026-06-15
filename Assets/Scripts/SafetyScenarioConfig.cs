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

        public PpeItem[] RequiredPpe => requiredPpe;
        public HazardItem[] Hazards => hazards;
        public int BaseScore => baseScore;
        public int MissingPpePenalty => missingPpePenalty;
        public int UninspectedHazardPenalty => uninspectedHazardPenalty;

        public void Configure(
            PpeItem[] ppeItems,
            HazardItem[] hazardItems,
            int configuredBaseScore,
            int configuredMissingPpePenalty,
            int configuredUninspectedHazardPenalty)
        {
            requiredPpe = ppeItems ?? Array.Empty<PpeItem>();
            hazards = hazardItems ?? Array.Empty<HazardItem>();
            baseScore = Mathf.Max(0, configuredBaseScore);
            missingPpePenalty = Mathf.Max(0, configuredMissingPpePenalty);
            uninspectedHazardPenalty = Mathf.Max(0, configuredUninspectedHazardPenalty);
        }
    }
}
