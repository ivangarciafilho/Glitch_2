using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EMD
{
    public class ShowIfAttribute : PropertyAttribute 
    {
        public string FieldName;

        public ShowIfAttribute(string fieldName)
        {
            FieldName = fieldName;
        }
    }
}