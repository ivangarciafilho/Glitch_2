using System;

using UnityEngine;

namespace Shashki.Attributes
{
    [RequireAttribute(typeof(SerializeField))]
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
    public sealed class MessageAttribute : PropertyAttribute
    {
        public readonly string message;
        public readonly MessageType messageType = MessageType.Info;

        public MessageAttribute(string message, MessageType messageType = MessageType.Info)
        {
            this.message = message;
            this.messageType = messageType;
        }
    }

    public enum MessageType { Info, Warning, Error }
}
