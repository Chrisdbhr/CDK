#if UNITY_ADDRESSABLES_EXIST
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace CDK {
    public static class CAddressablesExtensions {

        public static void ClearAddressables() {
            Caching.ClearCache();
            Addressables.UpdateCatalogs();
            Addressables.CleanBundleCache();
        }

    }
}
#endif