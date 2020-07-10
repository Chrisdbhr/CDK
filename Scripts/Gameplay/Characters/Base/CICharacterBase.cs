using UniRx;
using UnityEngine;

namespace CDK {
	public interface CICharacterBase {
		CMovState CurrentMovState { get; }
		Vector2 InputDirAbsolute { get; set; }
		Vector3 InputDirRelativeToCam { get; set; }
		bool InputSlowWalk { get; set; }
		bool InputAim { get; set; }
		float WalkSpeed { get; }
		float RunSpeed { get; }
		float SlideSpeed { get; }
		float SlideControlAmmount { get; }
	}
}