using System.Linq;
using UnityEngine;

namespace OilSafetyTrainer
{
    public sealed class PpeStation : InteractableItem
    {
        [SerializeField] private string ppeId = "helmet";
        [SerializeField] private string ppeLabel = "каска";
        [SerializeField] private Renderer statusRenderer;
        [SerializeField] private Color selectedColor = new Color(0.2f, 0.85f, 0.35f);

        private Color initialColor;
        private bool equipped;

        public string PpeId => ppeId;
        public string PpeLabel => ppeLabel;

        public void Configure(string id, string label, Renderer renderer, Color selected)
        {
            ppeId = id;
            ppeLabel = label;
            statusRenderer = renderer;
            selectedColor = selected;
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
            SetEquipped(equipped || changed || manager.State.EquippedPpe.Contains(ppeId));
        }

        public void SetEquipped(bool value)
        {
            equipped = value;
            if (statusRenderer != null)
            {
                GetEditableMaterial(statusRenderer).color = equipped ? selectedColor : initialColor;
            }
        }
    }
}
