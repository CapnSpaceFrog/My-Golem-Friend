using System;
using UnityEngine.InputSystem;
using UnityEngine;

public enum InteractInput
{
    E,
    RightClick,
    LeftClick
}

public class InputHandler : MonoBehaviour
{
    InputControls inputControls;

    static InputControls.GameplayActions Gameplay;
    static InputControls.UIActions UI;

    public static Vector2 MouseDelta;
    public static Vector2 MovementInput;
    private InteractInput InputType;

    public static event Action<InteractInput> OnInteractInput;
    public static event Action OnMenuInput;

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

        Gameplay.Interact.performed += EButtonInput;

        Gameplay.LftClick.performed += LeftClickInput;

        Gameplay.RghClick.performed += RightClickInput;

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

    private void EButtonInput(InputAction.CallbackContext ctx)
    {
        InputType = InteractInput.E;
        OnInteractInput?.Invoke(InputType);
    }

    private void LeftClickInput(InputAction.CallbackContext ctx)
    {
        InputType = InteractInput.LeftClick;
        OnInteractInput?.Invoke(InputType);
    }

    private void RightClickInput(InputAction.CallbackContext ctx)
    {
        InputType = InteractInput.RightClick;
        OnInteractInput?.Invoke(InputType);
    }

    private void CacheMenuInput(InputAction.CallbackContext ctx)
    {
        OnMenuInput?.Invoke();
    }

    public static void EnterUIMode()
    {
        MouseDelta = Vector2.zero;
        MovementInput = Vector2.zero;

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
        MouseDelta = Vector2.zero;
        MovementInput = Vector2.zero;

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

        Gameplay.Interact.performed -= EButtonInput;

        Gameplay.LftClick.performed -= LeftClickInput;

        Gameplay.RghClick.performed -= RightClickInput;

        UI.Menu.performed -= CacheMenuInput;

        UI.Disable();
        Gameplay.Disable();
    }
}
