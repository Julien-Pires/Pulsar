using System;

using Microsoft.Xna.Framework;

namespace Pulsar.Graphics.Fx
{
    using XnaTexture = Microsoft.Xna.Framework.Graphics.Texture;

    /// <summary>
    /// Manages a list of delegates used for automatic binding
    /// </summary>
    internal static class AutomaticDelegateMapper
    {
        #region Fields

        private static readonly DelegateMapper<int> DelegatesMap = new DelegateMapper<int>
        {
            {(int)ShaderConstantSemantic.Position, (Func<FrameContext, Vector3>)GetPosition},
            {(int)ShaderConstantSemantic.CameraPosition, (Func<FrameContext, Vector3>)GetCameraPosition},
            {(int)ShaderConstantSemantic.Diffuse, (Func<FrameContext, Color>)GetDiffuse},
            {(int)ShaderConstantSemantic.DiffuseMap, (Func<FrameContext, XnaTexture>)GetDiffuseMap},
            {(int)ShaderConstantSemantic.Opacity, (Func<FrameContext, float>)GetOpacity},
            {(int)ShaderConstantSemantic.Projection, (Func<FrameContext, Matrix>)GetProjection},
            {(int)ShaderConstantSemantic.ProjectionInverse, (Func<FrameContext, Matrix>)GetProjectionInverse},
            {(int)ShaderConstantSemantic.ProjectionInverseTranspose, (Func<FrameContext, Matrix>)GetProjectionInverseTranspose},
            {(int)ShaderConstantSemantic.ProjectionTranspose, (Func<FrameContext, Matrix>)GetProjectionTranspose},
            {(int)ShaderConstantSemantic.Specular, (Func<FrameContext, Color>)GetSpecular},
            {(int)ShaderConstantSemantic.SpecularMap, (Func<FrameContext, XnaTexture>)GetSpecularMap},
            {(int)ShaderConstantSemantic.SpecularPower, (Func<FrameContext, float>)GetSpecularPower},
            {(int)ShaderConstantSemantic.View, (Func<FrameContext, Matrix>)GetView},
            {(int)ShaderConstantSemantic.ViewInverse, (Func<FrameContext, Matrix>)GetViewInverse},
            {(int)ShaderConstantSemantic.ViewInverseTranspose, (Func<FrameContext, Matrix>)GetViewInverseTranspose},
            {(int)ShaderConstantSemantic.ViewProjection, (Func<FrameContext, Matrix>)GetViewProjection},
            {(int)ShaderConstantSemantic.ViewProjectionInverse, (Func<FrameContext, Matrix>)GetViewProjectionInverse},
            {(int)ShaderConstantSemantic.ViewProjectionInverseTranspose, (Func<FrameContext, Matrix>)GetViewProjectionInverseTranspose},
            {(int)ShaderConstantSemantic.ViewProjectionTranspose, (Func<FrameContext, Matrix>)GetViewProjectionTranspose},
            {(int)ShaderConstantSemantic.ViewTranspose, (Func<FrameContext, Matrix>)GetViewTranspose},
            {(int)ShaderConstantSemantic.World, (Func<FrameContext, Matrix>)GetWorld},
            {(int)ShaderConstantSemantic.WorldInverse, (Func<FrameContext, Matrix>)GetWorldInverse},
            {(int)ShaderConstantSemantic.WorldInverseTranspose, (Func<FrameContext, Matrix>)GetWorldInverseTranspose},
            {(int)ShaderConstantSemantic.WorldTranspose, (Func<FrameContext, Matrix>)GetWorldTranspose},
            {(int)ShaderConstantSemantic.WorldView, (Func<FrameContext, Matrix>)GetWorldView},
            {(int)ShaderConstantSemantic.WorldViewInverse, (Func<FrameContext, Matrix>)GetWorldViewInverse},
            {(int)ShaderConstantSemantic.WorldViewInverseTranspose, (Func<FrameContext, Matrix>)GetWorldViewInverseTranspose},
            {(int)ShaderConstantSemantic.WorldViewTranspose, (Func<FrameContext, Matrix>)GetWorldViewTranspose},
            {(int)ShaderConstantSemantic.WorldViewProjection, (Func<FrameContext, Matrix>)GetWorldViewProjection},
            {(int)ShaderConstantSemantic.WorldViewProjectionInverse, (Func<FrameContext, Matrix>)GetWorldViewProjectionInverse},
            {(int)ShaderConstantSemantic.WorldViewProjectionInverseTranspose, (Func<FrameContext, Matrix>)GetWorldViewProjectionInverseTranspose},
            {(int)ShaderConstantSemantic.WorldViewProjectionTranspose, (Func<FrameContext, Matrix>)GetWorldViewProjectionTranspose}
        };

        #endregion

        #region Static methods

        /// <summary>
        /// Gets the delegate for a specified key
        /// </summary>
        /// <typeparam name="T">Type of the return parameter</typeparam>
        /// <param name="key">Shader constant semantic</param>
        /// <returns>Returns a strongly typed function</returns>
        public static Func<FrameContext, T> GetMethod<T>(ShaderConstantSemantic key)
        {
            return DelegatesMap.GetTypedDelegate<Func<FrameContext, T>>((int)key);
        }

        /// <summary>
        /// Gets the position of the current object
        /// </summary>
        /// <param name="context">Frame context</param>
        /// <returns>Returns the position of the object</returns>
        private static Vector3 GetPosition(FrameContext context)
        {
            return default(Vector3);
        }

        /// <summary>
        /// Gets the position of the camera
        /// </summary>
        /// <param name="context">Frame context</param>
        /// <returns>Returns the position of the camera</returns>
        private static Vector3 GetCameraPosition(FrameContext context)
        {
            return context.Camera.Position;
        }

        /// <summary>
        /// Gets the diffuse color of the current material
        /// </summary>
        /// <param name="context">Frame context</param>
        /// <returns>Returns the diffuse color</returns>
        private static Color GetDiffuse(FrameContext context)
        {
            return context.Renderable.Material.Diffuse;
        }

        /// <summary>
        /// Gets the diffuse map of the current material
        /// </summary>
        /// <param name="context">Frame context</param>
        /// <returns>Returns the diffuse map</returns>
        private static XnaTexture GetDiffuseMap(FrameContext context)
        {
            return (XnaTexture)context.Renderable.Material.DiffuseMap;
        }

        /// <summary>
        /// Gets the opacity of the current material
        /// </summary>
        /// <param name="context">Frame context</param>
        /// <returns>Returns the opacity</returns>
        private static float GetOpacity(FrameContext context)
        {
            return context.Renderable.Material.Opacity;
        }

        /// <summary>
        /// Gets the projection matrix
        /// </summary>
        /// <param name="context">Frame context</param>
        /// <returns>Returns the projection matrix</returns>
        private static Matrix GetProjection(FrameContext context)
        {
            return context.Camera.Projection;
        }

        /// <summary>
        /// Gets the inverse projection matrix
        /// </summary>
        /// <param name="context">Frame context</param>
        /// <returns>Returns the inverse projection matrix</returns>
        private static Matrix GetProjectionInverse(FrameContext context)
        {
            Matrix projection = context.Camera.Projection;
            Matrix inverse;
            Matrix.Invert(ref projection, out inverse);

            return inverse;
        }

        /// <summary>
        /// Gets the transpose inverse of the projection matrix
        /// </summary>
        /// <param name="context">Frame context</param>
        /// <returns>Returns the inverse transpose of the projection matrix</returns>
        private static Matrix GetProjectionInverseTranspose(FrameContext context)
        {
            Matrix projection = context.Camera.Projection;
            Matrix result;
            Matrix.Invert(ref projection, out result);
            Matrix.Transpose(ref result, out result);

            return result;
        }

        /// <summary>
        /// Gets the transpose of the projection matrix
        /// </summary>
        /// <param name="context">Frame context</param>
        /// <returns>Returns the transpose of the projection matrix</returns>
        private static Matrix GetProjectionTranspose(FrameContext context)
        {
            Matrix projection = context.Camera.Projection;
            Matrix transpose;
            Matrix.Transpose(ref projection, out transpose);

            return transpose;
        }

        /// <summary>
        /// Gets the specular color of the current material
        /// </summary>
        /// <param name="context">Frame context</param>
        /// <returns>Returns the specular color</returns>
        private static Color GetSpecular(FrameContext context)
        {
            return context.Renderable.Material.Specular;
        }

        /// <summary>
        /// Gets the specular map of the current material
        /// </summary>
        /// <param name="context">Frame context</param>
        /// <returns>Returns the specular map</returns>
        private static XnaTexture GetSpecularMap(FrameContext context)
        {
            return (XnaTexture) context.Renderable.Material.SpecularMap;
        }

        /// <summary>
        /// Gets the specular power of the current material
        /// </summary>
        /// <param name="context">Frame context</param>
        /// <returns>Returns the specular power</returns>
        private static float GetSpecularPower(FrameContext context)
        {
            return context.Renderable.Material.SpecularPower;
        }

        /// <summary>
        /// Gets the view matrix
        /// </summary>
        /// <param name="context">Frame context</param>
        /// <returns>Returns the view matrix</returns>
        private static Matrix GetView(FrameContext context)
        {
            return context.Camera.View;
        }

        /// <summary>
        /// Gets the inverse of the view matrix
        /// </summary>
        /// <param name="context">Frame context</param>
        /// <returns>Returns the inverse of the view matrix</returns>
        private static Matrix GetViewInverse(FrameContext context)
        {
            Matrix view = context.Camera.View;
            Matrix inverse;
            Matrix.Invert(ref view, out inverse);

            return inverse;
        }

        /// <summary>
        /// Gets the transpose inverse of the view matrix
        /// </summary>
        /// <param name="context">Frame context</param>
        /// <returns>Returns the inverse transpose of the view matrix</returns>
        private static Matrix GetViewInverseTranspose(FrameContext context)
        {
            Matrix view = context.Camera.View;
            Matrix result;
            Matrix.Invert(ref view, out result);
            Matrix.Transpose(ref result, out result);

            return result;
        }

        /// <summary>
        /// Gets the view-projection matrix
        /// </summary>
        /// <param name="context">Frame context</param>
        /// <returns>Returns the view-projection matrix</returns>
        private static Matrix GetViewProjection(FrameContext context)
        {
            Matrix view = context.Camera.View;
            Matrix projection = context.Camera.Projection;
            Matrix viewProj;
            Matrix.Multiply(ref view, ref projection, out viewProj);

            return viewProj;
        }

        /// <summary>
        /// Gets the inverse of the view-projection matrix
        /// </summary>
        /// <param name="context">Frame context</param>
        /// <returns>Returns the inverse of the view-projection matrix</returns>
        private static Matrix GetViewProjectionInverse(FrameContext context)
        {
            Matrix view = context.Camera.View;
            Matrix projection = context.Camera.Projection;
            Matrix result;
            Matrix.Multiply(ref view, ref projection, out result);
            Matrix.Invert(ref result, out result);

            return result;
        }

        /// <summary>
        /// Gets the transpose inverse of the view-projection matrix
        /// </summary>
        /// <param name="context">Frame context</param>
        /// <returns>Returns the inverse transpose of the view-projection matrix</returns>
        private static Matrix GetViewProjectionInverseTranspose(FrameContext context)
        {
            Matrix view = context.Camera.View;
            Matrix projection = context.Camera.Projection;
            Matrix result;
            Matrix.Multiply(ref view, ref projection, out result);
            Matrix.Invert(ref result, out result);
            Matrix.Transpose(ref result, out result);

            return result;
        }

        /// <summary>
        /// Gets the transpose of the view-projection
        /// </summary>
        /// <param name="context">Frame context</param>
        /// <returns>Returns the transpose of the view-projection</returns>
        private static Matrix GetViewProjectionTranspose(FrameContext context)
        {
            Matrix view = context.Camera.View;
            Matrix projection = context.Camera.Projection;
            Matrix result;
            Matrix.Multiply(ref view, ref projection, out result);
            Matrix.Transpose(ref result, out result);

            return result;
        }

        /// <summary>
        /// Gets the transpose of the view matrix
        /// </summary>
        /// <param name="context">Frame context</param>
        /// <returns>Returns the transpose of the view matrix</returns>
        private static Matrix GetViewTranspose(FrameContext context)
        {
            Matrix view = context.Camera.View;
            Matrix transpose;
            Matrix.Transpose(ref view, out transpose);

            return transpose;
        }

        /// <summary>
        /// Gets the world matrix of the current object
        /// </summary>
        /// <param name="context">Frame context</param>
        /// <returns>Returns the world matrix</returns>
        private static Matrix GetWorld(FrameContext context)
        {
            return context.Renderable.Transform;
        }

        /// <summary>
        /// Gets the inverse of the world matrix
        /// </summary>
        /// <param name="context">Frame context</param>
        /// <returns>Returns the inverse of the world matrix</returns>
        private static Matrix GetWorldInverse(FrameContext context)
        {
            Matrix world = context.Renderable.Transform;
            Matrix invert;
            Matrix.Invert(ref world, out invert);

            return invert;
        }

        /// <summary>
        /// Gets the transpose inverse of the world matrix
        /// </summary>
        /// <param name="context">Frame context</param>
        /// <returns>Returns the transpose inverse of the world matrix</returns>
        private static Matrix GetWorldInverseTranspose(FrameContext context)
        {
            Matrix world = context.Renderable.Transform;
            Matrix result;
            Matrix.Invert(ref world, out result);
            Matrix.Transpose(ref result, out result);

            return result;
        }

        /// <summary>
        /// Gets the transpose of the world matrix
        /// </summary>
        /// <param name="context">Frame context</param>
        /// <returns>Returns the transpose of the world matrix</returns>
        private static Matrix GetWorldTranspose(FrameContext context)
        {
            Matrix world = context.Renderable.Transform;
            Matrix transpose;
            Matrix.Transpose(ref world, out transpose);

            return transpose;
        }

        /// <summary>
        /// Gets the world-view matrix
        /// </summary>
        /// <param name="context">Frame context</param>
        /// <returns>Returns the world-view matrix</returns>
        private static Matrix GetWorldView(FrameContext context)
        {
            Matrix world = context.Renderable.Transform;
            Matrix view = context.Camera.View;
            Matrix worldView;
            Matrix.Multiply(ref world, ref view, out worldView);

            return worldView;
        }

        /// <summary>
        /// Gets the inverse of the world-view matrix
        /// </summary>
        /// <param name="context">Frame context</param>
        /// <returns>Returns the inverse of the world-view matrix</returns>
        private static Matrix GetWorldViewInverse(FrameContext context)
        {
            Matrix world = context.Renderable.Transform;
            Matrix view = context.Camera.View;
            Matrix result;
            Matrix.Multiply(ref world, ref view, out result);
            Matrix.Invert(ref result, out result);

            return result;
        }

        /// <summary>
        /// Gets the transpose inverse of the world-view matrix
        /// </summary>
        /// <param name="context">Frame context</param>
        /// <returns>Returns the transpose inverse of the world-view matrix</returns>
        private static Matrix GetWorldViewInverseTranspose(FrameContext context)
        {
            Matrix world = context.Renderable.Transform;
            Matrix view = context.Camera.View;
            Matrix result;
            Matrix.Multiply(ref world, ref view, out result);
            Matrix.Invert(ref result, out result);
            Matrix.Transpose(ref result, out result);

            return result;
        }

        /// <summary>
        /// Gets the transpose of the world-view matrix
        /// </summary>
        /// <param name="context">Frame context</param>
        /// <returns>Returns the transpose of the world-view matrix</returns>
        private static Matrix GetWorldViewTranspose(FrameContext context)
        {
            Matrix world = context.Renderable.Transform;
            Matrix view = context.Camera.View;
            Matrix result;
            Matrix.Multiply(ref world, ref view, out result);
            Matrix.Transpose(ref result, out result);

            return result;
        }

        /// <summary>
        /// Gets the world-view-projection matrix
        /// </summary>
        /// <param name="context">Frame context</param>
        /// <returns>Returns the world-view-projection matrix</returns>
        private static Matrix GetWorldViewProjection(FrameContext context)
        {
            Matrix world = context.Renderable.Transform;
            Matrix view = context.Camera.View;
            Matrix projection = context.Camera.Projection;
            Matrix worldViewProj;
            Matrix.Multiply(ref world, ref view, out worldViewProj);
            Matrix.Multiply(ref worldViewProj, ref projection, out worldViewProj);

            return worldViewProj;
        }

        /// <summary>
        /// Gets the inverse of the world-view-projection matrix
        /// </summary>
        /// <param name="context">Frame context</param>
        /// <returns>Returns the inverse of the world-view-projection</returns>
        private static Matrix GetWorldViewProjectionInverse(FrameContext context)
        {
            Matrix world = context.Renderable.Transform;
            Matrix view = context.Camera.View;
            Matrix projection = context.Camera.Projection;
            Matrix result;
            Matrix.Multiply(ref world, ref view, out result);
            Matrix.Multiply(ref result, ref projection, out result);
            Matrix.Invert(ref result, out result);

            return result;
        }

        /// <summary>
        /// Gets the transpose inverse of the world-view-projection matrix
        /// </summary>
        /// <param name="context">Frame context</param>
        /// <returns>Returns the transpose inverse of the world-view-projection matrix</returns>
        private static Matrix GetWorldViewProjectionInverseTranspose(FrameContext context)
        {
            Matrix world = context.Renderable.Transform;
            Matrix view = context.Camera.View;
            Matrix projection = context.Camera.Projection;
            Matrix result;
            Matrix.Multiply(ref world, ref view, out result);
            Matrix.Multiply(ref result, ref projection, out result);
            Matrix.Invert(ref result, out result);
            Matrix.Transpose(ref result, out result);

            return result;
        }

        /// <summary>
        /// Gets the transpose of the world-view-projection matrix
        /// </summary>
        /// <param name="context">Frame context</param>
        /// <returns>Returns the transpose of the world-view-projection matrix</returns>
        private static Matrix GetWorldViewProjectionTranspose(FrameContext context)
        {
            Matrix world = context.Renderable.Transform;
            Matrix view = context.Camera.View;
            Matrix projection = context.Camera.Projection;
            Matrix result;
            Matrix.Multiply(ref world, ref view, out result);
            Matrix.Multiply(ref result, ref projection, out result);
            Matrix.Transpose(ref result, out result);

            return result;
        }

        #endregion
    }
}
