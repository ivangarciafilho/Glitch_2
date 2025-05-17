using System;
using UnityEngine;

namespace Shashki.Attributes
{
    [RequireAttribute(typeof(SerializeField))]
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class ExtendedHeaderAttribute : PropertyAttribute
    {
        public readonly string header;
        public readonly byte headerSize;
        public readonly HeaderBinding headerBinding;

        public ExtendedHeaderAttribute(string header) 
        {
            this.header = header;
        }

        public ExtendedHeaderAttribute(string header, byte headerSize) : this(header)
        {
            this.headerSize = headerSize;
        }

        public ExtendedHeaderAttribute(string header, HeaderBinding headerBinding) : this(header)
        {
            this.headerBinding = headerBinding;
        }

        public ExtendedHeaderAttribute(string header, byte headerSize, HeaderBinding headerBinding) : this(header, headerSize)
        {
            this.headerBinding = headerBinding;
        }
    }

    public enum HeaderBinding
    {
        Left, Middle, Right
    }
}
