using System.Reflection;
using NUnit.Framework;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

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

            var finalText = (Text)typeof(ScorePanelController)
                .GetField("finalText", BindingFlags.Instance | BindingFlags.NonPublic)
                .GetValue(scorePanel);
            StringAssert.Contains("100/100", finalText.text);
            StringAssert.Contains("зачёт", finalText.text);
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
    }
}
