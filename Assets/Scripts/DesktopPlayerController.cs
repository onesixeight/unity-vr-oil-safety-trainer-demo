using UnityEngine;

#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace OilSafetyTrainer
{
    [RequireComponent(typeof(CharacterController))]
    [RequireComponent(typeof(PlayerRig))]
    public sealed class DesktopPlayerController : MonoBehaviour
    {
        private const string DefaultPrompt = "WASD - движение | Мышь - обзор | E - действие | H - памятка | R - сброс | Q - выход";

        [SerializeField] private Camera viewCamera;
        [SerializeField] private float moveSpeed = 4.2f;
        [SerializeField] private float lookSensitivity = 0.12f;
        [SerializeField] private float interactionDistance = 3.2f;
        [SerializeField] private LayerMask interactionMask = ~0;

        private CharacterController characterController;
        private PlayerRig playerRig;
        private InteractableItem hoveredItem;
        private float cameraPitch;
        private float verticalVelocity;
        private bool paused;
        private int suppressLookFrames;

        private void Awake()
        {
            characterController = GetComponent<CharacterController>();
            playerRig = GetComponent<PlayerRig>();
            if (viewCamera == null)
            {
                viewCamera = GetComponentInChildren<Camera>();
            }

            cameraPitch = GetCameraPitch();
            ApplyCursorState();
        }

        private void Update()
        {
            if (WasPressedThisFrame(KeyCode.Escape))
            {
                SetPaused(!paused);
            }

            if (WasPressedThisFrame(KeyCode.H))
            {
                SafetyScenarioManager.Instance?.ToggleGuide();
            }

            if (WasPressedThisFrame(KeyCode.Q))
            {
                SafetyScenarioManager.Instance?.QuitDemo();
                return;
            }

            if (WasPressedThisFrame(KeyCode.R))
            {
                SafetyScenarioManager.Instance?.ResetScenario();
                return;
            }

            if (paused)
            {
                return;
            }

            Look();
            Move();
            UpdateHover();

            if (WasPressedThisFrame(KeyCode.E) && hoveredItem != null)
            {
                hoveredItem.Interact(playerRig);
                UpdateHover();
            }
        }

        private void Look()
        {
            if (suppressLookFrames > 0)
            {
                suppressLookFrames--;
                return;
            }

            var delta = ReadMouseDelta() * lookSensitivity;
            transform.Rotate(Vector3.up * delta.x);

            cameraPitch = Mathf.Clamp(cameraPitch - delta.y, -78f, 78f);
            if (viewCamera != null)
            {
                viewCamera.transform.localRotation = Quaternion.Euler(cameraPitch, 0f, 0f);
            }
        }

        private void Move()
        {
            var input = ReadMoveInput();
            var move = transform.right * input.x + transform.forward * input.y;
            move = Vector3.ClampMagnitude(move, 1f) * moveSpeed;

            if (characterController.isGrounded && verticalVelocity < 0f)
            {
                verticalVelocity = -1f;
            }

            verticalVelocity += Physics.gravity.y * Time.deltaTime;
            move.y = verticalVelocity;
            characterController.Move(move * Time.deltaTime);
        }

        private void UpdateHover()
        {
            var nextItem = FindLookedAtItem();
            if (hoveredItem != nextItem)
            {
                hoveredItem?.SetHighlighted(false);
                hoveredItem = nextItem;
                hoveredItem?.SetHighlighted(true);
            }

            var prompt = hoveredItem != null
                ? hoveredItem.GetPrompt()
                : DefaultPrompt;
            SafetyScenarioManager.Instance?.SetInteractionPrompt(prompt);
        }

        public void SetPaused(bool value)
        {
            paused = value;
            if (paused && hoveredItem != null)
            {
                hoveredItem.SetHighlighted(false);
                hoveredItem = null;
            }

            ApplyCursorState();
        }

        private InteractableItem FindLookedAtItem()
        {
            var origin = playerRig.InteractionOrigin;
            if (origin == null)
            {
                return null;
            }

            if (Physics.Raycast(origin.position, origin.forward, out var hit, interactionDistance, interactionMask, QueryTriggerInteraction.Collide))
            {
                return hit.collider.GetComponentInParent<InteractableItem>();
            }

            return null;
        }

        private static Vector2 ReadMoveInput()
        {
#if ENABLE_INPUT_SYSTEM
            if (Keyboard.current != null)
            {
                var move = Vector2.zero;
                if (Keyboard.current.aKey.isPressed) move.x -= 1f;
                if (Keyboard.current.dKey.isPressed) move.x += 1f;
                if (Keyboard.current.sKey.isPressed) move.y -= 1f;
                if (Keyboard.current.wKey.isPressed) move.y += 1f;
                if (move.sqrMagnitude > 0f)
                {
                    return Vector2.ClampMagnitude(move, 1f);
                }
            }
#endif

#if ENABLE_LEGACY_INPUT_MANAGER
            return Vector2.ClampMagnitude(new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")), 1f);
#else
            return Vector2.zero;
#endif
        }

        private static Vector2 ReadMouseDelta()
        {
#if ENABLE_INPUT_SYSTEM
            if (Mouse.current != null)
            {
                return Mouse.current.delta.ReadValue();
            }
#endif

#if ENABLE_LEGACY_INPUT_MANAGER
            return new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y")) * 18f;
#else
            return Vector2.zero;
#endif
        }

        private static bool WasPressedThisFrame(KeyCode keyCode)
        {
#if ENABLE_INPUT_SYSTEM
            if (Keyboard.current != null)
            {
                if (keyCode == KeyCode.E && Keyboard.current.eKey.wasPressedThisFrame) return true;
                if (keyCode == KeyCode.H && Keyboard.current.hKey.wasPressedThisFrame) return true;
                if (keyCode == KeyCode.Q && Keyboard.current.qKey.wasPressedThisFrame) return true;
                if (keyCode == KeyCode.R && Keyboard.current.rKey.wasPressedThisFrame) return true;
                if (keyCode == KeyCode.Escape && Keyboard.current.escapeKey.wasPressedThisFrame) return true;
            }
#endif

#if ENABLE_LEGACY_INPUT_MANAGER
            return Input.GetKeyDown(keyCode);
#else
            return false;
#endif
        }

        private void ApplyCursorState()
        {
            Cursor.lockState = paused ? CursorLockMode.None : CursorLockMode.Locked;
            Cursor.visible = paused;
            cameraPitch = GetCameraPitch();
            suppressLookFrames = paused ? 0 : 2;
        }

        private float GetCameraPitch()
        {
            if (viewCamera == null)
            {
                return 0f;
            }

            var pitch = viewCamera.transform.localEulerAngles.x;
            return pitch > 180f ? pitch - 360f : pitch;
        }
    }
}
