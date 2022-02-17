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
				if (this._currentHealth <= 0f) {
					this.OnDie?.Invoke();
					
					Debug.Log($"{this.name} died.");
				
					foreach (var obj in this._unparentOnDie) {
						obj.parent = null;
					}
					foreach (var obj in this._activateOnDie) {
						obj.SetActive(true);
					}
					foreach (var obj in this._destroyOnDie) {
						Destroy(obj.gameObject);
					}
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
				
			}
		}
		[SerializeField] private float _currentHealth;

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
		
		
		#region <<---------- Events ---------->>

		[SerializeField] private Transform[] _unparentOnDie;
		[SerializeField] private GameObject[] _activateOnDie;
		[SerializeField] private GameObject[] _destroyOnDie;

		[SerializeField] private CUnityEventString HealthChangedAsStringEvent;
		[SerializeField] private CUnityEventFloat HealthChangedPercentageEvent;
		
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

		#endregion <<---------- MonoBehaviour ---------->>
		



		public void FullCure() {
			this.CurrentHealth = this._maxHealth;
		}


		/// <summary>
		/// Returns the amount of damage taken.
		/// </summary>
		public float TakeDamage(CHitInfoData hitInfo) {
			if (this.IsDead) return 0f;
			var hitScriptObj = hitInfo.ScriptableObject;
			if (hitScriptObj.Damage <= 0f) return 0f;

			this._lastAttacker = hitInfo.AttackerRootTransform;

			// Start total damage calculation.
			float finalDamage = hitScriptObj.Damage;
			
			// external damage multiplier
			if (hitInfo.DamageMultiplier != 0f) {
				finalDamage *= hitInfo.DamageMultiplier;
			}

			//todo calculate armor damage reduction
			//todo calculate damage bonus
			//todo repulsion by animation
			//todo play damage animation if apply

			if (hitScriptObj.LookAtAttacker) {
				this._transform.LookAt(hitInfo.AttackerRootTransform.transform);
				this._transform.eulerAngles = new Vector3(0f, this._transform.eulerAngles.y, 0f);
			}

			if (finalDamage > 0f) {
				this.OnDamageTaken?.Invoke(finalDamage, hitInfo);
			}
			this.CurrentHealth -= finalDamage;

			// camera shake
			if (this._transformShake != null && hitInfo.AttackerRootTransform != null) {
				this._transformShake.RequestShake(
					(hitInfo.AttackerRootTransform.position - this._transform.position).normalized * (finalDamage * 0.01f), 
					hitScriptObj.DamageShakePattern,
					hitScriptObj.ShakeMultiplier
				);
			}

			this._lastAttacker = hitInfo.AttackerRootTransform;
			
			return finalDamage;
		}

		public void RecoverHealth(float recoverAmount) {
			this.CurrentHealth += recoverAmount;
		}

	}

}

