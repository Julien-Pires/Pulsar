// Data struct
struct baseVSInput
{
	float4 position : POSITION0;
	float4 normal : NORMAL0;
	float4 textureCoord : TEXCOORD0;
};

struct baseVSOutput
{
	float4 position : POSITION0;
	float4 textureCoord : TEXCOORD0;
};