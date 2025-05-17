using System;
using UnityEngine;

namespace Shashki.Attributes
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public sealed class DisableIfAttribute : PropertyAttribute
    {
        public readonly string fieldName;
        public readonly object desiredValue;

        public DisableIfAttribute(string fieldName, object desiredValue)
        {
            this.fieldName = fieldName;
            this.desiredValue = desiredValue;
        }
    }
}
