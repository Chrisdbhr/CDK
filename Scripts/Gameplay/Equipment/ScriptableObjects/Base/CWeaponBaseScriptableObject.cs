using System;
using UnityEngine;

namespace CDK.Weapons {
	public abstract class CWeaponBaseScriptableObject : CEquipableBaseScriptableObject {
		
		public CEquippedWeaponGameObject EquippedWeaponPrefab {
			get { return this._equippedWeaponPrefab; }
		}
		[SerializeField] private CEquippedWeaponGameObject _equippedWeaponPrefab;

	}
}
