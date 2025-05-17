using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EMD
{
    public class HideIfAttribute : PropertyAttribute
    {
        public string FieldName;

        public HideIfAttribute(string fieldName)
        {
            FieldName = fieldName;
        }
    }
}