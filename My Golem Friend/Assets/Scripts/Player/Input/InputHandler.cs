using System;
using UnityEngine.InputSystem;
using UnityEngine;

public class InputHandler : MonoBehaviour
{
    InputControls inputControls;

    static InputControls.GameplayActions Gameplay;
    static InputControls.UIActions UI;

    public static Vector2 MouseDelta;
    public static Vector2 MovementInput;

    public static event Action OnInteractInput;
    public static event Action OnMenuInput;
    public static event Action OnPlaceInput;
    public static event Action OnThrowInput;

    public void Awake()
    {
        inputControls = new InputControls();

        Gameplay = inputControls.Gameplay;
        UI = inputControls.UI;
    }

    public void OnEnable()
    {
        Gameplay.Enable();
        UI.Enable();

        Gameplay.MouseLook.performed += CacheMouseDelta;

        Gameplay.Movement.performed += CacheMovementVector;
        Gameplay.Movement.canceled += ClearMovementVector;

        Gameplay.Interact.performed += CacheInteractInput;

        Gameplay.Drop.performed += CacheDropInput;

        Gameplay.Throw.performed += CacheThrowInput;

        UI.Menu.performed += CacheMenuInput;
    }

    private void CacheMouseDelta(InputAction.CallbackContext ctx)
    {
        MouseDelta = ctx.ReadValue<Vector2>();
    }

    private void CacheMovementVector(InputAction.CallbackContext ctx)
    {
        MovementInput = ctx.ReadValue<Vector2>();
    }

    private void ClearMovementVector(InputAction.CallbackContext ctx)
    {
        MovementInput = Vector2.zero;
    }

    private void CacheInteractInput(InputAction.CallbackContext ctx)
    {
        OnInteractInput?.Invoke();
    }

    private void CacheDropInput(InputAction.CallbackContext ctx)
    {
        OnPlaceInput?.Invoke();
    }

    private void CacheThrowInput(InputAction.CallbackContext ctx)
    {
        OnThrowInput?.Invoke();
    }

    private void CacheMenuInput(InputAction.CallbackContext ctx)
    {
        OnMenuInput?.Invoke();
    }

    public static void EnterUIMode()
    {
        Gameplay.Disable();
        Cursor.lockState = CursorLockMode.None;
    }

    public static void ExitUIMode()
    {
        Gameplay.Enable();
        Cursor.lockState = CursorLockMode.Locked;
    }

    public static void EnterDialogueMode()
    {
        Gameplay.Disable();
        UI.Disable();
        Cursor.lockState = CursorLockMode.None;
    }

    public static void ExitDialogueMode()
    {
        Gameplay.Enable();
        UI.Enable();
        Cursor.lockState = CursorLockMode.Locked;
    }

    public static void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    public static void UnlockCursor()
    {
        Cursor.lockState = CursorLockMode.None;
    }

    public void OnDisable()
    {
        Gameplay.MouseLook.performed -= CacheMouseDelta;

        Gameplay.Movement.performed -= CacheMovementVector;
        Gameplay.Movement.canceled -= ClearMovementVector;

        Gameplay.Interact.performed -= CacheInteractInput;

        Gameplay.Drop.performed -= CacheDropInput;

        Gameplay.Throw.performed -= CacheThrowInput;

        UI.Menu.performed -= CacheMenuInput;

        UI.Disable();
        Gameplay.Disable();
    }
}
