using System;
using CDK;
using UnityEngine;

public class CFallCameraEffect : MonoBehaviour {

	[SerializeField] private Animator _animator;
	[SerializeField] private AudioSource _audioSource;
	[SerializeField] private float _ySpeed;
	[SerializeField] private float _valueMultiplier = 5f;
	
	[NonSerialized] private float _lastYPos;
	[NonSerialized] private Transform _transform;

	private readonly int ANIM_FALLSPEED = Animator.StringToHash("fallSpeed");
	
	
	
	
	private void Awake() {
		this._transform = this.transform;
	}

	private void Update() {
		var currentYPos = this._transform.position.y;
		float newSpeed = (currentYPos - this._lastYPos).CImprecise();
		this._lastYPos = currentYPos;
		if (this._ySpeed == newSpeed) return;
		this._ySpeed = newSpeed;
		
		var speedAbs = newSpeed.CAbs() * this._valueMultiplier;
		this._animator.SetFloat(this.ANIM_FALLSPEED, speedAbs);
		this._audioSource.volume = speedAbs;
		//this._audioSource.pitch = 1 + speedAbs * 0.1f;
		
	}
	
}
