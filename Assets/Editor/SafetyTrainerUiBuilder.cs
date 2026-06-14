using OilSafetyTrainer;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace OilSafetyTrainer.Editor
{
    internal static class SafetyTrainerUiBuilder
    {
        public static ScorePanelController Create()
        {
            CreateEventSystem();

            var canvasObject = new GameObject("Safety Trainer UI");
            var canvas = canvasObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            var scaler = canvasObject.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920f, 1080f);
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            canvasObject.AddComponent<GraphicRaycaster>();

            var font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            var objective = SafetyTrainerPrimitiveFactory.CreateText("Objective", canvasObject.transform, font, string.Empty, 20, TextAnchor.UpperLeft, new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(20f, -18f), new Vector2(760f, 34f), new Vector2(0f, 1f), true, 16, 20);
            var checklist = SafetyTrainerPrimitiveFactory.CreateText("Checklist", canvasObject.transform, font, string.Empty, 15, TextAnchor.UpperLeft, new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(20f, -78f), new Vector2(430f, 245f), new Vector2(0f, 1f), true, 12, 15);
            var score = SafetyTrainerPrimitiveFactory.CreateText("Score", canvasObject.transform, font, string.Empty, 16, TextAnchor.LowerLeft, new Vector2(0f, 0f), new Vector2(0f, 0f), new Vector2(20f, 90f), new Vector2(320f, 26f), new Vector2(0f, 0f), true, 13, 16);
            var prompt = SafetyTrainerPrimitiveFactory.CreateText("Prompt", canvasObject.transform, font, string.Empty, 18, TextAnchor.LowerCenter, new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(0f, 20f), new Vector2(900f, 34f), new Vector2(0.5f, 0f), true, 14, 18);
            var message = SafetyTrainerPrimitiveFactory.CreateText("Message", canvasObject.transform, font, string.Empty, 18, TextAnchor.UpperRight, new Vector2(1f, 1f), new Vector2(1f, 1f), new Vector2(-20f, -250f), new Vector2(580f, 170f), new Vector2(1f, 1f), true, 15, 18);
            SafetyTrainerPrimitiveFactory.CreateText("Crosshair", canvasObject.transform, font, "+", 24, TextAnchor.MiddleCenter, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(28f, 28f), new Vector2(0.5f, 0.5f));

            var guidePanel = SafetyTrainerPrimitiveFactory.CreatePanel("Guide Panel", canvasObject.transform, new Vector2(1f, 0f), new Vector2(1f, 0f), new Vector2(-20f, 20f), new Vector2(412f, 220f), new Color(0.02f, 0.06f, 0.08f, 0.94f), new Vector2(1f, 0f));
            var guideGroup = guidePanel.AddComponent<CanvasGroup>();
            SafetyTrainerPrimitiveFactory.CreateText("Guide Title", guidePanel.transform, font, "Памятка", 22, TextAnchor.UpperLeft, new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(18f, -16f), new Vector2(190f, 34f), new Vector2(0f, 1f), true, 18, 22);
            var guideText = SafetyTrainerPrimitiveFactory.CreateText("Guide Text", guidePanel.transform, font, string.Empty, 14, TextAnchor.UpperLeft, new Vector2(0f, 0f), new Vector2(1f, 1f), new Vector2(18f, -60f), new Vector2(-36f, -72f), new Vector2(0f, 1f), true, 11, 14);

            var finalPanel = SafetyTrainerPrimitiveFactory.CreatePanel("Final Panel", canvasObject.transform, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0f, 0f), new Vector2(760f, 430f), new Color(0f, 0f, 0f, 0.94f), new Vector2(0.5f, 0.5f));
            var group = finalPanel.AddComponent<CanvasGroup>();
            SafetyTrainerPrimitiveFactory.CreateText("Final Title", finalPanel.transform, font, "Результаты обхода", 27, TextAnchor.UpperCenter, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -20f), new Vector2(520f, 36f), new Vector2(0.5f, 1f), true, 24, 27);
            var finalText = SafetyTrainerPrimitiveFactory.CreateText("Final Text", finalPanel.transform, font, string.Empty, 20, TextAnchor.UpperLeft, new Vector2(0f, 1f), new Vector2(0f, 1f), new Vector2(24f, -70f), new Vector2(712f, 250f), new Vector2(0f, 1f), true, 15, 20);
            var resetButton = CreateButton("Reset Button", finalPanel.transform, font, "Начать заново", new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(-118f, 28f), new Vector2(220f, 50f), new Color(0.12f, 0.5f, 0.22f, 0.96f), new Vector2(0.5f, 0f));
            var quitButton = CreateButton("Quit Button", finalPanel.transform, font, "Выход", new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(118f, 28f), new Vector2(180f, 50f), new Color(0.5f, 0.12f, 0.12f, 0.96f), new Vector2(0.5f, 0f));

            var controller = canvasObject.AddComponent<ScorePanelController>();
            controller.Bind(objective, checklist, prompt, message, score, finalText, guideText, group, guideGroup, resetButton, quitButton);
            return controller;
        }

        private static Button CreateButton(string name, Transform parent, Font font, string label, Vector2 anchorMin, Vector2 anchorMax, Vector2 anchoredPosition, Vector2 sizeDelta, Color color, Vector2 pivot)
        {
            var buttonObject = SafetyTrainerPrimitiveFactory.CreatePanel(name, parent, anchorMin, anchorMax, anchoredPosition, sizeDelta, color, pivot);
            var button = buttonObject.AddComponent<Button>();
            var colors = button.colors;
            colors.normalColor = Color.white;
            colors.highlightedColor = new Color(0.95f, 0.95f, 0.95f, 1f);
            colors.pressedColor = new Color(0.82f, 0.82f, 0.82f, 1f);
            colors.selectedColor = colors.highlightedColor;
            button.colors = colors;
            SafetyTrainerPrimitiveFactory.CreateText("Label", buttonObject.transform, font, label, 18, TextAnchor.MiddleCenter, Vector2.zero, Vector2.one, Vector2.zero, new Vector2(-12f, -10f), new Vector2(0.5f, 0.5f), true, 14, 18);
            return button;
        }

        private static void CreateEventSystem()
        {
            if (Object.FindAnyObjectByType<EventSystem>() != null)
            {
                return;
            }

            var eventSystemObject = new GameObject("EventSystem");
            eventSystemObject.AddComponent<EventSystem>();
#if ENABLE_INPUT_SYSTEM
            eventSystemObject.AddComponent<UnityEngine.InputSystem.UI.InputSystemUIInputModule>();
#else
            eventSystemObject.AddComponent<StandaloneInputModule>();
#endif
        }
    }
}
