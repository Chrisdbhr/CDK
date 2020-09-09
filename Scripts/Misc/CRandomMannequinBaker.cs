using System;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace CDK {
	public class CRandomMannequinBaker : MonoBehaviour {
		
		[SerializeField] private AnimationClip[] _animationsToGetPose;
		[SerializeField] private CUnityEventGameObject _mannequinBakedEvent;
		[SerializeField] private Material _mannequinMaterial;

		[NonSerialized] private Animation _animationComponent;

		
		

		private void Awake() {
			this._animationComponent = this.gameObject.AddComponent<Animation>();
			foreach (var anim in this._animationsToGetPose.Where(x => x != null)) {
				this._animationComponent.AddClip(anim, anim.name);
			}
			this._animationComponent.playAutomatically = false;
			this.ChangePose();
		}

		public void ChangePose() {
			var chosenAnim = this._animationsToGetPose.RandomElement();
			if (chosenAnim == null) return;
			var time = Random.Range(0f, chosenAnim.length);
			this._animationComponent.clip = chosenAnim;
			this._animationComponent.CrossFade(chosenAnim.name, 0);
			this._animationComponent[chosenAnim.name].normalizedTime = time.CRemap(0f, chosenAnim.length, 0f, 1f);
			this._animationComponent.Sample();
			this.BakePoseAndDestroyOriginal();
		}

		private void BakePoseAndDestroyOriginal() {
			var allSkinned = this.GetComponentsInChildren<SkinnedMeshRenderer>();
			if (allSkinned.Length <= 0) return;
			var combine = new CombineInstance[allSkinned.Length];

			// bake meshes
			for (int i = 0; i < allSkinned.Length; i++) {
				var bakedMesh = new Mesh();
				allSkinned[i].BakeMesh(bakedMesh);
				combine[i].mesh = bakedMesh;
				combine[i].transform = allSkinned[i].transform.localToWorldMatrix;
			}
			
			var createdGo = new GameObject($"Baked Mesh from: {this.name}");
			createdGo.transform.SetParent(this.transform);
			var meshFilter = createdGo.AddComponent<MeshFilter>();
			meshFilter.sharedMesh = new Mesh();
			meshFilter.sharedMesh.CombineMeshes(combine);
			var meshRenderer = createdGo.AddComponent<MeshRenderer>();
			meshRenderer.sharedMaterial = this._mannequinMaterial ?? allSkinned[0].sharedMaterial;

			this._mannequinBakedEvent?.Invoke(createdGo);

			// destroy originals
			foreach (var smr in allSkinned) {
				smr.gameObject.CDestroy();
			}

		}
		
	}
}
