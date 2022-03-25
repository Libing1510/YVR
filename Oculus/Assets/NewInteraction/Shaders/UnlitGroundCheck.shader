Shader "Unlit/GroundCheck"
{
	Properties
	{
		_MainColor("MainColor", color) = (0,1,0,1)
		_SecondColor("SecondColor", color) = (1,0,0,1)
		_MainTex("Texturesss", 2D) = "white" {}
		_transparentInner("Transparent Inner", float) = 10
		_transparentOuter("Transparent Outer", float) = 20
		_edgeInner("Edge Inner", float) = 25
		_edgeOuter("Edge Outer", float) = 30
		_alphaGradual("Aplha Gradual", float) = 1
	}
		SubShader
		{
			Cull Off ZWrite Off ZTest Always

			Blend SrcAlpha OneMinusSrcAlpha // Support Transparent

			Pass
			{
				CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag

				#include "UnityCG.cginc"

				sampler2D _MainTex;
				float4 _MainTex_ST;
				half _transparentInner;
				half _transparentOuter;
				half _edgeInner;
				half _edgeOuter;
				half _alphaGradual;
				float4 _MainColor;
				float4 _SecondColor;

				float _PointPos[2];


				struct appdata
				{
					float4 vertex : POSITION;
					float2 uv : TEXCOORD0;
				};

				struct v2f
				{
					float2 transformedUV :TEXCOORD1;
					float2 uv : TEXCOORD0;
					float4 vertex : SV_POSITION;
				};

				v2f vert(appdata v)
				{
					v2f o;
					o.vertex = UnityObjectToClipPos(v.vertex);
					o.uv = v.uv;
					o.transformedUV = TRANSFORM_TEX(v.uv,_MainTex);
					return o;
				}


				fixed4 frag(v2f i) : SV_Target
				{
					fixed4 col = tex2D(_MainTex, i.transformedUV);

					half2 pos = half2(_PointPos[0],_PointPos[1]);
					half pointRadius = distance(pos,i.transformedUV);

					float alphaCircleOutter = smoothstep(_edgeOuter, _edgeInner, pointRadius);

					float alphaCircle = smoothstep(_transparentInner, _transparentOuter, pointRadius);
					alphaCircle = 1 - alphaCircle;

					float alpha = (alphaCircleOutter - alphaCircle) * _alphaGradual;

					fixed4 color = lerp(_MainColor, _SecondColor, pointRadius);

					return fixed4(color.x, color.y, color.z, col.w * alpha);
				}
				ENDCG
			}
		}
}
