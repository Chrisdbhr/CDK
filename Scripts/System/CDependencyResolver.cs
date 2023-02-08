using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace CDK {
	// [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
	// public class CInjectAttribute : Attribute {
	// 	
	// }
	
	
	public static class CDependencyResolver {

		
		private static Dictionary<Type, object> Instances;
        private static Dictionary<Type, Func<object>> Binds;
		private static bool _initialized;



		private static void InitializeIfNeeded() {
			if (_initialized) return;
			Instances = new Dictionary<Type, object>();
            Binds = new Dictionary<Type, Func<object>>();
			_initialized = true;
		}
		
		
		
		
		public static T Get<T>() where T : class {
			bool hasInstance = Instances.TryGetValue(typeof(T), out var value);
			
			if (!hasInstance) {
				return CreateInstance<T>();
			}
			
			if (value is T valueCast) {
				return valueCast;
			}

			throw new NullReferenceException($"Could not find a instance for {typeof(T).FullName}");
		}
        
        public static void Bind<T>(Func<object> creationHandler) { // TODO enum comportamento singleton
            InitializeIfNeeded();
            var typeofT = typeof(T);
            Debug.Log($"Binding creation handler for '{typeofT.Name}'");
            Binds.Add(typeofT, creationHandler);
        }

		private static T CreateInstance<T>() where T : class {
			var type = typeof(T);
            if (!Binds.TryGetValue(type, out var creationHandler)) {
                throw new NotImplementedException($"Could not find a way to create '{type.FullName}'");
            }
            var instance = creationHandler();
            Instances.Add(type, instance);
            if (Debug.isDebugBuild) {
                Debug.Log($"Created instance of '{type.Name}'.");
            }
            return instance as T;
        }
		
	}
}
