using NUnit.Framework;
using UnityEngine;

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
            var gameObject = new GameObject("Scenario Manager Test");
            var manager = gameObject.AddComponent<SafetyScenarioManager>();
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
    }
}
