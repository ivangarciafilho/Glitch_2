using System;
using UnityEngine;

namespace EMD
{
    [AttributeUsage(AttributeTargets.Method)]
    public class ButtonAttribute : PropertyAttribute 
    {
        public string ButtonName { get; }

        public ButtonAttribute(string buttonName)
        {
            ButtonName = buttonName;
        }
    }
}