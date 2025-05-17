using UnityEditor;
using UnityEngine;

namespace Shashki.Attributes.Editor
{
    [CustomPropertyDrawer(typeof(RenameInInspectorAttribute), true)]
    public sealed class RenameInInspectorDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var renameInInspector = attribute as RenameInInspectorAttribute;

            string newName = renameInInspector.newName;

            EditorGUI.PropertyField(position, property, new GUIContent(newName, property.tooltip));
        }


        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) => EditorGUI.GetPropertyHeight(property);
    }
}
