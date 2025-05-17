using System;
using UnityEngine;

namespace Shashki.Attributes
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public sealed class ShowIfAttribute : PropertyAttribute
    {
        public readonly string fieldName;
        public readonly object desiredValue;

        public ShowIfAttribute(string fieldName, object desiredValue)
        {
            this.fieldName = fieldName;
            this.desiredValue = desiredValue;
        }
    }
}
