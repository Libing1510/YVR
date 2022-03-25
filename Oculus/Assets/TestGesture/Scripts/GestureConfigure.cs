using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class GestureConfigure
{
    public bool saveData;
    public bool useMove;
    public bool autoClick;
}

[System.Serializable]
public class MoveConfigure
{
    public float maxMove;
    public float moveDistance;
}
