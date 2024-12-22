using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace CDK {
	[CreateAssetMenu(fileName = "Hit info", menuName = CConst.EDITOR_SCRIPTABLEOBJECT_CREATION_PREFIX + "Hit info data", order = 51)]
	public class CHitInfoData : ScriptableObject {
		
		public float RawDamage {
			get { return rawDamage; }
		}
		[FormerlySerializedAs("_damage")] [SerializeField] private float rawDamage = 1f;

		public bool LookAtAttacker {
			get { return _lookAtAttacker; }
		}
		[SerializeField] private bool _lookAtAttacker;

		public AnimationCurve DamageShakePattern {
			get {
				return _damageShakePattern;
			}	
		}
		[SerializeField] private AnimationCurve _damageShakePattern = new AnimationCurve(new [] {
																									new Keyframe(0f, 1f), 
																									new Keyframe(0.1f,-0.5f),
																									new Keyframe(0.2f,0f)
																								});

		public float ShakeMultiplier {
			get { return _shakeMultiplier; }
		}
		[SerializeField] private float _shakeMultiplier = 1f;

		public bool CanCritical => _canCritical;
		[SerializeField] bool _canCritical = true;

	}
}
