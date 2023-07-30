#if OPENGL
	#define SV_POSITION POSITION
	#define VS_SHADERMODEL vs_3_0
	#define PS_SHADERMODEL ps_3_0
#else
	#define VS_SHADERMODEL vs_4_0_level_9_1
	#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

Texture2D SpriteTexture;

sampler2D SpriteTextureSampler = sampler_state
{
	Texture = <SpriteTexture>;
};

float4 overlayColor;
float4 transparent = float4(0, 0, 0, 0);

struct PixelInput
{
    float4 Position : SV_POSITION;
    float4 Color : COLOR0;
    float2 TextureCoordinates : TEXCOORD0;
};

float4 SpritePixelShader(PixelInput input) : COLOR0
{
    float4 color = tex2D(SpriteTextureSampler, input.TextureCoordinates);
    if (color.a > 0)
        return float4(0, 0, 0, 0.4);
    return transparent;
}

technique SpriteDrawing
{
	pass P0
	{
        //VertexShader = compile VS_SHADERMODEL SpriteVertexShader();
        PixelShader = compile PS_SHADERMODEL SpritePixelShader();
    }
};