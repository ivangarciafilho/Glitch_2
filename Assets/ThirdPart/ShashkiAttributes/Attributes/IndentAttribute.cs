using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Shashki.Attributes
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public sealed class IndentAttribute : PropertyAttribute
    {
        public readonly byte indentLevel;
        public IndentAttribute(byte indentLevel = 1) 
        {
            this.indentLevel = indentLevel;
        }
    }

}
