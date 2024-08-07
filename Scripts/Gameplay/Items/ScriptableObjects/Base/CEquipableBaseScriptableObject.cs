using System;
using UnityEngine;

namespace CDK {
	public abstract class CEquipableBaseScriptableObject : CItemBaseScriptableObject {
		public enum AnimEquipStringType {
			noEquip, equipPistol, equipShotgun
		}

		public AnimEquipStringType AnimEquipString {
			get {
				return this._animEquipString;
			}
		}
		[Header("Equipable")]
		[SerializeField] private AnimEquipStringType _animEquipString;
	}
}
