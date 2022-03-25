using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestGesture : MonoBehaviour
{
    public HandAndControllerInputModule inputModule;
    public LaserManager laserManager;
    public Toggle toggle_UseMove;
    public Toggle toggle_SaveData;
    public Slider slider_MaxMove;
    public Toggle toggle_CustomizeDirection;
    public Toggle toggle_MoveTrigger;
    public GameObject scroll_Content;
    public GameObject moveTrigger;

    public Text text_MaxMove;
    public Text text_MoveDistance;
    public Text text_HandConfidence;

    private void Start()
    {
        toggle_UseMove.onValueChanged.AddListener((isOn) => { inputModule.gestureConfigure.useMove = isOn; });
        slider_MaxMove.onValueChanged.AddListener((value) => { inputModule.moveConfigure.maxMove = value * 0.2f; });
        toggle_SaveData.onValueChanged.AddListener((isOn) => { inputModule.gestureConfigure.saveData = isOn; OVRPlugin.gestureTest = isOn; });
        toggle_CustomizeDirection.onValueChanged.AddListener((isOn) => { laserManager.useCustomize = isOn; });
        toggle_MoveTrigger.onValueChanged.AddListener((isOn) => { moveTrigger?.SetActive(isOn); });
        CreateScrollContent();
    }

    private void Update()
    {
        if (OVRInput.GetDown(OVRInput.RawButton.B))
        {
            Application.Quit();
        }
        UpdateUIText();
    }

    private void CreateScrollContent()
    {
        if (scroll_Content != null)
        {
            int length = 9;
            var rect = scroll_Content.transform.parent.GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(length * 210, length * 210);
            for (int i = 0; i < length * length; i++)
            {
                GameObject obj = GameObject.Instantiate<GameObject>(scroll_Content);
                obj.transform.SetParent(scroll_Content.transform.parent);
                obj.GetComponentInChildren<Text>().text = i.ToString();
                obj.transform.localScale = Vector3.one;
                obj.transform.localPosition = Vector3.zero;
                obj.SetActive(true);
            }
        }
    }

    void UpdateUIText()
    {
        text_MaxMove.text = inputModule?.moveConfigure.maxMove.ToString("f3");
        text_MoveDistance.text = inputModule?.moveConfigure.moveDistance.ToString("f3");
        text_HandConfidence.text = laserManager.rightHand.IsDataHighConfidence.ToString();
    }




}
