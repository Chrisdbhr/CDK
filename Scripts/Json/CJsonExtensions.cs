#if Newtonsoft_Json_for_Unity
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.UnityConverters;
using Newtonsoft.Json.UnityConverters.Math;
using UnityEngine;
using UnityEngine.Scripting;

namespace CDK {
    [Preserve]
	public static class CJsonExtensions {
	
		public static readonly JsonSerializerSettings DefaultSettings = new JsonSerializerSettings {
			NullValueHandling = NullValueHandling.Ignore,
			MissingMemberHandling = MissingMemberHandling.Ignore,
            Error = (sender, e) => {
                Debug.LogError(e.ErrorContext.Error);
                e.ErrorContext.Handled = true;
            },
			Formatting = Formatting.Indented,
			Converters = new JsonConverter[] {
				new StringEnumConverter(),
				new VersionConverter(),
				new Vector2Converter(),
				new Vector3Converter(),
				new Vector2IntConverter(),
				new Vector3IntConverter(),
            },
			ContractResolver = new UnityTypeContractResolver(),
		};
		
	}
}
#endif