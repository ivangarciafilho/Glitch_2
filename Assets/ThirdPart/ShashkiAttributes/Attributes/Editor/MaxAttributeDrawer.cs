using System;
using UnityEditor;
using UnityEngine;

namespace Shashki.Attributes.Editor
{
    [CustomPropertyDrawer(typeof(MaxAttribute))]
    public sealed class MaxAttributeDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            MaxAttribute max = attribute as MaxAttribute;

            switch (property.propertyType)
            {
                case SerializedPropertyType.Integer:
                    property.intValue =
                    property.intValue > Convert.ToInt32(max.MaxValue) ? Convert.ToInt32(max.MaxValue) : property.intValue;
                    EditorGUI.PropertyField(position, property, label);
                    break;

                case SerializedPropertyType.Float:
                    property.floatValue =
                    property.floatValue > max.MaxValue ? max.MaxValue : property.floatValue;
                    EditorGUI.PropertyField(position, property, label);
                    break;

                default:
                    EditorGUI.LabelField(position, label.text, "Используй int или float");
                    break;
            }
        }
    }
}
