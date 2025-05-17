using System;

using UnityEngine;

namespace Shashki.Attributes
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
    [RequireAttribute(typeof(SerializeField))]
    public sealed class ExtendedRangeAttribute : PropertyAttribute
    {
        public readonly float Min;
        public readonly float Max;
        public readonly bool CanEditLimitsInInspector;

        public ExtendedRangeAttribute(float min, float max, bool canEditLimitsInInspector = true)
        {
            Min = min;
            Max = max;
            CanEditLimitsInInspector = canEditLimitsInInspector;
        }
    }
}
