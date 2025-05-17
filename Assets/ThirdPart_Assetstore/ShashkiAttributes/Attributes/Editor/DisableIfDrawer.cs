using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Shashki.Attributes.Editor
{
    [CustomPropertyDrawer(typeof(DisableIfAttribute))]
    public sealed class DisableIfDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            DisableIfAttribute disableIf = (DisableIfAttribute)attribute;

            Object targetObject = property.serializedObject.targetObject;
            System.Type targetType = targetObject.GetType();

            FieldInfo dependentField = targetType.GetField(disableIf.fieldName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            if (dependentField != null)
            {
                object dependentValue = dependentField.GetValue(targetObject);

                bool isDisabled = dependentValue != null && dependentValue.Equals(disableIf.desiredValue);

                GUI.enabled = !isDisabled;

                Color orig = GUI.color;
                if (GUI.enabled == false)
                    GUI.color = new Color(orig.r, orig.g, orig.b, 1);

                EditorGUI.PropertyField(position, property, label, true);

                GUI.color = orig;
                GUI.enabled = true;
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) =>
            EditorGUI.GetPropertyHeight(property, label, true);
    }
}
