using UnityEngine;

namespace CDK {
	[CreateAssetMenu(fileName = "Hit info", menuName = "Data/Hit info data", order = 51)]
	public class CHitInfoScriptableObject : ScriptableObject {
		
		public float Damage {
			get { return this._damage; }
		}
		[SerializeField] private float _damage = 1f;

		public bool LookAtAttacker {
			get { return this._lookAtAttacker; }
		}
		[SerializeField] private bool _lookAtAttacker;

	}
}
