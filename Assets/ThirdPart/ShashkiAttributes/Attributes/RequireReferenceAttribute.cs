using System;
using UnityEngine;

namespace Shashki.Attributes
{
    [RequireAttribute(typeof(SerializeField))]
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
    public sealed class RequireReferenceAttribute : PropertyAttribute
    {
        public readonly string errorMessage;
        public readonly bool throwErrorInConsole;

        public RequireReferenceAttribute(string errorMessage = "This reference is required!", bool throwErrorInConsole = false)
        {
            this.errorMessage = errorMessage;
            this.throwErrorInConsole = throwErrorInConsole;
        }
    }
}
