using UnityEditor;
using UnityEngine;

namespace Shashki.Attributes.Editor
{
    [CustomPropertyDrawer(typeof(ExtendedHeaderAttribute), true)]
    public sealed class ExtendedHeaderDrawer : PropertyDrawer
    {
        private const int DEFAULT_HEADER_SIZE = 15;
        private const int START_SPACE = 3;
        private const int END_SPACE = 7;
        private Color _lineColor => new Color(0.37f, 0.37f, 0.37f);

        private GUIStyle _headerStyle;
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            ExtendedHeaderAttribute headerAttribute = (ExtendedHeaderAttribute)attribute;
            SetStyles(headerAttribute);

            float headerHeight = _headerStyle.CalcHeight(new GUIContent(headerAttribute.header), position.width);
            var headerRect = new Rect(position.x, position.y + (3 * END_SPACE) + (START_SPACE * (_headerStyle.fontSize / 5)), position.width, headerHeight);

            EditorGUI.LabelField(headerRect, headerAttribute.header, _headerStyle);

            Rect lineRect = new Rect(position.x, headerRect.y + END_SPACE + _headerStyle.fontSize / 10 + (START_SPACE * (_headerStyle.fontSize / 10)) + _headerStyle.fontSize / 2, position.width, 1.5f);
            EditorGUI.DrawRect(lineRect, _lineColor);

            EditorGUILayout.PropertyField(property, true);
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            ExtendedHeaderAttribute headerAttribute = (ExtendedHeaderAttribute)attribute;
            SetStyles(headerAttribute);
            float headerHeight = _headerStyle.CalcHeight(new GUIContent(headerAttribute.header), EditorGUIUtility.currentViewWidth);

            return EditorGUI.GetPropertyHeight(property, true) + headerHeight + (START_SPACE * (_headerStyle.fontSize / 5)) + _headerStyle.fontSize / 3;
        }

        private TextAnchor CalculateHeaderAligment(ExtendedHeaderAttribute extendedHeader) =>
            extendedHeader.headerBinding switch
            {
                HeaderBinding.Middle => TextAnchor.MiddleCenter,
                HeaderBinding.Left => TextAnchor.MiddleLeft,
                HeaderBinding.Right => TextAnchor.MiddleRight,

                _ => throw new System.ArgumentException(nameof(extendedHeader.headerBinding))
            };

        private void SetStyles(ExtendedHeaderAttribute extendedHeader)
        {
            var headerHeight = extendedHeader.headerSize != 0 ? extendedHeader.headerSize : DEFAULT_HEADER_SIZE;

            _headerStyle = new GUIStyle(EditorStyles.boldLabel)
            {
                fontSize = headerHeight,
                alignment = CalculateHeaderAligment(extendedHeader),
                fixedHeight = 0,
            };
        }
    }
}
