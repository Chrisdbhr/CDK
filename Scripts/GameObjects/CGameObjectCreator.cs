using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace CDK
{
    public class CGameObjectCreator : CAutoTriggerCompBase
    {
        [SerializeField] private GameObject objToCreate;


        protected override void TriggerEvent() {
            if (!this.enabled) {
                Destroy(this);
                return;
            }
            this.CreateObjs();
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            var editorCam = Camera.current;
            if (editorCam == null) return;

            float distToDraw = 20f;
            float editorCamDist = Vector3.Distance(this.transform.position, editorCam.transform.position);
            if (editorCamDist > distToDraw) return;
            
            Vector3 currentPos = this.transform.position;
            Vector3 textSpace = Vector3.down * (editorCamDist * 0.025f);

            //Gizmos.color = Color.green;
            //Gizmos.DrawWireSphere(currentPos, 0.05f);

            Handles.color = Color.green;
            Handles.Label(currentPos + (textSpace * -1), "OBJs TO CREATE HERE");

            Handles.color = Color.white;
            if (this.objToCreate != null) {
                Handles.Label(currentPos + (textSpace), $"- {this.objToCreate.name}");
            }
            Gizmos.color = Color.white;
        }
#endif
        

        private void CreateObjs() {
            this.CInstantiate(this.objToCreate, this.transform.position, this.transform.rotation);
        }
    }
}