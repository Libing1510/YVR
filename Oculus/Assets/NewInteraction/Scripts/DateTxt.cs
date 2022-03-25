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
            m_dateText.text = $"{DateTime.Now.Month}月{DateTime.Now.Day}日 周{GetWeek()}";
        }
    }

    string GetWeek()
    {
        switch (DateTime.Now.DayOfWeek)
        {
            case DayOfWeek.Friday:
                return "五";
            case DayOfWeek.Monday:
                return "一";
            case DayOfWeek.Saturday:
                return "六";
            case DayOfWeek.Sunday:
                return "日";
            case DayOfWeek.Thursday:
                return "四";
            case DayOfWeek.Tuesday:
                return "二";
            case DayOfWeek.Wednesday:
                return "三";
        }
        return "一";
    }
}
