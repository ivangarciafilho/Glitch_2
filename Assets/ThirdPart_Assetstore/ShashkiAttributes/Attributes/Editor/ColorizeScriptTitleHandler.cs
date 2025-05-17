using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Shashki.Attributes.Editor
{
    public sealed class ColorizeScriptTitleHandler
    {
        private const float TITLE_HEIGHT = 38;
        private const float BORDER_WIDTH = 2;

        private const float FONT_SIZE_SUMMAND_DENOMINATOR = 5;
        private const float MAX_FONT_SIZE = 20;

        public static void DrawTitle(UnityEngine.Object target)
        {
            var colorizeTitleAttribute = target.GetType().GetCustomAttribute(typeof(ColorizeScriptTitleAttribute), true) as ColorizeScriptTitleAttribute;

            if (colorizeTitleAttribute != null)
            {
                Color colorizeTitleColor;

                Rect rect = EditorGUILayout.GetControlRect(false, TITLE_HEIGHT);

                if (colorizeTitleAttribute.hexTitleColor != string.Empty)
                {
                    if (ColorUtility.TryParseHtmlString(colorizeTitleAttribute.hexTitleColor, out colorizeTitleColor))
                    {
                        DrawColoredRectWithBorder(rect, colorizeTitleColor, Color.black, BORDER_WIDTH);
                    }
                    else throw new Exception("This colordoes not exists!");
                }
                else
                {
                    colorizeTitleColor = colorizeTitleAttribute.titleColor;
                    DrawColoredRectWithBorder(rect, colorizeTitleAttribute.titleColor, Color.black, BORDER_WIDTH);
                }

                var text = target.GetType().Name;

                int fontSize = (int)(EditorGUIUtility.currentViewWidth / Mathf.Clamp(
                    Mathf.Abs(text.Length + FONT_SIZE_SUMMAND_DENOMINATOR - TITLE_HEIGHT / 2) + FONT_SIZE_SUMMAND_DENOMINATOR, 
                    TITLE_HEIGHT / 3,
                    MAX_FONT_SIZE));

                float r = 1.00f - colorizeTitleColor.r;
                float g = 1.00f - colorizeTitleColor.g;
                float b = 1.00f - colorizeTitleColor.b;

                Color inversedColor = new Color(r, g, b);

                DrawTextWithOutline(rect, text, fontSize, inversedColor, Color.black);

                DrawButton(target, rect);

                GUILayout.Space(10);
            }
        }

        private static void DrawColoredRectWithBorder(Rect rect, Color fillColor, Color borderColor, float borderWidth)
        {
            EditorGUI.DrawRect(new Rect(rect.x - borderWidth, rect.y - borderWidth, rect.width + borderWidth * 2, rect.height + borderWidth * 2), borderColor);

            EditorGUI.DrawRect(rect, fillColor);
        }

        private static void DrawTextWithOutline(Rect rect, string text, int fontSize, Color textColor, Color outlineColor)
        {
            GUIStyle style = new GUIStyle(EditorStyles.label);
            style.alignment = TextAnchor.MiddleCenter;
            style.fontSize = fontSize;

            style.normal.textColor = outlineColor;
            EditorGUI.LabelField(new Rect(rect.x - 1, rect.y, rect.width, rect.height), text, style);
            EditorGUI.LabelField(new Rect(rect.x + 1, rect.y, rect.width, rect.height), text, style);
            EditorGUI.LabelField(new Rect(rect.x, rect.y - 1, rect.width, rect.height), text, style);
            EditorGUI.LabelField(new Rect(rect.x, rect.y + 1, rect.width, rect.height), text, style);

            style.normal.textColor = textColor;
            EditorGUI.LabelField(rect, text, style);
        }

        private static void DrawButton(UnityEngine.Object target, Rect rect)
        {
            UnityEngine.Object targetScript = target as MonoBehaviour;
            if (targetScript is null) targetScript = target as ScriptableObject;

            MonoScript script = (targetScript as MonoBehaviour) != null
                ? MonoScript.FromMonoBehaviour(targetScript as MonoBehaviour)
                : (targetScript as ScriptableObject) != null
                ? MonoScript.FromScriptableObject(targetScript as ScriptableObject)
                : throw new ArgumentException(nameof(target));

            if (GUI.Button(rect, "", GUIStyle.none))
            {
                AssetDatabase.OpenAsset(script);
            }
        }
    }
}
