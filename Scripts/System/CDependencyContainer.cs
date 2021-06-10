using System;
using System.Collections.Generic;
using UnityEngine;

namespace CDK {
	// [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
	// public class CInjectAttribute : Attribute {
	// 	
	// }
	
	public static class CDependencyContainer {

		private static Dictionary<Type, object> Instances;
		private static Dictionary<Type, Func<object>> Binds;




		public static void Initialize() {
			Instances = new Dictionary<Type, object>();
			Binds = new Dictionary<Type, Func<object>>();
		}
		
		
		
		
		public static T Get<T>() {
			bool hasInstance = Instances.TryGetValue(typeof(T), out var value);
			
			if (!hasInstance) {
				value = CreateInstance<T>();
			}
			
			if (value is T valueCast) {
				return valueCast;
			}

			throw new NullReferenceException($"Could not find a instance for {typeof(T).FullName}");
		}

		public static void Bind<T>(Func<object> creationHandler) {
			Binds.Add(typeof(T), creationHandler);
		}
		
		private static object CreateInstance<T>() {
			var type = typeof(T);
			if (Binds.TryGetValue(type, out var creationHandler)) {
				var instance = creationHandler();
				Instances.Add(type, instance);
				if(Debug.isDebugBuild) Debug.Log($"Creating instance of '{type.Name}'.");
				return instance;
			}
			throw new NotImplementedException($"Could not find a way to create '{nameof(T)}'");
		}
		
	}
}
