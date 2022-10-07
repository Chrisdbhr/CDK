using UnityEditor;
using UnityEngine;

namespace CDK {
    [ExecuteInEditMode]
    public class CBakeSceneOnOpenEditor : MonoBehaviour {
        private void Awake() {
            if (Application.isPlaying) return;
            #if UNITY_EDITOR
            Debug.Log("Auto baking scene.", this);
            Lightmapping.BakeAsync();
            #endif
        }
    }
}