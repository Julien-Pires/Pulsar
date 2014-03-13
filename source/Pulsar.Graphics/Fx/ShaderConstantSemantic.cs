namespace Pulsar.Graphics.Fx
{
    /// <summary>
    /// Enumerates semantics that can be used to automatically update a variable
    /// </summary>
    public enum ShaderConstantSemantic
    {
        Position,
        CameraPosition,
        Diffuse,
        DiffuseMap,
        Opacity,
        Specular,
        SpecularMap,
        SpecularPower,
        Projection,
        ProjectionInverse,
        ProjectionTranspose,
        ProjectionInverseTranspose,
        View,
        ViewInverse,
        ViewTranspose,
        ViewInverseTranspose,
        ViewProjection,
        ViewProjectionInverse,
        ViewProjectionTranspose,
        ViewProjectionInverseTranspose,
        World,
        WorldInverse,
        WorldTranspose,
        WorldInverseTranspose,
        WorldView,
        WorldViewInverse,
        WorldViewTranspose,
        WorldViewInverseTranspose,
        WorldViewProjection,
        WorldViewProjectionInverse,
        WorldViewProjectionTranspose,
        WorldViewProjectionInverseTranspose
    }
}
