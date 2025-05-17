using System;
using UnityEngine;

namespace Shashki.Attributes
{
    [RequireAttribute(typeof(SerializeField))]
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public sealed class FoldoutAttribute : PropertyAttribute
    {
        public readonly string Title;
        public readonly string[] FieldsNames;
        public Color backColor { get; private set; } = new Color(1, 1, 1);

        public FoldoutAttribute(string foldoutName, string[] fieldsName)
        {
            backColor = new(1, 1, 1);

            Title = foldoutName;
            FieldsNames = fieldsName;
        }

        public FoldoutAttribute(string hexColor, string foldoutName, string[] fieldsName)
        {
            if (ColorUtility.TryParseHtmlString(hexColor, out Color color) == false) throw new ArgumentException(hexColor);

            backColor = color + new Color(0.5f, 0.5f, 0.5f);

            Title = foldoutName;
            FieldsNames = fieldsName;
        }

        /// <summary>
        /// Parameters <b>backR</b>, <b>backG</b>, <b>backB</b> should be limited from 0 to 1
        /// </summary>
        public FoldoutAttribute(float backR, float backG, float backB, string foldoutName, string[] fieldsName)
        {
            backColor = new Color(backR, backG, backB) + new Color(0.5f, 0.5f, 0.5f);

            Title = foldoutName;
            FieldsNames = fieldsName;
        }
    }
}
