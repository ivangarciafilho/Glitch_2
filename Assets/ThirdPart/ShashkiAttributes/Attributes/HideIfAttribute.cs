using System;
using UnityEngine;

namespace Shashki.Attributes
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public sealed class HideIfAttribute : PropertyAttribute
    {
        public readonly string fieldName;
        public readonly object desiredValue;

        public HideIfAttribute(string fieldName, object desiredValue)
        {
            this.fieldName = fieldName;
            this.desiredValue = desiredValue;
        }
    }
}
