using System;
using System.Collections.Generic;

namespace CDK {
    public static class CIDisposableExtensions {

        public static void CAddTo(this IDisposable diposable, ICollection<IDisposable> disposableCollection) {
            disposableCollection.Add(diposable);
        }
        
    }
}