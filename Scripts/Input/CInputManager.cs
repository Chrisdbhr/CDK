using System;
using System.Linq;
using UniRx;
using UnityEngine;

#if Rewired
using Rewired;
#endif

namespace CDK {
	public static class CInputManager {

        #region <<---------- Enums ---------->>

        public enum InputType { // beware when changing name because there is compile conditions using this values
            Keyboard,
            Mouse,
            JoystickController,
            Mobile
        }

        #endregion <<---------- Enums ---------->>

        
        
        
        #region <<---------- Properties and Fields ---------->>

		public static InputType ActiveInputType {
			get => _activeInputType;
			private set {
				if (_activeInputType == value) return;
                if(!value.IsMouseOrKeyboard() && !_activeInputType.IsMouseOrKeyboard()) Debug.Log($"Input type changed to '{value.ToString()}'");
				_activeInputType = value;
				_inputTypeChanged?.Invoke(value);
			}
		}
		private static InputType _activeInputType;

		public static event Action<InputType> InputTypeChanged {
			add {
				_inputTypeChanged -= value;
				_inputTypeChanged += value;
			}
			remove => _inputTypeChanged -= value;
		}
		private static Action<InputType> _inputTypeChanged;
        
        #endregion <<---------- Properties and Fields ---------->>


        

		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
		private static void InitializeBeforeSceneLoad() {
			Debug.Log($"Initializing {nameof(CInputManager)}");

			SetControllerTypeBasedOnPlatform();
			
			#if Rewired
			if (ReInput.isReady) Initialize();
			else ReInput.InitializedEvent += Initialize;
			#endif
		}

		#if Rewired
		private static void Initialize() {
			
			ReInput.InitializedEvent -= Initialize;

            AssignConnectedControllersToSystemPlayer();

            ReInput.ControllerConnectedEvent += ControllerConnectedEvent;
            
			Observable.EveryUpdate().Subscribe(_ => {
                if (CApplication.IsQuitting || ReInput.controllers == null) {
                    return;
                }
                
				if (ActiveInputType != InputType.Keyboard &&
                    (ReInput.controllers.GetAnyButton(ControllerType.Keyboard))
                    ) {
                    ActiveInputType = InputType.Keyboard;
                    return;
                }
                
                if (ActiveInputType != InputType.Mouse &&
                    (ReInput.controllers.GetAnyButtonChanged(ControllerType.Mouse) || ReInput.controllers.Mouse.screenPositionDelta.sqrMagnitude > 0.1f)
                    ) {
                    ActiveInputType = InputType.Mouse;
                    return;
                }
                
                if (ActiveInputType != InputType.JoystickController &&
                    (ReInput.controllers.GetAnyButtonChanged(ControllerType.Joystick) 
                        || ReInput.controllers.GetAnyButton(ControllerType.Custom) 
                        || ReInput.controllers.GetJoysticks().Any(j => j.PollForFirstAxis().success)
                    )) {
					ActiveInputType = InputType.JoystickController;
                    return;
                }
                
				// TODO check mobile input
                
			});
		}

        static void AssignConnectedControllersToSystemPlayer() {
            
            // Joysticks
            foreach (var joystick in ReInput.controllers.Joysticks) {
                if (joystick == null) continue;
                Debug.Log($"Add joystick to SystemPlayer: {joystick.name}");
                ReInput.players.SystemPlayer.controllers.AddController(joystick.type, joystick.id, false);
            }
            
            // Custom Controllers
            foreach (var cc in ReInput.controllers.CustomControllers) {
                if (cc == null) continue;
                Debug.Log($"Add custom controller to SystemPlayer: {cc.name}");
                ReInput.players.SystemPlayer.controllers.AddController(cc.type, cc.id, false);
            }
        }

        static void ControllerConnectedEvent(ControllerStatusChangedEventArgs c) {
            if (!ReInput.players.SystemPlayer.controllers.ContainsController(c.controller)) {
                Debug.Log($"New controller connected: {c.name}, assigning to System Player.");
                ReInput.players.SystemPlayer.controllers.AddController(c.controllerType, c.controllerId, false);
            }

            var player = ReInput.players.GetPlayer(0); 
            if (player != null && !player.controllers.ContainsController(c.controller)) {
                Debug.Log($"New controller connected: {c.name}, assigning to Player {player.id} ({player.name}).");
                player.controllers.AddController(c.controllerType, c.controllerId, false);
            }

            UpdatePlayerInputLayout(player);
        }
        
		#endif

		private static void SetControllerTypeBasedOnPlatform() {
			#if UNITY_STANDALONE || UNITY_WEBGL
			ActiveInputType = InputType.Keyboard;
			#elif UNITY_ANDROID || UNITY_IOS || UNITY_IPHONE
			ActiveInputType = InputType.Mobile;
			#else
			ActiveInputType = InputType.JoystickController;
			#endif
			Debug.Log($"{nameof(SetControllerTypeBasedOnPlatform)} input type auto set to: {ActiveInputType.ToString()}");
		}
		
        #if Rewired
        public static void UpdatePlayerInputLayout(Rewired.Player rePlayer) {
            if (CApplication.IsQuitting) return;
            
            if (rePlayer == null) {
                Debug.LogError($"cannot set input for a null Rewired.Player");
                return;
            }
            
            if (rePlayer.controllers == null) {
                Debug.LogError($"cannot set input for a Rewired.Player with null controllers");
                return;
            }

            bool onMenu = CBlockingEventsManager.get.IsOnMenu;

            int joystickControllersCount = rePlayer.controllers.joystickCount;
            int customControllersCount = rePlayer.controllers.customControllerCount;
            
            rePlayer.controllers.maps.SetMapsEnabled(!onMenu, "Default");
            rePlayer.controllers.maps.SetMapsEnabled(onMenu, "UI"); 
			
            Debug.Log($"Player ID '{rePlayer.id}' controllers maps onMenu changed to '{onMenu}'\nCustom Controllers: {customControllersCount}, JoystickControllers: {joystickControllersCount}");
        }
        #endif
        
	}
    
    public static class InputTypeExtension {
        public static bool IsMouseOrKeyboard(this CInputManager.InputType input) {
            return (input == CInputManager.InputType.Keyboard || input == CInputManager.InputType.Mouse);
        }
    }
}