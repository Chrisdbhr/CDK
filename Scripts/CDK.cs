// CDK (Chris Development Kit) is developed and frequently updated by @Chrisdbhr
// Those are part of Source Code of all my games developed with Unity
// Don't forget to Star and check https://github.com/Chrisdbhr/CDK for updates. 
using System;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace CDK {
	[InitializeOnLoad]
	public static class CDK {

		#if UNITY_EDITOR
		static CDK() {
			CGameSettings.EditorCreateGameSettingsResourceIfNeeded();
		} 
		#endif
		
		public static readonly Version VERSION = new Version(0,1,1);
		private const string VERSION_UPDATE_URL = "https://raw.githubusercontent.com/Chrisdbhr/CDK/master/Resources/CDKVersion.txt";

	}
}
