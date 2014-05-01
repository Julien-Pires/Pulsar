/******** Includes ********/
#include <../Common/Declarations.fxh>

/******** Constants ********/
float4x4 worldViewProjection : WorldViewProjection;

/******** Textures ********/
texture mainTexture;
sampler2D mainTextureSampler = sampler_state
{
	Texture = <mainTexture>;
	ADDRESSU = WRAP;
	ADDRESSV = WRAP;
	MinFilter = ANISOTROPIC;
	MagFilter = ANISOTROPIC;
};

/******** Vertex shader ********/
baseVSOutput vs_main(baseVSInput input)
{
	baseVSOutput output;
	output.position = mul(input.position, worldViewProjection);
	output.textureCoord = input.textureCoord;

    return output;
}

/******** Pixel shader ********/
float4 ps_main(baseVSOutput input) : COLOR0
{
	return tex2D(mainTextureSampler, input.textureCoord);
}

/******** Techniques ********/
technique UnlitTexture
{
    pass Pass0
    {
        VertexShader = compile vs_3_0 vs_main();
        PixelShader = compile ps_3_0 ps_main();
    }
}
