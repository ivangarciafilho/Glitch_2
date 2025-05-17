using UnityEngine;
using System;

namespace Shashki.Attributes
{
    [RequireAttribute(typeof(SerializeField))]
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true, Inherited = true)]
    public sealed class SpliteAttribute : PropertyAttribute
    {
        public readonly float splitterSize = 2;
        public readonly float splitterSpacing = 10;
        public readonly Color splitterColor = defaultColor;

        public static Color defaultColor => new Color(0.38f, 0.38f, 0.38f);

        public SpliteAttribute(string hexColor, float splitterSize = 2, float splitterSpacing = 10)
        {
            if (ColorUtility.TryParseHtmlString(hexColor, out Color color) == false) throw new ArgumentException(nameof(hexColor));

            this.splitterColor = color;
            this.splitterSize = splitterSize;
            this.splitterSpacing = splitterSpacing;
        }

        public SpliteAttribute(float splitterSize = 2, float splitterSpacing = 10) 
        { 
            this.splitterSize = splitterSize;
            this.splitterSpacing = splitterSpacing;
        }

        /// <summary>
        /// Parameters <b>r</b>, <b>g</b>, <b>b</b> should be limited from 0 to 1
        /// </summary>
        public SpliteAttribute(float r, float g, float b, float splitterSize = 2, float splitterSpacing = 10)
        {
            var newColor = new Color(r, g, b);
            splitterColor = newColor;

            this.splitterSize = splitterSize;
            this.splitterSpacing = splitterSpacing;
        }
    }
}
