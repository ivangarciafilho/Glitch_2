using UnityEditor;

using UnityEngine;

namespace Shashki.Attributes.Editor
{
    [CustomPropertyDrawer(typeof(TagSelectorAttribute), true)]
    public sealed class TagSelectorDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            switch (property.propertyType)
            {
                case SerializedPropertyType.String:
                    property.stringValue = EditorGUI.TagField(position, label, property.stringValue);
                    break;

                default:
                    EditorGUILayout.LabelField("Use TagSelector attribute only with strings");
                    break;
            }
        }
    }
}
