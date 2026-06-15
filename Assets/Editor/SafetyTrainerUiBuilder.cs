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
            scaler.referenceResolution = SafetyTrainerUiLayout.ReferenceResolution;
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            canvasObject.AddComponent<GraphicRaycaster>();

            var font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            var objective = SafetyTrainerPrimitiveFactory.CreateText("Objective", canvasObject.transform, font, string.Empty, 20, TextAnchor.UpperLeft, SafetyTrainerUiLayout.TopLeftAnchor, SafetyTrainerUiLayout.TopLeftAnchor, SafetyTrainerUiLayout.ObjectivePosition, SafetyTrainerUiLayout.ObjectiveSize, SafetyTrainerUiLayout.TopLeftAnchor, true, 16, 20);
            var checklist = SafetyTrainerPrimitiveFactory.CreateText("Checklist", canvasObject.transform, font, string.Empty, 15, TextAnchor.UpperLeft, SafetyTrainerUiLayout.TopLeftAnchor, SafetyTrainerUiLayout.TopLeftAnchor, SafetyTrainerUiLayout.ChecklistPosition, SafetyTrainerUiLayout.ChecklistSize, SafetyTrainerUiLayout.TopLeftAnchor, true, 12, 15);
            var score = SafetyTrainerPrimitiveFactory.CreateText("Score", canvasObject.transform, font, string.Empty, 16, TextAnchor.LowerLeft, SafetyTrainerUiLayout.BottomLeftAnchor, SafetyTrainerUiLayout.BottomLeftAnchor, SafetyTrainerUiLayout.ScorePosition, SafetyTrainerUiLayout.ScoreSize, SafetyTrainerUiLayout.BottomLeftAnchor, true, 13, 16);
            var prompt = SafetyTrainerPrimitiveFactory.CreateText("Prompt", canvasObject.transform, font, string.Empty, 18, TextAnchor.LowerCenter, SafetyTrainerUiLayout.BottomCenterAnchor, SafetyTrainerUiLayout.BottomCenterAnchor, SafetyTrainerUiLayout.PromptPosition, SafetyTrainerUiLayout.PromptSize, SafetyTrainerUiLayout.BottomCenterAnchor, true, 14, 18);
            var message = SafetyTrainerPrimitiveFactory.CreateText("Message", canvasObject.transform, font, string.Empty, 18, TextAnchor.UpperRight, SafetyTrainerUiLayout.TopRightAnchor, SafetyTrainerUiLayout.TopRightAnchor, SafetyTrainerUiLayout.MessagePosition, SafetyTrainerUiLayout.MessageSize, SafetyTrainerUiLayout.TopRightAnchor, true, 15, 18);
            SafetyTrainerPrimitiveFactory.CreateText("Crosshair", canvasObject.transform, font, "+", 24, TextAnchor.MiddleCenter, SafetyTrainerUiLayout.CenterAnchor, SafetyTrainerUiLayout.CenterAnchor, Vector2.zero, SafetyTrainerUiLayout.CrosshairSize, SafetyTrainerUiLayout.CenterAnchor);

            var guidePanel = SafetyTrainerPrimitiveFactory.CreatePanel("Guide Panel", canvasObject.transform, SafetyTrainerUiLayout.BottomRightAnchor, SafetyTrainerUiLayout.BottomRightAnchor, SafetyTrainerUiLayout.GuidePanelPosition, SafetyTrainerUiLayout.GuidePanelSize, new Color(0.02f, 0.06f, 0.08f, 0.94f), SafetyTrainerUiLayout.BottomRightAnchor);
            var guideGroup = guidePanel.AddComponent<CanvasGroup>();
            SafetyTrainerPrimitiveFactory.CreateText("Guide Title", guidePanel.transform, font, "Памятка", 22, TextAnchor.UpperLeft, SafetyTrainerUiLayout.TopLeftAnchor, SafetyTrainerUiLayout.TopLeftAnchor, SafetyTrainerUiLayout.GuideTitlePosition, SafetyTrainerUiLayout.GuideTitleSize, SafetyTrainerUiLayout.TopLeftAnchor, true, 18, 22);
            var guideText = SafetyTrainerPrimitiveFactory.CreateText("Guide Text", guidePanel.transform, font, string.Empty, 14, TextAnchor.UpperLeft, SafetyTrainerUiLayout.StretchAnchorMin, SafetyTrainerUiLayout.StretchAnchorMax, SafetyTrainerUiLayout.GuideTextPosition, SafetyTrainerUiLayout.GuideTextSize, SafetyTrainerUiLayout.TopLeftAnchor, true, 11, 14);

            var finalPanel = SafetyTrainerPrimitiveFactory.CreatePanel("Final Panel", canvasObject.transform, SafetyTrainerUiLayout.CenterAnchor, SafetyTrainerUiLayout.CenterAnchor, SafetyTrainerUiLayout.FinalPanelPosition, SafetyTrainerUiLayout.FinalPanelSize, new Color(0f, 0f, 0f, 0.94f), SafetyTrainerUiLayout.CenterAnchor);
            var group = finalPanel.AddComponent<CanvasGroup>();
            SafetyTrainerPrimitiveFactory.CreateText("Final Title", finalPanel.transform, font, "Результаты обхода", 27, TextAnchor.UpperCenter, new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), SafetyTrainerUiLayout.FinalTitlePosition, SafetyTrainerUiLayout.FinalTitleSize, new Vector2(0.5f, 1f), true, 24, 27);
            var finalText = SafetyTrainerPrimitiveFactory.CreateText("Final Text", finalPanel.transform, font, string.Empty, 20, TextAnchor.UpperLeft, SafetyTrainerUiLayout.TopLeftAnchor, SafetyTrainerUiLayout.TopLeftAnchor, SafetyTrainerUiLayout.FinalTextPosition, SafetyTrainerUiLayout.FinalTextSize, SafetyTrainerUiLayout.TopLeftAnchor, true, 15, 20);
            var resetButton = CreateButton("Reset Button", finalPanel.transform, font, "Начать заново", SafetyTrainerUiLayout.BottomCenterAnchor, SafetyTrainerUiLayout.BottomCenterAnchor, SafetyTrainerUiLayout.ResetButtonPosition, SafetyTrainerUiLayout.ResetButtonSize, new Color(0.12f, 0.5f, 0.22f, 0.96f), SafetyTrainerUiLayout.BottomCenterAnchor);
            var quitButton = CreateButton("Quit Button", finalPanel.transform, font, "Выход", SafetyTrainerUiLayout.BottomCenterAnchor, SafetyTrainerUiLayout.BottomCenterAnchor, SafetyTrainerUiLayout.QuitButtonPosition, SafetyTrainerUiLayout.QuitButtonSize, new Color(0.5f, 0.12f, 0.12f, 0.96f), SafetyTrainerUiLayout.BottomCenterAnchor);

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
            SafetyTrainerPrimitiveFactory.CreateText("Label", buttonObject.transform, font, label, 18, TextAnchor.MiddleCenter, SafetyTrainerUiLayout.StretchAnchorMin, SafetyTrainerUiLayout.StretchAnchorMax, Vector2.zero, SafetyTrainerUiLayout.ButtonLabelSize, SafetyTrainerUiLayout.CenterAnchor, true, 14, 18);
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
