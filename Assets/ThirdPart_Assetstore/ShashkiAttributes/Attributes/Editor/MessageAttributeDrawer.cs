using UnityEditor;

using UnityEngine;

namespace Shashki.Attributes.Editor
{
    [CustomPropertyDrawer(typeof(MessageAttribute), true)]
    public sealed class MessageAttributeDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var messageAttribute = attribute as MessageAttribute;

            EditorGUI.PropertyField(position, property, true);
            var type = messageAttribute.messageType switch
            {
                MessageType.Info => UnityEditor.MessageType.Info,
                MessageType.Warning => UnityEditor.MessageType.Warning,
                MessageType.Error => UnityEditor.MessageType.Error,
            };
            EditorGUILayout.HelpBox(messageAttribute.message, type);
        }
    }
}