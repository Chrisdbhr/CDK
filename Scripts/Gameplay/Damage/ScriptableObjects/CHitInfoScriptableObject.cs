using System;
using UnityEngine;

namespace CDK {
	[CreateAssetMenu(fileName = "Hit info", menuName = CConst.EDITOR_SCRIPTABLEOBJECT_CREATION_PREFIX + "Hit info data", order = 51)]
	public class CHitInfoScriptableObject : ScriptableObject {
		
		public float Damage {
			get { return this._damage; }
		}
		[SerializeField] private float _damage = 1f;

		public bool LookAtAttacker {
			get { return this._lookAtAttacker; }
		}
		[SerializeField] private bool _lookAtAttacker;

		public AnimationCurve DamageShakePattern {
			get {
				return this._damageShakePattern;
			}	
		}
		[SerializeField] private AnimationCurve _damageShakePattern = new AnimationCurve(new [] {
																									new Keyframe(0f, 1f), 
																									new Keyframe(0.1f,-0.5f),
																									new Keyframe(0.2f,0f)
																								});

		public float ShakeMultiplier {
			get { return this._shakeMultiplier; }
		}
		[SerializeField] private float _shakeMultiplier = 1f;
		

	}
}
