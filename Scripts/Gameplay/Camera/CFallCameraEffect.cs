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
		get { return _ySpeed; }
		set {
			if (_ySpeed == value) return;
			if (value == 0) {
				TimeFalling = 0;
				_audioSource.Stop();
			}else if (!_audioSource.isPlaying) {
				_audioSource.Play();
			}
			
			_ySpeed = value;
			
			if (_ySpeed < 0) {
				// was falling
				if (value > _ySpeed) {
					// landed
					TimeFalling = 0;
				}
				else {
					// continued falling
					TimeFalling += CTime.DeltaTimeScaled;
				}
			}
			
			_ySpeed = value;
		}
	}
	[SerializeField] private float _ySpeed;

	private float TimeFalling {
		get { return _timeFalling; }
		set {
			if (_timeFalling == value) return;

			float factor = value + (YSpeed).CAbs();
			_animator.SetFloat(ANIM_FALLTIME, factor * _animTimeMultiplier);
			_audioSource.volume = (factor.CAbs() * _volumeMultiplier).CClamp01();
			_audioSource.pitch = 1 + (factor * _pitchMultiplier);

			_timeFalling = value;
		}
	}
	[SerializeField] private float _timeFalling;
	
	
	[NonSerialized] private float _lastYPos;
	[NonSerialized] private Transform _transform;
	
	
	
	
	private void Awake() {
		_transform = transform;
	}

	private void Update() {
		var currentYPos = _transform.position.y;
		YSpeed = (currentYPos - _lastYPos).CImprecise();
		_lastYPos = currentYPos;
		
		#if UNITY_EDITOR
		if (Input.GetKeyDown(KeyCode.R)) {
			SceneManager.LoadSceneAsync(gameObject.scene.buildIndex);
		}
		#endif
	}
	
}
