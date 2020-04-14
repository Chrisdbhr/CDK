using System.Collections.Generic;

namespace CDK {
	public static class CReflectionHelpers {
		
		// Code by @Bunny83 from this Unity Answer: https://answers.unity.com/questions/983125/c-using-reflection-to-automate-finding-classes.html
		public static System.Type[] GetAllDerivedTypes(this System.AppDomain aAppDomain, System.Type aType)
		{
			var result = new List<System.Type>();
			var assemblies = aAppDomain.GetAssemblies();
			foreach (var assembly in assemblies)
			{
				var types = assembly.GetTypes();
				foreach (var type in types)
				{
					if (type.IsSubclassOf(aType))
						result.Add(type);
				}
			}
			return result.ToArray();
		}
	}
}
