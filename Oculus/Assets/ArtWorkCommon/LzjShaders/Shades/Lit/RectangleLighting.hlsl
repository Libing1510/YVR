#ifndef RECTANGLE_LIGHTING_INCLUDED
#define RECTANGLE_LIGHTING_INCLUDED

half      _LodLevel;
half	  _Intensity;
TEXTURE2D (_CookieMap);
SAMPLER   (sampler_CookieMap);
float3    _RectLightForward;
float4x4  _RectWorldToLightMatrix;
float3	  _RectLightWorldCenter;
float3	  _RectLightLocalSize;


half3 SampleRectangleLightingCookie(float3 positionWS,float3 normalWS)
{	
	float3 posOS = mul(_RectWorldToLightMatrix, float4(positionWS - _RectLightWorldCenter, 1.0)).xyz;
	
	float3 localForward = mul((float3x3)_RectWorldToLightMatrix, _RectLightForward);
	
	float3 normalOS = mul((float3x3)_RectWorldToLightMatrix, normalWS);

	float3 dir = -SafeNormalize(posOS);
    float  NdotL = max(dot(dir, normalOS), 0);
	float  NdotL2 = max(dot(-localForward, normalOS) * 0.7 + 0.3 , 0);

	float3 bounds = _RectLightLocalSize * 0.5;
    float3 ndcHalfPos = posOS * rcp(bounds);

	float proj = dot(localForward, posOS.xyz);
	
    float dist = proj < 0.0 ?  0.0 : 1.0 - saturate(proj * rcp(_RectLightLocalSize.z));
    float dist2Reduce = dist * dist;

    half3 decalColor = SAMPLE_TEXTURE2D_LOD(_CookieMap, sampler_CookieMap, ndcHalfPos.xy * 0.5 + 0.5, _LodLevel).rgb * _Intensity * NdotL * NdotL2 * dist2Reduce;

    return decalColor;
}


#endif
