Shader "Hidden/TerrainMap"
{
    Properties
    {
		
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        
        Pass
        {
			ZWrite Off ZTest Always Blend Off Cull Off
            
			HLSLPROGRAM
			#include "UnityCG.cginc"
			
            #pragma vertex Vert
            #pragma fragment Frag
			
			float2 GetFullScreenTriangleTexCoord(uint vertexID)
			{
			#if UNITY_UV_STARTS_AT_TOP
				return float2((vertexID << 1) & 2, 1.0 - (vertexID & 2));
			#else
				return float2((vertexID << 1) & 2, vertexID & 2);
			#endif
			}

			float4 GetFullScreenTriangleVertexPosition(uint vertexID, float z = UNITY_NEAR_CLIP_VALUE)
			{
				float2 uv = float2((vertexID << 1) & 2, vertexID & 2);
				return float4(uv * 2.0 - 1.0, z, 1.0);
			}

			struct Attributes
			{
				uint vertexID : SV_VertexID;
			};

			struct Varyings
			{
				float4 positionCS : SV_POSITION;
				float2 texcoord   : TEXCOORD0;
			};

			Varyings Vert(Attributes input)
			{
				Varyings output;
				output.positionCS = GetFullScreenTriangleVertexPosition(input.vertexID);
				output.texcoord = GetFullScreenTriangleTexCoord(input.vertexID);
				return output;
			}
			
			SamplerState sampler_LinearRepeat;
			int _TextureCount;
			StructuredBuffer<float4> _ScaleOffsetBuff;
			Texture2DArray<float4> _DiffuseMapArray;
			Texture2DArray<float4> _NormalMapArray;
			Texture2DArray<float4> _MaskMapArray;

			#define SAMPLE_TEXTURE_ARRAY(tex,uv2,index) tex.SampleLevel(sampler_LinearRepeat, float3(uv2, index), 0)
			
			void Frag(Varyings input
				, out float4 outDiffuse: SV_Target0
				, out float3 outNormal : SV_Target1)
			{
				float4 diffuse = 0;
				float3 normal = 0;

				for (int i = 0; i < _TextureCount; i++)
				{	
					float4 alpha = SAMPLE_TEXTURE_ARRAY(_MaskMapArray, input.texcoord, i / 4);
					float mask = 0;
					switch (i % 4)
					{
					case 0:
						mask = alpha.r;
						break;
					case 1:
						mask = alpha.g;
						break;
					case 2:
						mask = alpha.b;
						break;
					case 3:
						mask = alpha.a;
						break;
					}
					float4 scaleOffset = _ScaleOffsetBuff[i];
					float3 nor = UnpackNormal(SAMPLE_TEXTURE_ARRAY(_NormalMapArray, input.texcoord * scaleOffset.xy + scaleOffset.zw, i));
					float4 diff = SAMPLE_TEXTURE_ARRAY(_DiffuseMapArray, input.texcoord * scaleOffset.xy + scaleOffset.zw, i);
					diffuse += diff * mask;
					normal += nor * mask;
				}
				outDiffuse = diffuse;
				outNormal = normalize(normal) * 0.5 + 0.5;
			}
            ENDHLSL
        }
    }
}
