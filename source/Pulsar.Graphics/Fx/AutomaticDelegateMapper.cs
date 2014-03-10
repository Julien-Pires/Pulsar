using System;

using Microsoft.Xna.Framework;

namespace Pulsar.Graphics.Fx
{
    using XnaTexture = Microsoft.Xna.Framework.Graphics.Texture;

    internal static class AutomaticDelegateMapper
    {
        #region Fields

        private static readonly DelegateMapper<int> DelegatesMap = new DelegateMapper<int>
        {
            {(int)ShaderVariableSemantic.Position, (Func<FrameContext, Vector3>)GetPosition},
            {(int)ShaderVariableSemantic.CameraPosition, (Func<FrameContext, Vector3>)GetCameraPosition},
            {(int)ShaderVariableSemantic.Diffuse, (Func<FrameContext, Color>)GetDiffuse},
            {(int)ShaderVariableSemantic.DiffuseMap, (Func<FrameContext, XnaTexture>)GetDiffuseMap},
            {(int)ShaderVariableSemantic.Opacity, (Func<FrameContext, float>)GetOpacity},
            {(int)ShaderVariableSemantic.Projection, (Func<FrameContext, Matrix>)GetProjection},
            {(int)ShaderVariableSemantic.ProjectionInverse, (Func<FrameContext, Matrix>)GetProjectionInverse},
            {(int)ShaderVariableSemantic.ProjectionInverseTranspose, (Func<FrameContext, Matrix>)GetProjectionInverseTranspose},
            {(int)ShaderVariableSemantic.ProjectionTranspose, (Func<FrameContext, Matrix>)GetProjectionTranspose},
            {(int)ShaderVariableSemantic.Specular, (Func<FrameContext, Color>)GetSpecular},
            {(int)ShaderVariableSemantic.SpecularMap, (Func<FrameContext, XnaTexture>)GetSpecularMap},
            {(int)ShaderVariableSemantic.SpecularPower, (Func<FrameContext, float>)GetSpecularPower},
            {(int)ShaderVariableSemantic.View, (Func<FrameContext, Matrix>)GetView},
            {(int)ShaderVariableSemantic.ViewInverse, (Func<FrameContext, Matrix>)GetViewInverse},
            {(int)ShaderVariableSemantic.ViewInverseTranspose, (Func<FrameContext, Matrix>)GetViewInverseTranspose},
            {(int)ShaderVariableSemantic.ViewProjection, (Func<FrameContext, Matrix>)GetViewProjection},
            {(int)ShaderVariableSemantic.ViewProjectionInverse, (Func<FrameContext, Matrix>)GetViewProjectionInverse},
            {(int)ShaderVariableSemantic.ViewProjectionInverseTranspose, (Func<FrameContext, Matrix>)GetViewProjectionInverseTranspose},
            {(int)ShaderVariableSemantic.ViewProjectionTranspose, (Func<FrameContext, Matrix>)GetViewProjectionTranspose},
            {(int)ShaderVariableSemantic.ViewTranspose, (Func<FrameContext, Matrix>)GetViewTranspose},
            {(int)ShaderVariableSemantic.World, (Func<FrameContext, Matrix>)GetWorld},
            {(int)ShaderVariableSemantic.WorldInverse, (Func<FrameContext, Matrix>)GetWorldInverse},
            {(int)ShaderVariableSemantic.WorldInverseTranspose, (Func<FrameContext, Matrix>)GetWorldInverseTranspose},
            {(int)ShaderVariableSemantic.WorldTranspose, (Func<FrameContext, Matrix>)GetWorldTranspose},
            {(int)ShaderVariableSemantic.WorldView, (Func<FrameContext, Matrix>)GetWorldView},
            {(int)ShaderVariableSemantic.WorldViewInverse, (Func<FrameContext, Matrix>)GetWorldViewInverse},
            {(int)ShaderVariableSemantic.WorldViewInverseTranspose, (Func<FrameContext, Matrix>)GetWorldViewInverseTranspose},
            {(int)ShaderVariableSemantic.WorldViewTranspose, (Func<FrameContext, Matrix>)GetWorldViewTranspose},
            {(int)ShaderVariableSemantic.WorldViewProjection, (Func<FrameContext, Matrix>)GetWorldViewProjection},
            {(int)ShaderVariableSemantic.WorldViewProjectionInverse, (Func<FrameContext, Matrix>)GetWorldViewProjectionInverse},
            {(int)ShaderVariableSemantic.WorldViewProjectionInverseTranspose, (Func<FrameContext, Matrix>)GetWorldViewProjectionInverseTranspose},
            {(int)ShaderVariableSemantic.WorldViewProjectionTranspose, (Func<FrameContext, Matrix>)GetWorldViewProjectionTranspose}
        };

        #endregion

        #region Static methods

        public static Func<FrameContext, T> GetMethod<T>(int key)
        {
            return DelegatesMap.GetTypedDelegate<Func<FrameContext, T>>(key);
        }

        private static Vector3 GetPosition(FrameContext context)
        {
            return default(Vector3);
        }

        private static Vector3 GetCameraPosition(FrameContext context)
        {
            return context.Camera.Position;
        }

        private static Color GetDiffuse(FrameContext context)
        {
            return context.Renderable.Material.Diffuse;
        }

        private static XnaTexture GetDiffuseMap(FrameContext context)
        {
            return (XnaTexture)context.Renderable.Material.DiffuseMap;
        }

        private static float GetOpacity(FrameContext context)
        {
            return context.Renderable.Material.Opacity;
        }

        private static Matrix GetProjection(FrameContext context)
        {
            return context.Camera.Projection;
        }

        private static Matrix GetProjectionInverse(FrameContext context)
        {
            Matrix projection = context.Camera.Projection;
            Matrix inverse;
            Matrix.Invert(ref projection, out inverse);

            return inverse;
        }

        private static Matrix GetProjectionInverseTranspose(FrameContext context)
        {
            Matrix projection = context.Camera.Projection;
            Matrix result;
            Matrix.Invert(ref projection, out result);
            Matrix.Transpose(ref result, out result);

            return result;
        }

        private static Matrix GetProjectionTranspose(FrameContext context)
        {
            Matrix projection = context.Camera.Projection;
            Matrix transpose;
            Matrix.Transpose(ref projection, out transpose);

            return transpose;
        }

        private static Color GetSpecular(FrameContext context)
        {
            return context.Renderable.Material.Specular;
        }

        private static XnaTexture GetSpecularMap(FrameContext context)
        {
            return (XnaTexture) context.Renderable.Material.SpecularMap;
        }

        private static float GetSpecularPower(FrameContext context)
        {
            return context.Renderable.Material.SpecularPower;
        }

        private static Matrix GetView(FrameContext context)
        {
            return context.Camera.View;
        }

        private static Matrix GetViewInverse(FrameContext context)
        {
            Matrix view = context.Camera.View;
            Matrix inverse;
            Matrix.Invert(ref view, out inverse);

            return inverse;
        }

        private static Matrix GetViewInverseTranspose(FrameContext context)
        {
            Matrix view = context.Camera.View;
            Matrix result;
            Matrix.Invert(ref view, out result);
            Matrix.Transpose(ref result, out result);

            return result;
        }

        private static Matrix GetViewProjection(FrameContext context)
        {
            Matrix view = context.Camera.View;
            Matrix projection = context.Camera.Projection;
            Matrix viewProj;
            Matrix.Multiply(ref view, ref projection, out viewProj);

            return viewProj;
        }

        private static Matrix GetViewProjectionInverse(FrameContext context)
        {
            Matrix view = context.Camera.View;
            Matrix projection = context.Camera.Projection;
            Matrix result;
            Matrix.Multiply(ref view, ref projection, out result);
            Matrix.Invert(ref result, out result);

            return result;
        }

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

        private static Matrix GetViewProjectionTranspose(FrameContext context)
        {
            Matrix view = context.Camera.View;
            Matrix projection = context.Camera.Projection;
            Matrix result;
            Matrix.Multiply(ref view, ref projection, out result);
            Matrix.Transpose(ref result, out result);

            return result;
        }

        private static Matrix GetViewTranspose(FrameContext context)
        {
            Matrix view = context.Camera.View;
            Matrix transpose;
            Matrix.Transpose(ref view, out transpose);

            return transpose;
        }

        private static Matrix GetWorld(FrameContext context)
        {
            return context.Renderable.Transform;
        }

        private static Matrix GetWorldInverse(FrameContext context)
        {
            Matrix world = context.Renderable.Transform;
            Matrix invert;
            Matrix.Invert(ref world, out invert);

            return invert;
        }

        private static Matrix GetWorldInverseTranspose(FrameContext context)
        {
            Matrix world = context.Renderable.Transform;
            Matrix result;
            Matrix.Invert(ref world, out result);
            Matrix.Transpose(ref result, out result);

            return result;
        }

        private static Matrix GetWorldTranspose(FrameContext context)
        {
            Matrix world = context.Renderable.Transform;
            Matrix transpose;
            Matrix.Transpose(ref world, out transpose);

            return transpose;
        }

        private static Matrix GetWorldView(FrameContext context)
        {
            Matrix world = context.Renderable.Transform;
            Matrix view = context.Camera.View;
            Matrix worldView;
            Matrix.Multiply(ref world, ref view, out worldView);

            return worldView;
        }

        private static Matrix GetWorldViewInverse(FrameContext context)
        {
            Matrix world = context.Renderable.Transform;
            Matrix view = context.Camera.View;
            Matrix result;
            Matrix.Multiply(ref world, ref view, out result);
            Matrix.Invert(ref result, out result);

            return result;
        }

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

        private static Matrix GetWorldViewTranspose(FrameContext context)
        {
            Matrix world = context.Renderable.Transform;
            Matrix view = context.Camera.View;
            Matrix result;
            Matrix.Multiply(ref world, ref view, out result);
            Matrix.Transpose(ref result, out result);

            return result;
        }

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
