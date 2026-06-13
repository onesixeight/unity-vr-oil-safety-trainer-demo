using UnityEngine;

namespace OilSafetyTrainer
{
    public sealed class WorkZoneGate : MonoBehaviour
    {
        [SerializeField] private Transform returnPoint;

        public void Configure(Transform fallbackPoint)
        {
            returnPoint = fallbackPoint;
        }

        private void OnTriggerEnter(Collider other)
        {
            var rig = other.GetComponentInParent<PlayerRig>();
            if (rig == null)
            {
                return;
            }

            if (SafetyScenarioManager.Instance != null && !SafetyScenarioManager.Instance.TryEnterWorkZone())
            {
                var fallback = returnPoint != null ? returnPoint : transform;
                rig.TeleportTo(fallback.position, fallback.rotation);
            }
        }
    }
}
