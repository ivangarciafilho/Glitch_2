using System;
using UnityEngine;

namespace Shashki.Attributes
{
    [RequireAttribute(typeof(SerializeField))]
    [AttributeUsage(AttributeTargets.Field)]
    [Obsolete("If you need header, use ExtendedHeaderAttribute")]
    public sealed class LabelAttribute : PropertyAttribute
    {
        public readonly Color color = new(1, 1, 1);
        public readonly string label;
        public readonly byte labelFontSize;
        public readonly LabelSize labelSize = LabelSize.None;
        public readonly TextBinding labelAnchor = TextBinding.Middle;

        public LabelAttribute(string hexColor, string label, LabelSize size, TextBinding labelAnchor = TextBinding.Middle)
        {
            if (ColorUtility.TryParseHtmlString(hexColor, out color) == false) throw new ArgumentException(nameof(hexColor));

            this.label = label;
            this.labelSize = size;
            this.labelAnchor = labelAnchor;
        }

        public LabelAttribute(string hexColor, string label, byte fontSize, TextBinding labelAnchor = TextBinding.Middle)
        {
            if (ColorUtility.TryParseHtmlString(hexColor, out color) == false) throw new ArgumentException(nameof(hexColor));

            this.label = label;
            this.labelFontSize = fontSize;
            this.labelAnchor = labelAnchor;
        }

        /// <summary>
        /// Parameters <b>r</b>, <b>g</b>, <b>b</b> should be limited from 0 to 1
        /// </summary>
        public LabelAttribute(float r, float g, float b, string label, LabelSize size, TextBinding labelAnchor = TextBinding.Middle)
        {
            color = new(r, g, b);

            this.label = label;
            this.labelSize = size;
            this.labelAnchor = labelAnchor;
        }

        /// <summary>
        /// Parameters <b>r</b>, <b>g</b>, <b>b</b> should be limited from 0 to 1
        /// </summary>
        public LabelAttribute(float r, float g, float b, string label, byte fontSize, TextBinding labelAnchor = TextBinding.Middle)
        {
            color = new(r, g, b);

            this.label = label;
            this.labelFontSize = fontSize;
            this.labelAnchor = labelAnchor;
        }

        public LabelAttribute(string label, LabelSize size, TextBinding labelAnchor = TextBinding.Middle)
        {
            this.label = label;
            this.labelSize = size;
            this.labelAnchor = labelAnchor;
        }

        public LabelAttribute(string label, byte fontSize, TextBinding labelAnchor = TextBinding.Middle)
        {
            this.label = label;
            this.labelFontSize = fontSize;
            this.labelAnchor = labelAnchor;
        }
    }

    public enum LabelSize : byte
    {
        None = 0,
        Huge = 30,
        Big = 25,
        Default = 15,
        Small = 12,
        Tiny = 9
    }


    public enum TextBinding : byte
    {
        Middle, Left, Right
    }
}
