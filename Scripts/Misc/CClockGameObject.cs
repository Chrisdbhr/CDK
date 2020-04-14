using System;
using UnityEngine;
using UniRx;

namespace CDK {
	public class CClockGameObject : MonoBehaviour {
		//-- set start time 00:00
		public int seconds = 0;
		public int minutes = 0;
		public int hour = 0;
		public bool realTime = true;

		public Transform pointerSeconds;
		public Transform pointerMinutes;
		public Transform pointerHours;

		//-- time speed factor
		public float clockSpeed = 1.0f; // 1.0f = realtime, < 1.0f = slower, > 1.0f = faster

		//-- internal vars
		float msecs = 0;

	
		
	
		private void OnEnable() {
			
			Observable.Timer(TimeSpan.FromSeconds(1)).RepeatUntilDisable(this).Subscribe(_ => {
				if (this.realTime) {
					//-- set real time
					var dateNow = DateTime.Now;
					this.hour = dateNow.Hour;
					this.minutes = dateNow.Minute;
					this.seconds = dateNow.Second;
				}
				else {
					//-- calculate time
					this.msecs += 1f * this.clockSpeed;
					if (this.msecs >= 1.0f) {
						this.msecs -= 1.0f;
						this.seconds++;
						if (this.seconds >= 60) {
							this.seconds = 0;
							this.minutes++;
							if (this.minutes > 60) {
								this.minutes = 0;
								this.hour++;
								if (this.hour >= 24) this.hour = 0;
							}
						}
					}
				}

				//-- calculate pointer angles
				float rotationSeconds = (360.0f / 60.0f) * this.seconds;
				float rotationMinutes = (360.0f / 60.0f) * this.minutes;
				float rotationHours = ((360.0f / 12.0f) * this.hour) + ((360.0f / (60.0f * 12.0f)) * this.minutes);

				//-- draw pointers
				this.pointerSeconds.localEulerAngles = new Vector3(0.0f, 0.0f, rotationSeconds);
				this.pointerMinutes.localEulerAngles = new Vector3(0.0f, 0.0f, rotationMinutes);
				this.pointerHours.localEulerAngles = new Vector3(0.0f, 0.0f, rotationHours);
			});

		}

	}

}