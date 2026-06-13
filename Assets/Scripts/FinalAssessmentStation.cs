using UnityEngine;

namespace OilSafetyTrainer
{
    public sealed class FinalAssessmentStation : InteractableItem
    {
        public override string GetPrompt()
        {
            return "E - завершить обход и получить оценку";
        }

        public override void Interact(PlayerRig playerRig)
        {
            SafetyScenarioManager.Instance?.CompleteScenario();
        }
    }
}
