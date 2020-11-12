using UniRx;
using UnityEngine;

namespace CDK {
	public interface ICCharacterBase {
		CMovState CurrentMovState { get; }
		Vector2 InputMovementRaw { get; set; }
		Vector3 InputMovementDirRelativeToCam { get; set; }
		bool InputRun { get; set; }
		bool InputAim { get; set; }
		float WalkSpeed { get; }
		float RunSpeed { get; }
		float SlideSpeed { get; }
		float SlideControlAmmount { get; }
	}
}