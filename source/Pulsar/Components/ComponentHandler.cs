namespace Pulsar.Components
{
    /// <summary>
    /// Represents the method that will handle component related event
    /// </summary>
    /// <param name="manager">Component manager that the component</param>
    /// <param name="component">Component</param>
    public delegate void ComponentHandler(ComponentManager manager, Component component);
}
