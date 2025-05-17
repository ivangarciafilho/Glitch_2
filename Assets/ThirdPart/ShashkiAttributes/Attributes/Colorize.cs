using System;
using UnityEngine;

namespace Shashki.Attributes
{
    [RequireAttribute(typeof(SerializeField))]
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
    public sealed class Colorize : PropertyAttribute
    {
        public readonly Color color;
        public readonly string hexColor = string.Empty;
        public readonly ColorizeTarget target = ColorizeTarget.Name;

        public Colorize(string hexColor, ColorizeTarget target = ColorizeTarget.Name)
        {
            if (ColorUtility.TryParseHtmlString(hexColor, out color) == false) throw new ArgumentException(hexColor);

            if(target is ColorizeTarget.Back) color.a = 0.5f;

            this.target = target;
            this.hexColor = hexColor;
        }

        /// <summary>
        /// Parameters <b>r</b>, <b>g</b>, <b>b</b>, <b>a</b> should be limited from 0 to 1
        /// </summary>
        public Colorize(float r, float g, float b, float a = 0.5f, ColorizeTarget target = ColorizeTarget.Name) 
        {
            Color newColor = new Color(r, g, b, a);

            this.target = target;
            color = newColor;
        }
    }

    public enum ColorizeTarget : byte { Name, Back }
}
