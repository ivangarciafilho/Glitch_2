﻿using System.Diagnostics;
using SaintsField.Interfaces;
using UnityEngine;

namespace SaintsField
{
    [Conditional("UNITY_EDITOR")]
    public class RequiredAttribute: PropertyAttribute, ISaintsAttribute
    {
        public SaintsAttributeType AttributeType => SaintsAttributeType.Other;
        public string GroupBy => "";

        // ReSharper disable once InconsistentNaming
        public readonly string ErrorMessage;
        public readonly EMessageType MessageType;

        public RequiredAttribute(string errorMessage = null, EMessageType messageType = EMessageType.Error)
        {
            ErrorMessage = errorMessage;
            MessageType = messageType;
        }
    }
}
