using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace CDK {
	public class CRetainable {

		#region <<---------- Properties and Fields ---------->>

        public bool IsRetained {
            get {
                UpdateRetainedState();
                return _isRetained;
            }
            set {
                if(_isRetained == value) return;
                _isRetained = value;
                OnRetainedStateChanged.Invoke(this, value);
            }
        }
        [NonSerialized] bool _isRetained;

        public event EventHandler<bool> OnRetainedStateChanged = delegate { };

        #if UNITY_EDITOR
        public IReadOnlyCollection<object> DebugRetainedObjectsCollection => _retainedObjects;
        #endif
        [NonSerialized] List<object> _retainedObjects;

		#endregion <<---------- Properties and Fields ---------->>




		#region <<---------- Initializers ---------->>

		public CRetainable() {
            _retainedObjects = new ();
        }

		#endregion <<---------- Initializers ---------->>




		#region <<---------- General ---------->>

		public void Retain(object source) {
            if (_retainedObjects.Contains(source)) return;
            _retainedObjects.Add(source);
            UpdateRetainedState();
		}

		public void Release(object source) {
            if (!_retainedObjects.Remove(source)) return;
            UpdateRetainedState();
        }

        void UpdateRetainedState() {
            int count = _retainedObjects.Count;
            if (count <= 0) {
                IsRetained = false;
                return;
            }
            for (int i = count - 1; i >= 0; i--) {
                var currentObject = _retainedObjects[i];
                if (currentObject is Object uObject) {
                    if (uObject != null) continue;
                }
                else {
                    if (currentObject != null) continue;
                }
                Debug.LogWarning($"Removing null retainable at index {i}/{count}.");
                _retainedObjects.RemoveAt(i);
            }
            IsRetained = _retainedObjects.Count > 0;
        }

        #endregion <<---------- General ---------->>

    }
}