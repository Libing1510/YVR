
using System.Collections;
using System.Collections.Generic;
using Oculus.Platform;
using UnityEngine;
using UnityEngine.EventSystems;

public class LaserManager : MonoBehaviour
{
    public OVRManager ovrManager;
    public OVRInputModule inputModule;
    public Transform controllerTransform;

    public bool useCustomize;
    private Transform customizeRay;
    private Vector3 pos;
    private Vector3 startPoint;
    private Vector3 endPoint;

    public OVRHand rightHand;
    public OVRHand leftHand;

    private OVRSkeleton rightSkeleton;
    private OVRSkeleton leftSkeleton;

    private void Start()
    {
        customizeRay = new GameObject("customize_ray").transform;
        rightSkeleton = rightHand.GetComponent<OVRSkeleton>();
        leftSkeleton = leftHand.GetComponent<OVRSkeleton>();
        var sphereColl = rightHand.PointerPose.gameObject.AddComponent<SphereCollider>();
        sphereColl.radius = 0.02f;
        rightHand.PointerPose.tag = "hand";
        Quaternion qua = new Quaternion(0, 1, 0, 0);
        Debug.LogError(qua.eulerAngles);
    }

    private void Update()
    {
        UpdateRayTransform();
        HandOrController();
    }

    private void UpdateRayTransform2()
    {

    }
    private void UpdateRayTransform()
    {
        Vector3 currentStartPoint = rightSkeleton.Bones[(int)OVRSkeleton.BoneId.Hand_ForearmStub].Transform.position;
        if (Vector3.Distance(startPoint, currentStartPoint) < 0.05f)
        {
            startPoint = Vector3.Lerp(startPoint, currentStartPoint, Time.deltaTime * 2);
        }
        else
        {
            startPoint = Vector3.Lerp(startPoint, currentStartPoint, Time.deltaTime * 12);
            // startPoint = currentStartPoint;
        }

        // Vector3 currentEndPoint = (rightSkeleton.Bones[(int)OVRSkeleton.BoneId.Hand_Index1].Transform.position
        // + rightSkeleton.Bones[(int)OVRSkeleton.BoneId.Hand_Thumb2].Transform.position) * 0.5f;

        // Vector3 currentEndPoint = rightSkeleton.Bones[(int)OVRSkeleton.BoneId.Hand_Index1].Transform.position;

        Vector3 currentEndPoint = rightHand.PointerPose.position;
        if (Vector3.Distance(currentEndPoint, endPoint) < 0.05f)
        {
            endPoint = Vector3.Lerp(endPoint, currentEndPoint, Time.deltaTime);
        }
        else
        {
            // endPoint = currentEndPoint;
            endPoint = Vector3.Lerp(endPoint, currentEndPoint, Time.deltaTime * 5);
        }

        customizeRay.position = currentStartPoint;
        customizeRay.forward = (currentEndPoint - currentStartPoint).normalized;
    }

    private void HandOrController()
    {
        bool controllerConnected = OVRInput.IsControllerConnected(OVRInput.Controller.RTouch);
        bool handConnected = rightHand.IsDataHighConfidence;
        if (controllerConnected)
        {
            inputModule.rayTransform = controllerTransform;
        }
        else if (handConnected)
        {
            if (useCustomize)
            {
                inputModule.rayTransform = customizeRay;
            }
            else
            {
                inputModule.rayTransform = rightHand.PointerPose;
            }

        }
    }

}
