using System;

using UnityEngine;

namespace Shashki.Attributes
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
    [RequireAttribute(typeof(SerializeField))]
    public sealed class TagSelectorAttribute : PropertyAttribute
    {
    }
}
