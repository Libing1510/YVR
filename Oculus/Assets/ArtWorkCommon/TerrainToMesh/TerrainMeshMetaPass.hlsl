#ifndef TERRAIN_MESH_META_PASS_INCLUDED
#define TERRAIN_MESH_META_PASS_INCLUDED

#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/MetaInput.hlsl"

struct Attributes
{
    float4 positionOS   : POSITION;
    float3 normalOS     : NORMAL;
    float2 uv0          : TEXCOORD0;
    float2 uv1          : TEXCOORD1;
    float2 uv2          : TEXCOORD2;
#ifdef _TANGENT_TO_WORLD
    float4 tangentOS     : TANGENT;
#endif
};

struct Varyings
{
    float4 positionCS   : SV_POSITION;
    float2 uv           : TEXCOORD0;
    SLOT(1, 2)
};

Varyings TerrainMeshVertex(Attributes input)
{
    Varyings output;
    output.positionCS = MetaVertexPosition(input.positionOS, input.uv1, input.uv2,
        unity_LightmapST, unity_DynamicLightmapST);
    output.uv = input.uv0;
    SLOT_VERTEX(output, input.uv0);
    return output;
}

half4 TerrainMeshFragment(Varyings input) : SV_Target
{
    float2 uv = input.uv;

    float2 uv0 = input.slot0UV.xy;
    float2 uv1 = input.slot0UV.zw;
    float2 uv2 = input.slot1UV.xy;

    MetaInput metaInput;

    half4 baseColor;
    SampleAlbedoMap(uv, uv0, uv1, uv2 , baseColor);

    metaInput.Albedo = baseColor.rgb;
    metaInput.SpecularColor = half3(0.0, 0.0, 0.0);
    metaInput.Emission = half3(0.0,0.0,0.0);

    return MetaFragment(metaInput);
}


//LWRP -> Universal Backwards Compatibility
Varyings LightweightVertexMeta(Attributes input)
{
    return TerrainMeshVertex(input);
}

half4 LightweightFragmentMetaSimple(Varyings input) : SV_Target
{
    return TerrainMeshFragment(input);
}

#endif
