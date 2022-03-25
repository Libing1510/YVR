using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundRenderFix : MonoBehaviour
{
    [SerializeField]
    private MeshRenderer renderer;
    [SerializeField]
    private string arrayName;

    float _minX;
    float _maxX;

    float _minY;
    float _maxY;


    Vector2 _scale;
    private void Awake()
    {
        _scale = renderer.material.GetTextureScale("_MainTex");

        _minX = renderer.transform.localScale.x * -5f;
        _maxX = renderer.transform.localScale.x * 5f;

        _minY = renderer.transform.localScale.z * -5f;
        _maxY = renderer.transform.localScale.z * 5f;
    }

    float _x;
    float _y;
    public void SetPos(Vector2 pos)
    {
        _x = (pos.x - _minX) / (_maxX - _minX) * _scale.x;
        _y = (pos.y - _minY) / (_maxY - _minY) * _scale.y;

        renderer.material.SetFloatArray(arrayName, new float[] { _x, _y });
    }

    #region UnlitGroundCheck.shader ONLY
    private Action finishCB;
    public void StartSpreadAnim(Action cb)
    {
        finishCB = cb;
        startSpread = true;
    }

    public void ResetAnim()
    {
        renderer.material.SetFloat("_edgeOuter", 1);
        renderer.material.SetFloat("_edgeInner", 0.7f);
        renderer.material.SetFloat("_transparentOuter", 0.4f);
        renderer.material.SetFloat("_transparentInner", 0.2f);
        renderer.material.SetFloat("_alphaGradual", 1);
    }

    private bool startSpread = false;
    private float eAddPerFrame = 0.001f;
    private float tAddPerFrame = 0.001f;
    private float mAlphaPerFrame = -0.01f;
    private void LateUpdate()
    {
        if (!startSpread)
            return;

        float eo = renderer.material.GetFloat("_edgeOuter");
        renderer.material.SetFloat("_edgeOuter", eo + eAddPerFrame);

        float ei = renderer.material.GetFloat("_edgeInner");
        renderer.material.SetFloat("_edgeInner", ei + eAddPerFrame);

        float to = renderer.material.GetFloat("_transparentOuter");
        renderer.material.SetFloat("_transparentOuter", to + tAddPerFrame);

        float ti = renderer.material.GetFloat("_transparentInner");
        renderer.material.SetFloat("_transparentInner", ti + tAddPerFrame);

        float ag = renderer.material.GetFloat("_alphaGradual");
        renderer.material.SetFloat("_alphaGradual", Mathf.Max(0f, ag + mAlphaPerFrame));

        eAddPerFrame += 0.0005f;
        tAddPerFrame += 0.0005f;

        if (ag <= 0f)
        {
            finishCB?.Invoke();
            ResetAnim();
            startSpread = false;
        }
    }
    #endregion
}
