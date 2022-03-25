
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class HandAndControllerInputModule : OVRInputModule
{
    public OVRHand leftHand;
    public OVRHand rightHand;

    override protected PointerEventData.FramePressState GetGazeButtonState()
    {
        var pressed = Input.GetKeyDown(gazeClickKey) || OVRInput.GetDown(joyPadClickButton) || GetGestureDown();
        var released = Input.GetKeyUp(gazeClickKey) || OVRInput.GetUp(joyPadClickButton) || GetGestureUp();

#if UNITY_ANDROID && !UNITY_EDITOR
            pressed |= Input.GetMouseButtonDown(0);
            released |= Input.GetMouseButtonUp(0);
#endif

        if (pressed && released)
            return PointerEventData.FramePressState.PressedAndReleased;
        if (pressed)
            return PointerEventData.FramePressState.Pressed;
        if (released)
            return PointerEventData.FramePressState.Released;
        return PointerEventData.FramePressState.NotChanged;
    }

    public int clickFrame;
    public bool down;
    public Vector3 originPos = Vector3.zero;
    public bool distanceClick;

    public MoveConfigure moveConfigure;
    public GestureConfigure gestureConfigure;

    private bool GetGestureDown()
    {
        if (gestureConfigure.useMove)
        {
            if (originPos.Equals(Vector3.zero) || !rightHand.IsDataHighConfidence)
            {
                originPos = rightHand.PointerPose.position;
            }
            else
            {
                var dir = rightHand.PointerPose.position - originPos;
                Vector3 move = Vector3.Project(dir, rightHand.PointerPose.forward);
                moveConfigure.moveDistance = Vector3.Angle(rightHand.PointerPose.forward, dir) > 90 ? move.magnitude * -1f : move.magnitude;
                distanceClick = moveConfigure.moveDistance > moveConfigure.maxMove;
                if (distanceClick && !down)
                {
                    down = true;
                    return true;
                }
            }
        }
        return false;
    }

    private bool GetGestureUp()
    {
        if (gestureConfigure.useMove)
        {
            if (rightHand.IsDataHighConfidence && !distanceClick && down)
            {
                down = false;
                return true;
            }
        }
        return false;
    }



}
