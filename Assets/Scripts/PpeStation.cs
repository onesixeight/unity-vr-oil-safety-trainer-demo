using System.Linq;
using UnityEngine;

namespace OilSafetyTrainer
{
    public sealed class PpeStation : InteractableItem
    {
        [SerializeField] private string ppeId = "helmet";
        [SerializeField] private string ppeLabel = "Каска";
        [SerializeField] private Renderer statusRenderer;
        [SerializeField] private Color selectedColor = new Color(0.2f, 0.85f, 0.35f);

        private Color initialColor;
        private bool hasInitialColor;
        private bool equipped;

        public string PpeId => ppeId;
        public string PpeLabel => ppeLabel;

        public void Configure(string id, string label, Renderer renderer, Color selected)
        {
            ppeId = id;
            ppeLabel = label;
            statusRenderer = renderer;
            selectedColor = selected;
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
                SetStatusColor(equipped ? selectedColor : initialColor);
            }
        }

        public override string GetPrompt()
        {
            return equipped ? $"{ppeLabel} уже выбрано" : $"E - надеть: {ppeLabel}";
        }

        public override void Interact(PlayerRig playerRig)
        {
            var manager = SafetyScenarioManager.Instance;
            if (manager == null)
            {
                return;
            }

            var changed = manager.EquipPpe(ppeId, ppeLabel);
            SetEquipped(changed || manager.State.EquippedPpe.Contains(ppeId));
        }

        public void SetEquipped(bool value)
        {
            EnsureInitialColor();
            equipped = value;
            if (statusRenderer != null)
            {
                SetStatusColor(equipped ? selectedColor : initialColor);
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
