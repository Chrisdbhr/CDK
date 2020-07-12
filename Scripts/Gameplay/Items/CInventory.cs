using System;
using UniRx;
using UnityEngine;
using UnityEngine.Events;

namespace CDK {
	public class CInventory : MonoBehaviour {
		
		#region <<---------- Properties and Fields ---------->>

		// animation
		[SerializeField] private Animator _characterAnimator;
		
		// items
		[SerializeField] private CInventoryView _inventoryViewPrefab;
		[NonSerialized] private CInventoryView _inventoryViewSpawned;
	
		public int Size {
			get { return this._size; }
		}
		[SerializeField] private int _size = 8;

		[field: NonSerialized]
		public CIItemBase[] InventoryItems { get; private set; }

		// equipament
		[SerializeField] private CWeaponData _firstEquipedWeapon;
		[SerializeField] private Transform _handTransform;
		[SerializeField] private UnityEvent _onAttackEvent;
		[SerializeField] private CUnityEventFloat _onAttackRecoilEvent;
		
		[NonSerialized] private CWeaponGameObject _currentSpawnedWeapon;
		
		public CWeaponData EquippedWeapon {
			get { return this._equippedWeaponRx.Value; }
		}
		[NonSerialized] private ReactiveProperty<CWeaponData> _equippedWeaponRx;
		[NonSerialized] private CWeaponScriptableObject[] _quickSlotWeapons = new CWeaponScriptableObject[4];

		[NonSerialized] private CompositeDisposable _compositeDisposable;
		
		
		#endregion <<---------- Properties and Fields ---------->>


		

		#region <<---------- MonoBehaviour ---------->>
		private void Awake() {
			this._equippedWeaponRx = new ReactiveProperty<CWeaponData>();
			if (this._handTransform) {
				Debug.Log($"HandTransform object is null!");
			}
			this.InventoryItems = new CIItemBase[this.Size];
		}

		private void OnEnable() {
			this._compositeDisposable?.Dispose();
			this._compositeDisposable = new CompositeDisposable();

			this._equippedWeaponRx.Value = this._firstEquipedWeapon;

			this._equippedWeaponRx.Subscribe(newWeapon => {
				if(this._currentSpawnedWeapon != null) this._currentSpawnedWeapon.gameObject.CDestroy();
				
				// reset animator parameters
				foreach (var animEquipString in Enum.GetNames(typeof(CEquipableBaseScriptableObject.AnimEquipStringType))) {
					this._characterAnimator.SetBool(animEquipString, false);
				}

				if (newWeapon == null) {
					// no equipment
					return;
				}
				if (newWeapon.GetScriptableObject() is CWeaponScriptableObject weaponScripObj) {
					if (weaponScripObj.EquippedWeaponPrefab != null) {
						// create world object representation
						this._currentSpawnedWeapon = Instantiate(weaponScripObj.EquippedWeaponPrefab, this._handTransform);
					
						// set animator parameters
						this._characterAnimator.SetBool(weaponScripObj.AnimEquipString.ToString(), true);
					}
				}
				else {
					Debug.LogError($"New weapon is not a weapon of type {nameof(CWeaponScriptableObject)}");
				}
			}).AddTo(this._compositeDisposable);
		}

		private void OnDisable() {
			this._compositeDisposable?.Dispose();
			this._compositeDisposable = null;
		}

		private void Update() {
			if (CBlockingEventsManager.get.IsBlockingEventHappening) return;

			// input menu
			if (Input.GetButtonDown(CInputKeys.MENU) && this._inventoryViewPrefab != null) {
				this.PauseGame();
			}
		}
		
		#endregion <<---------- MonoBehaviour ---------->>


		
		
		#region <<---------- Managment ---------->>

		public void PauseGame() {
			this.CreateView();

		}

		private void CreateView() {
			this._inventoryViewSpawned = Instantiate(this._inventoryViewPrefab, this.transform);
			this._inventoryViewSpawned.CreateInventoryViewItens(this);
			CBlockingEventsManager.get.IsOnMenu.Value = true;
			this._inventoryViewSpawned.OnInventoryClose += () => {
				CBlockingEventsManager.get.IsOnMenu.Value = false;
			};
		}

		private void UpdateView() {
			if(this._inventoryViewSpawned != null) this._inventoryViewSpawned.UpdateInventoryView();
		}

		/// <summary>
		/// Returns TRUE if item add to inventory.
		/// </summary>
		public bool AddItem(CIItemBase item, bool hideNotification = true) {
			if (item == null) {
				Debug.LogError($"Could not add item because item do add is null!");
				return false;
			}
			
			if (!hideNotification) {
				Debug.Log("TODO show new item get notitifcation");
			}
			
			bool alreadyHasThisItem = false;

			// check if already has item to stack
			for (int i = 0; i < this.InventoryItems.Length; i++) {
				if (this.InventoryItems[i] == null) continue;
				alreadyHasThisItem = this.InventoryItems[i].GetScriptableObject() == item.GetScriptableObject();
				if (alreadyHasThisItem) {
					// item exists, increase it quantity
					this.InventoryItems[i].Add(item.Count);
					this.UpdateView();
					return true;
				}
			}
			
			// check for a null space to fill
			for (int i = 0; i < this.InventoryItems.Length; i++) {
				if (this.InventoryItems[i] != null) continue;

				Debug.Log($"Collected null item o empty space.");
				this.InventoryItems[i] = item;

				if (item is CWeaponData weapon) {
					// auto equip gun
					Debug.Log($"Auto equipping collected weapon.");
					this._equippedWeaponRx.Value = weapon;
				}

				this.UpdateView();
				return true;
			}

			// inventory is full
			Debug.Log($"Inventory is full.");
			this.UpdateView();
			return false;
		}

		public void DropItem(CItemBaseScriptableObject item) {
			
			this.UpdateView();
		}
		
		#endregion <<---------- Managment ---------->>


		
		
		#region <<---------- Actions ---------->>

		public void UseOrEquipItem(int itemIndex) {
			var rootInventory = this.transform.root.GetComponent<CInventory>();
			if (rootInventory == null) {
				Debug.LogError($"Could not find root inventory.");
			}

			var item = this.InventoryItems[itemIndex];
			if (item == null) {
				Debug.LogError($"Could not equip or use item at inventory index {itemIndex} because itemData is null");
				return;
			}

			if (item is CWeaponData weapon) {
				// equip
				this._equippedWeaponRx.Value = weapon;
			}
			else {
				// use
				print($"TODO implement usage of item {this.InventoryItems[itemIndex].GetScriptableObject().ItemName}");
			}
			
			this.UpdateView();
		}
		
		public  void ExamineItem(int itemIndex) {
			print($"TODO implement Examine on item {this.InventoryItems[itemIndex].GetScriptableObject().ItemName}");
			this.UpdateView();
		}
		
		public void DiscardOrDropItem(int itemIndex, bool drop = false) {
			if (drop) {
				var posToDrop = this.transform.position + Vector3.up * 0.5f;
				var itemScriptObj = this.InventoryItems[itemIndex].GetScriptableObject();
				var droppedItem = Instantiate(itemScriptObj.ItemMeshWhenDropped, posToDrop, itemScriptObj.ItemMeshWhenDropped.transform.rotation);
				var collectable = droppedItem.GetComponent<CCollectableItemGameObject>(); 
				collectable.SetItemHere(this.InventoryItems[itemIndex]);
			}
			
			// unequip weapon when dropped
			if (this.InventoryItems[itemIndex] as CWeaponData == this.EquippedWeapon) { 
				this._equippedWeaponRx.Value = null;
			}
			this.InventoryItems[itemIndex] = null;
			this.UpdateView();
		}

		#endregion <<---------- Actions ---------->>

		private void OnAttack(float damage) {
			this._onAttackEvent?.Invoke();
			Debug.Log($"Camera shake");
		}

		/// <summary>
		/// Returns ammo data consumed by the weapon.
		/// </summary>
		public CAmmoData GunShootConsumeAmmo() {
			if (this.EquippedWeapon == null) {
				Debug.LogError($"Could not fire Weapon because it is not a Gun!");
				return null;
			}

			if (this.EquippedWeapon.EquippedAmmoData.Count <= 0) {
				Debug.Log($"Cant fire gun because its has no more ammo.");
				return null;
			}
			float damage = ((CAmmoScriptableObject) this.EquippedWeapon.EquippedAmmoData.GetScriptableObject()).HitInfo.ScriptableObject.Damage;

			this._onAttackRecoilEvent?.Invoke(damage * 0.1f);
			
			this.OnAttack(damage);
			this.EquippedWeapon.EquippedAmmoData.ConsumeAmmo();
			return this.EquippedWeapon.EquippedAmmoData;
		}

	}
}
