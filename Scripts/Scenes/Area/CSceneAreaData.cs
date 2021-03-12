using UnityEngine;

namespace CDK {
	[CreateAssetMenu(fileName = "SceneArea", menuName = CConst.EDITOR_SCRIPTABLEOBJECT_CREATION_PREFIX + "Scene Area data", order = 101)]
	public class CSceneAreaData : ScriptableObject {
		public int Priority => _priority;
		[SerializeField] private int _priority;
		
		public bool CanRun => _canRun;
		[SerializeField] private bool _canRun;
		
	}
}
