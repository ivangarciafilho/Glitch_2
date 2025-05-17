using UnityEditor;
using UnityEngine;

namespace Shashki.Attributes.Editor
{
    [CustomPropertyDrawer(typeof(ToggleLeftAttribute), true)]
    public sealed class ToggleLeftDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.propertyType is not SerializedPropertyType.Boolean)
                throw new System.Exception($"Attribute {typeof(ToggleLeftAttribute)} must be declared for a field with the <b>bool</b> type");

            property.boolValue = EditorGUI.ToggleLeft(position, label, property.boolValue);
        }
    }
}
