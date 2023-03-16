using UnityEngine;

namespace CDK {
	public static class CConst {
		public const string EDITOR_SCRIPTABLEOBJECT_CREATION_PREFIX = "ScriptableObject/";

        public static WaitForEndOfFrame WaitForEndOfFrame => _waitForEndOfFrame;
        private static WaitForEndOfFrame _waitForEndOfFrame = new WaitForEndOfFrame();

	}
}
