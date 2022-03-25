#ifndef YVR_RECTANGLE_LIGHTING_INCLUDED
#define YVR_RECTANGLE_LIGHTING_INCLUDED

half      _LodLevel;
half	  _Intensity;
TEXTURE2D (_CookieMap);
SAMPLER   (sampler_CookieMap);
float4x4  _RectWorldToLightMatrix;
float3	  _RectLightLocalCenterPos;
float3	  _RectLightLocalSize;

half3 SampleRectangleLightingCookie(float3 positionWS,float3 normalWS)
{	
	float3 posOS = mul(_RectWorldToLightMatrix, float4(positionWS, 1.0)).xyz - _RectLightLocalCenterPos;
	
	float3 localForward = -SafeNormalize(_RectWorldToLightMatrix._m20_m21_m22);
	
	float3 normalOS = mul((float3x3)_RectWorldToLightMatrix, normalWS);

	float3 dir = -SafeNormalize(posOS);
    half  NdotL = max(dot(dir, normalOS), 0);
	half  NdotL2 = max(dot(-localForward, normalOS) * 0.7 + 0.3 ,0.0);

	float3 bounds = _RectLightLocalSize * 0.5;
    float3 ndcHalfPos = posOS * rcp(bounds);

	half proj = dot(localForward, posOS.xyz);
	
	half dist = proj < 0.0 ?  0.0 : 1.0 - saturate(proj * rcp(_RectLightLocalSize.z));
	half dist2Reduce = dist * dist;
	half3 decalColor = SAMPLE_TEXTURE2D_LOD(_CookieMap, sampler_CookieMap, ndcHalfPos.xy * 0.5 + 0.5, _LodLevel).rgb * _Intensity * NdotL * NdotL2 * dist2Reduce;

    return decalColor;
}


#endif
