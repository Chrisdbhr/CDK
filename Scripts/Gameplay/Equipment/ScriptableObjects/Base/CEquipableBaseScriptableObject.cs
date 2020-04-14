using System;
using UnityEngine;

namespace CDK.Weapons {
	public abstract class CEquipableBaseScriptableObject : CItemBaseScriptableObject {
		public enum AnimEquipStringType {
			equipPistol, equipShotgun
		}

		public AnimEquipStringType AnimEquipString {
			get {
				return this._animEquipString;
			}
		}
		[SerializeField] private AnimEquipStringType _animEquipString;
	}
}
