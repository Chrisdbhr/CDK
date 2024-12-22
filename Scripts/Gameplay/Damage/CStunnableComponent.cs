using System;
using System.Collections;
using CDK.Enums;
using UnityEngine;

namespace CDK {
	/// <summary>
	/// Game object with this component can be stunned.
	/// </summary>
	[RequireComponent(typeof(CHealthComponent))]
	public class CStunnableComponent : MonoBehaviour {
		
		#region <<---------- Properties and Fields ---------->>
		
		// References
		[NonSerialized] CHitInfoData _lastAttackData;
		[NonSerialized] CHealthComponent _health;
		
		// Animation
		[NonSerialized] Animator _animator;
		[NonSerialized] int ANIM_LIGHTSTUN = Animator.StringToHash("stunLight");
		[NonSerialized] int ANIM_MEDIUMSTUN = Animator.StringToHash("stunMedium");
		[NonSerialized] int ANIM_HEAVYSTUN = Animator.StringToHash("stunHeavy");
		
		// Stun
		const float LIGHT_STUN_FRACTION = 0.20f;
		const float MEDIUM_STUN_FRACTION = 0.60f;
		[NonSerialized] bool _stunned;
		[SerializeField, Min(0f)] float _stunRecoveryRatePerSecond = 0.3f;
		[SerializeField] float _heavyStunResistance = 50f;
		
		public CStunStatus StunStatus {
			get { return _stunStatus; }
			private set {
				if (_stunStatus == value) return;
				_stunStatus = value;

				switch (_stunStatus) {
					case CStunStatus.lightStun: {
						_animator.CSetTriggerSafe(ANIM_LIGHTSTUN);
						break;
					}
					case CStunStatus.mediumStun: {
						_animator.CSetTriggerSafe(ANIM_MEDIUMSTUN);
						break;
					}
					case CStunStatus.heavyStun: {
						_animator.CSetTriggerSafe(ANIM_HEAVYSTUN);
						break;
					}
				}

			}
		}
		CStunStatus _stunStatus;

		float StunProgress {
			get { return _stunProgress; }
			set {
				if (_stunProgress == value) return;
				_stunProgress = value;
				if (_stunProgress < 0) {
					_stunProgress = 0f;
					return;
				}

				float stunPercentage = _stunProgress / _heavyStunResistance;

				if (stunPercentage >= 1f) {
					StunStatus = CStunStatus.heavyStun;
				}else if (stunPercentage >= MEDIUM_STUN_FRACTION) {
					StunStatus = CStunStatus.mediumStun;
				}else if (stunPercentage >= LIGHT_STUN_FRACTION) {
					StunStatus = CStunStatus.lightStun;
				}
				else {
					StunStatus = CStunStatus.none;
				}
				
			}
		}
		[NonSerialized] float _stunProgress;
		
		#endregion <<---------- Properties and Fields ---------->>

		

		
		#region <<---------- MonBehaviour ---------->>

		void Awake() {
			_health = GetComponent<CHealthComponent>();
			_animator = GetComponent<Animator>();
		}

		void OnEnable() {
			_health.OnDamageTaken += DamageTake;
			_health.OnRevive += Revived;
			this.CStartCoroutine(StunRecoveryRoutine());
		}

		void OnDisable() {
			_health.OnDamageTaken -= DamageTake;
			_health.OnRevive -= Revived;
		}
		
		#endregion <<---------- MonBehaviour ---------->>

		IEnumerator StunRecoveryRoutine()
		{
			while (enabled) {
				yield return new WaitForSeconds(_stunRecoveryRatePerSecond);
				if (_health.IsDead) continue;
				StunProgress -= _stunRecoveryRatePerSecond;
			}
		}

		void DamageTake(float dmgAmount, CHitInfoData attack, Transform attacker) {
			StunProgress += dmgAmount;
			_lastAttackData = attack;
		}

		void Revived() {
			_stunProgress = 0f;
		}

	}
}
