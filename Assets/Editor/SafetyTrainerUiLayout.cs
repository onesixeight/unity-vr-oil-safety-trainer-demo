using UnityEngine;

namespace OilSafetyTrainer.Editor
{
    internal static class SafetyTrainerUiLayout
    {
        public static readonly Vector2 ReferenceResolution = new(1920f, 1080f);

        public static readonly Vector2 TopLeftAnchor = new(0f, 1f);
        public static readonly Vector2 BottomLeftAnchor = new(0f, 0f);
        public static readonly Vector2 BottomCenterAnchor = new(0.5f, 0f);
        public static readonly Vector2 CenterAnchor = new(0.5f, 0.5f);
        public static readonly Vector2 TopRightAnchor = new(1f, 1f);
        public static readonly Vector2 BottomRightAnchor = new(1f, 0f);

        public static readonly Vector2 StretchAnchorMin = Vector2.zero;
        public static readonly Vector2 StretchAnchorMax = Vector2.one;

        public static readonly Vector2 ObjectivePosition = new(20f, -18f);
        public static readonly Vector2 ObjectiveSize = new(760f, 34f);
        public static readonly Vector2 ChecklistPosition = new(20f, -78f);
        public static readonly Vector2 ChecklistSize = new(430f, 245f);
        public static readonly Vector2 ScorePosition = new(20f, 90f);
        public static readonly Vector2 ScoreSize = new(320f, 26f);
        public static readonly Vector2 PromptPosition = new(0f, 20f);
        public static readonly Vector2 PromptSize = new(900f, 34f);
        public static readonly Vector2 MessagePosition = new(-20f, -250f);
        public static readonly Vector2 MessageSize = new(580f, 170f);
        public static readonly Vector2 CrosshairSize = new(28f, 28f);

        public static readonly Vector2 GuidePanelPosition = new(-20f, 20f);
        public static readonly Vector2 GuidePanelSize = new(412f, 220f);
        public static readonly Vector2 GuideTitlePosition = new(18f, -16f);
        public static readonly Vector2 GuideTitleSize = new(190f, 34f);
        public static readonly Vector2 GuideTextPosition = new(18f, -60f);
        public static readonly Vector2 GuideTextSize = new(-36f, -72f);

        public static readonly Vector2 FinalPanelPosition = Vector2.zero;
        public static readonly Vector2 FinalPanelSize = new(760f, 430f);
        public static readonly Vector2 FinalTitlePosition = new(0f, -20f);
        public static readonly Vector2 FinalTitleSize = new(520f, 36f);
        public static readonly Vector2 FinalTextPosition = new(24f, -70f);
        public static readonly Vector2 FinalTextSize = new(712f, 250f);
        public static readonly Vector2 ResetButtonPosition = new(-118f, 28f);
        public static readonly Vector2 ResetButtonSize = new(220f, 50f);
        public static readonly Vector2 QuitButtonPosition = new(118f, 28f);
        public static readonly Vector2 QuitButtonSize = new(180f, 50f);
        public static readonly Vector2 ButtonLabelSize = new(-12f, -10f);
    }
}
