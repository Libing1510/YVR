using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class RectangleLight : MonoBehaviour
{
	[Range(0,2)]
	public float LightMapIntensity = 1;
	[Range(0,6)]
	public int LodLevel = 6;
    [Range(0,10)]
    public float Intensity = 1;
    public float Range  = 20f;
    [Range(0,12)]
    public int mipLevel = 6;
    public Texture Cookie;

    private Bounds boundingBox;
	private Vector3 boundBoxSize = Vector3.zero;
	private RenderTexture miniCookie;

	private static int _LightMapIntensity	   = Shader.PropertyToID("_LightMapIntensity");
	private static int _Intensity              = Shader.PropertyToID("_Intensity");
    private static int _CookieMap              = Shader.PropertyToID("_CookieMap");
    private static int _RectWorldToLightMatrix = Shader.PropertyToID("_RectWorldToLightMatrix");
    private static int _RectLightLocalCenterPos = Shader.PropertyToID("_RectLightLocalCenterPos");
    private static int _RectLightLocalSize     = Shader.PropertyToID("_RectLightLocalSize");

    // Start is called before the first frame update
    void Start()
    {
        var meshFilter = this.GetComponent<MeshFilter>();
        if (meshFilter && meshFilter.sharedMesh)
        {
            boundingBox = meshFilter.sharedMesh.bounds;
        }
		miniCookie = new RenderTexture(Cookie.width >> LodLevel, Cookie.height >> LodLevel, 0, RenderTextureFormat.RGB111110Float, 0);
		if (miniCookie)
		{
			miniCookie.name = "VideoPlayerRT";
			miniCookie.useMipMap = false;
			miniCookie.autoGenerateMips = false;
			miniCookie.wrapMode = TextureWrapMode.Clamp;
			miniCookie.filterMode = FilterMode.Bilinear;
			if (!miniCookie.IsCreated())
			{
				miniCookie.Create();
			}
		}
		
#if !UNITY_EDITOR
		boundBoxSize.Set(boundingBox.size.x, boundingBox.size.y, Range);
#endif
	}

	private void OnDestroy()
	{
		if (miniCookie != null)
		{
			miniCookie.DiscardContents();
			miniCookie.Release();
			miniCookie = null;
		}
	}

	void OnEnable()
    {
		Shader.EnableKeyword("_RECTANGLELIT_COOKIE_SUPPORT");
    }

	void OnDisable()
	{
		Shader.DisableKeyword("_RECTANGLELIT_COOKIE_SUPPORT");
	}

	// Update is called once per frame
	void Update()
    {
#if UNITY_EDITOR
		boundBoxSize.Set(boundingBox.size.x, boundingBox.size.y, Range);
#endif
		Graphics.Blit(Cookie, miniCookie);
		Shader.SetGlobalFloat(_LightMapIntensity, LightMapIntensity);
        Shader.SetGlobalFloat(_Intensity, Intensity);
        Shader.SetGlobalTexture(_CookieMap, miniCookie);
        Shader.SetGlobalMatrix(_RectWorldToLightMatrix, this.transform.worldToLocalMatrix);
        Shader.SetGlobalVector(_RectLightLocalCenterPos, boundingBox.center);
        Shader.SetGlobalVector(_RectLightLocalSize, boundBoxSize);
    }
#if UNITY_EDITOR
	void OnDrawGizmos()
    {
        var tempMatrix = Gizmos.matrix;
        Gizmos.color = Color.green;
        Gizmos.matrix = this.transform.localToWorldMatrix;
        Gizmos.DrawWireCube(boundingBox.center - new Vector3(0,0, Range * 0.5f), new Vector3(boundingBox.size.x, boundingBox.size.y, Range));

        Gizmos.matrix = tempMatrix;
    }
#endif
}
