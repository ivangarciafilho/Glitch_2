using System;
using UnityEngine;

namespace Shashki.Attributes
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public sealed class MethodAsButtonAttribute : PropertyAttribute
    {
        public readonly string buttonName = string.Empty;
        public readonly byte buttonHeight = 20;

        public MethodAsButtonAttribute(string name = "", byte buttonHeight = 20)
        {
            buttonName = name;
            this.buttonHeight = buttonHeight;
        }
    }
}
