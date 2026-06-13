using System.Linq;
using UnityEngine;

namespace OilSafetyTrainer
{
    public sealed class HazardInspectionPoint : InteractableItem
    {
        [SerializeField] private string hazardId = "spill";
        [SerializeField] private string hazardLabel = "разлив масла";
        [TextArea]
        [SerializeField] private string recommendation = "Сообщите мастеру и оградите зону.";
        [SerializeField] private Renderer statusRenderer;
        [SerializeField] private Color inspectedColor = new Color(0.25f, 0.7f, 1f);

        private Color initialColor;
        private bool inspected;

        public string HazardId => hazardId;
        public string HazardLabel => hazardLabel;

        public void Configure(string id, string label, string mitigation, Renderer renderer, Color inspected)
        {
            hazardId = id;
            hazardLabel = label;
            recommendation = mitigation;
            statusRenderer = renderer;
            inspectedColor = inspected;
        }

        private void Awake()
        {
            if (statusRenderer == null)
            {
                statusRenderer = GetComponentInChildren<Renderer>();
            }

            if (statusRenderer != null)
            {
                initialColor = GetEditableMaterial(statusRenderer).color;
            }
        }

        public override string GetPrompt()
        {
            return inspected ? $"{hazardLabel} уже проверено" : $"E - осмотреть опасность: {hazardLabel}";
        }

        public override void Interact(PlayerRig playerRig)
        {
            var manager = SafetyScenarioManager.Instance;
            if (manager == null)
            {
                return;
            }

            var changed = manager.InspectHazard(hazardId, hazardLabel, recommendation);
            SetInspected(inspected || changed || manager.State.InspectedHazards.Contains(hazardId));
        }

        public void SetInspected(bool value)
        {
            inspected = value;
            if (statusRenderer != null)
            {
                GetEditableMaterial(statusRenderer).color = inspected ? inspectedColor : initialColor;
            }
        }
    }
}
