using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Shashki.Attributes.Editor
{
    [CustomPropertyDrawer(typeof(HideIfAttribute))]
    public sealed class HideIfDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var hideIf = (HideIfAttribute)attribute;

            Object targetObject = property.serializedObject.targetObject;
            System.Type targetType = targetObject.GetType();

            FieldInfo dependentField = targetType.GetField(hideIf.fieldName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            if (dependentField != null)
            {
                object dependentValue = dependentField.GetValue(targetObject);

                if (dependentValue != null && dependentValue.Equals(hideIf.desiredValue)) return;
            }

            EditorGUI.PropertyField(position, property, label, true);
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            HideIfAttribute hideIf = (HideIfAttribute)attribute;
            Object targetObject = property.serializedObject.targetObject;
            System.Type targetType = targetObject.GetType();

            FieldInfo dependentField = targetType.GetField(hideIf.fieldName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            if (dependentField != null)
            {
                object dependentValue = dependentField.GetValue(targetObject);
                if (dependentValue != null && dependentValue.Equals(hideIf.desiredValue)) return 0;
            }

            return EditorGUI.GetPropertyHeight(property, label, true);
        }
    }
}
