using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Shashki.Attributes.Editor
{
    [CustomPropertyDrawer(typeof(ShowIfAttribute))]
    public sealed class ShowIfDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            ShowIfAttribute showIf = (ShowIfAttribute)attribute;

            Object targetObject = property.serializedObject.targetObject;
            System.Type targetType = targetObject.GetType();

            FieldInfo dependentField = targetType.GetField(showIf.fieldName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            if (dependentField != null)
            {
                object dependentValue = dependentField.GetValue(targetObject);

                if (dependentValue != null && dependentValue.Equals(showIf.desiredValue))
                {
                    EditorGUI.PropertyField(position, property, label, true);
                }
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            ShowIfAttribute showIf = (ShowIfAttribute)attribute;
            Object targetObject = property.serializedObject.targetObject;
            System.Type targetType = targetObject.GetType();

            FieldInfo dependentField = targetType.GetField(showIf.fieldName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            if (dependentField != null)
            {
                object dependentValue = dependentField.GetValue(targetObject);
                if (dependentValue != null && dependentValue.Equals(showIf.desiredValue))
                {
                    return EditorGUI.GetPropertyHeight(property, label, true);
                }
            }

            return 0;
        }
    }
}
