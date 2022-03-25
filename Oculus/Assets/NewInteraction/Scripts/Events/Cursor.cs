using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cursor : OVRCursor
{
    public GameObject cursor;
    public LineRenderer lineRenderer;
    int delayFrame = 0;

    private void Start()
    {
        lineRenderer.startWidth = 0.005f;
        lineRenderer.endWidth = 0.00001f;
    }

    private void Update()
    {

        if (delayFrame > 0)
        {
            delayFrame--;
        }
        else
        {
            lineRenderer.enabled = false;
            delayFrame = 500;

            if (cursor.activeSelf)
                cursor.SetActive(false);

        }
    }

    public override void SetCursorRay(Transform ray)
    {

    }

    public override void SetCursorStartDest(Vector3 start, Vector3 dest, Vector3 normal)
    {
        if (!cursor.activeSelf)
            cursor.SetActive(true);

        Vector3 point = dest - normal * 0.002f;
        transform.localScale = Vector3.one * Mathf.Clamp(Vector3.Distance(start, dest) * 0.025f, 0.05f, 1f);
        transform.position = point;
        transform.forward = normal;
        delayFrame = 5;
        lineRenderer.enabled = true;
        lineRenderer.SetPosition(0, start);
        lineRenderer.SetPosition(1, point);
    }
}
