using System;
using CDK.Damage;
using CDK.Data;
using UniRx;
using UnityEngine;

namespace CDK {
	/// <summary>
	/// Makes something have health, can be hit and die.
	/// </summary>
	public class CHealthComponent : MonoBehaviour {

		#region <<---------- Properties and Fields ---------->>
	
		#region <<---------- References ---------->>

		[SerializeField] private Animator _animator;
		[Space]
		[SerializeField] private CTransformShake _transformShake;

		private Transform _transform;
		public Transform LastAttacker => this._lastAttacker;
		private Transform _lastAttacker;
		
		#endregion <<---------- References ---------->>


		#region <<---------- Animations ---------->>
		
		protected readonly int ANIM_STUN_LIGHT = Animator.StringToHash("lightStun");
		protected readonly int ANIM_STUN_MEDIUM = Animator.StringToHash("mediumStun");
		protected readonly int ANIM_STUN_HEAVY = Animator.StringToHash("heavyStun");
		protected readonly int ANIM_FALLEN = Animator.StringToHash("fallen");
		
		#endregion <<---------- Animations ---------->>

		
		#region <<---------- Health ---------->>
		
		[Header("Health")]
		[SerializeField] protected float _maxHealth = 3f;
		public float MaxHealth {
			get {
				return this._maxHealth;
			}
		}
		
		public float CurrentHealth {
			get { return this._currentHealth; }
			private set {
				var clampedValue = value.CClamp(0f, this._maxHealth);
				if (clampedValue == this._currentHealth || clampedValue == this._maxHealth && this._currentHealth == this._maxHealth) return;

				float oldHealth = this._currentHealth;
				this._currentHealth = clampedValue;

				if (oldHealth <= 0f) {
					this.OnRevive?.Invoke();
				}

				this.OnHealthChanged?.Invoke(this._currentHealth);
				this.HealthChangedAsStringEvent?.Invoke(this._currentHealth.ToString("0"));
				this.HealthChangedPercentageEvent?.Invoke(this._maxHealth != 0 ? this._currentHealth / this._maxHealth : 0);

				if (this._currentHealth > oldHealth) {
					this.OnRecoverHealth?.Invoke(this._currentHealth);
				}
				else {
					this._lastDamageTakenTime = Time.timeSinceLevelLoad;
				}
				
				if (this._currentHealth <= 0f) {
					foreach (var obj in this._unparentOnDie) {
						obj.parent = null;
					}
					foreach (var obj in this._activateOnDie) {
						obj.SetActive(true);
					}
					foreach (var obj in this._destroyOnDie) {
						Destroy(obj.gameObject);
					}
					this.OnDie?.Invoke();
				}
			}
		}
		[SerializeField] private float _currentHealth;

		public float CurrentHealthNormalized => this._maxHealth != 0f ? this._currentHealth / this._maxHealth : 0f;
		
		public bool IsDead {
			get { return this._currentHealth <= 0f; }
		}
		
		#endregion <<---------- Health ---------->>


		#region <<---------- Partial Health Regeneration ---------->>

		[SerializeField] private float _regenAmountPerTick = 0.20f;
		[SerializeField] private float _regenDelayAfterHit = 3f;
		private float _lastDamageTakenTime;
		private float _lastDamageValue;

		#endregion <<---------- Partial Health Regeneration ---------->>


		#region <<---------- Immunity ---------->>

		private float _immuneTimer;

		#endregion <<---------- Immunity ---------->>
		
		
		#region <<---------- Events ---------->>

		[SerializeField] private Transform[] _unparentOnDie;
		[SerializeField] private GameObject[] _activateOnDie;
		[SerializeField] private GameObject[] _destroyOnDie;

		[SerializeField] private CUnityEventString HealthChangedAsStringEvent;
		[SerializeField] private CUnityEventFloat HealthChangedPercentageEvent;
		[SerializeField] private CUnityEvent TokeDamageEvent;
		
		public event Action<float> OnHealthChanged;
		public event Action<float, CHitInfoData> OnDamageTaken;
		public event Action<float> OnRecoverHealth;
		public event Action OnDie;

		public event Action OnRevive;

		#endregion <<---------- Events ---------->>
		
		#endregion <<---------- Properties and Fields ---------->>

		
		

		#region <<---------- MonoBehaviour ---------->>
		
		private void Awake() {
			this._transform = this.transform;
			this.FullCure();
		}

		private void OnEnable() {
			// regen only one hearth
			Observable.Timer(TimeSpan.FromSeconds(1f)).RepeatUntilDisable(this).Subscribe(_ => {
				if (this.IsDead) return;
				if (Time.timeSinceLevelLoad < (this._lastDamageTakenTime + this._regenDelayAfterHit)) return;

				int healthInt = (int)this.CurrentHealth;

				// dont heal if full hearth.
				if (healthInt == this.CurrentHealth) return;

				// check if will heal all
				float nextHealth = this.CurrentHealth + this._regenAmountPerTick;
				if (nextHealth > healthInt + 1) {
					// will heal more than necessary, fix it
					nextHealth = healthInt + 1;
				}

				this.CurrentHealth = nextHealth;
			});
		}

		private void Update() {
			if (_immuneTimer > 0f) {
				_immuneTimer -= CTime.DeltaTimeScaled;
			}
		}
		#endregion <<---------- MonoBehaviour ---------->>



		public void SetImmunityTime(float time) {
			var newTime = Mathf.Max(this._immuneTimer, time);
			Debug.Log($"Setting immune time to {newTime}");
			this._immuneTimer = newTime;
		}

		public void FullCure() {
			this.CurrentHealth = this._maxHealth;
		}

        public void Kill() {
            CurrentHealth = 0f;
        }

		/// <summary>
		/// Returns the amount of damage taken.
		/// </summary>
		public float TakeDamage(CHitInfoData hitInfo, float damageMultiplier) {
			if (this.IsDead) return 0f;
			if (this._immuneTimer > 0f) return 0f;
			var hitScriptObj = hitInfo.ScriptableObject;
			if (hitScriptObj.Damage <= 0f) return 0f;

			this._lastAttacker = hitInfo.AttackerTransform;

			// Start total damage calculation.
			float finalDamage = hitScriptObj.Damage * damageMultiplier;
			
			//todo calculate armor damage reduction
			//todo calculate damage bonus
			//todo repulsion by animation
			//todo play damage animation if apply

			if (hitScriptObj.LookAtAttacker) {
				this._transform.LookAt(hitInfo.AttackerTransform.transform);
				this._transform.eulerAngles = new Vector3(0f, this._transform.eulerAngles.y, 0f);
			}

			this.CurrentHealth -= finalDamage;
			if (finalDamage > 0f) {
				this.OnDamageTaken?.Invoke(finalDamage, hitInfo);
				this.TokeDamageEvent?.Invoke();
			}

			// camera shake
			if (this._transformShake != null && hitInfo.AttackerTransform != null) {
				this._transformShake.RequestShake(
					(hitInfo.AttackerTransform.position - this._transform.position).normalized * (finalDamage * 0.01f), 
					hitScriptObj.DamageShakePattern,
					hitScriptObj.ShakeMultiplier
				);
			}

			this._lastAttacker = hitInfo.AttackerTransform;
			
			return finalDamage;
		}

		public void RecoverHealth(float recoverAmount) {
			this.CurrentHealth += recoverAmount;
		}

	}

}

