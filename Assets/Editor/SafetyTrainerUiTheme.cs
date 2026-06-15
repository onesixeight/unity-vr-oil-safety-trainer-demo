using UnityEngine;

namespace OilSafetyTrainer.Editor
{
    internal static class SafetyTrainerUiTheme
    {
        public static readonly Color GuidePanel = new(0.02f, 0.06f, 0.08f, 0.94f);
        public static readonly Color FinalPanel = new(0f, 0f, 0f, 0.94f);
        public static readonly Color ResetButton = new(0.12f, 0.5f, 0.22f, 0.96f);
        public static readonly Color QuitButton = new(0.5f, 0.12f, 0.12f, 0.96f);

        public static readonly Color ButtonNormal = Color.white;
        public static readonly Color ButtonHighlighted = new(0.95f, 0.95f, 0.95f, 1f);
        public static readonly Color ButtonPressed = new(0.82f, 0.82f, 0.82f, 1f);
    }
}
