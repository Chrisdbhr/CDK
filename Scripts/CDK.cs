// CDK (Chris Development Kit) is developed and frequently updated by @Chrisdbhr
// Those are part of Source Code of all my games developed with Unity
// Don't forget to Star it and check https://github.com/Chrisdbhr/CDK for updates. 
//
// Also check my website for my projects! https://chrisjogos.com

using System;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditorInternal;
#endif

namespace CDK 
{
	#if UNITY_EDITOR
	[InitializeOnLoad]
	#endif
	public static class CDK 
	{
		static CDK() 
		{
			Debug.Log($"CDK Version {Version}");
		} 
		public static readonly Version Version = new (5,0,5);
	}
}