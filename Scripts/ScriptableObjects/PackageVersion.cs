using UnityEngine;
using System.Linq;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Compilation;
using PackageInfo = UnityEditor.PackageManager.PackageInfo;
#endif

public class PackageVersion : ScriptableObject
{
    [HideInInspector] public string Version;

#if UNITY_EDITOR
    // Called automatically after open Unity and each recompilation
    // See https://docs.unity3d.com/ScriptReference/InitializeOnLoadMethodAttribute.html
    [InitializeOnLoadMethod]
    private static void Init()
    {
        // See https://docs.unity3d.com/ScriptReference/Compilation.CompilationPipeline-compilationFinished.html
        // Removing the callback before adding it makes sure it is only added once at a time
        CompilationPipeline.compilationFinished -= OnCompilationFinished;
        CompilationPipeline.compilationFinished += OnCompilationFinished;
    }

    private static void OnCompilationFinished(object o)
    {
        // First get the path of the Package
        // This is quite easy since this script itself belongs to your package's assemblies
        var assembly = typeof(PackageVersion).Assembly;

        // See https://docs.unity3d.com/ScriptReference/PackageManager.PackageInfo.FindForAssembly.html
        var packageInfo = PackageInfo.FindForAssembly(assembly);

        // Finally we have access to the version!
        var version = packageInfo.version;

        // Now to the ScriptableObject instance
        // Try to find the first instance
        // See https://docs.unity3d.com/ScriptReference/AssetDatabase.FindAssets.html
        // See https://docs.unity3d.com/ScriptReference/PackageManager.PackageInfo-assetPath.html
        var guid = AssetDatabase.FindAssets($"t:{nameof(PackageVersion)}", new []{packageInfo.assetPath}).FirstOrDefault();
        PackageVersion asset;
        if(!string.IsNullOrWhiteSpace(guid))
        {
            var path = AssetDatabase.GUIDToAssetPath(guid);
            asset = AssetDatabase.LoadAssetAtPath<PackageVersion>(path);
        }
        else
        {
            // None found -> create a new one
            asset = ScriptableObject.CreateInstance<PackageVersion>();
            asset.name = nameof(PackageVersion);
            // make it non editable via the Inspector
            // See https://docs.unity3d.com/ScriptReference/HideFlags.NotEditable.html
            asset.hideFlags = HideFlags.NotEditable;

            // Store the asset as actually asset
            // See https://docs.unity3d.com/ScriptReference/AssetDatabase.CreateAsset.html
            AssetDatabase.CreateAsset(asset, $"{packageInfo.assetPath}/{nameof(PackageVersion)}");
        }

        asset.Version = version;

        // See https://docs.unity3d.com/ScriptReference/EditorUtility.SetDirty.html
        EditorUtility.SetDirty(asset);
    }
#endif
}
