using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.UnityConverters;
using Newtonsoft.Json.UnityConverters.Math;

namespace CDK.Json {
	public static class CJsonExtensions {
	
		public static readonly JsonSerializerSettings DefaultSettings = new JsonSerializerSettings {
			NullValueHandling = NullValueHandling.Ignore,
			MissingMemberHandling = MissingMemberHandling.Ignore,
			Formatting = Formatting.Indented,
			Converters = new JsonConverter[] {
				new StringEnumConverter(),
				new VersionConverter(),
				new Vector2Converter(),
				new Vector3Converter(),
				new Vector2IntConverter(),
				new Vector3IntConverter()
			},
			ContractResolver = new UnityTypeContractResolver(),
		};
		
	}
}
