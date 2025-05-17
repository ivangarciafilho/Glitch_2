using System;
using UnityEngine;

namespace Shashki.Attributes
{
    [RequireAttribute(typeof(SerializeField))]
    [AttributeUsage(AttributeTargets.Field)]
    public sealed class MaxAttribute : PropertyAttribute
    {
        public readonly float MaxValue;

        public MaxAttribute(float maxValue)
        {
            MaxValue = maxValue;
        }
    }
}
