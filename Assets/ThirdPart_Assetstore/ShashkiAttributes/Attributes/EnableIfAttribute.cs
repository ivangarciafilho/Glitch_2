using System;
using UnityEngine;

namespace Shashki.Attributes
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public sealed class EnableIfAttribute : PropertyAttribute
    {
        public readonly string fieldName;
        public readonly object desiredValue;

        public EnableIfAttribute(string fieldName, object desiredValue)
        {
            this.fieldName = fieldName;
            this.desiredValue = desiredValue;
        }
    }
}
