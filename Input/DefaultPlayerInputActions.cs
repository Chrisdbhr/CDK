// GENERATED AUTOMATICALLY FROM 'Assets/CDK/Input/DefaultPlayerInputActions.inputactions'

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

namespace GameInput
{
    public class @DefaultPlayerInputActions : IInputActionCollection, IDisposable
    {
        public InputActionAsset asset { get; }
        public @DefaultPlayerInputActions()
        {
            asset = InputActionAsset.FromJson(@"{
    ""name"": ""DefaultPlayerInputActions"",
    ""maps"": [
        {
            ""name"": ""gameplay"",
            ""id"": ""6055826c-1c46-4dde-9c37-1f6f2cc9a6c8"",
            ""actions"": [
                {
                    ""name"": ""move"",
                    ""type"": ""Value"",
                    ""id"": ""9025893a-35ef-47b8-ba4a-a9ace8be7813"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""look"",
                    ""type"": ""Value"",
                    ""id"": ""383d00a8-a343-435e-aff5-9654ea7e8a24"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""run"",
                    ""type"": ""Button"",
                    ""id"": ""cf0c62a5-6e1c-4fd1-a60c-9ebeb51a99f7"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": ""2D Vector"",
                    ""id"": ""853a9c9d-251e-4ba8-892f-8faddf7e1562"",
                    ""path"": ""2DVector(mode=1)"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""move"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""1c60543f-3c0b-4b04-bb9b-a73d8a41147d"",
                    ""path"": ""<Keyboard>/w"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard and Mouse"",
                    ""action"": ""move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""a6cdcf4b-ec4d-4774-80cf-453caa40c6e8"",
                    ""path"": ""<Keyboard>/s"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard and Mouse"",
                    ""action"": ""move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""26fb6b38-e8cc-4c84-8bac-ea8ae478cf70"",
                    ""path"": ""<Keyboard>/a"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard and Mouse"",
                    ""action"": ""move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""4ce2f281-9afe-4747-894b-d60c3f8f8e2b"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard and Mouse"",
                    ""action"": ""move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""8b01e112-7767-4bf7-8abb-fa3849c8c0e3"",
                    ""path"": ""<Gamepad>/leftStick"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Controller"",
                    ""action"": ""move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""3225bb9f-ac8b-45d3-990f-c7fc5c3ab2e7"",
                    ""path"": ""<Pointer>/delta"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard and Mouse"",
                    ""action"": ""look"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""cb283fa1-9738-4144-9c24-3ceac9c230f6"",
                    ""path"": ""<Gamepad>/rightStick"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Controller"",
                    ""action"": ""look"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""0cc6396b-2502-447c-a242-1c0819bdcd7a"",
                    ""path"": ""<Keyboard>/shift"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Keyboard and Mouse"",
                    ""action"": ""run"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""279c4f42-b5c5-49bc-9d39-363a85b4c555"",
                    ""path"": ""<XInputController>/buttonSouth"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": ""Controller"",
                    ""action"": ""run"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": [
        {
            ""name"": ""Keyboard and Mouse"",
            ""bindingGroup"": ""Keyboard and Mouse"",
            ""devices"": [
                {
                    ""devicePath"": ""<Keyboard>"",
                    ""isOptional"": false,
                    ""isOR"": false
                },
                {
                    ""devicePath"": ""<Mouse>"",
                    ""isOptional"": false,
                    ""isOR"": false
                }
            ]
        },
        {
            ""name"": ""Controller"",
            ""bindingGroup"": ""Controller"",
            ""devices"": [
                {
                    ""devicePath"": ""<XInputController>"",
                    ""isOptional"": true,
                    ""isOR"": false
                }
            ]
        }
    ]
}");
            // gameplay
            m_gameplay = asset.FindActionMap("gameplay", throwIfNotFound: true);
            m_gameplay_move = m_gameplay.FindAction("move", throwIfNotFound: true);
            m_gameplay_look = m_gameplay.FindAction("look", throwIfNotFound: true);
            m_gameplay_run = m_gameplay.FindAction("run", throwIfNotFound: true);
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

        // gameplay
        private readonly InputActionMap m_gameplay;
        private IGameplayActions m_GameplayActionsCallbackInterface;
        private readonly InputAction m_gameplay_move;
        private readonly InputAction m_gameplay_look;
        private readonly InputAction m_gameplay_run;
        public struct GameplayActions
        {
            private @DefaultPlayerInputActions m_Wrapper;
            public GameplayActions(@DefaultPlayerInputActions wrapper) { m_Wrapper = wrapper; }
            public InputAction @move => m_Wrapper.m_gameplay_move;
            public InputAction @look => m_Wrapper.m_gameplay_look;
            public InputAction @run => m_Wrapper.m_gameplay_run;
            public InputActionMap Get() { return m_Wrapper.m_gameplay; }
            public void Enable() { Get().Enable(); }
            public void Disable() { Get().Disable(); }
            public bool enabled => Get().enabled;
            public static implicit operator InputActionMap(GameplayActions set) { return set.Get(); }
            public void SetCallbacks(IGameplayActions instance)
            {
                if (m_Wrapper.m_GameplayActionsCallbackInterface != null)
                {
                    @move.started -= m_Wrapper.m_GameplayActionsCallbackInterface.OnMove;
                    @move.performed -= m_Wrapper.m_GameplayActionsCallbackInterface.OnMove;
                    @move.canceled -= m_Wrapper.m_GameplayActionsCallbackInterface.OnMove;
                    @look.started -= m_Wrapper.m_GameplayActionsCallbackInterface.OnLook;
                    @look.performed -= m_Wrapper.m_GameplayActionsCallbackInterface.OnLook;
                    @look.canceled -= m_Wrapper.m_GameplayActionsCallbackInterface.OnLook;
                    @run.started -= m_Wrapper.m_GameplayActionsCallbackInterface.OnRun;
                    @run.performed -= m_Wrapper.m_GameplayActionsCallbackInterface.OnRun;
                    @run.canceled -= m_Wrapper.m_GameplayActionsCallbackInterface.OnRun;
                }
                m_Wrapper.m_GameplayActionsCallbackInterface = instance;
                if (instance != null)
                {
                    @move.started += instance.OnMove;
                    @move.performed += instance.OnMove;
                    @move.canceled += instance.OnMove;
                    @look.started += instance.OnLook;
                    @look.performed += instance.OnLook;
                    @look.canceled += instance.OnLook;
                    @run.started += instance.OnRun;
                    @run.performed += instance.OnRun;
                    @run.canceled += instance.OnRun;
                }
            }
        }
        public GameplayActions @gameplay => new GameplayActions(this);
        private int m_KeyboardandMouseSchemeIndex = -1;
        public InputControlScheme KeyboardandMouseScheme
        {
            get
            {
                if (m_KeyboardandMouseSchemeIndex == -1) m_KeyboardandMouseSchemeIndex = asset.FindControlSchemeIndex("Keyboard and Mouse");
                return asset.controlSchemes[m_KeyboardandMouseSchemeIndex];
            }
        }
        private int m_ControllerSchemeIndex = -1;
        public InputControlScheme ControllerScheme
        {
            get
            {
                if (m_ControllerSchemeIndex == -1) m_ControllerSchemeIndex = asset.FindControlSchemeIndex("Controller");
                return asset.controlSchemes[m_ControllerSchemeIndex];
            }
        }
        public interface IGameplayActions
        {
            void OnMove(InputAction.CallbackContext context);
            void OnLook(InputAction.CallbackContext context);
            void OnRun(InputAction.CallbackContext context);
        }
    }
}
