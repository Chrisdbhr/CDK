using UniRx;
using UnityEngine;

namespace CDK {
	public interface ICCharacterBase {
		CMovState CurrentMovState { get; }
		Vector2 InputMovementDirAbsolute { get; set; }
		Vector3 InputMovementDirRelativeToCam { get; set; }
		bool InputSlowWalk { get; set; }
		bool InputAim { get; set; }
		float WalkSpeed { get; }
		float RunSpeed { get; }
		float SlideSpeed { get; }
		float SlideControlAmmount { get; }
	}
}