using System;
using UnityEngine;

namespace Shashki.Attributes
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public sealed class RenameInInspectorAttribute : PropertyAttribute
    {
        public readonly string newName;

        public RenameInInspectorAttribute(string newName) 
        { 
            this.newName = newName; 
        }
    }
}
