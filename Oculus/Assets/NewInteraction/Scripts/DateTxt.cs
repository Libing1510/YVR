using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DateTxt : MonoBehaviour
{
    private Text m_dateText;
    // Start is called before the first frame update
    void Start()
    {
        m_dateText = GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.frameCount%60==1)
        {
            m_dateText.text = $"{DateTime.Now.Month}��{DateTime.Now.Day}�� ��{GetWeek()}";
        }
    }

    string GetWeek()
    {
        switch (DateTime.Now.DayOfWeek)
        {
            case DayOfWeek.Friday:
                return "��";
            case DayOfWeek.Monday:
                return "һ";
            case DayOfWeek.Saturday:
                return "��";
            case DayOfWeek.Sunday:
                return "��";
            case DayOfWeek.Thursday:
                return "��";
            case DayOfWeek.Tuesday:
                return "��";
            case DayOfWeek.Wednesday:
                return "��";
        }
        return "һ";
    }
}
