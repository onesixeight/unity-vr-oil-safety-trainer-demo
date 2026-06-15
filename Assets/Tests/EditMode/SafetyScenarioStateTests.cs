using NUnit.Framework;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace OilSafetyTrainer.Tests
{
    public class SafetyScenarioStateTests
    {
        [Test]
        public void CanEnterWorkZoneRequiresAllRequiredPpe()
        {
            var state = new SafetyScenarioState(
                new[] { "helmet", "goggles", "gloves" },
                new[] { "spill" },
                baseScore: 100,
                missingPpePenalty: 15);

            state.EquipPpe("helmet");
            state.EquipPpe("goggles");

            Assert.False(state.HasAllRequiredPpe);
            CollectionAssert.AreEquivalent(new[] { "gloves" }, state.GetMissingPpe());

            state.EquipPpe("gloves");

            Assert.True(state.HasAllRequiredPpe);
            Assert.IsEmpty(state.GetMissingPpe());
        }

        [Test]
        public void InspectingHazardOnlyScoresOnce()
        {
            var state = new SafetyScenarioState(
                new[] { "helmet" },
                new[] { "spill", "hot_pipe" },
                baseScore: 100,
                missingPpePenalty: 15);

            Assert.True(state.InspectHazard("spill"));
            Assert.False(state.InspectHazard("spill"));

            Assert.AreEqual(1, state.InspectedHazardCount);
            Assert.AreEqual(1, state.RemainingHazardCount);
        }

        [Test]
        public void MissingPpeAttemptAppliesPenaltyOncePerMissingItem()
        {
            var state = new SafetyScenarioState(
                new[] { "helmet", "gloves" },
                new[] { "spill" },
                baseScore: 100,
                missingPpePenalty: 20);

            state.RegisterMissingPpeAttempt();
            state.RegisterMissingPpeAttempt();

            Assert.AreEqual(60, state.CalculateScore());
            CollectionAssert.AreEquivalent(new[] { "helmet", "gloves" }, state.PenalizedMissingPpe);
        }

        [Test]
        public void ResetClearsProgressAndPenalties()
        {
            var state = new SafetyScenarioState(
                new[] { "helmet" },
                new[] { "spill" },
                baseScore: 100,
                missingPpePenalty: 20);

            state.EquipPpe("helmet");
            state.InspectHazard("spill");
            state.RegisterMissingPpeAttempt();

            state.Reset();

            Assert.False(state.HasAllRequiredPpe);
            Assert.AreEqual(0, state.InspectedHazardCount);
            Assert.AreEqual(1, state.RemainingHazardCount);
            Assert.AreEqual(100, state.CalculateScore());
        }

        [Test]
        public void QuitDemoInvokesQuitRequestHandler()
        {
            OpenIsolatedScene();
            var gameObject = new GameObject("Scenario Manager Test");
            LogAssert.Expect(LogType.Error, new Regex("^SafetyScenarioManager: scorePanel is not assigned"));
            var manager = gameObject.AddComponent<SafetyScenarioManager>();
            InvokeLifecycle(manager, "Awake");
            var wasCalled = false;
            var originalHandler = SafetyScenarioManager.QuitRequestHandler;

            try
            {
                SafetyScenarioManager.QuitRequestHandler = () => wasCalled = true;
                manager.QuitDemo();
                Assert.True(wasCalled);
            }
            finally
            {
                SafetyScenarioManager.QuitRequestHandler = originalHandler;
                Object.DestroyImmediate(gameObject);
            }
        }

        [Test]
        public void DestroyingCurrentManagerClearsInstance()
        {
            OpenIsolatedScene();
            var gameObject = new GameObject("Scenario Manager Instance Test");
            LogAssert.Expect(LogType.Error, new Regex("^SafetyScenarioManager: scorePanel is not assigned"));
            var manager = gameObject.AddComponent<SafetyScenarioManager>();
            InvokeLifecycle(manager, "Awake");

            Object.DestroyImmediate(gameObject);

            Assert.IsTrue(SafetyScenarioManager.Instance == null);
        }

        [Test]
        public void MissingScorePanelLogsExplicitError()
        {
            OpenIsolatedScene();
            var gameObject = new GameObject("Scenario Manager Missing UI Test");
            LogAssert.Expect(LogType.Error, new Regex("^SafetyScenarioManager: scorePanel is not assigned"));

            var manager = gameObject.AddComponent<SafetyScenarioManager>();
            InvokeLifecycle(manager, "Awake");

            Object.DestroyImmediate(gameObject);
        }

        private static void OpenIsolatedScene()
        {
            EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        }

        private static void InvokeLifecycle(MonoBehaviour behaviour, string methodName)
        {
            var method = behaviour.GetType().GetMethod(methodName, BindingFlags.Instance | BindingFlags.NonPublic);
            Assert.NotNull(method, $"Could not find lifecycle method {methodName} on {behaviour.GetType().Name}.");
            method.Invoke(behaviour, null);
        }
    }
}
