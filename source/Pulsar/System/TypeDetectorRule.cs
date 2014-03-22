using System;

namespace Pulsar.System
{
    [Flags]
    public enum TypeDetectorRule
    {
        Public = 1,
        Private = 2,
        Interface = 4,
        Class = 8,
        ValueType = 16,
        Abstract = 32,
        Sealed = 64,
        Nested = 128,
        NoParameterLessCtor = 256
    }
}
