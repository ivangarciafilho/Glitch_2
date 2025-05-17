using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Shashki.Attributes.Editor
{
    [CustomPropertyDrawer(typeof(EnableIfAttribute))]
    public sealed class EnableIfDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EnableIfAttribute enableIf = (EnableIfAttribute)attribute;

            Object targetObject = property.serializedObject.targetObject;
            System.Type targetType = targetObject.GetType();

            FieldInfo dependentField = targetType.GetField(enableIf.fieldName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            if (dependentField != null)
            {
                object dependentValue = dependentField.GetValue(targetObject);

                bool isEnabled = dependentValue != null && dependentValue.Equals(enableIf.desiredValue);

                GUI.enabled = isEnabled;

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
