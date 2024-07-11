using CDK.UI;
using Reflex.Core;
using UnityEngine;

namespace CDK.DI {
    [AddComponentMenu("CDK/DI/CDK ProjectInstaller")]
    public class ProjectInstaller : MonoBehaviour, IInstaller
    {
        public void InstallBindings(ContainerBuilder builder)
        {
            builder
                .AddSingleton(container => new CBlockingEventsManager(), typeof(CBlockingEventsManager))
                .AddSingleton(container => new CInputManager(container), typeof(CInputManager))
                .AddSingleton(container => new CCursorManager(container), typeof(CCursorManager))
                .AddSingleton(container => new CUINavigationManager(container), typeof(CUINavigationManager))
            ;
        }
    }
}