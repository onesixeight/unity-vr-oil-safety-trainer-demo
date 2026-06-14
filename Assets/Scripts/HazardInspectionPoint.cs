using System.Linq;
using UnityEngine;

namespace OilSafetyTrainer
{
    public sealed class HazardInspectionPoint : InteractableItem
    {
        [SerializeField] private string hazardId = "spill";
        [SerializeField] private string hazardLabel = "Разлив масла";
        [TextArea]
        [SerializeField] private string recommendation = "Сообщите мастеру и оградите зону.";
        [SerializeField] private Renderer statusRenderer;
        [SerializeField] private Color inspectedColor = new Color(0.25f, 0.7f, 1f);

        private Color initialColor;
        private bool hasInitialColor;
        private bool inspected;

        public string HazardId => hazardId;
        public string HazardLabel => hazardLabel;

        public void Configure(string id, string label, string mitigation, Renderer renderer, Color inspectedStateColor)
        {
            hazardId = id;
            hazardLabel = NormalizeRussianText(label);
            recommendation = NormalizeRussianText(mitigation);
            statusRenderer = renderer;
            inspectedColor = inspectedStateColor;
            EnsureInitialColor();
            SetHighlightRenderer(statusRenderer);
            SetStatusColor(initialColor);
        }

        private void Awake()
        {
            if (statusRenderer == null)
            {
                statusRenderer = GetComponentInChildren<Renderer>();
            }

            if (statusRenderer != null)
            {
                EnsureInitialColor();
                SetHighlightRenderer(statusRenderer);
                SetStatusColor(inspected ? inspectedColor : initialColor);
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
            EnsureInitialColor();
            inspected = value;
            if (statusRenderer != null)
            {
                SetStatusColor(inspected ? inspectedColor : initialColor);
            }
        }

        private void EnsureInitialColor()
        {
            if (hasInitialColor)
            {
                return;
            }

            if (statusRenderer == null)
            {
                statusRenderer = GetComponentInChildren<Renderer>();
            }

            initialColor = ReadMaterialColor(statusRenderer);
            hasInitialColor = true;
        }
    }
}
