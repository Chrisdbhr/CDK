using System;
using System.Collections;
using CDK.Damage;
using CDK.Data;
using UnityEngine;

namespace CDK {
	/// <summary>
	/// Makes something have health, can be hit and die.
	/// </summary>
	public class CHealthComponent : CHitbox {

		#region <<---------- Properties and Fields ---------->>

		#region <<---------- References ---------->>

		[SerializeField] private Animator _animator;
		[Space]
		[SerializeField] private CTransformShake _transformShake;

		private Transform _transform;
		public Transform LastAttacker => _lastAttacker;
		private Transform _lastAttacker;

		#endregion <<---------- References ---------->>


		#region <<---------- Animations ---------->>

		protected readonly int ANIM_STUN_LIGHT = Animator.StringToHash("lightStun");
		protected readonly int ANIM_STUN_MEDIUM = Animator.StringToHash("mediumStun");
		protected readonly int ANIM_STUN_HEAVY = Animator.StringToHash("heavyStun");
		protected readonly int ANIM_FALLEN = Animator.StringToHash("fallen");

		#endregion <<---------- Animations ---------->>


		#region <<---------- Health ---------->>

		public override CHealthComponent Health => this;

		[Header("Health")]
		[SerializeField] protected float _maxHealth = 3f;
		public float MaxHealth {
			get {
				return _maxHealth;
			}
			set {
				_maxHealth = Mathf.Max(0f, value);
			}
		}

		public float CurrentHealth {
			get { return _currentHealth; }
			private set {
				var clampedValue = value.CClamp(0f, MaxHealth);
				if (clampedValue == _currentHealth || clampedValue == MaxHealth && _currentHealth == MaxHealth) return;

				float oldHealth = _currentHealth;
				_currentHealth = clampedValue;

				if (oldHealth <= 0f) {
					OnRevive?.Invoke();
					_isAliveEvent?.Invoke(true);
				}

				OnHealthChanged?.Invoke(_currentHealth);
				HealthChangedAsStringEvent?.Invoke(_currentHealth.ToString("0"));
				HealthChangedPercentageEvent?.Invoke(MaxHealth != 0 ? _currentHealth / MaxHealth : 0);

				if (_currentHealth > oldHealth) {
					OnRecoverHealth?.Invoke(_currentHealth);
				}
				else {
					_lastDamageTakenTime = Time.timeSinceLevelLoad;
				}

				if (_currentHealth <= 0f) {
					foreach (var obj in _unparentOnDie) {
						obj.parent = null;
					}
					foreach (var obj in _activateOnDie) {
						obj.SetActive(true);
					}
					foreach (var obj in _destroyOnDie) {
						Destroy(obj.gameObject);
					}
					OnDie?.Invoke();
					OnDieEvent?.Invoke();
					_isAliveEvent?.Invoke(false);
				}
			}
		}
		[SerializeField] private float _currentHealth;

		public float CurrentHealthNormalized => MaxHealth != 0f ? _currentHealth / MaxHealth : 0f;

		public bool IsDead {
			get { return _currentHealth <= 0f; }
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
		[SerializeField] private CUnityEventBool _isAliveEvent;
		[SerializeField] private CUnityEvent OnDieEvent;

		public event Action<float> OnHealthChanged;
		public event Action<float, CHitInfoData, Transform> OnDamageTaken;
		public event Action<float> OnRecoverHealth;
		public event Action OnDie;

		public event Action OnRevive;

		#endregion <<---------- Events ---------->>

		#endregion <<---------- Properties and Fields ---------->>




		#region <<---------- MonoBehaviour ---------->>

		protected virtual void Awake() {
			_transform = transform;
			FullCure();
		}

		void OnEnable()
		{
			this.CStartCoroutine(HealthRegenRoutine());
		}

		protected virtual void Update() {
			if (_immuneTimer > 0f) {
				_immuneTimer -= CTime.DeltaTimeScaled;
			}
		}

		#endregion <<---------- MonoBehaviour ---------->>


		IEnumerator HealthRegenRoutine()
		{
			var wait = new WaitForSeconds(1f);
			while (enabled) {
				yield return wait;
				if(IsDead) continue;
				if (Time.timeSinceLevelLoad < (_lastDamageTakenTime + _regenDelayAfterHit)) continue;

				int healthInt = (int)CurrentHealth;

				// dont heal if full hearth.
				if (healthInt == CurrentHealth) continue;

				// check if will heal all
				float nextHealth = CurrentHealth + _regenAmountPerTick;
				if (nextHealth > healthInt + 1) {
					// will heal more than necessary, fix it
					nextHealth = healthInt + 1;
				}

				CurrentHealth = nextHealth;
			}
		}

		public void SetImmunityTime(float time) {
			var newTime = Mathf.Max(_immuneTimer, time);
			Debug.Log($"Setting immune time to {newTime}");
			_immuneTimer = newTime;
		}

		public void FullCure() {
			CurrentHealth = MaxHealth;
		}

        public void Kill() {
            CurrentHealth = 0f;
        }

        public void RecoverHealth(float recoverAmount) {
	        CurrentHealth += recoverAmount;
        }

        #region ICDamageable

		/// <summary>
		/// Returns the amount of damage taken. If the target is already dead, returns 0. If the damage is bigger than the health before, returns the health before.
		/// </summary>
		public override float TakeHit(CHitInfoData attack, Transform attacker, float damageMultiplier) {
			if (IsDead) return 0f;
			if (_immuneTimer > 0f) return 0f;
			var hitScriptObj = attack;
			if (hitScriptObj.RawDamage <= 0f) return 0f;

			_lastAttacker = attacker;

			// Start total damage calculation.
			float multipliedDamage = hitScriptObj.RawDamage * damageMultiplier;
			float finalDamage = multipliedDamage;

			//todo calculate armor damage reduction
			//todo calculate damage bonus
			//todo repulsion by animation
			//todo play damage animation if apply

			if (hitScriptObj.LookAtAttacker) {
				_transform.LookAt(attacker);
				_transform.eulerAngles = new Vector3(0f, _transform.eulerAngles.y, 0f);
			}

			if (finalDamage >= _currentHealth) {
				finalDamage = _currentHealth;
				CurrentHealth = 0f;
			}
			else CurrentHealth -= finalDamage;

			if (finalDamage > 0f) {
				OnDamageTaken?.Invoke(finalDamage, attack, attacker);
				TokeDamageEvent?.Invoke();
			}

			// camera shake
			if (_transformShake != null && attacker != null) {
				_transformShake.RequestShake(
					(attacker.position - _transform.position).normalized * (finalDamage * 0.01f),
					hitScriptObj.DamageShakePattern,
					hitScriptObj.ShakeMultiplier
				);
			}

			return finalDamage;
		}

		#endregion ICDamageable

	}

}

