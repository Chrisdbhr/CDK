using System;
using CDK.UI;
using Reflex.Core;
using UnityEngine;

namespace CDK {
    [AddComponentMenu("CDK/DI/CDK ProjectInstaller")]
    public class ProjectInstaller : MonoBehaviour, IInstaller
    {
        public void InstallBindings(ContainerBuilder builder)
        {
            builder
                .AddSingleton(container => Resources.LoadAll<UISoundsBankSO>(String.Empty)[0], typeof(UISoundsBankSO))
                .AddSingleton(typeof(CBlockingEventsManager))
                .AddSingleton(container => CAssets.LoadResourceAndInstantiate<CLoadingCanvas>("System/Loading Canvas"))
                .AddSingleton(typeof(CInputManager))
                .AddSingleton(typeof(CCursorManager))
                .AddSingleton(container => new GameObject("UI Navigation Manager").AddComponent<CUINavigationManager>().Init(container), typeof(CUINavigationManager))
            ;
        }
    }
}