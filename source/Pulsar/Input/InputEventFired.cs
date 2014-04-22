namespace Pulsar.Input
{
    /// <summary>
    /// Delegate used when an input event is triggerd
    /// </summary>
    /// <param name="inputEvent">InputEvent that trigger the event</param>
    /// <param name="propagate">Use to indicate that after the end of the method event should stop calling anothers</param>
    public delegate void InputEventFired(InputEvent inputEvent, ref bool propagate);
}