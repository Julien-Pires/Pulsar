namespace Pulsar.Graphics.Fx
{
    /// <summary>
    /// Enumerates sources that can be used to update a variable
    /// </summary>
    public enum ShaderVariableSource
    {
        Custom = 0,
        Auto = 1,
        Delegate = 2,
        Keyed = 4,
        Constant = 3
    }
}
