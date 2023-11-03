using System.Text;
using TMPro;
using UnityEditor;
using UnityEngine;

namespace CDK.Editor {
    
    [InitializeOnLoad]
    public class CImprovedHierarchyView  {

        private static StringBuilder sb = new StringBuilder ();
        private static GUIStyle guiStyleRight = new GUIStyle();
        private static string text = null;
        private static float gray = 0.5f;
        private static Component[] componentList;
        private static bool applicationNameShown;
        
        static CImprovedHierarchyView() {
            EditorApplication.hierarchyWindowItemOnGUI += HierarchyWindowItemOnGUI;
            guiStyleRight.alignment = TextAnchor.MiddleRight;
        }

        static void HierarchyWindowItemOnGUI (int instanceID, Rect selectionRect) {
            if(!(EditorUtility.InstanceIDToObject(instanceID) is GameObject obj))return;

            if (Event.current.type != EventType.Repaint) {
                applicationNameShown = false;
                return;
            }

            // repaint
            
            WriteApplicationName(selectionRect);

            guiStyleRight.normal.textColor = new Color(gray,gray,gray);

           
            // ITERATE ON EVERY COMPONENT
            componentList = obj.GetComponents(typeof(Component));

            string objName = obj.name;

            if (componentList.Length == 1) {
                // Managers titles
                Color rectColor = new Color(0.3f, 0.3f, 0.3f);
                EditorGUI.DrawRect(selectionRect, rectColor);
                GUI.Label(selectionRect, objName
                    .Replace("---", string.Empty)
                    .Replace("--", string.Empty)
                    .Replace("# ", string.Empty)
                    .Trim()
                    , EditorStyles.boldLabel);
                if (!obj.activeInHierarchy) {
                    rectColor = new Color(rectColor.r, rectColor.g, rectColor.b, rectColor.a * 0.5f);
                    EditorGUI.DrawRect(selectionRect, rectColor);
                }
                return;
            }

            foreach (var comp in componentList) {
                if(comp == null) continue;
                // Contains any Trigger Component
                text = "Trigger";
                if (comp.GetType().Name.Contains(text)) {
                    guiStyleRight.normal.textColor = new Color(0.4f,0.4f, .4f);
                    guiStyleRight.Draw(selectionRect, new GUIContent(text), instanceID );
                    return;
                }
            
                // Contains any Controller Component
                text = "Controller";
                if (comp.GetType().Name.Contains(text)) {
                    guiStyleRight.normal.textColor = new Color(0.45f,0.45f, .45f);
                    guiStyleRight.Draw(selectionRect, new GUIContent(text), instanceID );
                    return;
                }
    
            }

            
            
            
            #region <<---------- Unity Objects ---------->>

            // Canvas
            if (obj.GetComponent<Canvas>() != null) {
                guiStyleRight.Draw(selectionRect, new GUIContent("Canvas"), instanceID );
                return;
            }
            
            // Particle System
            if (obj.GetComponent<ParticleSystem>() != null) {
                guiStyleRight.Draw(selectionRect, new GUIContent("Particle System"), instanceID );
                return;
            }

            // Text Mesh Pro
            if (obj.GetComponent<TextMeshPro>() != null) {
                guiStyleRight.normal.textColor = new Color(0.4f,0.4f, 1f);
                guiStyleRight.Draw(selectionRect, new GUIContent("Text Mesh Pro"), instanceID );
                return;
            }
            
            // Text Mesh Pro UGUI
            if (obj.GetComponent<TextMeshProUGUI>() != null) {
                guiStyleRight.normal.textColor = new Color(0.5f,0.5f, 1f);
                guiStyleRight.Draw(selectionRect, new GUIContent("Text Mesh Pro UGUI"), instanceID );
                return;
            }

            // Text (Legacy)
            if (obj.GetComponent<UnityEngine.UI.Text>() != null) {
                guiStyleRight.normal.textColor = new Color(.9f,.7f, .4f);
                guiStyleRight.Draw(selectionRect, new GUIContent("Text (Legacy)"), instanceID );
                return;
            }

            // Image
            if (obj.GetComponent<UnityEngine.UI.Image>() != null) {
                guiStyleRight.normal.textColor = new Color(.8f,.8f, .5f);
                guiStyleRight.Draw(selectionRect, new GUIContent("Image"), instanceID );
                return;
            }

            // Camera
            if (obj.GetComponent<Camera>() != null) {
                guiStyleRight.Draw(selectionRect, new GUIContent("Camera"), instanceID );
                return;
            }
            
            // Rigidbody
            if (obj.GetComponent<Rigidbody>() != null) {
                guiStyleRight.normal.textColor = new Color(.7f,.3f, .3f);
                guiStyleRight.Draw(selectionRect, new GUIContent("Rigidbody"), instanceID );
                return;
            }

            // Collider
            if (obj.GetComponent<Collider>() != null) {
                guiStyleRight.normal.textColor = new Color(.1f,.6f, .1f);
                guiStyleRight.Draw(selectionRect, new GUIContent("Collider"), instanceID );
                return;
            }

            #endregion <<---------- Unity Objects ---------->>
            
        }

        private static void WriteApplicationName(Rect selectionRect) {
            if (applicationNameShown) return;
            guiStyleRight.normal.textColor = new Color(.5f,.5f,.5f,.5f);
            EditorGUI.LabelField(new Rect(0f,0f, selectionRect.width, selectionRect.height), GetProjectName(), guiStyleRight );
            applicationNameShown = true;
        }
        
        
        public static string GetProjectName()
        {
            string[] s = Application.dataPath.Split('/');
            string projectName = s[s.Length - 2];
            return projectName;
        }
        
    }
}