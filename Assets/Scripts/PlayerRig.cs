using UnityEngine;

namespace OilSafetyTrainer
{
    public class PlayerRig : MonoBehaviour
    {
        [SerializeField] private Transform head;
        [SerializeField] private Transform interactionOrigin;

        public Transform Head => head != null ? head : transform;
        public Transform InteractionOrigin => interactionOrigin != null ? interactionOrigin : Head;

        public void Configure(Transform headTransform, Transform interactionTransform)
        {
            head = headTransform;
            interactionOrigin = interactionTransform;
        }

        public void TeleportTo(Vector3 position, Quaternion rotation)
        {
            var characterController = GetComponent<CharacterController>();
            if (characterController != null)
            {
                characterController.enabled = false;
            }

            transform.SetPositionAndRotation(position, rotation);

            if (characterController != null)
            {
                characterController.enabled = true;
            }
        }
    }
}
