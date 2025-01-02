using System;
using System.Linq;
using CDK.UI;
using Reflex.Core;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace CDK {
    [AddComponentMenu("CDK/DI/CDK ProjectInstaller")]
    public class ProjectInstaller : MonoBehaviour, IInstaller
    {
        public void InstallBindings(ContainerBuilder builder)
        {
            builder
                .AddSingleton(container => {
                    var all = Resources.LoadAll<UISoundsBankSO>("");
                    #if UNITY_EDITOR
                    if(!all.CIsNullOrEmpty()) return all.First();
                    return CScriptableObjectExtensions.EditorCreateInResourcesFolder<UISoundsBankSO>();
                    #endif
                    return all.First();
                }, typeof(UISoundsBankSO))
                .AddSingleton(typeof(CBlockingEventsManager))
                .AddSingleton(container => CAssets.LoadResourceAndInstantiate<CLoadingCanvas>("System/Loading Canvas"))
                .AddSingleton(typeof(CInputManager))
                .AddSingleton(typeof(CCursorManager))
                .AddSingleton(container => new GameObject("UI Navigation Manager").AddComponent<CUINavigationManager>().Init(container), typeof(CUINavigationManager))
            ;
        }
    }
}