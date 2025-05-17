using UnityEditor;

using UnityEngine;

namespace Shashki.Attributes.Editor
{
    [CustomPropertyDrawer(typeof(RequireReferenceAttribute), true)]
    public sealed class RequireReferenceDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            RequireReferenceAttribute requiredReference = (RequireReferenceAttribute)attribute;

            EditorGUI.PropertyField(position, property, label);

            if (property.propertyType == SerializedPropertyType.ObjectReference && property.objectReferenceValue == null)
            {
                EditorGUILayout.HelpBox(requiredReference.errorMessage, UnityEditor.MessageType.Error);

                if (requiredReference.throwErrorInConsole)
                    throw new System.NullReferenceException("Field: <color=red>" + property.name + "</color> need reference.\n" + requiredReference.errorMessage);
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            float baseHeight = EditorGUI.GetPropertyHeight(property, label, true);

            return baseHeight;
        }
    }
}
