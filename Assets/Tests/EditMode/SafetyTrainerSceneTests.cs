using NUnit.Framework;
using UnityEditor.SceneManagement;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace OilSafetyTrainer.Tests
{
    public class SafetyTrainerSceneTests
    {
        [Test]
        public void GeneratedSceneContainsMvpTrainerParts()
        {
            EditorSceneManager.OpenScene("Assets/Scenes/OilSafetyTrainerDemo.unity");

            var manager = Object.FindAnyObjectByType<SafetyScenarioManager>();
            Assert.NotNull(manager);
            Assert.NotNull(Object.FindAnyObjectByType<DesktopPlayerController>());
            var scorePanel = Object.FindAnyObjectByType<ScorePanelController>();
            Assert.NotNull(scorePanel);
            Assert.NotNull(Object.FindAnyObjectByType<FinalAssessmentStation>());
            Assert.NotNull(Object.FindAnyObjectByType<WorkZoneGate>());
            Assert.NotNull(Object.FindAnyObjectByType<EventSystem>());
            Assert.True(scorePanel.HasGuidePanel);
            Assert.True(scorePanel.HasFinalActions);
            Assert.AreEqual(4, manager.requiredPpe.Length);
            Assert.AreEqual(5, manager.hazards.Length);
            Assert.AreEqual(4, Object.FindObjectsByType<PpeStation>(FindObjectsInactive.Exclude).Length);
            Assert.AreEqual(5, Object.FindObjectsByType<HazardInspectionPoint>(FindObjectsInactive.Exclude).Length);
        }

        [Test]
        public void ImportedArtDisplaysAreBoundIntoTheScene()
        {
            EditorSceneManager.OpenScene("Assets/Scenes/OilSafetyTrainerDemo.unity");

            AssertDisplayHasTexture("Checkpoint Desk Display");
            AssertDisplayHasTexture("Checkpoint Poster Display");
            AssertDisplayHasTexture("Work Zone Poster Display");
            AssertDisplayHasTexture("Terminal Screen Display");
            AssertDisplayHasTexture("PPE Helmet Placard Display");
            AssertDisplayHasTexture("PPE Goggles Placard Display");
            AssertDisplayHasTexture("PPE Gloves Placard Display");
            AssertDisplayHasTexture("PPE Boots Placard Display");
            AssertDisplayHasTexture("Hazard Reference Guardrail");
            AssertDisplayHasTexture("Hazard Reference Oil Spill");
            AssertDisplayHasTexture("Hazard Reference Hot Pipe");
            AssertDisplayHasTexture("Hazard Reference Gas Warning");
            AssertDisplayHasTexture("Hazard Reference Unsafe Valve");
        }

        [Test]
        public void PlayerStartHasClearSpaceInFrontOfView()
        {
            EditorSceneManager.OpenScene("Assets/Scenes/OilSafetyTrainerDemo.unity");

            var rig = Object.FindAnyObjectByType<PlayerRig>();
            Assert.NotNull(rig);

            var probeCenter = rig.Head.position + (rig.Head.forward * 0.9f);
            var nearbyColliders = Physics.OverlapSphere(probeCenter, 0.45f, ~0, QueryTriggerInteraction.Ignore)
                .Where(collider => !collider.transform.IsChildOf(rig.transform))
                .Select(collider => collider.name)
                .Distinct()
                .ToArray();

            Assert.IsEmpty(
                nearbyColliders,
                $"Player start view is obstructed by nearby geometry: {string.Join(", ", nearbyColliders)}");
        }

        [Test]
        public void PlayerStartStandsOnWalkableSurface()
        {
            EditorSceneManager.OpenScene("Assets/Scenes/OilSafetyTrainerDemo.unity");

            var rig = Object.FindAnyObjectByType<PlayerRig>();
            Assert.NotNull(rig);

            var origin = rig.transform.position + Vector3.up * 0.5f;
            var hasGround = Physics.Raycast(origin, Vector3.down, out var hit, 1.2f, ~0, QueryTriggerInteraction.Ignore);

            Assert.IsTrue(hasGround, "Player start has no ground support beneath the character.");
            Assert.GreaterOrEqual(hit.normal.y, 0.85f, $"Player start stands on a non-walkable surface: {hit.collider.name}");
        }

        [Test]
        public void WorkZoneGateCoversCorridorBetweenFences()
        {
            EditorSceneManager.OpenScene("Assets/Scenes/OilSafetyTrainerDemo.unity");

            var gate = Object.FindAnyObjectByType<WorkZoneGate>();
            var leftFence = GameObject.Find("Left Fence");
            var rightFence = GameObject.Find("Right Fence");

            Assert.NotNull(gate);
            Assert.NotNull(leftFence);
            Assert.NotNull(rightFence);

            var gateBounds = gate.GetComponent<BoxCollider>().bounds;
            var leftFenceBounds = leftFence.GetComponent<BoxCollider>().bounds;
            var rightFenceBounds = rightFence.GetComponent<BoxCollider>().bounds;

            Assert.LessOrEqual(
                gateBounds.min.x,
                leftFenceBounds.max.x + 0.5f,
                "The PPE gate leaves a bypass gap on the left side.");
            Assert.GreaterOrEqual(
                gateBounds.max.x,
                rightFenceBounds.min.x - 0.5f,
                "The PPE gate leaves a bypass gap on the right side.");
        }

        [Test]
        public void GuidePanelUsesCompactViewportFriendlySize()
        {
            EditorSceneManager.OpenScene("Assets/Scenes/OilSafetyTrainerDemo.unity");

            var guidePanel = GameObject.Find("Guide Panel");
            Assert.NotNull(guidePanel);

            var rect = guidePanel.GetComponent<RectTransform>();
            Assert.LessOrEqual(rect.sizeDelta.x, 420f, "Guide panel is too wide for the first viewport.");
            Assert.LessOrEqual(rect.sizeDelta.y, 220f, "Guide panel is too tall for the first viewport.");
        }

        [Test]
        public void HudElementsStayInsideCanvasBounds()
        {
            EditorSceneManager.OpenScene("Assets/Scenes/OilSafetyTrainerDemo.unity");

            var canvasRect = GameObject.Find("Safety Trainer UI").GetComponent<RectTransform>();
            AssertRectInsideParent(canvasRect, "Objective");
            AssertRectInsideParent(canvasRect, "Checklist");
            AssertRectInsideParent(canvasRect, "Score");
            AssertRectInsideParent(canvasRect, "Prompt");
            AssertRectInsideParent(canvasRect, "Message");
            AssertRectInsideParent(canvasRect, "Guide Panel");
            AssertRectInsideParent(canvasRect, "Final Panel");
        }

        [Test]
        public void RepresentativeRussianTextsFitTheirUiAreas()
        {
            EditorSceneManager.OpenScene("Assets/Scenes/OilSafetyTrainerDemo.unity");

            AssertTextFits("Objective", "VR-тренажёр: безопасный обход нефтедобывающей площадки");
            AssertTextFits("Checklist",
                "СИЗ перед входом:\n[x] Каска\n[x] Защитные очки\n[x] Перчатки\n[x] Диэлектрические ботинки\n\n" +
                "Опасности на площадке:\n[x] Разрыв ограждения\n[x] Разлив нефти/масла\n[x] Горячая поверхность трубопровода\n[x] Сигнал газоанализатора\n[x] Открытый/непромаркированный клапан");
            AssertTextFits("Score", "Текущая оценка: 100/100");
            AssertTextFits("Prompt", "WASD - движение | Мышь - обзор | E - действие | H - памятка | R - сброс | Q - выход");
            AssertTextFits("Message", "Опасность выявлена: Горячая поверхность трубопровода. Оградите зону и дождитесь охлаждения оборудования.");
            AssertTextFits("Guide Text",
                "1. Наденьте 4 обязательных СИЗ у КПП.\n2. Пройдите рамку допуска в рабочую зону.\n" +
                "3. Найдите 5 опасностей и отметьте их клавишей E.\n4. Завершите обход у терминала оценки.\n\n" +
                "Управление: WASD - движение, мышь - обзор.\nE - действие, H - памятка, R - сброс, Q - выход.");
            AssertTextFits("Final Text",
                "Итог тренажёра\nОценка: 76/100\nСИЗ: 3/4\nОпасности: 3/5\nНе хватает СИЗ: Диэлектрические ботинки\n" +
                "Не найдены опасности: Разлив нефти/масла, Сигнал газоанализатора\n\nРезультат: повторите инструктаж.\n" +
                "R - начать заново | Q - выйти | мышью можно нажать кнопки ниже");
        }

        private static void AssertRectInsideParent(RectTransform parentRect, string childName, float tolerance = 0.5f)
        {
            var childRect = GameObject.Find(childName).GetComponent<RectTransform>();
            Assert.NotNull(childRect, $"Missing UI element: {childName}");

            var parentCorners = new Vector3[4];
            var childCorners = new Vector3[4];
            parentRect.GetWorldCorners(parentCorners);
            childRect.GetWorldCorners(childCorners);

            Assert.GreaterOrEqual(childCorners[0].x, parentCorners[0].x - tolerance, $"{childName} overflows left edge of the canvas.");
            Assert.GreaterOrEqual(childCorners[0].y, parentCorners[0].y - tolerance, $"{childName} overflows bottom edge of the canvas.");
            Assert.LessOrEqual(childCorners[2].x, parentCorners[2].x + tolerance, $"{childName} overflows right edge of the canvas.");
            Assert.LessOrEqual(childCorners[2].y, parentCorners[2].y + tolerance, $"{childName} overflows top edge of the canvas.");
        }

        private static void AssertTextFits(string objectName, string sampleText, float tolerance = 4f)
        {
            var text = GameObject.Find(objectName).GetComponent<Text>();
            Assert.NotNull(text, $"Missing text element: {objectName}");

            text.text = sampleText;
            Canvas.ForceUpdateCanvases();

            var rect = text.rectTransform.rect.size;
            var settings = text.GetGenerationSettings(rect);
            var preferredHeight = text.cachedTextGeneratorForLayout.GetPreferredHeight(sampleText, settings) / text.pixelsPerUnit;

            Assert.LessOrEqual(
                preferredHeight,
                rect.y + tolerance,
                $"{objectName} cannot fit representative text. Preferred height {preferredHeight:F1} exceeds rect height {rect.y:F1}.");
        }

        private static void AssertDisplayHasTexture(string objectName)
        {
            var display = GameObject.Find(objectName);
            Assert.NotNull(display, $"Missing display object: {objectName}");

            var renderer = display.GetComponent<Renderer>();
            Assert.NotNull(renderer, $"Display object has no renderer: {objectName}");
            Assert.NotNull(renderer.sharedMaterial, $"Display object has no material: {objectName}");
            Assert.NotNull(renderer.sharedMaterial.mainTexture, $"Display object has no texture assigned: {objectName}");
        }
    }
}
