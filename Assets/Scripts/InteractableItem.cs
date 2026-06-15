using UnityEngine;

namespace OilSafetyTrainer
{
    public abstract class InteractableItem : MonoBehaviour
    {
        [SerializeField] private string displayName = "Объект";
        [SerializeField] private string interactionPrompt = "Нажмите E";
        [SerializeField] private Renderer highlightRenderer;

        private MaterialPropertyBlock propertyBlock;
        private Color statusColor = Color.white;
        private bool hasStatusColor;
        private bool highlighted;

        public string DisplayName => displayName;

        public void ConfigureInteraction(string itemDisplayName, string prompt, Renderer renderer)
        {
            displayName = itemDisplayName;
            interactionPrompt = prompt;
            highlightRenderer = renderer;
            CaptureStatusColorIfNeeded();
            ApplyVisualColor();
        }

        public virtual string GetPrompt()
        {
            return $"{interactionPrompt}: {displayName}";
        }

        public abstract void Interact(PlayerRig playerRig);

        public virtual void SetHighlighted(bool highlighted)
        {
            this.highlighted = highlighted;
            CaptureStatusColorIfNeeded();
            ApplyVisualColor();
        }

        protected void SetHighlightRenderer(Renderer renderer)
        {
            highlightRenderer = renderer;
            CaptureStatusColorIfNeeded();
            ApplyVisualColor();
        }

        protected void SetStatusColor(Color color)
        {
            statusColor = color;
            hasStatusColor = true;
            ApplyVisualColor();
        }

        protected static Color ReadMaterialColor(Renderer renderer)
        {
            if (renderer == null || renderer.sharedMaterial == null || !renderer.sharedMaterial.HasProperty("_Color"))
            {
                return Color.white;
            }

            return renderer.sharedMaterial.color;
        }

        private void CaptureStatusColorIfNeeded()
        {
            if (hasStatusColor)
            {
                return;
            }

            if (highlightRenderer == null)
            {
                highlightRenderer = GetComponentInChildren<Renderer>();
            }

            if (highlightRenderer == null)
            {
                return;
            }

            statusColor = ReadMaterialColor(highlightRenderer);
            hasStatusColor = true;
        }

        private void ApplyVisualColor()
        {
            if (highlightRenderer == null || !hasStatusColor)
            {
                return;
            }

            propertyBlock ??= new MaterialPropertyBlock();
            var color = highlighted ? Color.Lerp(statusColor, Color.white, 0.35f) : statusColor;
            highlightRenderer.GetPropertyBlock(propertyBlock);
            propertyBlock.SetColor("_Color", color);
            highlightRenderer.SetPropertyBlock(propertyBlock);
        }
    }
}
