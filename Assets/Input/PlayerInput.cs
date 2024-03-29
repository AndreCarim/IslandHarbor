//------------------------------------------------------------------------------
// <auto-generated>
//     This code was auto-generated by com.unity.inputsystem:InputActionCodeGenerator
//     version 1.7.0
//     from Assets/Input/PlayerInput.inputactions
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

public partial class @PlayerInput: IInputActionCollection2, IDisposable
{
    public InputActionAsset asset { get; }
    public @PlayerInput()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""PlayerInput"",
    ""maps"": [
        {
            ""name"": ""OnFoot"",
            ""id"": ""0e111e0a-1b38-42a8-9800-40f26023df52"",
            ""actions"": [
                {
                    ""name"": ""Movement"",
                    ""type"": ""Value"",
                    ""id"": ""a0b53a11-b7f3-4dd5-99e6-d430b4c816a8"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""Jump"",
                    ""type"": ""Button"",
                    ""id"": ""1ee315f5-95bd-4234-8742-7313741e50f1"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Look"",
                    ""type"": ""Value"",
                    ""id"": ""c56141b8-5a75-4083-9e8a-f45162ba7fbf"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""LMClick"",
                    ""type"": ""Button"",
                    ""id"": ""d6cfe22b-5f1d-4043-9d9c-fac9ae7532a7"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""EInteraction"",
                    ""type"": ""Button"",
                    ""id"": ""9616da3f-2c17-4e76-a330-5813fa40c3fa"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""OpenInventory"",
                    ""type"": ""Button"",
                    ""id"": ""a65b4db6-042d-48d5-8d5c-c76740885ec7"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""ToolOne"",
                    ""type"": ""Button"",
                    ""id"": ""b1813f22-1f19-4190-a23a-be8f30af9d2f"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""ToolTwo"",
                    ""type"": ""Button"",
                    ""id"": ""736d2c5e-4b08-4b32-9701-ab4a80152005"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""ToolThree"",
                    ""type"": ""Button"",
                    ""id"": ""0c45cbcf-1591-4bc4-bd87-db3f517d0bbd"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                }
            ],
            ""bindings"": [
                {
                    ""name"": ""WASD"",
                    ""id"": ""a76116ef-f836-4fd0-97e0-cda81b517773"",
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
                    ""id"": ""e5c4af29-bdc7-46a1-b2b4-f1262bf8dd6e"",
                    ""path"": ""<Keyboard>/w"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""f5298f18-d4c9-46f8-b04f-cdfecd6cf571"",
                    ""path"": ""<Keyboard>/s"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""13a527bf-78dd-40f1-826c-9eccd23e62ac"",
                    ""path"": ""<Keyboard>/a"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""6074bcbf-f42e-4320-9598-d5a80097da9e"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""LeftStick"",
                    ""id"": ""c937ea82-e6c4-49a7-9b83-8732737aeb81"",
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
                    ""id"": ""608d22a8-aaa2-44ec-a0b9-4f8739fea3ca"",
                    ""path"": ""<Gamepad>/leftStick/up"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""b0e911e6-1789-4ba7-b592-243d740c7a5d"",
                    ""path"": ""<Gamepad>/leftStick/down"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""70b41a8e-d2c4-45a0-8cd7-c77e3e00bbea"",
                    ""path"": ""<Gamepad>/leftStick/left"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""10537ac4-3786-4ee4-b56f-b5af191273ec"",
                    ""path"": ""<Gamepad>/leftStick/right"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""56c7c328-a03f-4aa2-8871-2c52a79889d9"",
                    ""path"": ""<Keyboard>/space"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Jump"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""35bfaac8-5922-4b12-a823-797eafb82b42"",
                    ""path"": ""<Gamepad>/buttonSouth"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Jump"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""b3fa1075-2df5-4404-a05d-5510a0169096"",
                    ""path"": ""<Mouse>/delta"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Look"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""2fd9a501-f551-4682-a8bf-42a9ea54bf04"",
                    ""path"": ""<Gamepad>/rightStick"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Look"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""ee0bd465-972f-43a8-8d43-ec4efe0aa54a"",
                    ""path"": ""<Mouse>/leftButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""LMClick"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""9495cf25-821e-42bf-b2e0-3d6ee13d014b"",
                    ""path"": ""<Keyboard>/e"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""EInteraction"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""84e981e1-11a2-4f52-8aca-bfc2a2a4fe36"",
                    ""path"": ""<Gamepad>/buttonEast"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""EInteraction"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""9379f9ea-7bd2-40e7-8094-1a6f315aeeca"",
                    ""path"": ""<Keyboard>/tab"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""OpenInventory"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""b1a698ea-8bae-42b4-a56b-996b07629ca8"",
                    ""path"": ""<Gamepad>/select"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""OpenInventory"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""84368ba1-b65f-441e-a0e1-fe78a5b56ac7"",
                    ""path"": ""<Keyboard>/i"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""OpenInventory"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""588c5b75-8f10-4393-89ea-84460df03651"",
                    ""path"": ""<Keyboard>/1"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""ToolOne"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""92c86bbe-fcf1-4f94-81de-b3dc8bb046cf"",
                    ""path"": ""<Keyboard>/2"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""ToolTwo"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""4bbf3dc5-2ccf-4ec1-9ef1-bce51316ffdc"",
                    ""path"": ""<Keyboard>/3"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""ToolThree"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        },
        {
            ""name"": ""Gliding"",
            ""id"": ""5d1b5a2c-2206-4efb-ba40-0c11361f7e16"",
            ""actions"": [
                {
                    ""name"": ""New action"",
                    ""type"": ""Button"",
                    ""id"": ""cc6c7406-b134-4031-8823-b2465ff5bffc"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""39028e22-4fff-4fc3-ad20-00034d71b387"",
                    ""path"": """",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""New action"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        },
        {
            ""name"": ""GeneralInput"",
            ""id"": ""460911b5-f013-4405-8940-581edc99b53f"",
            ""actions"": [
                {
                    ""name"": ""OpenMenu"",
                    ""type"": ""Button"",
                    ""id"": ""2ee9ca12-336b-4ce6-8dab-0292cf0e2afa"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""327faf46-8e5c-4a5b-b8cd-37440a3a11f1"",
                    ""path"": ""<Keyboard>/escape"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""OpenMenu"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""8fee02be-5d86-490b-8adf-22ce9964d35c"",
                    ""path"": ""<Gamepad>/start"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""OpenMenu"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": []
}");
        // OnFoot
        m_OnFoot = asset.FindActionMap("OnFoot", throwIfNotFound: true);
        m_OnFoot_Movement = m_OnFoot.FindAction("Movement", throwIfNotFound: true);
        m_OnFoot_Jump = m_OnFoot.FindAction("Jump", throwIfNotFound: true);
        m_OnFoot_Look = m_OnFoot.FindAction("Look", throwIfNotFound: true);
        m_OnFoot_LMClick = m_OnFoot.FindAction("LMClick", throwIfNotFound: true);
        m_OnFoot_EInteraction = m_OnFoot.FindAction("EInteraction", throwIfNotFound: true);
        m_OnFoot_OpenInventory = m_OnFoot.FindAction("OpenInventory", throwIfNotFound: true);
        m_OnFoot_ToolOne = m_OnFoot.FindAction("ToolOne", throwIfNotFound: true);
        m_OnFoot_ToolTwo = m_OnFoot.FindAction("ToolTwo", throwIfNotFound: true);
        m_OnFoot_ToolThree = m_OnFoot.FindAction("ToolThree", throwIfNotFound: true);
        // Gliding
        m_Gliding = asset.FindActionMap("Gliding", throwIfNotFound: true);
        m_Gliding_Newaction = m_Gliding.FindAction("New action", throwIfNotFound: true);
        // GeneralInput
        m_GeneralInput = asset.FindActionMap("GeneralInput", throwIfNotFound: true);
        m_GeneralInput_OpenMenu = m_GeneralInput.FindAction("OpenMenu", throwIfNotFound: true);
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

    // OnFoot
    private readonly InputActionMap m_OnFoot;
    private List<IOnFootActions> m_OnFootActionsCallbackInterfaces = new List<IOnFootActions>();
    private readonly InputAction m_OnFoot_Movement;
    private readonly InputAction m_OnFoot_Jump;
    private readonly InputAction m_OnFoot_Look;
    private readonly InputAction m_OnFoot_LMClick;
    private readonly InputAction m_OnFoot_EInteraction;
    private readonly InputAction m_OnFoot_OpenInventory;
    private readonly InputAction m_OnFoot_ToolOne;
    private readonly InputAction m_OnFoot_ToolTwo;
    private readonly InputAction m_OnFoot_ToolThree;
    public struct OnFootActions
    {
        private @PlayerInput m_Wrapper;
        public OnFootActions(@PlayerInput wrapper) { m_Wrapper = wrapper; }
        public InputAction @Movement => m_Wrapper.m_OnFoot_Movement;
        public InputAction @Jump => m_Wrapper.m_OnFoot_Jump;
        public InputAction @Look => m_Wrapper.m_OnFoot_Look;
        public InputAction @LMClick => m_Wrapper.m_OnFoot_LMClick;
        public InputAction @EInteraction => m_Wrapper.m_OnFoot_EInteraction;
        public InputAction @OpenInventory => m_Wrapper.m_OnFoot_OpenInventory;
        public InputAction @ToolOne => m_Wrapper.m_OnFoot_ToolOne;
        public InputAction @ToolTwo => m_Wrapper.m_OnFoot_ToolTwo;
        public InputAction @ToolThree => m_Wrapper.m_OnFoot_ToolThree;
        public InputActionMap Get() { return m_Wrapper.m_OnFoot; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(OnFootActions set) { return set.Get(); }
        public void AddCallbacks(IOnFootActions instance)
        {
            if (instance == null || m_Wrapper.m_OnFootActionsCallbackInterfaces.Contains(instance)) return;
            m_Wrapper.m_OnFootActionsCallbackInterfaces.Add(instance);
            @Movement.started += instance.OnMovement;
            @Movement.performed += instance.OnMovement;
            @Movement.canceled += instance.OnMovement;
            @Jump.started += instance.OnJump;
            @Jump.performed += instance.OnJump;
            @Jump.canceled += instance.OnJump;
            @Look.started += instance.OnLook;
            @Look.performed += instance.OnLook;
            @Look.canceled += instance.OnLook;
            @LMClick.started += instance.OnLMClick;
            @LMClick.performed += instance.OnLMClick;
            @LMClick.canceled += instance.OnLMClick;
            @EInteraction.started += instance.OnEInteraction;
            @EInteraction.performed += instance.OnEInteraction;
            @EInteraction.canceled += instance.OnEInteraction;
            @OpenInventory.started += instance.OnOpenInventory;
            @OpenInventory.performed += instance.OnOpenInventory;
            @OpenInventory.canceled += instance.OnOpenInventory;
            @ToolOne.started += instance.OnToolOne;
            @ToolOne.performed += instance.OnToolOne;
            @ToolOne.canceled += instance.OnToolOne;
            @ToolTwo.started += instance.OnToolTwo;
            @ToolTwo.performed += instance.OnToolTwo;
            @ToolTwo.canceled += instance.OnToolTwo;
            @ToolThree.started += instance.OnToolThree;
            @ToolThree.performed += instance.OnToolThree;
            @ToolThree.canceled += instance.OnToolThree;
        }

        private void UnregisterCallbacks(IOnFootActions instance)
        {
            @Movement.started -= instance.OnMovement;
            @Movement.performed -= instance.OnMovement;
            @Movement.canceled -= instance.OnMovement;
            @Jump.started -= instance.OnJump;
            @Jump.performed -= instance.OnJump;
            @Jump.canceled -= instance.OnJump;
            @Look.started -= instance.OnLook;
            @Look.performed -= instance.OnLook;
            @Look.canceled -= instance.OnLook;
            @LMClick.started -= instance.OnLMClick;
            @LMClick.performed -= instance.OnLMClick;
            @LMClick.canceled -= instance.OnLMClick;
            @EInteraction.started -= instance.OnEInteraction;
            @EInteraction.performed -= instance.OnEInteraction;
            @EInteraction.canceled -= instance.OnEInteraction;
            @OpenInventory.started -= instance.OnOpenInventory;
            @OpenInventory.performed -= instance.OnOpenInventory;
            @OpenInventory.canceled -= instance.OnOpenInventory;
            @ToolOne.started -= instance.OnToolOne;
            @ToolOne.performed -= instance.OnToolOne;
            @ToolOne.canceled -= instance.OnToolOne;
            @ToolTwo.started -= instance.OnToolTwo;
            @ToolTwo.performed -= instance.OnToolTwo;
            @ToolTwo.canceled -= instance.OnToolTwo;
            @ToolThree.started -= instance.OnToolThree;
            @ToolThree.performed -= instance.OnToolThree;
            @ToolThree.canceled -= instance.OnToolThree;
        }

        public void RemoveCallbacks(IOnFootActions instance)
        {
            if (m_Wrapper.m_OnFootActionsCallbackInterfaces.Remove(instance))
                UnregisterCallbacks(instance);
        }

        public void SetCallbacks(IOnFootActions instance)
        {
            foreach (var item in m_Wrapper.m_OnFootActionsCallbackInterfaces)
                UnregisterCallbacks(item);
            m_Wrapper.m_OnFootActionsCallbackInterfaces.Clear();
            AddCallbacks(instance);
        }
    }
    public OnFootActions @OnFoot => new OnFootActions(this);

    // Gliding
    private readonly InputActionMap m_Gliding;
    private List<IGlidingActions> m_GlidingActionsCallbackInterfaces = new List<IGlidingActions>();
    private readonly InputAction m_Gliding_Newaction;
    public struct GlidingActions
    {
        private @PlayerInput m_Wrapper;
        public GlidingActions(@PlayerInput wrapper) { m_Wrapper = wrapper; }
        public InputAction @Newaction => m_Wrapper.m_Gliding_Newaction;
        public InputActionMap Get() { return m_Wrapper.m_Gliding; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(GlidingActions set) { return set.Get(); }
        public void AddCallbacks(IGlidingActions instance)
        {
            if (instance == null || m_Wrapper.m_GlidingActionsCallbackInterfaces.Contains(instance)) return;
            m_Wrapper.m_GlidingActionsCallbackInterfaces.Add(instance);
            @Newaction.started += instance.OnNewaction;
            @Newaction.performed += instance.OnNewaction;
            @Newaction.canceled += instance.OnNewaction;
        }

        private void UnregisterCallbacks(IGlidingActions instance)
        {
            @Newaction.started -= instance.OnNewaction;
            @Newaction.performed -= instance.OnNewaction;
            @Newaction.canceled -= instance.OnNewaction;
        }

        public void RemoveCallbacks(IGlidingActions instance)
        {
            if (m_Wrapper.m_GlidingActionsCallbackInterfaces.Remove(instance))
                UnregisterCallbacks(instance);
        }

        public void SetCallbacks(IGlidingActions instance)
        {
            foreach (var item in m_Wrapper.m_GlidingActionsCallbackInterfaces)
                UnregisterCallbacks(item);
            m_Wrapper.m_GlidingActionsCallbackInterfaces.Clear();
            AddCallbacks(instance);
        }
    }
    public GlidingActions @Gliding => new GlidingActions(this);

    // GeneralInput
    private readonly InputActionMap m_GeneralInput;
    private List<IGeneralInputActions> m_GeneralInputActionsCallbackInterfaces = new List<IGeneralInputActions>();
    private readonly InputAction m_GeneralInput_OpenMenu;
    public struct GeneralInputActions
    {
        private @PlayerInput m_Wrapper;
        public GeneralInputActions(@PlayerInput wrapper) { m_Wrapper = wrapper; }
        public InputAction @OpenMenu => m_Wrapper.m_GeneralInput_OpenMenu;
        public InputActionMap Get() { return m_Wrapper.m_GeneralInput; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(GeneralInputActions set) { return set.Get(); }
        public void AddCallbacks(IGeneralInputActions instance)
        {
            if (instance == null || m_Wrapper.m_GeneralInputActionsCallbackInterfaces.Contains(instance)) return;
            m_Wrapper.m_GeneralInputActionsCallbackInterfaces.Add(instance);
            @OpenMenu.started += instance.OnOpenMenu;
            @OpenMenu.performed += instance.OnOpenMenu;
            @OpenMenu.canceled += instance.OnOpenMenu;
        }

        private void UnregisterCallbacks(IGeneralInputActions instance)
        {
            @OpenMenu.started -= instance.OnOpenMenu;
            @OpenMenu.performed -= instance.OnOpenMenu;
            @OpenMenu.canceled -= instance.OnOpenMenu;
        }

        public void RemoveCallbacks(IGeneralInputActions instance)
        {
            if (m_Wrapper.m_GeneralInputActionsCallbackInterfaces.Remove(instance))
                UnregisterCallbacks(instance);
        }

        public void SetCallbacks(IGeneralInputActions instance)
        {
            foreach (var item in m_Wrapper.m_GeneralInputActionsCallbackInterfaces)
                UnregisterCallbacks(item);
            m_Wrapper.m_GeneralInputActionsCallbackInterfaces.Clear();
            AddCallbacks(instance);
        }
    }
    public GeneralInputActions @GeneralInput => new GeneralInputActions(this);
    public interface IOnFootActions
    {
        void OnMovement(InputAction.CallbackContext context);
        void OnJump(InputAction.CallbackContext context);
        void OnLook(InputAction.CallbackContext context);
        void OnLMClick(InputAction.CallbackContext context);
        void OnEInteraction(InputAction.CallbackContext context);
        void OnOpenInventory(InputAction.CallbackContext context);
        void OnToolOne(InputAction.CallbackContext context);
        void OnToolTwo(InputAction.CallbackContext context);
        void OnToolThree(InputAction.CallbackContext context);
    }
    public interface IGlidingActions
    {
        void OnNewaction(InputAction.CallbackContext context);
    }
    public interface IGeneralInputActions
    {
        void OnOpenMenu(InputAction.CallbackContext context);
    }
}
