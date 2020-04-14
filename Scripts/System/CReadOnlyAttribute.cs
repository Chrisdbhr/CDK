using UnityEngine;
using System;

namespace CDK {
	[AttributeUsage( AttributeTargets.Field, Inherited = true )]
	public class CReadOnlyAttribute : PropertyAttribute { }

	#if UNITY_EDITOR
	[UnityEditor.CustomPropertyDrawer( typeof( CReadOnlyAttribute ) )]
	public class CReadOnlyAttributeDrawer : UnityEditor.PropertyDrawer {
		public override void OnGUI( Rect rect, UnityEditor.SerializedProperty prop, GUIContent label ) {
			bool wasEnabled = GUI.enabled;
			GUI.enabled = false;
			UnityEditor.EditorGUI.PropertyField( rect, prop );
			GUI.enabled = wasEnabled;
		}
	}
	#endif
}
