using System;
using UnityEngine;

namespace CDK {
	public abstract class CEquipableBaseScriptableObject : CItemBaseScriptableObject {
		public enum AnimEquipStringType {
			noEquip, equipPistol, equipShotgun
		}

		public AnimEquipStringType AnimEquipString {
			get {
				return _animEquipString;
			}
		}
		[Header("Equipable")]
		[SerializeField] AnimEquipStringType _animEquipString;
	}
}
