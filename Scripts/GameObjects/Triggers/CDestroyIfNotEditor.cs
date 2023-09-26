using System.Linq;
using Unity.Linq;
using UnityEngine;

namespace CDK {
    public class CDestroyIfNotEditor : MonoBehaviour {
        private void Awake() {
            if (Application.isEditor) return;
            this.DestroyItselfAndLog();
        }

        void DestroyItselfAndLog() {
            Debug.Log($"Destroying {this.name} and all its children: {string.Join(", ", this.gameObject.Children().Select(c=>c.name))}");
            this.gameObject.CDestroy();
        }
    }
}