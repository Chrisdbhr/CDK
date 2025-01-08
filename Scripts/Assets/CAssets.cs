using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace CDK {
    public static class CAssets {

        #region <<---------- Load From Resources ---------->>

        /// <summary>
        /// Load a Resource asset from the Resources folder.
        /// </summary>
        public static T LoadResource<T>(string address) where T : UnityEngine.Object {
            return Resources.Load<T>(address);
        }

        /// <summary>
        /// Load a Resource from the Resources folder and instantiate it.
        /// </summary>
        public static T LoadResourceAndInstantiate<T>(string address, Vector3 position, Quaternion rotation, Transform parent = null) where T : UnityEngine.Component {
            if (!Application.isPlaying) {
                Debug.LogError($"Will not load from resources because application is not playing.");
                return null;
            }
            var resource = Resources.Load<GameObject>(address);
            if (resource == null) {
                Debug.LogError($"Could not {nameof(LoadResourceAndInstantiate)} from key '{address}'");
                return null;
            }
            return Object.Instantiate(resource, position, rotation, parent).GetComponent<T>();
        }
        
        /// <summary>
        /// Load a Resource from the Resources folder and instantiate it at zero position and identity quaterion rotation.
        /// </summary>
        public static T LoadResourceAndInstantiate<T>(string address, Transform parent = null) where T : UnityEngine.Component {
            return LoadResourceAndInstantiate<T>(address, Vector3.zero, Quaternion.identity, parent);
        }
        
        #endregion <<---------- Load From Resources ---------->>




        #region <<---------- Unloaders ---------->>

        public static void UnloadAsset(GameObject goToUnload, bool releaseAsset = false) {
            if (goToUnload == null) {
                Debug.LogError($"Can't unload a null asset.");
                return;
            }

            goToUnload.CDestroy();
            if (releaseAsset) {
                Resources.UnloadAsset(goToUnload);
            }
        }

        public static void UnloadAsset(Object obj) {
            if (obj == null) {
                Debug.LogError($"Will not unload a null asset.");
                return;
            }
            Debug.Log($"Unloading obj {obj.name}");
            Resources.UnloadAsset(obj);
        }

        #endregion <<---------- Unloaders ---------->>



        public static IEnumerable<T> FindAssetsByType<T>() where T : Object {
            var guids = AssetDatabase.FindAssets($"t:{typeof(T)}");
            foreach (var t in guids) {
                var assetPath = AssetDatabase.GUIDToAssetPath(t);
                var asset = AssetDatabase.LoadAssetAtPath<T>(assetPath);
                if (asset != null) {
                    yield return asset;
                }
            }
        }

	}
}