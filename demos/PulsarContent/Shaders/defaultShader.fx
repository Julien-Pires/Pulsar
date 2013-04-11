float4x4 World;
float4x4 View;
float4x4 Projection;

texture DiffuseMap;

sampler TextureSampler = sampler_state 
{ 
	texture = <DiffuseMap> ; 
	magfilter = LINEAR; 
	minfilter = LINEAR; 
	mipfilter=LINEAR; 
	AddressU = mirror; 
	AddressV = mirror;
};

struct VertexShaderInput
{
    float4 Position		: POSITION0;
	float3 Normal		: NORMAL;
    float2 TexCoord		: TEXCOORD0;
};

struct VertexShaderOutput
{
    float4 Position		: POSITION0;
    float3 TexCoord		: TEXCOORD0;
};

struct PixelShaderInput
{
    float4 Position		: POSITION0;
    float3 TexCoord		: TEXCOORD0;
};

struct PixelShaderOutput
{
    float4 Color	: COLOR0;
};

VertexShaderOutput RenderToGBufferVertexCommon(VertexShaderInput input, float4x4 instanceWorld) 
{
	VertexShaderOutput output = (VertexShaderOutput)0;
	float4x4 worldView = mul(instanceWorld, View);
	float4x4 worldViewProj = mul(worldView, Projection);

	output.Position = mul(input.Position, worldViewProj);
	output.TexCoord.xy = input.TexCoord;

	return output;
}

VertexShaderOutput RenderToGBufferVertexShader(VertexShaderInput input)
{
	return RenderToGBufferVertexCommon(input, World);
}

VertexShaderOutput RenderToGBufferInstancedVertexShader(VertexShaderInput input, float4x4 instanceTransform : TEXCOORD1)
{
	return RenderToGBufferVertexCommon(input, transpose(instanceTransform));
}

PixelShaderOutput RenderToGBufferPixelShader(PixelShaderInput input)
{
	PixelShaderOutput output = (PixelShaderOutput)0;

	output.Color = tex2D(TextureSampler, input.TexCoord);

	return output;
}

technique RenderToGBuffer
{
	pass RenderToGBufferPass
	{
		VertexShader = compile vs_3_0 RenderToGBufferVertexShader();
        PixelShader = compile ps_3_0 RenderToGBufferPixelShader();
	}
}

technique RenderToGBufferInstanced
{
	pass RenderToGBufferInstancedPass
	{
		VertexShader = compile vs_3_0 RenderToGBufferInstancedVertexShader();
		PixelShader = compile ps_3_0 RenderToGBufferPixelShader();
	}
}