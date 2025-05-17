using System;

using UnityEngine;

namespace Shashki.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public sealed class ColorizeScriptTitleAttribute : Attribute
    {
        public readonly Color titleColor = Color.black;
        public readonly string hexTitleColor = string.Empty;

        public ColorizeScriptTitleAttribute(float r, float g, float b)
        {
            titleColor = new Color(r, g, b, 1);
        }

        public ColorizeScriptTitleAttribute(string hex)
        {
            hexTitleColor = hex;
        }
    }
}
