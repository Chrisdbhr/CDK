using System;
using UnityEngine;
using R3;

namespace CDK {
	public class CClockGameObject : MonoBehaviour {
	
		#region <<---------- Properties and Fields ---------->>
		
		public int seconds = 0;
		public int minutes = 0;
		public int hour = 0;
		public bool realTime = true;

		public Transform pointerSeconds;
		public Transform pointerMinutes;
		public Transform pointerHours;

		public float clockSpeed = 1.0f; // 1.0f = realtime, < 1.0f = slower, > 1.0f = faster

		[NonSerialized] private float msecs = 0;
	
		#endregion <<---------- Properties and Fields ---------->>

		
		
		
		#region <<---------- MonoBehaviour ---------->>
	
		private void Awake() {
			this.UpdateTime();
            Observable.Timer(TimeSpan.FromSeconds(1),TimeSpan.FromSeconds(1))
            .Subscribe(_ => {
                this.UpdateTime();
            })
            .AddTo(this);
		}

		#endregion <<---------- MonoBehaviour ---------->>

		
		
		
		#region <<---------- General ---------->>

		private void UpdateTime() {
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
			if(this.pointerSeconds) this.pointerSeconds.localEulerAngles = new Vector3(0.0f, 0.0f, rotationSeconds);
			if(this.pointerMinutes) this.pointerMinutes.localEulerAngles = new Vector3(0.0f, 0.0f, rotationMinutes);
			if(this.pointerHours) this.pointerHours.localEulerAngles = new Vector3(0.0f, 0.0f, rotationHours);
		}
		
		#endregion <<---------- General ---------->>

	}

}