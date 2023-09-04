/// From @austintaylorx in https://forum.unity.com/threads/how-to-create-an-avatar-mask-for-custom-gameobject-hierarchy-from-scene.574270/#post-4398478

using UnityEditor;
using UnityEngine;

namespace CDK.Editor {
    public class AvatarMaker {
        [MenuItem("Tools/Avatar Mask/Make Avatar Mask")]
        private static void MakeAvatarMask() {
            GameObject activeGameObject = Selection.activeGameObject;
            if (activeGameObject == null) return;
            UnityEngine.AvatarMask avatarMask = new UnityEngine.AvatarMask();
            avatarMask.AddTransformPath(activeGameObject.transform);
            var path = string.Format("Assets/{0}.mask", activeGameObject.name.Replace(':', '_'));
            AssetDatabase.CreateAsset(avatarMask, path);
        }

        [MenuItem("Tools/Avatar Mask/Make Generic Avatar")]
        private static void MakeAvatar() {
            GameObject activeGameObject = Selection.activeGameObject;
            if (activeGameObject == null) return;
            Avatar avatar = AvatarBuilder.BuildGenericAvatar(activeGameObject, "");
            avatar.name = activeGameObject.name;
            Debug.Log(avatar.isHuman ? "is human" : "is generic");
            var path = string.Format("Assets/{0}.ht", avatar.name.Replace(':', '_'));
            AssetDatabase.CreateAsset(avatar, path);
        }
    }
}