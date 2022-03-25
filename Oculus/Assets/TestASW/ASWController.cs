using System.Collections;
using System.Collections.Generic;
using Unity.XR.Oculus;
using UnityEngine;

public class ASWController : MonoBehaviour
{
    public float updateInterval =1.0f;
    private float lastInterval;
    private int frames = 0;
    private float fps;

    public TextMesh textObj;

    string asw = "ASW=false";
    private float[] _rates = null ;
    private int _index = 0;

    private void Start()
    {
        Application.targetFrameRate = 90;
        Performance.TrySetDisplayRefreshRate(90);

        if (Performance.TryGetAvailableDisplayRefreshRates(out _rates))
        {

        }
    }


    private void Update()
    {
        if (OVRInput.GetDown(OVRInput.RawButton.A))
        {
            OVRManager.SetSpaceWarp(true);
            asw = $"ASW=true";
        }

        if (OVRInput.GetDown(OVRInput.RawButton.B))
        {
            OVRManager.SetSpaceWarp(false);
            asw = $"ASW=false";
        }

        ++frames;
        float timeNow = Time.realtimeSinceStartup;
        if (timeNow >= lastInterval + updateInterval)
        {
            fps = frames / (timeNow - lastInterval);
            frames = 0;
            lastInterval = timeNow;
        }

        textObj.text = $"{asw}\n{fps}\n{OVRManager.display?.appFramerate}";

        if (_rates!=null)
        {
            if (OVRInput.GetDown(OVRInput.RawButton.RIndexTrigger))
            {
                _index = _index++ < _rates.Length ? _index : 0;
                Performance.TrySetDisplayRefreshRate(_rates[_index]);
            }
        }



    }






}
