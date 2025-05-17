using System;
using UnityEngine;

namespace Shashki.Attributes
{
    [RequireAttribute(typeof(SerializeField))]
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public sealed class ViewOnlyAttribute : PropertyAttribute
    { 
    }
}
