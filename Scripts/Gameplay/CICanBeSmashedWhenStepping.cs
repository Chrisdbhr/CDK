
using UnityEngine;

namespace CDK {
	public interface CICanBeSmashedWhenStepping {

		bool IsSmashed { get; }
		void Smash(Transform smashingTransform);

	}
}