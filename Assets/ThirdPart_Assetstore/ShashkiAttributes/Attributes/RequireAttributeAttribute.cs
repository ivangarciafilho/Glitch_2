using System;
using System.Linq;

namespace Shashki.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public sealed class RequireAttributeAttribute : Attribute
    {
        public Type[] RequiredAttributes { get; }

        public RequireAttributeAttribute(Type requiredAttribute, params Type[] requiredAttributes)
        {
            RequiredAttributes = requiredAttributes.Append(requiredAttribute).ToArray();
        }
    }
}
