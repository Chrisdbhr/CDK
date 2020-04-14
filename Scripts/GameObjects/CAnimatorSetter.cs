using System;
using UnityEngine;

namespace CDK
{
    public class CAnimatorSetter : MonoBehaviour
    {
        [SerializeField] private RuntimeAnimatorController _runtimeAnimatorController;


        private void Awake()
        {
            this.GetAnimator();
        }

        private void OnDrawGizmosSelected()
        {
            this.GetAnimator();
        }

        private void GetAnimator()
        {
            var animator = GetComponent<Animator>();
            if (animator != null && this._runtimeAnimatorController != null)
            {
                animator.runtimeAnimatorController = this._runtimeAnimatorController;
#if !UNITY_EDITOR
                Destroy(this);                
#endif
            }
        }
        
    }

}
