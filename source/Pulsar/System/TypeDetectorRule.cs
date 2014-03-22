using System;

namespace Pulsar.System
{
    [Flags]
    public enum TypeDetectorRule
    {
        Public = 0,
        Private = 1,
        Interface = 2,
        Class = 4,
        ValueType = 8,
        Abstract = 16,
        Sealed = 32,
        Nested = 64,
        NoParameterLessCtor = 128
    }
}
