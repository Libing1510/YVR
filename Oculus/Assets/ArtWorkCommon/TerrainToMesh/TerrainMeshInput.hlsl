#ifndef TERRAIN_MESH_INPUT_INCLUDED
#define TERRAIN_MESH_INPUT_INCLUDED

#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/SurfaceInput.hlsl"

TEXTURE2D_ARRAY(_BaseMapArray);       SAMPLER(sampler_BaseMapArray);
TEXTURE2D_ARRAY(_BumpMapArray);       SAMPLER(sampler_BumpMapArray);
TEXTURE2D(_LayerMaskMap);             SAMPLER(sampler_LayerMaskMap);

CBUFFER_START(UnityPerMaterial)
float4 _Slot0;
float4 _Slot1;
float4 _Slot2;
CBUFFER_END

#ifdef UNITY_DOTS_INSTANCING_ENABLED
UNITY_DOTS_INSTANCING_START(MaterialPropertyMetadata)
UNITY_DOTS_INSTANCED_PROP(float4, _Slot0)
UNITY_DOTS_INSTANCED_PROP(float4, _Slot1)
UNITY_DOTS_INSTANCED_PROP(float4, _Slot2)
UNITY_DOTS_INSTANCING_END(MaterialPropertyMetadata)

#define _Slot0                UNITY_ACCESS_DOTS_INSTANCED_PROP_FROM_MACRO(float4 , Metadata__Slot0)
#define _Slot1                UNITY_ACCESS_DOTS_INSTANCED_PROP_FROM_MACRO(float4 , Metadata__Slot1)
#define _Slot2                UNITY_ACCESS_DOTS_INSTANCED_PROP_FROM_MACRO(float4 , Metadata__Slot2)
#endif


#define SLOT(idx1,idx2)          float4 slot0UV : TEXCOORD##idx1; float2 slot1UV : TEXCOORD##idx2;
#define SLOT_VERTEX(input,uv)    \
input.slot0UV.xy = _Slot0.xy * uv + _Slot0.zw; \
input.slot0UV.zw = _Slot1.xy * uv + _Slot1.zw; \
input.slot1UV.xy = _Slot2.xy * uv + _Slot2.zw;

inline void SampleAlbedoMap(float2 uv, float2 uv0, float2 uv1, float2 uv2, out half4 baseColor)
{
    half3 layerMask = SAMPLE_TEXTURE2D(_LayerMaskMap, sampler_LayerMaskMap, uv).xyz;

    baseColor = half4(0.0, 0.0, 0.0, 0.0);

    baseColor += SAMPLE_TEXTURE2D_ARRAY(_BaseMapArray, sampler_BaseMapArray, uv0, 0) * layerMask.x;
    baseColor += SAMPLE_TEXTURE2D_ARRAY(_BaseMapArray, sampler_BaseMapArray, uv1, 1) * layerMask.y;
    baseColor += SAMPLE_TEXTURE2D_ARRAY(_BaseMapArray, sampler_BaseMapArray, uv2, 2) * layerMask.z;
}

inline void SampleTerrainMap(float2 uv, float2 uv0, float2 uv1, float2 uv2, out half4 baseColor, out half3 normal)
{
    half3 layerMask = SAMPLE_TEXTURE2D(_LayerMaskMap, sampler_LayerMaskMap, uv).xyz;

    baseColor = half4(0.0, 0.0, 0.0, 0.0);

    baseColor += SAMPLE_TEXTURE2D_ARRAY(_BaseMapArray, sampler_BaseMapArray, uv0, 0) * layerMask.x;
    baseColor += SAMPLE_TEXTURE2D_ARRAY(_BaseMapArray, sampler_BaseMapArray, uv1, 1) * layerMask.y;
    baseColor += SAMPLE_TEXTURE2D_ARRAY(_BaseMapArray, sampler_BaseMapArray, uv2, 2) * layerMask.z;

    normal = half3(0.0, 0.0, 0.0);

    normal += UnpackNormal(SAMPLE_TEXTURE2D_ARRAY(_BumpMapArray, sampler_BumpMapArray, uv0, 0)) * layerMask.x;
    normal += UnpackNormal(SAMPLE_TEXTURE2D_ARRAY(_BumpMapArray, sampler_BumpMapArray, uv1, 1)) * layerMask.y;
    normal += UnpackNormal(SAMPLE_TEXTURE2D_ARRAY(_BumpMapArray, sampler_BumpMapArray, uv2, 2)) * layerMask.z;

    normal = SafeNormalize(normal);
}

inline void InitializeTerrainMeshSurfaceData(float2 uv, float2 uv0, float2 uv1, float2 uv2, out SurfaceData outSurfaceData)
{
    outSurfaceData = (SurfaceData)0;

    half4 baseColor;
    half3 normal;
    SampleTerrainMap(uv, uv0, uv1, uv2, baseColor, normal);

    outSurfaceData.albedo = baseColor.rgb;
    outSurfaceData.alpha = 1.0;
    outSurfaceData.metallic = 0.0; // unused
    outSurfaceData.specular = half3(0.0, 0.0, 0.0);
    outSurfaceData.smoothness = baseColor.a;
    outSurfaceData.normalTS = normal;
    outSurfaceData.occlusion = 1.0; // unused
    outSurfaceData.emission = half3(0.0, 0.0, 0.0);
}

#endif
