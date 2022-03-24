using System;
using UnityEngine.InputSystem;
using UnityEngine;

public class InputHandler : MonoBehaviour
{
    private InputControls inputControls;

    private static InputControls.Gameplay3DActions Gameplay3D;

    private static InputControls.Gameplay2DActions Gameplay2D;

    public static Vector2 MouseDelta;
    public static Vector2 MovementInput;

    public static event Action OnInteractInput;
    public static event Action OnPlaceInput;
    public static event Action OnThrowInput;

    public void Awake()
    {
        inputControls = new InputControls();

        Gameplay3D = inputControls.Gameplay3D;
    }

    public void OnEnable()
    {
        Gameplay3D.Enable();

        Gameplay3D.MouseLook.performed += CacheMouseDelta;

        Gameplay3D.Movement.performed += CacheMovementVector;
        Gameplay3D.Movement.canceled += ClearMovementVector;

        Gameplay3D.Interact.performed += CacheInteractInput;

        Gameplay3D.Place.performed += CachePlaceInput;

        Gameplay3D.Throw.performed += CacheThrowInput;
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

    private void CachePlaceInput(InputAction.CallbackContext ctx)
    {
        OnPlaceInput?.Invoke();
    }

    private void CacheThrowInput(InputAction.CallbackContext ctx)
    {
        OnThrowInput?.Invoke();
    }

    public static void Enable3DGameplayControls()
    {
        Gameplay3D.Enable();
    }

    public static void Disable3DGameplayControls()
    {
        Gameplay3D.Disable();

        MouseDelta = Vector2.zero;
        MovementInput = Vector2.zero;
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
        Gameplay3D.MouseLook.performed -= CacheMouseDelta;

        Gameplay3D.Movement.performed -= CacheMovementVector;
        Gameplay3D.Movement.canceled -= ClearMovementVector;

        Gameplay3D.Interact.performed -= CacheInteractInput;

        inputControls.Gameplay3D.Disable();
    }
}
