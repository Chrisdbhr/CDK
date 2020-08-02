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
		
		[NonSerialized] private CHealthComponent _health;
		[NonSerialized] private bool _stunned;
		
		public CStunStatus StunStatus {
			get { return this._stunStatus; }
			set {
				if (this._stunStatus == value) return;
				this._stunStatus = value;
				
			}
		}
		private CStunStatus _stunStatus;
		
		[SerializeField] private float _stunRecoveryRatePerSecond = 0.3f;
		[SerializeField] private float _heavyStunResistance = 50f;

		private float StunProgress {
			get { return this._stunProgress; }
			set {
				if (this._stunProgress == value) return;
				
				if (this._stunProgress < 0) {
					this._stunProgress = 0f;
					return;
				}
				
				
			}
		}
		[NonSerialized] private float _stunProgress;
		private const float LIGHT_STUN_MULTIPLIER = 0.20f;
		private const float MEDIUM_STUN_MULTIPLIER = 0.60f;
		
		#endregion <<---------- Properties and Fields ---------->>
		
		
		
		
		#region <<---------- MonBehaviour ---------->>
		
		private void Awake() {
			this._health = this.GetComponent<CHealthComponent>();

			
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
		}

		private void Revived() {
			this._stunProgress = 0f;
		}

		#endregion <<---------- Callbacks ---------->>
		
	}
}
