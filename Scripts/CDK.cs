// CDK (Chris Development Kit) is developed and frequently updated by @Chrisdbhr
// Those are part of Source Code of all my games developed with Unity
// Don't forget to Star it and check https://github.com/Chrisdbhr/CDK for updates. 
//
// Also check my website for my projects! https://chrisjogos.com

using System;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace CDK {
	#if UNITY_EDITOR
	[InitializeOnLoad]
	#endif
	public static class CDK {

		static CDK() {
			#if UNITY_EDITOR
			CGameSettings.EditorCreateGameSettingsResourceIfNeeded();
			#endif
		} 
		
		public static readonly Version VERSION = new Version(2,6,0);

	}
}