using System;

namespace Pulsar.Pipeline.Serialization
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public sealed class ContentReaderAttribute : Attribute
    {
    }
}
