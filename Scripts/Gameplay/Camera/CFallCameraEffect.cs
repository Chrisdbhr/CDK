using System;
using CDK;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CFallCameraEffect : MonoBehaviour {

	private readonly int ANIM_FALLTIME = Animator.StringToHash("fallTime");
	
	[SerializeField] private Animator _animator;
	[SerializeField] private AudioSource _audioSource;
	
	[Header("Multipliers")]
	[SerializeField] private float _animTimeMultiplier = 1f;
	[SerializeField] private float _volumeMultiplier = 10f;
	[SerializeField] private float _pitchMultiplier = 10f;

	private float YSpeed {
		get { return this._ySpeed; }
		set {
			if (this._ySpeed == value) return;
			if (value == 0) {
				this.TimeFalling = 0;
				this._audioSource.Stop();
			}else if (!this._audioSource.isPlaying) {
				this._audioSource.Play();
			}
			
			this._ySpeed = value;
			
			if (this._ySpeed < 0) {
				// was falling
				if (value > this._ySpeed) {
					// landed
					this.TimeFalling = 0;
				}
				else {
					// continued falling
					this.TimeFalling += CTime.DeltaTimeScaled;
				}
			}
			
			this._ySpeed = value;
		}
	}
	[SerializeField] private float _ySpeed;

	private float TimeFalling {
		get { return this._timeFalling; }
		set {
			if (this._timeFalling == value) return;

			float factor = value + (this.YSpeed).CAbs();
			this._animator.SetFloat(this.ANIM_FALLTIME, factor * this._animTimeMultiplier);
			this._audioSource.volume = (factor.CAbs() * this._volumeMultiplier).CClamp01();
			this._audioSource.pitch = 1 + (factor * this._pitchMultiplier);

			this._timeFalling = value;
		}
	}
	[SerializeField] private float _timeFalling;
	
	
	[NonSerialized] private float _lastYPos;
	[NonSerialized] private Transform _transform;
	
	
	
	
	private void Awake() {
		this._transform = this.transform;
	}

	private void Update() {
		var currentYPos = this._transform.position.y;
		this.YSpeed = (currentYPos - this._lastYPos).CImprecise();
		this._lastYPos = currentYPos;
		
		#if UNITY_EDITOR
		if (Input.GetKeyDown(KeyCode.R)) {
			SceneManager.LoadSceneAsync(this.gameObject.scene.buildIndex);
		}
		#endif
	}
	
}
