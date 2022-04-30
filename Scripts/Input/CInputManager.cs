using System;
using UniRx;
using UnityEngine;

#if Rewired
using Rewired;
#endif

namespace CDK {
	public static class CInputManager {

		public enum InputType { // beware when changing name because there is compile conditions using this values
			MouseAndKeyboard,
			JoystickController,
			Mobile
		}

		public static InputType ActiveInputType {
			get => _activeInputType;
			private set {
				if (value == _activeInputType) return;
				Debug.Log($"Input type changed to '{value.ToString()}'");
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




		/// <summary>
		/// ANTES da scene load.
		/// </summary>
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSplashScreen)]
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

			// wait one frame
			Observable.EveryUpdate().Subscribe(_ => {
				if (ReInput.controllers.GetAnyButtonChanged(ControllerType.Mouse) || ReInput.controllers.GetAnyButton(ControllerType.Keyboard)) {
					ActiveInputType = InputType.MouseAndKeyboard;
				}else if (ReInput.controllers.GetAnyButtonChanged(ControllerType.Joystick) || ReInput.controllers.GetAnyButton(ControllerType.Custom)) {
					ActiveInputType = InputType.JoystickController;
				}
				// TODO check mobile input
			});
		}
		#endif

		private static void SetControllerTypeBasedOnPlatform() {
			#if UNITY_STANDALONE || UNITY_WEBGL
			ActiveInputType = InputType.MouseAndKeyboard;
			#elif UNITY_ANDROID || UNITY_IOS || UNITY_IPHONE
			ActiveInputType = InputType.Mobile;
			#else
			ActiveInputType = InputType.JoystickController;
			#endif
			Debug.Log($"Active input type set to: {ActiveInputType.ToString()}");
		}
		
	}
}