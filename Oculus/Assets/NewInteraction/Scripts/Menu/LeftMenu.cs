using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LeftMenu : MonoBehaviour
{

    public Animator contentAnimator;
    public Button mainPanel;
    public MenuPanel mainMenuPanel;

    private void Start()
    {
        mainPanel.onClick.AddListener(() => { mainMenuPanel.Show(true); });
    }


    public void Show(bool isShow)
    {
        if (isShow)
        {
            contentAnimator.SetTrigger("Open");
        }
        else
        {
            contentAnimator.SetTrigger("Close");
        }
    }



}
