using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace CDK {
	public class CStartTrigger : MonoBehaviour{

        [SerializeField, Min(0f)] private float _delay;
		[SerializeField] private UnityEvent Event;


        IEnumerator Start() {
            if(_delay > 0f) yield return new WaitForSecondsRealtime(_delay);
            Event?.Invoke();
            yield break;
        }
	}
}