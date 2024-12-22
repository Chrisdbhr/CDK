using System;
using UnityEngine;
using UnityEngine.Events;

#if UNITY_ADDRESSABLES_EXIST
using UnityEngine.AddressableAssets;
#endif

namespace CDK {
	public class CInventory : MonoBehaviour {
		
		#region <<---------- Properties and Fields ---------->>

		// animation
		[SerializeField] Animator _characterAnimator;
		
		// items
		#if UNITY_ADDRESSABLES_EXIST
		[SerializeField] private AssetReference _inventoryViewRef;
		#endif
		[NonSerialized] CInventoryView _inventoryViewSpawned;
	
		public int Size {
			get { return _size; }
		}
		[SerializeField] int _size = 8;

		[field: NonSerialized]
		public CIItemBase[] InventoryItems { get; private set; }

		// equipament
		[SerializeField] CWeaponData _firstEquipedWeapon;
		[SerializeField] Transform _handTransform;
		[SerializeField] UnityEvent _onAttackEvent;
		[SerializeField] CUnityEventFloat _onAttackRecoilEvent;
		
		[NonSerialized] CWeaponGameObject _currentSpawnedWeapon;

		public CWeaponData EquippedWeapon {
			get { return _equippedWeapon; }
			set {
				if(_equippedWeapon == value) return;
				_equippedWeapon = value;
				OnEquippedWeaponChanged(this, value);
			}
		}
		[NonSerialized] CWeaponData _equippedWeapon;
		public event EventHandler<CWeaponData> OnEquippedWeaponChanged = delegate { };
		[NonSerialized] CWeaponScriptableObject[] _quickSlotWeapons = new CWeaponScriptableObject[4];

		#endregion <<---------- Properties and Fields ---------->>


		

		#region <<---------- MonoBehaviour ---------->>

		void Awake() {
			if (_handTransform) {
				Debug.Log($"HandTransform object is null!");
			}
			InventoryItems = new CIItemBase[Size];
		}

		void OnEnable() {
			EquippedWeapon = _firstEquipedWeapon;
			OnEquippedWeaponChanged += EquippedWeaponChanged;
		}

		void OnDisable() {
			OnEquippedWeaponChanged -= EquippedWeaponChanged;
		}

		#endregion <<---------- MonoBehaviour ---------->>


		
		
		#region <<---------- Managment ---------->>

		void UpdateView() {
			if(_inventoryViewSpawned != null) _inventoryViewSpawned.UpdateInventoryView();
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
			for (int i = 0; i < InventoryItems.Length; i++) {
				if (InventoryItems[i] == null) continue;
				alreadyHasThisItem = InventoryItems[i].GetScriptableObject() == item.GetScriptableObject();
				if (alreadyHasThisItem) {
					// item exists, increase it quantity
					InventoryItems[i].Add(item.Count);
					UpdateView();
					return true;
				}
			}
			
			// check for a null space to fill
			for (int i = 0; i < InventoryItems.Length; i++) {
				if (InventoryItems[i] != null) continue;

				Debug.Log($"Collected null item o empty space.");
				InventoryItems[i] = item;

				if (item is CWeaponData weapon) {
					// auto equip gun
					Debug.Log($"Auto equipping collected weapon.");
					EquippedWeapon = weapon;
				}

				UpdateView();
				return true;
			}

			// inventory is full
			Debug.Log($"Inventory is full.");
			UpdateView();
			return false;
		}

		public void DropItem(CItemBaseScriptableObject item) {
			
			UpdateView();
		}
		
		#endregion <<---------- Managment ---------->>


		
		
		#region <<---------- Actions ---------->>

		public void UseOrEquipItem(int itemIndex) {
			var rootInventory = transform.root.GetComponent<CInventory>();
			if (rootInventory == null) {
				Debug.LogError($"Could not find root inventory.");
			}

			var item = InventoryItems[itemIndex];
			if (item == null) {
				Debug.LogError($"Could not equip or use item at inventory index {itemIndex} because itemData is null");
				return;
			}

			if (item is CWeaponData weapon) {
				// equip
				EquippedWeapon = weapon;
			}
			else {
				// use
				print($"TODO implement usage of item {InventoryItems[itemIndex].GetScriptableObject().ItemName}");
			}
			
			UpdateView();
		}
		
		public  void ExamineItem(int itemIndex) {
			print($"TODO implement Examine on item {InventoryItems[itemIndex].GetScriptableObject().ItemName}");
			UpdateView();
		}
		
		public void DiscardOrDropItem(int itemIndex, bool drop = false) {
			if (drop) {
				var posToDrop = transform.position + Vector3.up * 0.5f;
				var itemScriptObj = InventoryItems[itemIndex].GetScriptableObject();
				var droppedItem = Instantiate(itemScriptObj.ItemMeshWhenDropped, posToDrop, itemScriptObj.ItemMeshWhenDropped.transform.rotation);
				var collectable = droppedItem.GetComponent<CCollectableItemGameObject>(); 
				collectable.SetItemHere(InventoryItems[itemIndex]);
			}
			
			// unequip weapon when dropped
			if (InventoryItems[itemIndex] as CWeaponData == EquippedWeapon) { 
				EquippedWeapon = null;
			}
			InventoryItems[itemIndex] = null;
			UpdateView();
		}

		#endregion <<---------- Actions ---------->>

		#region Callbacks

		void EquippedWeaponChanged(object sender, CWeaponData newWeapon)
		{
			if(_currentSpawnedWeapon != null) _currentSpawnedWeapon.gameObject.CDestroy();

			// reset animator parameters
			foreach (var animEquipString in Enum.GetNames(typeof(CEquipableBaseScriptableObject.AnimEquipStringType))) {
				_characterAnimator.SetBool(animEquipString, false);
			}

			if (newWeapon == null) {
				// no equipment
				return;
			}
			if (newWeapon.GetScriptableObject() is CWeaponScriptableObject weaponScripObj) {
				if (weaponScripObj.EquippedWeaponPrefab != null) {
					// create world object representation
					_currentSpawnedWeapon = Instantiate(weaponScripObj.EquippedWeaponPrefab, _handTransform);

					// set animator parameters
					_characterAnimator.SetBool(weaponScripObj.AnimEquipString.ToString(), true);
				}
			}
			else {
				Debug.LogError($"New weapon is not a weapon of type {nameof(CWeaponScriptableObject)}");
			}
		}

		void OnAttack(float damage) {
			_onAttackEvent?.Invoke();
			Debug.Log($"Camera shake");
		}

		#endregion Callbacks

		/// <summary>
		/// Returns ammo data consumed by the weapon.
		/// </summary>
		public CAmmoData GunShootConsumeAmmo() {
			if (EquippedWeapon == null) {
				Debug.LogError($"Could not fire Weapon because it is not a Gun!");
				return null;
			}

			if (EquippedWeapon.EquippedAmmoData.Count <= 0) {
				Debug.Log($"Cant fire gun because its has no more ammo.");
				return null;
			}
			float damage = ((CAmmoScriptableObject) EquippedWeapon.EquippedAmmoData.GetScriptableObject()).AttackData.data.RawDamage;

			_onAttackRecoilEvent?.Invoke(damage * 0.1f);
			
			OnAttack(damage);
			EquippedWeapon.EquippedAmmoData.ConsumeAmmo();
			return EquippedWeapon.EquippedAmmoData;
		}

	}
}