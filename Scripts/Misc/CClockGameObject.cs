using System;
using System.Collections;
using UnityEngine;

namespace CDK {
	public class CClockGameObject : MonoBehaviour {
	
		public int seconds = 0;
		public int minutes = 0;
		public int hour = 0;
		public bool realTime = true;

		public Transform pointerSeconds;
		public Transform pointerMinutes;
		public Transform pointerHours;

		public float clockSpeed = 1.0f; // 1.0f = realtime, < 1.0f = slower, > 1.0f = faster

		[NonSerialized] float msecs = 0;

		
		
		
		void Awake() {
			UpdateTime();
			this.CStartCoroutine(ClockTickRoutine());
		}

		IEnumerator ClockTickRoutine() {
			var wait = new WaitForSeconds(1.0f);
			while (enabled) {
				yield return wait;
				UpdateTime();
			}
		}

		void UpdateTime() {
			if (realTime) {
				//-- set real time
				var dateNow = DateTime.Now;
				hour = dateNow.Hour;
				minutes = dateNow.Minute;
				seconds = dateNow.Second;
			}
			else {
				//-- calculate time
				msecs += 1f * clockSpeed;
				if (msecs >= 1.0f) {
					msecs -= 1.0f;
					seconds++;
					if (seconds >= 60) {
						seconds = 0;
						minutes++;
						if (minutes > 60) {
							minutes = 0;
							hour++;
							if (hour >= 24) hour = 0;
						}
					}
				}
			}

			//-- calculate pointer angles
			float rotationSeconds = (360.0f / 60.0f) * seconds;
			float rotationMinutes = (360.0f / 60.0f) * minutes;
			float rotationHours = ((360.0f / 12.0f) * hour) + ((360.0f / (60.0f * 12.0f)) * minutes);

			//-- draw pointers
			if(pointerSeconds) pointerSeconds.localEulerAngles = new Vector3(0.0f, 0.0f, rotationSeconds);
			if(pointerMinutes) pointerMinutes.localEulerAngles = new Vector3(0.0f, 0.0f, rotationMinutes);
			if(pointerHours) pointerHours.localEulerAngles = new Vector3(0.0f, 0.0f, rotationHours);
		}
		
	}

}