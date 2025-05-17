using UnityEditor;
using UnityEngine;

namespace Shashki.Attributes.Editor
{
    [CustomPropertyDrawer(typeof(LabelAttribute), true)]
    public sealed class LabelDrawer : PropertyDrawer
    {
        private string _label;
        private byte _fontSize;

        private Color _color;

        private TextBinding _labelAnchor;

        private GUIStyle _labelStyle;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var labelAttribute = attribute as LabelAttribute;
            _fontSize = (byte)labelAttribute.labelSize;
            _color = labelAttribute.color;

            if (_fontSize == 0) _fontSize = labelAttribute.labelFontSize;
            _labelAnchor = labelAttribute.labelAnchor;
            _label = labelAttribute.label;

            SetStyles();

            EditorGUILayout.Space(EditorGUIUtility.standardVerticalSpacing + _fontSize / 10);
            EditorGUILayout.LabelField(labelAttribute.label, _labelStyle, GUILayout.Height(CalcHeight().y));
            EditorGUILayout.Space(EditorGUIUtility.standardVerticalSpacing + _fontSize / 10);
            EditorGUILayout.PropertyField(property, true);
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label) => 0;

        private void SetStyles()
        {
            _labelStyle = new GUIStyle(EditorStyles.boldLabel)
            {
                fixedHeight = 0,
                wordWrap = true,
                fontSize = _fontSize,
                normal = new() { textColor = _color},
                alignment = _labelAnchor switch
                {
                    TextBinding.Left => TextAnchor.MiddleLeft,
                    TextBinding.Right => TextAnchor.MiddleRight,
                    TextBinding.Middle => TextAnchor.MiddleCenter,
                    _ => throw new System.NotImplementedException(),
                },
            };
        }

        private Vector2 CalcHeight()
        {
            return _labelStyle.CalcSize(new GUIContent(_label));
        }
    }
}
