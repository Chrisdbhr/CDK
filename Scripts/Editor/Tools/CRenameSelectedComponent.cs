using System;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace CDK.Editor {
    public class CRenameSelectedComponent {
        [MenuItem("CONTEXT/Component/Rename GameObject with this component name")]
        private static void RenameGameObjectWithThisComponentName(MenuCommand data) {
            Object context = data.context;
        
            if (!context) return;
        
            var comp = context as Component;
            if (comp == null) return;

            var newName = comp.GetType().Name;

            newName = AddSpacesToCamelCaseSentence(newName);
            newName = RemovePrefix(newName);

            Undo.RecordObject(comp.gameObject, "Rename");
            comp.gameObject.name = newName;
        }
    
        public static string AddSpacesToCamelCaseSentence(string text) {
            if (text.CIsNullOrEmpty()) return text;
            var newText = new StringBuilder(text);
            try {
                for (int i = 1; i < newText.Length; i++) {
                    if (EditorUtility.DisplayCancelableProgressBar("Renaming", "", (float)i / (float)newText.Length)) {
                        if (i == newText.Length - 1) break;
                        if (char.IsLower(newText[i])) continue;
                        newText.Insert(i - 1, ' ');
                    }
                }
            }
            catch (Exception e) {
                Debug.LogException(e);
            }
            finally {
                EditorUtility.ClearProgressBar();
            }
            return newText.ToString();
        }

        public static string RemovePrefix(string text) {
            if (text.CIsNullOrEmpty()) return text;
            var splitted = text.Split(' ');
            if (splitted.Length <= 1) return text;
            if (splitted[0].Length > 1) return text;
            return string.Join(' ', splitted.Skip(1));
        }
    
    }
}
