// Unity built-in shader source. Copyright (c) 2016 Unity Technologies. MIT license (see license.txt)

Shader "Skybox/SimpleProcedural" 
{
Properties {

    _SkyColor ("Sky Tint", Color) = (.5, .5, .5, 1)
    _GroundColor ("Ground", Color) = (.369, .349, .341, 1)
    _RemapMin("Remap Min",Range(-1,1)) = 0
    _RemapMax("Remap Max",Range(-1,1)) = 1
}

SubShader {
    Tags { "Queue"="Background" "RenderType"="Background" "PreviewType"="Skybox" }
    Cull Off ZWrite Off

    Pass {

        HLSLPROGRAM
        #pragma vertex vert
        #pragma fragment frag


        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

        struct appdata_t
        {
            float4 vertex : POSITION;
            UNITY_VERTEX_INPUT_INSTANCE_ID
        };

        struct v2f
        {
            float4  pos             : SV_POSITION;
            float3  vertex          : TEXCOORD0;
            UNITY_VERTEX_OUTPUT_STEREO
        };

        v2f vert (appdata_t v)
        {
            v2f OUT;
            UNITY_SETUP_INSTANCE_ID(v);
            UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);
            OUT.pos = TransformObjectToHClip(v.vertex.xyz);
            OUT.vertex = v.vertex.xyz;
            return OUT;
        }
        
        half4 _SkyColor;
        half4 _GroundColor;
        half  _RemapMin;
        half  _RemapMax;
        half4 frag(v2f IN) : SV_Target
        {
            half3 col = lerp(_GroundColor.rgb,_SkyColor.rgb,smoothstep(_RemapMin,_RemapMax,IN.vertex.y));
            
            return half4(col,1.0);

        }
        ENDHLSL
    }
}


Fallback Off
}
