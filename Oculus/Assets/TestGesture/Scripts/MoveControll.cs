
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MoveControll : MonoBehaviour
{
    public UICursor uICursor;
    public Scrollbar scroller_H;
    public Scrollbar scroller_V;
    private Color mat_color;
    private Material mat;
    private bool enterHand;
    private Vector3 enterPose;
    public int maxPackage = 2;
    public float h;
    public float v;
    public bool test;

    private void Start()
    {
        mat = GetComponent<MeshRenderer>().material;
    }

    private async void Update()
    {
        if (test)
        {
            scroller_H.value = h;
            scroller_V.value = v;
        }
        else if (!enterHand)
        {
            float halfPageValue = 1.0f / (maxPackage - 1) * 0.5f;
            Debug.Log(halfPageValue);
            for (int i = 0; i < maxPackage; i++)
            {
                float pageValue = 1.0f / (maxPackage - 1) * i;
                float minPageValue = pageValue - halfPageValue;
                float maxPageValue = pageValue + halfPageValue;
                // Debug.Log($"{i},{pageValue},{minPageValue},{maxPageValue}");
                if (minPageValue < scroller_H.value && scroller_H.value < maxPageValue)
                {
                    // Debug.Log($"H:{ scroller_H.value},to,{pageValue},{minPageValue},{maxPageValue}");
                    scroller_H.value = Mathf.Lerp(scroller_H.value, pageValue, Time.deltaTime * 5);
                }

                if (minPageValue < scroller_V.value && scroller_V.value < maxPageValue)
                {
                    scroller_V.value = Mathf.Lerp(scroller_V.value, pageValue, Time.deltaTime * 5);
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("hand"))
        {
            mat_color.r = 1;
            enterHand = true;
            enterPose = other.transform.position;
            mat.SetColor("_Color", mat_color);
            uICursor?.Show(false);
        }

    }

    private void OnTriggerStay(Collider other)
    {
        var movePose = other.transform.position - enterPose;
        if (Mathf.Abs(movePose.x) > 0.025f || Mathf.Abs(movePose.y) > 0.025f)
        {
            scroller_H.value += movePose.x / 1f;
            scroller_V.value -= movePose.y / 1f;
            enterPose = other.transform.position;

        }

    }


    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("hand"))
        {
            mat_color.r = 0;
            enterHand = false;
            enterPose = Vector3.zero;
            mat.SetColor("_Color", mat_color);
            uICursor?.Show(true);
        }

    }





}
