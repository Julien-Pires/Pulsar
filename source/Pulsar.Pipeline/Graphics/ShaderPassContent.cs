using System;

using Microsoft.Xna.Framework.Graphics;

namespace Pulsar.Pipeline.Graphics
{
    public sealed class ShaderPassContent
    {
        #region Constructors

        public ShaderPassContent(string name)
        {
            Name = name;

            Cull = CullMode.CullCounterClockwiseFace;
            FillMode = FillMode.Solid;

            DepthWrite = true;
            DepthCompare = CompareFunction.LessEqual;

            StencilMask = Int32.MaxValue;
            StencilWriteMask = Int32.MaxValue;
            StencilCompare = CompareFunction.Always;
            StencilPass = StencilOperation.Keep;
            StencilFail = StencilOperation.Keep;
            StencilDepthFail = StencilOperation.Keep;

            ColorBlend = BlendFunction.Add;
            AlphaBlend = BlendFunction.Add;
            ColorSource = Blend.One;
            ColorDestination = Blend.One;
            AlphaSource = Blend.One;
            AlphaDestination = Blend.One;
        }

        #endregion

        #region Properties

        public string Name { get; private set; }

        public CullMode Cull { get; set; }

        public FillMode FillMode { get; set; }

        public bool DepthWrite { get; set; }

        public CompareFunction DepthCompare { get; set; }

        public int StencilRef { get; set; }

        public int StencilMask { get; set; }

        public int StencilWriteMask { get; set; }

        public CompareFunction StencilCompare { get; set; }

        public StencilOperation StencilPass { get; set; }

        public StencilOperation StencilFail { get; set; }

        public StencilOperation StencilDepthFail { get; set; }

        public BlendFunction ColorBlend { get; set; }

        public BlendFunction AlphaBlend { get; set; }

        public Blend ColorSource { get; set; }

        public Blend ColorDestination { get; set; }

        public Blend AlphaSource { get; set; }

        public Blend AlphaDestination { get; set; }

        #endregion
    }
}
