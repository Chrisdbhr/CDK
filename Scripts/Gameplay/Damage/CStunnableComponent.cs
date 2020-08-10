using System;
using CDK.Data;
using CDK.Enums;
using UniRx;
using UnityEngine;

namespace CDK {
	/// <summary>
	/// Game object with this component can be stunned.
	/// </summary>
	[RequireComponent(typeof(CHealthComponent))]
	public class CStunnableComponent : MonoBehaviour {
		
		#region <<---------- Properties and Fields ---------->>
		
		// References
		[NonSerialized] private CHitInfoData _lastHitInfoData;
		[NonSerialized] private CHealthComponent _health;
		
		// Animation
		[NonSerialized] private Animator _animator;
		[NonSerialized] private int ANIM_LIGHTSTUN = Animator.StringToHash("stunLight");
		[NonSerialized] private int ANIM_MEDIUMSTUN = Animator.StringToHash("stunMedium");
		[NonSerialized] private int ANIM_HEAVYSTUN = Animator.StringToHash("stunHeavy");
		
		// Stun
		private const float LIGHT_STUN_FRACTION = 0.20f;
		private const float MEDIUM_STUN_FRACTION = 0.60f;
		[NonSerialized] private bool _stunned;
		[SerializeField] private float _stunRecoveryRatePerSecond = 0.3f;
		[SerializeField] private float _heavyStunResistance = 50f;
		
		public CStunStatus StunStatus {
			get { return this._stunStatus; }
			private set {
				if (this._stunStatus == value) return;
				this._stunStatus = value;

				switch (this._stunStatus) {
					case CStunStatus.lightStun: {
						this._animator.SetTrigger(this.ANIM_LIGHTSTUN);
						break;
					}
					case CStunStatus.mediumStun: {
						this._animator.SetTrigger(this.ANIM_MEDIUMSTUN);
						break;
					}
					case CStunStatus.heavyStun: {
						this._animator.SetTrigger(this.ANIM_HEAVYSTUN);
						break;
					}
				}


				if (this._lastHitInfoData.ScriptableObject != null && this._lastHitInfoData.ScriptableObject.LookAtAttacker) {
					Debug.Log("TODO look to attacker");
				}

			}
		}
		private CStunStatus _stunStatus;
		
		private float StunProgress {
			get { return this._stunProgress; }
			set {
				if (this._stunProgress == value) return;
				this._stunProgress = value;
				if (this._stunProgress < 0) {
					this._stunProgress = 0f;
					return;
				}

				float stunPercentage = this._stunProgress / this._heavyStunResistance;

				if (stunPercentage >= 1f) {
					this.StunStatus = CStunStatus.heavyStun;
				}else if (stunPercentage >= MEDIUM_STUN_FRACTION) {
					this.StunStatus = CStunStatus.mediumStun;
				}else if (stunPercentage >= LIGHT_STUN_FRACTION) {
					this.StunStatus = CStunStatus.lightStun;
				}
				else {
					this.StunStatus = CStunStatus.none;
				}
				
			}
		}
		[NonSerialized] private float _stunProgress;
		
		#endregion <<---------- Properties and Fields ---------->>

		

		
		#region <<---------- MonBehaviour ---------->>
		
		private void Awake() {
			this._health = this.GetComponent<CHealthComponent>();
			this._animator = this.GetComponent<Animator>();
		}

		private void OnEnable() {
			this._health.OnDamageTaken += this.DamageTake;
			this._health.OnRevive += this.Revived;
			
			// stun
			Observable.Timer(TimeSpan.FromSeconds(this._stunRecoveryRatePerSecond)).RepeatUntilDisable(this).Subscribe(_ => {
				if (this._health.IsDead) return;
				this.StunProgress -= this._stunRecoveryRatePerSecond;
			});
		}

		private void OnDisable() {
			this._health.OnDamageTaken -= this.DamageTake;
			this._health.OnRevive -= this.Revived;
		}
		
		#endregion <<---------- MonBehaviour ---------->>
		
		
		
		
		#region <<---------- Callbacks ---------->>
		
		private void DamageTake(float dmgAmount, CHitInfoData hitInfo) {
			this.StunProgress += dmgAmount;
			this._lastHitInfoData = hitInfo;
		}

		private void Revived() {
			this._stunProgress = 0f;
		}

		#endregion <<---------- Callbacks ---------->>
		
	}
}
