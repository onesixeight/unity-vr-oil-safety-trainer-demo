using System.Reflection;
using System.Linq;
using NUnit.Framework;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace OilSafetyTrainer.Tests
{
    public class SafetyTrainerGameplayTests
    {
        [Test]
        public void EnteringWorkZoneWithoutPpeAppliesPenaltyAndDeniesAccess()
        {
            var manager = LoadInitializedScenario();

            Assert.False(manager.TryEnterWorkZone());
            Assert.AreEqual(52, manager.State.CalculateScore());
        }

        [Test]
        public void FullWalkthroughAwardsPerfectScoreAndShowsFinalPanel()
        {
            var manager = LoadInitializedScenario();
            var playerRig = Object.FindAnyObjectByType<PlayerRig>();

            foreach (var station in Object.FindObjectsByType<PpeStation>(FindObjectsInactive.Exclude))
            {
                station.Interact(playerRig);
            }

            Assert.True(manager.TryEnterWorkZone());

            foreach (var hazard in Object.FindObjectsByType<HazardInspectionPoint>(FindObjectsInactive.Exclude))
            {
                hazard.Interact(playerRig);
            }

            manager.CompleteScenario();

            var scorePanel = Object.FindAnyObjectByType<ScorePanelController>();
            Assert.NotNull(scorePanel);
            Assert.True(scorePanel.IsFinalVisible);

            StringAssert.Contains("100/100", scorePanel.FinalResultText);
            StringAssert.Contains("зачёт", scorePanel.FinalResultText);
        }

        [Test]
        public void ResetScenarioClearsProgressAndHidesFinalPanel()
        {
            var manager = LoadInitializedScenario();
            var playerRig = Object.FindAnyObjectByType<PlayerRig>();

            Object.FindAnyObjectByType<PpeStation>().Interact(playerRig);
            Object.FindAnyObjectByType<HazardInspectionPoint>().Interact(playerRig);
            manager.CompleteScenario();

            manager.ResetScenario();

            var scorePanel = Object.FindAnyObjectByType<ScorePanelController>();
            Assert.NotNull(scorePanel);
            Assert.False(scorePanel.IsFinalVisible);
            Assert.AreEqual(0, manager.State.EquippedPpe.Count);
            Assert.AreEqual(0, manager.State.InspectedHazardCount);
            Assert.AreEqual(100, manager.State.CalculateScore());
        }

        [Test]
        public void ResetScenarioRestoresPlayerViewPitch()
        {
            var manager = LoadInitializedScenario();
            var playerRig = Object.FindAnyObjectByType<PlayerRig>();
            Assert.NotNull(playerRig);

            playerRig.Head.localRotation = Quaternion.Euler(65f, 0f, 0f);

            manager.ResetScenario();

            Assert.That(Quaternion.Angle(Quaternion.identity, playerRig.Head.localRotation), Is.LessThan(0.1f));
        }

        [Test]
        public void EquippedPpeKeepsSelectedColorAfterHoverEnds()
        {
            LoadInitializedScenario();
            var playerRig = Object.FindAnyObjectByType<PlayerRig>();
            var station = Object.FindObjectsByType<PpeStation>(FindObjectsInactive.Exclude)
                .First(item => item.PpeId == "gloves");
            var renderer = GetBoardRenderer(station.gameObject);

            station.SetHighlighted(true);
            station.Interact(playerRig);

            station.SetHighlighted(false);

            AssertColorApproximately(new Color(0.1f, 0.6f, 0.24f), GetRenderedColor(renderer), "Equipped PPE lost its selected color after hover ended.");
        }

        [Test]
        public void InspectedHazardKeepsInspectedColorAfterHoverEnds()
        {
            LoadInitializedScenario();
            var playerRig = Object.FindAnyObjectByType<PlayerRig>();
            var hazard = Object.FindObjectsByType<HazardInspectionPoint>(FindObjectsInactive.Exclude)
                .First(item => item.HazardId == "hot_pipe");
            var renderer = GetBoardRenderer(hazard.gameObject);

            hazard.SetHighlighted(true);
            hazard.Interact(playerRig);

            hazard.SetHighlighted(false);

            AssertColorApproximately(new Color(0.1f, 0.5f, 0.95f), GetRenderedColor(renderer), "Inspected hazard lost its status color after hover ended.");
        }

        [Test]
        public void RepeatingPpeInteractionDoesNotChangeProgressOrScore()
        {
            var manager = LoadInitializedScenario();
            var playerRig = Object.FindAnyObjectByType<PlayerRig>();
            var station = Object.FindObjectsByType<PpeStation>(FindObjectsInactive.Exclude)
                .First(item => item.PpeId == "helmet");

            station.Interact(playerRig);
            var scoreAfterFirstInteraction = manager.State.CalculateScore();
            var equippedAfterFirstInteraction = manager.State.EquippedPpe.Count;

            station.Interact(playerRig);

            Assert.AreEqual(scoreAfterFirstInteraction, manager.State.CalculateScore());
            Assert.AreEqual(equippedAfterFirstInteraction, manager.State.EquippedPpe.Count);
            Assert.True(manager.State.EquippedPpe.Contains("helmet"));
        }

        [Test]
        public void RepeatingHazardInteractionDoesNotDoubleCountProgressOrPenalty()
        {
            var manager = LoadInitializedScenario();
            var playerRig = Object.FindAnyObjectByType<PlayerRig>();
            var hazard = Object.FindObjectsByType<HazardInspectionPoint>(FindObjectsInactive.Exclude)
                .First(item => item.HazardId == "oil_spill");

            hazard.Interact(playerRig);
            var scoreAfterFirstInteraction = manager.State.CalculateScore(includeRemainingHazardPenalty: true);
            var inspectedAfterFirstInteraction = manager.State.InspectedHazardCount;

            hazard.Interact(playerRig);

            Assert.AreEqual(scoreAfterFirstInteraction, manager.State.CalculateScore(includeRemainingHazardPenalty: true));
            Assert.AreEqual(inspectedAfterFirstInteraction, manager.State.InspectedHazardCount);
            Assert.True(manager.State.InspectedHazards.Contains("oil_spill"));
        }

        private static SafetyScenarioManager LoadInitializedScenario()
        {
            EditorSceneManager.OpenScene("Assets/Scenes/OilSafetyTrainerDemo.unity");

            var manager = Object.FindAnyObjectByType<SafetyScenarioManager>();
            Assert.NotNull(manager);

            InvokeLifecycle(manager, "Awake");
            InvokeLifecycle(manager, "Start");
            return manager;
        }

        private static void InvokeLifecycle(MonoBehaviour behaviour, string methodName)
        {
            var method = behaviour.GetType().GetMethod(methodName, BindingFlags.Instance | BindingFlags.NonPublic);
            Assert.NotNull(method, $"Could not find lifecycle method {methodName} on {behaviour.GetType().Name}.");
            method.Invoke(behaviour, null);
        }

        private static Renderer GetBoardRenderer(GameObject item)
        {
            var renderer = item.GetComponentsInChildren<Renderer>()
                .FirstOrDefault(candidate => candidate.name.EndsWith("Board"));
            Assert.NotNull(renderer, $"Could not find board renderer under {item.name}.");
            return renderer;
        }

        private static void AssertColorApproximately(Color expected, Color actual, string message)
        {
            Assert.That(actual.r, Is.EqualTo(expected.r).Within(0.002f), message);
            Assert.That(actual.g, Is.EqualTo(expected.g).Within(0.002f), message);
            Assert.That(actual.b, Is.EqualTo(expected.b).Within(0.002f), message);
            Assert.That(actual.a, Is.EqualTo(expected.a).Within(0.002f), message);
        }

        private static Color GetRenderedColor(Renderer renderer)
        {
            var block = new MaterialPropertyBlock();
            renderer.GetPropertyBlock(block);
            var blockColor = block.GetColor("_Color");
            return blockColor.a > 0f ? blockColor : renderer.sharedMaterial.color;
        }
    }
}
