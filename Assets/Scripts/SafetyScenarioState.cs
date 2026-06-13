using System;
using System.Collections.Generic;
using System.Linq;

namespace OilSafetyTrainer
{
    public sealed class SafetyScenarioState
    {
        private readonly string[] requiredPpe;
        private readonly string[] hazardIds;
        private readonly HashSet<string> equippedPpe = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        private readonly HashSet<string> inspectedHazards = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        private readonly HashSet<string> penalizedMissingPpe = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        private readonly int baseScore;
        private readonly int missingPpePenalty;
        private readonly int uninspectedHazardPenalty;

        public SafetyScenarioState(
            IEnumerable<string> requiredPpe,
            IEnumerable<string> hazardIds,
            int baseScore,
            int missingPpePenalty,
            int uninspectedHazardPenalty = 0)
        {
            this.requiredPpe = NormalizeSet(requiredPpe);
            this.hazardIds = NormalizeSet(hazardIds);
            this.baseScore = Math.Max(0, baseScore);
            this.missingPpePenalty = Math.Max(0, missingPpePenalty);
            this.uninspectedHazardPenalty = Math.Max(0, uninspectedHazardPenalty);
        }

        public IReadOnlyCollection<string> RequiredPpe => requiredPpe;
        public IReadOnlyCollection<string> EquippedPpe => equippedPpe;
        public IReadOnlyCollection<string> HazardIds => hazardIds;
        public IReadOnlyCollection<string> InspectedHazards => inspectedHazards;
        public IReadOnlyCollection<string> PenalizedMissingPpe => penalizedMissingPpe;
        public bool HasAllRequiredPpe => GetMissingPpe().Length == 0;
        public int InspectedHazardCount => inspectedHazards.Count;
        public int RemainingHazardCount => Math.Max(0, hazardIds.Length - inspectedHazards.Count);

        public bool EquipPpe(string ppeId)
        {
            var normalized = Normalize(ppeId);
            if (string.IsNullOrEmpty(normalized) || !requiredPpe.Contains(normalized, StringComparer.OrdinalIgnoreCase))
            {
                return false;
            }

            return equippedPpe.Add(normalized);
        }

        public bool InspectHazard(string hazardId)
        {
            var normalized = Normalize(hazardId);
            if (string.IsNullOrEmpty(normalized) || !hazardIds.Contains(normalized, StringComparer.OrdinalIgnoreCase))
            {
                return false;
            }

            return inspectedHazards.Add(normalized);
        }

        public string[] GetMissingPpe()
        {
            return requiredPpe
                .Where(id => !equippedPpe.Contains(id))
                .ToArray();
        }

        public void RegisterMissingPpeAttempt()
        {
            foreach (var missing in GetMissingPpe())
            {
                penalizedMissingPpe.Add(missing);
            }
        }

        public int CalculateScore(bool includeRemainingHazardPenalty = false)
        {
            var penalty = penalizedMissingPpe.Count * missingPpePenalty;
            if (includeRemainingHazardPenalty)
            {
                penalty += RemainingHazardCount * uninspectedHazardPenalty;
            }

            return Math.Max(0, baseScore - penalty);
        }

        public void Reset()
        {
            equippedPpe.Clear();
            inspectedHazards.Clear();
            penalizedMissingPpe.Clear();
        }

        private static string[] NormalizeSet(IEnumerable<string> values)
        {
            if (values == null)
            {
                return Array.Empty<string>();
            }

            return values
                .Select(Normalize)
                .Where(value => !string.IsNullOrEmpty(value))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToArray();
        }

        private static string Normalize(string value)
        {
            return string.IsNullOrWhiteSpace(value) ? string.Empty : value.Trim();
        }
    }
}
