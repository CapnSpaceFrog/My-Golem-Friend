//------------------------------------------------------------------------------
// <auto-generated>
//     This code was auto-generated by com.unity.inputsystem:InputActionCodeGenerator
//     version 1.3.0
//     from Assets/Scripts/Player/Input/InputControls.inputactions
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public partial class @InputControls : IInputActionCollection2, IDisposable
{
    public InputActionAsset asset { get; }
    public @InputControls()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""InputControls"",
    ""maps"": [
        {
            ""name"": ""Gameplay3D"",
            ""id"": ""0f4c16d9-05a0-4cf7-a73f-70887ee819da"",
            ""actions"": [
                {
                    ""name"": ""Movement"",
                    ""type"": ""Value"",
                    ""id"": ""64395e10-eebe-43a7-8d79-7247ae672928"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""Interact"",
                    ""type"": ""Value"",
                    ""id"": ""057826f1-0437-46af-bcb7-9fda14e4ff3a"",
                    ""expectedControlType"": ""Integer"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""Mouse Look"",
                    ""type"": ""PassThrough"",
                    ""id"": ""e1090a35-a692-45b5-9990-6ff5257f6f48"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Throw"",
                    ""type"": ""Button"",
                    ""id"": ""b1864566-87f8-404d-b5de-18920093b691"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Place"",
                    ""type"": ""Button"",
                    ""id"": ""141553ba-4d37-4218-bb2a-429d9e3f84d5"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Inventory"",
                    ""type"": ""Button"",
                    ""id"": ""a602e15c-bc20-471b-a9a6-bcf9ea88f380"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""98d88d1c-b2cb-47c2-9f18-20af3ff0bc8f"",
                    ""path"": ""<Mouse>/delta"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard & Mouse"",
                    ""action"": ""Mouse Look"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""2D Vector"",
                    ""id"": ""774b36c3-60b8-4c81-a7ca-2412d45cfd19"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""94a4cf9d-f1ee-44d4-8610-4d5b0147d722"",
                    ""path"": ""<Keyboard>/w"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard & Mouse"",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""b343968e-a331-4f03-9a6b-c8e133505a35"",
                    ""path"": ""<Keyboard>/s"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard & Mouse"",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""cd24d2ec-bc3c-49ad-a2cb-88d30cd72fbb"",
                    ""path"": ""<Keyboard>/a"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard & Mouse"",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""496de8b5-04c7-4ffc-a52b-579ea0b1de6d"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard & Mouse"",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""0e88b24c-e14b-4b29-904b-734954a7ade3"",
                    ""path"": ""<Keyboard>/e"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard & Mouse"",
                    ""action"": ""Interact"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""7ca8a955-61ec-402f-8b82-c58493034bca"",
                    ""path"": ""<Mouse>/rightButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard & Mouse"",
                    ""action"": ""Place"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""c99e578e-88ee-4683-a1f6-7b5a678a8127"",
                    ""path"": ""<Mouse>/leftButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard & Mouse"",
                    ""action"": ""Throw"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""7abdd10d-7f09-43b9-8b1e-bce6c9c74a90"",
                    ""path"": ""<Keyboard>/i"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Inventory"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        },
        {
            ""name"": ""Gameplay2D"",
            ""id"": ""6dc58961-2121-4ec0-b1eb-8d48931d178e"",
            ""actions"": [
                {
                    ""name"": ""New action"",
                    ""type"": ""Button"",
                    ""id"": ""cf00c8e1-c83a-4db3-b45a-274128715a85"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""07fe7161-042b-4aaf-af8b-3831ed412660"",
                    ""path"": """",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""New action"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": [
        {
            ""name"": ""Keyboard & Mouse"",
            ""bindingGroup"": ""Keyboard & Mouse"",
            ""devices"": [
                {
                    ""devicePath"": ""<Mouse>"",
                    ""isOptional"": false,
                    ""isOR"": false
                },
                {
                    ""devicePath"": ""<Keyboard>"",
                    ""isOptional"": false,
                    ""isOR"": false
                }
            ]
        }
    ]
}");
        // Gameplay3D
        m_Gameplay3D = asset.FindActionMap("Gameplay3D", throwIfNotFound: true);
        m_Gameplay3D_Movement = m_Gameplay3D.FindAction("Movement", throwIfNotFound: true);
        m_Gameplay3D_Interact = m_Gameplay3D.FindAction("Interact", throwIfNotFound: true);
        m_Gameplay3D_MouseLook = m_Gameplay3D.FindAction("Mouse Look", throwIfNotFound: true);
        m_Gameplay3D_Throw = m_Gameplay3D.FindAction("Throw", throwIfNotFound: true);
        m_Gameplay3D_Place = m_Gameplay3D.FindAction("Place", throwIfNotFound: true);
        m_Gameplay3D_Inventory = m_Gameplay3D.FindAction("Inventory", throwIfNotFound: true);
        // Gameplay2D
        m_Gameplay2D = asset.FindActionMap("Gameplay2D", throwIfNotFound: true);
        m_Gameplay2D_Newaction = m_Gameplay2D.FindAction("New action", throwIfNotFound: true);
    }

    public void Dispose()
    {
        UnityEngine.Object.Destroy(asset);
    }

    public InputBinding? bindingMask
    {
        get => asset.bindingMask;
        set => asset.bindingMask = value;
    }

    public ReadOnlyArray<InputDevice>? devices
    {
        get => asset.devices;
        set => asset.devices = value;
    }

    public ReadOnlyArray<InputControlScheme> controlSchemes => asset.controlSchemes;

    public bool Contains(InputAction action)
    {
        return asset.Contains(action);
    }

    public IEnumerator<InputAction> GetEnumerator()
    {
        return asset.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void Enable()
    {
        asset.Enable();
    }

    public void Disable()
    {
        asset.Disable();
    }
    public IEnumerable<InputBinding> bindings => asset.bindings;

    public InputAction FindAction(string actionNameOrId, bool throwIfNotFound = false)
    {
        return asset.FindAction(actionNameOrId, throwIfNotFound);
    }
    public int FindBinding(InputBinding bindingMask, out InputAction action)
    {
        return asset.FindBinding(bindingMask, out action);
    }

    // Gameplay3D
    private readonly InputActionMap m_Gameplay3D;
    private IGameplay3DActions m_Gameplay3DActionsCallbackInterface;
    private readonly InputAction m_Gameplay3D_Movement;
    private readonly InputAction m_Gameplay3D_Interact;
    private readonly InputAction m_Gameplay3D_MouseLook;
    private readonly InputAction m_Gameplay3D_Throw;
    private readonly InputAction m_Gameplay3D_Place;
    private readonly InputAction m_Gameplay3D_Inventory;
    public struct Gameplay3DActions
    {
        private @InputControls m_Wrapper;
        public Gameplay3DActions(@InputControls wrapper) { m_Wrapper = wrapper; }
        public InputAction @Movement => m_Wrapper.m_Gameplay3D_Movement;
        public InputAction @Interact => m_Wrapper.m_Gameplay3D_Interact;
        public InputAction @MouseLook => m_Wrapper.m_Gameplay3D_MouseLook;
        public InputAction @Throw => m_Wrapper.m_Gameplay3D_Throw;
        public InputAction @Place => m_Wrapper.m_Gameplay3D_Place;
        public InputAction @Inventory => m_Wrapper.m_Gameplay3D_Inventory;
        public InputActionMap Get() { return m_Wrapper.m_Gameplay3D; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(Gameplay3DActions set) { return set.Get(); }
        public void SetCallbacks(IGameplay3DActions instance)
        {
            if (m_Wrapper.m_Gameplay3DActionsCallbackInterface != null)
            {
                @Movement.started -= m_Wrapper.m_Gameplay3DActionsCallbackInterface.OnMovement;
                @Movement.performed -= m_Wrapper.m_Gameplay3DActionsCallbackInterface.OnMovement;
                @Movement.canceled -= m_Wrapper.m_Gameplay3DActionsCallbackInterface.OnMovement;
                @Interact.started -= m_Wrapper.m_Gameplay3DActionsCallbackInterface.OnInteract;
                @Interact.performed -= m_Wrapper.m_Gameplay3DActionsCallbackInterface.OnInteract;
                @Interact.canceled -= m_Wrapper.m_Gameplay3DActionsCallbackInterface.OnInteract;
                @MouseLook.started -= m_Wrapper.m_Gameplay3DActionsCallbackInterface.OnMouseLook;
                @MouseLook.performed -= m_Wrapper.m_Gameplay3DActionsCallbackInterface.OnMouseLook;
                @MouseLook.canceled -= m_Wrapper.m_Gameplay3DActionsCallbackInterface.OnMouseLook;
                @Throw.started -= m_Wrapper.m_Gameplay3DActionsCallbackInterface.OnThrow;
                @Throw.performed -= m_Wrapper.m_Gameplay3DActionsCallbackInterface.OnThrow;
                @Throw.canceled -= m_Wrapper.m_Gameplay3DActionsCallbackInterface.OnThrow;
                @Place.started -= m_Wrapper.m_Gameplay3DActionsCallbackInterface.OnPlace;
                @Place.performed -= m_Wrapper.m_Gameplay3DActionsCallbackInterface.OnPlace;
                @Place.canceled -= m_Wrapper.m_Gameplay3DActionsCallbackInterface.OnPlace;
                @Inventory.started -= m_Wrapper.m_Gameplay3DActionsCallbackInterface.OnInventory;
                @Inventory.performed -= m_Wrapper.m_Gameplay3DActionsCallbackInterface.OnInventory;
                @Inventory.canceled -= m_Wrapper.m_Gameplay3DActionsCallbackInterface.OnInventory;
            }
            m_Wrapper.m_Gameplay3DActionsCallbackInterface = instance;
            if (instance != null)
            {
                @Movement.started += instance.OnMovement;
                @Movement.performed += instance.OnMovement;
                @Movement.canceled += instance.OnMovement;
                @Interact.started += instance.OnInteract;
                @Interact.performed += instance.OnInteract;
                @Interact.canceled += instance.OnInteract;
                @MouseLook.started += instance.OnMouseLook;
                @MouseLook.performed += instance.OnMouseLook;
                @MouseLook.canceled += instance.OnMouseLook;
                @Throw.started += instance.OnThrow;
                @Throw.performed += instance.OnThrow;
                @Throw.canceled += instance.OnThrow;
                @Place.started += instance.OnPlace;
                @Place.performed += instance.OnPlace;
                @Place.canceled += instance.OnPlace;
                @Inventory.started += instance.OnInventory;
                @Inventory.performed += instance.OnInventory;
                @Inventory.canceled += instance.OnInventory;
            }
        }
    }
    public Gameplay3DActions @Gameplay3D => new Gameplay3DActions(this);

    // Gameplay2D
    private readonly InputActionMap m_Gameplay2D;
    private IGameplay2DActions m_Gameplay2DActionsCallbackInterface;
    private readonly InputAction m_Gameplay2D_Newaction;
    public struct Gameplay2DActions
    {
        private @InputControls m_Wrapper;
        public Gameplay2DActions(@InputControls wrapper) { m_Wrapper = wrapper; }
        public InputAction @Newaction => m_Wrapper.m_Gameplay2D_Newaction;
        public InputActionMap Get() { return m_Wrapper.m_Gameplay2D; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(Gameplay2DActions set) { return set.Get(); }
        public void SetCallbacks(IGameplay2DActions instance)
        {
            if (m_Wrapper.m_Gameplay2DActionsCallbackInterface != null)
            {
                @Newaction.started -= m_Wrapper.m_Gameplay2DActionsCallbackInterface.OnNewaction;
                @Newaction.performed -= m_Wrapper.m_Gameplay2DActionsCallbackInterface.OnNewaction;
                @Newaction.canceled -= m_Wrapper.m_Gameplay2DActionsCallbackInterface.OnNewaction;
            }
            m_Wrapper.m_Gameplay2DActionsCallbackInterface = instance;
            if (instance != null)
            {
                @Newaction.started += instance.OnNewaction;
                @Newaction.performed += instance.OnNewaction;
                @Newaction.canceled += instance.OnNewaction;
            }
        }
    }
    public Gameplay2DActions @Gameplay2D => new Gameplay2DActions(this);
    private int m_KeyboardMouseSchemeIndex = -1;
    public InputControlScheme KeyboardMouseScheme
    {
        get
        {
            if (m_KeyboardMouseSchemeIndex == -1) m_KeyboardMouseSchemeIndex = asset.FindControlSchemeIndex("Keyboard & Mouse");
            return asset.controlSchemes[m_KeyboardMouseSchemeIndex];
        }
    }
    public interface IGameplay3DActions
    {
        void OnMovement(InputAction.CallbackContext context);
        void OnInteract(InputAction.CallbackContext context);
        void OnMouseLook(InputAction.CallbackContext context);
        void OnThrow(InputAction.CallbackContext context);
        void OnPlace(InputAction.CallbackContext context);
        void OnInventory(InputAction.CallbackContext context);
    }
    public interface IGameplay2DActions
    {
        void OnNewaction(InputAction.CallbackContext context);
    }
}
