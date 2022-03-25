
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UICursor : OVRCursor
{

    LineRenderer lineRend;
    int delayFrame = 0;
    public GameObject sphere;
    public Transform cube;
    public OVRSkeleton rightSkeleton;
    bool show = true;

    private void Start()
    {
        lineRend = GetComponent<LineRenderer>();
        cube.SetParent(null);
    }

    private void Update()
    {
        cube.position = (rightSkeleton.Bones[(int)OVRSkeleton.BoneId.Hand_IndexTip].Transform.position
         + rightSkeleton.Bones[(int)OVRSkeleton.BoneId.Hand_ThumbTip].Transform.position) * 0.5f;

        if (delayFrame > 0 && show)
        {
            delayFrame--;
        }
        else
        {
            lineRend.SetPosition(1, Vector3.forward);
            delayFrame = 500;
            cube.localScale = new Vector3(0.01f, 0.01f, 0.05f);
            if (sphere.activeSelf)
                sphere.SetActive(false);
            if (cube.gameObject.activeSelf)
                cube.gameObject.SetActive(false);
        }
    }

    public override void SetCursorRay(Transform ray)
    {
        transform.SetParent(ray);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
    }

    public override void SetCursorStartDest(Vector3 start, Vector3 dest, Vector3 normal)
    {
        if (!show)
        {
            return;
        }
        Vector3 pos = Vector3.forward * Vector3.Distance(start, dest) - Vector3.forward * 0.2f;
        lineRend.SetPosition(1, pos);
        delayFrame = 5;
        sphere.transform.localPosition = pos;
        if (!sphere.activeSelf)
            sphere.SetActive(true);

        if (!cube.gameObject.activeSelf)
            cube.gameObject.SetActive(true);
        cube.LookAt(dest);
        cube.localScale = new Vector3(0.01f, 0.01f, 0.05f);

    }

    public void Show(bool show)
    {
        this.show = show;
        lineRend.enabled = show;
    }
}
