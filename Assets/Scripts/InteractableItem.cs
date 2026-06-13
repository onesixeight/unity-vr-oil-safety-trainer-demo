using UnityEngine;

namespace OilSafetyTrainer
{
    public abstract class InteractableItem : MonoBehaviour
    {
        [SerializeField] private string displayName = "Объект";
        [SerializeField] private string interactionPrompt = "Нажмите E";
        [SerializeField] private Renderer highlightRenderer;

        private Color originalColor;
        private bool hasOriginalColor;

        public string DisplayName => displayName;

        public void ConfigureInteraction(string itemDisplayName, string prompt, Renderer renderer)
        {
            displayName = itemDisplayName;
            interactionPrompt = prompt;
            highlightRenderer = renderer;
        }

        public virtual string GetPrompt()
        {
            return $"{interactionPrompt}: {displayName}";
        }

        public abstract void Interact(PlayerRig playerRig);

        public virtual void SetHighlighted(bool highlighted)
        {
            if (highlightRenderer == null)
            {
                highlightRenderer = GetComponentInChildren<Renderer>();
            }

            if (highlightRenderer == null)
            {
                return;
            }

            if (!hasOriginalColor)
            {
                originalColor = GetEditableMaterial(highlightRenderer).color;
                hasOriginalColor = true;
            }

            GetEditableMaterial(highlightRenderer).color = highlighted
                ? Color.Lerp(originalColor, Color.white, 0.35f)
                : originalColor;
        }

        protected static Material GetEditableMaterial(Renderer renderer)
        {
            return Application.isPlaying ? renderer.material : renderer.sharedMaterial;
        }
    }
}
