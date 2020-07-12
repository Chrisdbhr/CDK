using System;
using CDK.Damage;
using CDK.Data;
using CDK.Enums;
using UniRx;
using UnityEngine;

namespace CDK {
	/// <summary>
	/// Makes something have health, can be hit and die.
	/// </summary>
	public class CHealthComponent : MonoBehaviour, ICDamageable {

		#region <<---------- Properties and Fields ---------->>
	
		#region <<---------- References ---------->>

		[SerializeField] private Animator _animator;
		[NonSerialized] private Transform _lastAttacker;
		
		#endregion <<---------- References ---------->>


		#region <<---------- Animations ---------->>
		
		protected readonly int ANIM_STUN_LIGHT = Animator.StringToHash("lightStun");
		protected readonly int ANIM_STUN_MEDIUM = Animator.StringToHash("mediumStun");
		protected readonly int ANIM_STUN_HEAVY = Animator.StringToHash("heavyStun");
		protected readonly int ANIM_FALLEN = Animator.StringToHash("fallen");
		
		#endregion <<---------- Animations ---------->>

		
		#region <<---------- Health ---------->>
		
		[Header("Health")]
		[SerializeField] protected float _maxHealth = 100f;
		public float MaxHealth {
			get {
				return this._maxHealth;
			}
		}
		
		public float CurrentHealth {
			get { return this._currentHealth; }
			private set {
				if (value == this._currentHealth) return;

				this.OnHealthChanged?.Invoke(value);

				float oldHealth = this._currentHealth;
				this._currentHealth = value;

				if (this._currentHealth > oldHealth) {
					this.OnRecoverHealth?.Invoke(this._currentHealth);
				}
				else {
					this.OnLostHealth?.Invoke(this._currentHealth);
				}
				
				if (this._currentHealth <= 0f) {
					this.IsDead = true;
					return;
				}
				
				if (oldHealth > this._currentHealth) {
					float damage = oldHealth - this._currentHealth;
					this._stunProgress += damage;
				}

			}
		}
		[SerializeField] private float _currentHealth;

		public bool IsDead {
			get { return this._isDead; }
			private set {
				if (this._isDead == value) return;
				this._isDead = value;
				
				this.OnDie?.Invoke();
				
				Debug.Log($"{this.name} died.");
				
				if (this._currentHealth > 0f) {
					this._currentHealth = 0f;
				}
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
		}
		[NonSerialized] private bool _isDead;
		
		#endregion <<---------- Health ---------->>


		#region <<---------- Partial Health Regeneration ---------->>

		[NonSerialized] private float _regenRatePerSecond = 1f;
		[NonSerialized] private float _regenDelayAfterHit = 3f;

		#endregion <<---------- Partial Health Regeneration ---------->>
		

		#region <<---------- Stun ---------->>

		[NonSerialized] private bool _stunned;
		
		public CStunStatus StunStatus {
			get { return this._stunStatus; }
			set {
				if (this._stunStatus == value) return;
				this._stunStatus = value;
				
			}
		}
		private CStunStatus _stunStatus;
		
		[SerializeField] private float _stunRecoveryRatePerSecond = 4f;
		[SerializeField] private float _heavyStunResistance = 50f;

		private float StunProgress {
			get { return this._stunProgress; }
			set {
				if (this._stunProgress == value) return;
				
			}
		}
		[NonSerialized] private float _stunProgress;
		private const float LIGHT_STUN_MULTIPLIER = 0.20f;
		private const float MEDIUM_STUN_MULTIPLIER = 0.60f;
		
		#endregion <<---------- Stun ---------->>
		
		
		[SerializeField] private Transform[] _unparentOnDie;
		[SerializeField] private GameObject[] _activateOnDie;
		[SerializeField] private GameObject[] _destroyOnDie;

		
		
		
		#region <<---------- Actions ---------->>

		public event Action<float> OnHealthChanged;
		public event Action<float> OnRecoverHealth;
		public event Action<float> OnLostHealth;
		public event Action OnDie;
		public event Action<float, CHitInfoData> OnTakeDamage;
		
		#endregion <<---------- Actions ---------->>
		
		#endregion <<---------- Properties and Fields ---------->>

		
		

		#region <<---------- MonoBehaviour ---------->>
		
		#if UNITY_EDITOR
		private void Reset() {
			if (this.transform != this.transform.root) {
				Debug.LogWarning("Health component is not on a root transform! This object will not take damage!");
			}
		}
		#endif
		
		private void Awake() {
			if (this.transform != this.transform.root) {
				Debug.LogWarning("Health component is not on a root transform! This object will not take damage!");
			}
			this.Revive();
		}

		private void OnEnable() {
			// regen
			Observable.Timer(TimeSpan.FromSeconds(this._regenRatePerSecond)).RepeatUntilDisable(this).Subscribe(_ => {
				if (this.IsDead) return;
				Debug.Log("Regen");
			});

			// stun
			Observable.Timer(TimeSpan.FromSeconds(this._stunRecoveryRatePerSecond)).RepeatUntilDisable(this).Subscribe(_ => {
				if (this.IsDead) return;
				this._stunProgress -= this._stunRecoveryRatePerSecond;
			});

		}

		#endregion <<---------- MonoBehaviour ---------->>
		



		private void Revive() {
			this._currentHealth = this._maxHealth;
			this._stunProgress = 0f;
		}


		public bool TakeDamage(CHitInfoData hitInfo) {
			if (this.IsDead) return false;
			var hitScriptObj = hitInfo.ScriptableObject;
			if (hitScriptObj.Damage <= 0f) return false;

			this._lastAttacker = hitInfo.AttackerTransform;

			// Start total damage calculation.
			float finalDamage = hitScriptObj.Damage;

			//todo calculate armor damage reduction
			//todo calculate damage bonus
			//todo repulsion by animation
			//todo play damage animation if apply

			if (hitScriptObj.LookAtAttacker) {
				this.transform.LookAt(hitInfo.AttackerTransform.transform);
				this.transform.eulerAngles = new Vector3(0f, this.transform.eulerAngles.y, 0f);
			}


			if (finalDamage > 0f) {
				this.OnTakeDamage?.Invoke(finalDamage, hitInfo);
			}
			this.CurrentHealth -= finalDamage;
			
			//TODO camera shake
			return true;
		}

	}

}

