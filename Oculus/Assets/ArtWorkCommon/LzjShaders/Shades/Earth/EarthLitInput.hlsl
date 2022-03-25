#ifndef UNIVERSAL_EARTH_LIT_INPUT_INCLUDED
#define UNIVERSAL_EARTH_LIT_INPUT_INCLUDED

#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/CommonMaterial.hlsl"
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/SurfaceInput.hlsl"
#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/ParallaxMapping.hlsl"

#if defined(_DETAIL_MULX2) || defined(_DETAIL_SCALED)
#define _DETAIL
#endif

// NOTE: Do not ifdef the properties here as SRP batcher can not handle different layouts.
CBUFFER_START(UnityPerMaterial)
float4 _BaseMap_ST;
half4  _BaseColor;
half4  _EmissionColor;
half   _Cutoff;
half   _Smoothness;
half   _Metallic;
half   _BumpScale;
half3  _SeaColor;
half3  _RimColor;
half   _RotSpeed;
half   _CloudRotSpeed;
half   _CloudIntensity;
half   _CloudRamp;
CBUFFER_END

TEXTURE2D(_RoughnessMap);        SAMPLER(sampler_RoughnessMap);
TEXTURE2D(_CloudMap);			 SAMPLER(sampler_CloudMap);

inline void InitializeStandardLitSurfaceData(float2 uv, out SurfaceData outSurfaceData)
{
    half4 albedoAlpha = SampleAlbedoAlpha(uv, TEXTURE2D_ARGS(_BaseMap, sampler_BaseMap));
    outSurfaceData.alpha = Alpha(albedoAlpha.a, _BaseColor, _Cutoff);
    outSurfaceData.albedo = albedoAlpha.rgb * _BaseColor.rgb;

	outSurfaceData.specular = half3(0.0h, 0.0h, 0.0h);

	half roughness = SAMPLE_TEXTURE2D(_RoughnessMap, sampler_RoughnessMap, uv).r;

	outSurfaceData.albedo = lerp(outSurfaceData.albedo, _SeaColor, smoothstep(0.9, 1, 1.0 - sqrt(roughness)));

	half smoothness = saturate(0.96 - 0.9 * roughness);

	smoothness = pow(smoothness, 6);

	half metallic = pow(saturate(roughness + 0.1),0.4545);
	outSurfaceData.metallic = metallic * _Metallic;
    outSurfaceData.smoothness = _Smoothness * smoothness;
    outSurfaceData.normalTS = SampleNormal(uv, TEXTURE2D_ARGS(_BumpMap, sampler_BumpMap), _BumpScale);
    outSurfaceData.occlusion = 1.0h;
    outSurfaceData.emission = SampleEmission(uv, _EmissionColor.rgb, TEXTURE2D_ARGS(_EmissionMap, sampler_EmissionMap));

	

	outSurfaceData.clearCoatMask = 0.0h;
	outSurfaceData.clearCoatSmoothness = 0.0h;
}

#endif // UNIVERSAL_EARTH_LIT_INPUT_INCLUDED
