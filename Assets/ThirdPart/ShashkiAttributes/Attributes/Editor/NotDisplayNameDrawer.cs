using UnityEngine;
using UnityEditor;

namespace Shashki.Attributes.Editor
{
    [CustomPropertyDrawer(typeof(NotDisplayName), true)]
    public sealed class NotDisplayNameDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) =>
            EditorGUI.PropertyField(position, property, new GUIContent(string.Empty));

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) =>
            EditorGUI.GetPropertyHeight(property, label);
    }
}
