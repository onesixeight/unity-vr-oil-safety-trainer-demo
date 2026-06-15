using NUnit.Framework;
using UnityEditor.SceneManagement;
using System.IO;
using System.Linq;
using System.Text;
using TMPro;
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
            AssertDisplayHasTexture("PPE Helmet Image");
            AssertDisplayHasTexture("PPE Goggles Image");
            AssertDisplayHasTexture("PPE Gloves Image");
            AssertDisplayHasTexture("PPE Boots Image");
            AssertDisplayHasTexture("Hazard Guardrail Gap Image");
            AssertDisplayHasTexture("Oil Spill Image");
            AssertDisplayHasTexture("Hazard Hot Pipe Marker Image");
            AssertDisplayHasTexture("Hazard Gas Warning Beacon Image");
            AssertDisplayHasTexture("Hazard Unsafe Valve Marker Image");
            Assert.IsNull(GameObject.Find("Checkpoint Poster Display Backface"));
            Assert.IsNull(GameObject.Find("Work Zone Poster Display Backface"));
            Assert.IsNull(GameObject.Find("Terminal Screen Display Backface"));
            Assert.IsNull(GameObject.Find("Hazard Reference Wall"));
            Assert.IsNull(GameObject.Find("PPE Helmet Placard Display"));
            Assert.IsNull(GameObject.Find("Board Checkpoint Text"));
            Assert.IsNull(GameObject.Find("Board Work Zone Text"));
            AssertPpeHasInteractionZone("PPE Helmet");
            AssertPpeHasInteractionZone("PPE Goggles");
            AssertPpeHasInteractionZone("PPE Gloves");
            AssertPpeHasInteractionZone("PPE Boots");
            Assert.IsNull(GameObject.Find("PPE Helmet Label"));
            Assert.IsNull(GameObject.Find("PPE Goggles Label"));
            Assert.IsNull(GameObject.Find("PPE Gloves Label"));
            Assert.IsNull(GameObject.Find("PPE Boots Label"));
        }

        [Test]
        public void CheckpointPosterDisplayFacesTheCheckpointArea()
        {
            EditorSceneManager.OpenScene("Assets/Scenes/OilSafetyTrainerDemo.unity");

            var board = GameObject.Find("Checkpoint Poster Board");
            var display = GameObject.Find("Checkpoint Poster Display");

            Assert.NotNull(board);
            Assert.NotNull(display);
            Assert.Greater(
                display.transform.position.x,
                board.transform.position.x,
                "Checkpoint poster image should sit on the yard-facing side of the board.");
            AssertDisplayFrontFaces(display.transform, Vector3.right, "Checkpoint poster image should face into the checkpoint area.");
        }

        [Test]
        public void DeskAndValveImagesClearTheirSupportingGeometry()
        {
            EditorSceneManager.OpenScene("Assets/Scenes/OilSafetyTrainerDemo.unity");

            AssertDisplayBottomClears("Checkpoint Desk Display", "Checkpoint Desk", 0.02f);
            AssertDisplayBottomClears("Hazard Unsafe Valve Marker Image", "Valve Manifold Base", 0.08f);
        }

        [Test]
        public void GuardrailGapImageFacesThePlayableYard()
        {
            EditorSceneManager.OpenScene("Assets/Scenes/OilSafetyTrainerDemo.unity");

            var board = GameObject.Find("Hazard Guardrail Gap Board");
            var display = GameObject.Find("Hazard Guardrail Gap Image");

            Assert.NotNull(board);
            Assert.NotNull(display);
            Assert.Less(
                display.transform.position.z,
                board.transform.position.z,
                "Guardrail gap image should sit on the playable-yard side of the board.");
            AssertDisplayFrontFaces(display.transform, Vector3.back, "Guardrail gap image should face the playable yard.");
        }

        [Test]
        public void MainGroundCoversAllGameplayStations()
        {
            EditorSceneManager.OpenScene("Assets/Scenes/OilSafetyTrainerDemo.unity");

            var points = new[]
            {
                GameObject.Find("Desktop PlayerRig").transform.position,
                GameObject.Find("PPE Helmet").transform.position,
                GameObject.Find("PPE Boots").transform.position,
                GameObject.Find("Hazard Guardrail Gap").transform.position,
                GameObject.Find("Hazard Unsafe Valve Marker").transform.position,
                GameObject.Find("Final Assessment Terminal").transform.position,
                new Vector3(-4.2f, 0.5f, -3.2f),
                new Vector3(20.2f, 0.5f, -3.2f)
            };

            foreach (var point in points)
            {
                var origin = point + Vector3.up * 0.75f;
                var hasGround = Physics.Raycast(origin, Vector3.down, out var hit, 2f, ~0, QueryTriggerInteraction.Ignore);
                Assert.IsTrue(hasGround, $"Missing ground support near {point}.");
                Assert.GreaterOrEqual(hit.normal.y, 0.85f, $"Ground support near {point} is not walkable: {hit.collider.name}.");
            }
        }

        [Test]
        public void RuntimeAndBuilderSourceDoNotContainMojibakeRussianText()
        {
            var source = string.Join("\n", Directory
                .GetFiles("Assets", "*.cs", SearchOption.AllDirectories)
                .Select(File.ReadAllText));
            var mojibakeFragments = new[]
            {
                "Каска",
                "Нажмите",
                "Разлив",
                "Горячая",
                "Сигнал",
                "Открытый"
            }.Select(ToWindows1251Mojibake);

            foreach (var fragment in mojibakeFragments)
            {
                Assert.False(source.Contains(fragment), $"Builder source still contains mojibake text: {fragment}");
            }
        }

        [Test]
        public void PpePlacardBuilderDoesNotOverridePassedPosition()
        {
            var source = string.Join("\n", Directory
                .GetFiles("Assets/Editor", "SafetyTrainer*.cs")
                .Select(File.ReadAllText));

            StringAssert.DoesNotContain(
                "position = new Vector3(position.x",
                source,
                "CreatePpePlacard should use explicit caller-provided coordinates instead of overriding y/z internally.");
        }

        [Test]
        public void ScenarioManagerDoesNotSearchWholeSceneDuringRuntimeOperations()
        {
            var source = File.ReadAllText("Assets/Scripts/SafetyScenarioManager.cs");

            StringAssert.DoesNotContain("FindObjectsByType", source);
            StringAssert.DoesNotContain("FindAnyObjectByType", source);
        }

        [Test]
        public void InteractableSourceDoesNotRepairRussianTextAtRuntime()
        {
            var source = string.Join("\n", Directory
                .GetFiles("Assets/Scripts", "*.cs")
                .Select(File.ReadAllText));

            StringAssert.DoesNotContain("NormalizeRussianText", source);
        }

        [Test]
        public void PlayerStartHasClearSpaceInFrontOfView()
        {
            EditorSceneManager.OpenScene("Assets/Scenes/OilSafetyTrainerDemo.unity");

            var rig = Object.FindAnyObjectByType<PlayerRig>();
            Assert.NotNull(rig);

            var viewCamera = Object.FindAnyObjectByType<Camera>();
            Assert.NotNull(viewCamera);

            var blockingHits = new[] { 0.22f, 0.5f, 0.78f }
                .Select(viewportX => viewCamera.ViewportPointToRay(new Vector3(viewportX, 0.52f, 0f)))
                .Select(ray => Physics.Raycast(ray, out var hit, 2.75f, ~0, QueryTriggerInteraction.Ignore) ? hit.collider : null)
                .Where(collider => collider != null && !collider.transform.IsChildOf(rig.transform))
                .Select(collider => collider.name)
                .Distinct()
                .ToArray();

            Assert.IsEmpty(
                blockingHits,
                $"Player start view is obstructed by nearby geometry: {string.Join(", ", blockingHits)}");
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
        public void GeneratedUiUsesTextMeshProForReadableRussianText()
        {
            EditorSceneManager.OpenScene("Assets/Scenes/OilSafetyTrainerDemo.unity");

            var textObjects = new[]
            {
                "Objective",
                "Checklist",
                "Score",
                "Prompt",
                "Message",
                "Guide Title",
                "Guide Text",
                "Final Title",
                "Final Text",
                "Crosshair"
            };

            foreach (var objectName in textObjects)
            {
                var textObject = GameObject.Find(objectName);
                Assert.NotNull(textObject, $"Missing UI text object: {objectName}");
                Assert.NotNull(textObject.GetComponent<TextMeshProUGUI>(), $"{objectName} should use TextMeshProUGUI.");
                Assert.IsNull(textObject.GetComponent<Text>(), $"{objectName} should not keep legacy UnityEngine.UI.Text.");
            }

            foreach (var label in GameObject.FindObjectsByType<TextMeshProUGUI>(FindObjectsInactive.Exclude)
                .Where(item => item.name == "Label"))
            {
                Assert.IsNull(label.GetComponent<Text>(), "Button labels should not keep legacy UnityEngine.UI.Text.");
            }
        }

        [Test]
        public void RepresentativeRussianTextsFitTheirUiAreas()
        {
            EditorSceneManager.OpenScene("Assets/Scenes/OilSafetyTrainerDemo.unity");

            AssertTextFits("Objective", SafetyTrainerText.Objective);
            AssertTextFits("Checklist",
                "СИЗ перед входом:\n[x] Каска\n[x] Защитные очки\n[x] Перчатки\n[x] Диэлектрические ботинки\n\n" +
                "Опасности на площадке:\n[x] Разрыв ограждения\n[x] Разлив нефти/масла\n[x] Горячая поверхность трубопровода\n[x] Сигнал газоанализатора\n[x] Открытый/непромаркированный клапан");
            AssertTextFits("Score", "Текущая оценка: 100/100");
            AssertTextFits("Prompt", SafetyTrainerText.DefaultPrompt);
            AssertTextFits("Message", "Опасность выявлена: Горячая поверхность трубопровода. Оградите зону и дождитесь охлаждения оборудования.");
            AssertTextFits("Guide Text", SafetyTrainerText.GuideText);
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
            var text = GameObject.Find(objectName).GetComponent<TextMeshProUGUI>();
            Assert.NotNull(text, $"Missing text element: {objectName}");

            text.text = sampleText;
            text.ForceMeshUpdate();
            Canvas.ForceUpdateCanvases();

            var rect = text.rectTransform.rect.size;
            var preferredHeight = text.GetPreferredValues(sampleText, rect.x, 0f).y;

            Assert.LessOrEqual(
                preferredHeight,
                rect.y + tolerance,
                $"{objectName} cannot fit representative text. Preferred height {preferredHeight:F1} exceeds rect height {rect.y:F1}.");
        }

        private static string ToWindows1251Mojibake(string value)
        {
            return Encoding.GetEncoding(1251).GetString(Encoding.UTF8.GetBytes(value));
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

        private static void AssertDisplayFrontFaces(Transform display, Vector3 expectedFrontDirection, string message)
        {
            var quadFront = -display.forward;
            Assert.GreaterOrEqual(
                Vector3.Dot(quadFront.normalized, expectedFrontDirection.normalized),
                0.92f,
                message);
        }

        private static void AssertDisplayBottomClears(string displayName, string supportName, float clearance)
        {
            var display = GameObject.Find(displayName);
            var support = GameObject.Find(supportName);

            Assert.NotNull(display, $"Missing display object: {displayName}");
            Assert.NotNull(support, $"Missing support object: {supportName}");

            var displayBounds = display.GetComponent<Renderer>().bounds;
            var supportBounds = support.GetComponent<Renderer>().bounds;
            Assert.GreaterOrEqual(
                displayBounds.min.y,
                supportBounds.max.y + clearance,
                $"{displayName} is clipped into or hidden behind {supportName}.");
        }

        private static void AssertPpeHasInteractionZone(string stationName)
        {
            var station = GameObject.Find(stationName);
            Assert.NotNull(station, $"Missing PPE station: {stationName}");

            var collider = station.GetComponentsInChildren<BoxCollider>()
                .FirstOrDefault(item => item.name == $"{stationName} Interaction Zone");
            Assert.NotNull(collider, $"Missing PPE interaction zone: {stationName}");
            Assert.GreaterOrEqual(collider.size.x, 0.9f, $"{stationName} interaction zone is too narrow.");
            Assert.GreaterOrEqual(collider.size.y, 0.9f, $"{stationName} interaction zone is too low.");
        }
    }
}
