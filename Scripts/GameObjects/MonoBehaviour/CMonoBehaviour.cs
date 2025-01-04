using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace CDK
{
    public class CMonoBehaviour : MonoBehaviour
    {
        protected new Transform transform;

        protected virtual void Awake()
        {
            this.transform = base.transform;
            gameObject.Inject();
            InjectFields();
        }

        void InjectFields()
        {
            InjectGetComponent();
            InjectGetComponentInChildren();
            InjectGetComponentInParent();
        }

        void InjectGetComponentInParent()
        {
            foreach (var field in GetFieldsWithAttribute(typeof(GetComponentInParentAttribute))) {
                TrySetFieldValue(field, GetComponentInParent(field.FieldType));
            }
        }
        void InjectGetComponentInChildren()
        {
            foreach (var field in GetFieldsWithAttribute(typeof(GetComponentInChildrenAttribute))) {
                TrySetFieldValue(field, GetComponentInChildren(field.FieldType));
            }
        }
        void InjectGetComponent()
        {
            foreach (var field in GetFieldsWithAttribute(typeof(GetComponentAttribute)))
            {
                TrySetFieldValue(field, GetComponent(field.FieldType));
            }
        }

        IEnumerable<FieldInfo> GetFieldsWithAttribute(Type attributeType)
        {
            var fields = GetType()
                .GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                .Where(field => field.GetCustomAttributes(attributeType, true).FirstOrDefault() != null);

            return fields;
        }

        void TrySetFieldValue(FieldInfo field, Component component)
        {
            if (field.GetValue(this) != null) {
                Debug.LogWarning($"Will not override field value of '{field.FieldType.Name}'", this);
                return;
            }
            if (component == null) {
                Debug.LogError("Component '" + field.FieldType.Name + "' in game object '" + gameObject.name + "' is null", this);
                return;
            }
            field.SetValue(this, component);
        }
    }
}