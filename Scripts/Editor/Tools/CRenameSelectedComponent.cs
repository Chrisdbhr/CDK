using System.Text;
using UnityEditor;
using UnityEngine;

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
            newName = RemovePrefixes(newName, new[] { "C" });

            Undo.RecordObject(comp.gameObject, "Rename");
            comp.gameObject.name = newName;
        }
    
        public static string AddSpacesToCamelCaseSentence(string text) {
            if (string.IsNullOrEmpty(text))
                return "";
            StringBuilder newText = new StringBuilder(text.Length * 2);
            newText.Append(text[0]);
            for (int i = 1; i < text.Length; i++)
            {
                if (char.IsUpper(text[i]) && text[i - 1] != ' ' && char.IsLower(newText[newText.Length - 1]))
                    newText.Append(' ');
                newText.Append(text[i]);
            }
            return newText.ToString();
        }

        public static string RemovePrefixes(string text, string[] prefixesToRemove) {
            foreach (var prefix in prefixesToRemove) {
                if (
                    text.StartsWith(prefix) && prefix.Length-1 >= 0 && char.IsUpper(text[prefix.Length-1]) 
                    && prefix.Length < text.Length && text[prefix.Length] == ' ') {
                    return text.Substring(prefix.Length);
                }            
            }
            return text;
        }
    
    }
}
