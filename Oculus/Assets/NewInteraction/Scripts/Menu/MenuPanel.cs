using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuPanel : MonoBehaviour
{
    private Vector3 m_scale;
    private Animator m_animator;
    public Transform head;
    public MoveAndScale moveAndScale;
    public Transform moveImageRoot;
    public Transform scaleImageRoot;


    public Transform friendRoot;
    public Transform apkRoot;
    public Transform mainRoot;
    public Transform sceneRoot;



    private void Start()
    {
        m_scale = transform.localScale;
        m_animator = GetComponentInChildren<Animator>();
        moveAndScale.onMoveTrigger += enter => { moveImageRoot.GetComponent<Image>().enabled = enter; moveImageRoot.GetChild(0).GetComponent<Image>().enabled = false; };
        moveAndScale.onMove += enter => { moveImageRoot.GetChild(0).GetComponent<Image>().enabled = enter; };
        moveAndScale.onScaleTrigger += (trigger,enter) => {
            string imageName = trigger.triggerName.Replace("Scale","Image");
           var trans=scaleImageRoot.Find(imageName);
            trans.GetComponent<Image>().enabled=enter;
            trans.GetChild(0).GetComponent<Image>().enabled=false;
        };
        moveAndScale.onScale += (trigger) => {
            string imageName = trigger.triggerName.Replace("Scale", "Image");
            var trans = scaleImageRoot.Find(imageName);
            trans.GetChild(0).GetComponent<Image>().enabled = true;
        };


        Show(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.O))
        {
            Show(true);
        }
        if (Input.GetKeyDown(KeyCode.C))
        {
            Show(false);
        }

        foreach (OVRInput.Button key in System.Enum.GetValues(typeof(OVRInput.Button)))
        {
            if (OVRInput.GetDown(key))
            {
                Debug.Log($"key:{key} down");
            }
        }


    }
    public void Show(bool isShow)
    {
        if (isShow)
        {
            transform.position = head.position + head.forward * 0.45f+head.right*0.1f;
            transform.forward = head.forward;
            m_animator.SetTrigger("Open");
            //m_animator.gameObject.SetActive(true);
            StartCoroutine(ShowUI(isShow));
        }
        else
        {
            m_animator.SetTrigger("Close");

            for (int i = 0; i < friendRoot.childCount; i++)
            {
                friendRoot.GetChild(i).gameObject.SetActive(isShow);
            }
            for (int i = 0; i < apkRoot.childCount; i++)
            {
                apkRoot.GetChild(i).gameObject.SetActive(isShow);
            }
            for (int i = 0; i < mainRoot.childCount; i++)
            {
                mainRoot.GetChild(i).gameObject.SetActive(isShow);
            }
            for (int i = 0; i < sceneRoot.childCount; i++)
            {
                sceneRoot.GetChild(i).gameObject.SetActive(isShow);
            }
            //m_animator.gameObject.SetActive(false);
        }
    }

    IEnumerator ShowUI(bool isShow)
    {

        yield return new WaitForSeconds(0.35f);
        if (isShow)
        {
            List<GameObject> centerList = new List<GameObject>();

            for (int i = 0; i < mainRoot.childCount; i++)
            {
                centerList.Add(mainRoot.GetChild(i).gameObject);
            }
            for (int i = 0; i < sceneRoot.childCount; i++)
            {
                centerList.Add(sceneRoot.GetChild(i).gameObject);
            }

            int start = (int)(centerList.Count * 0.6f);
            for (int i = 0; i < 20; i++)
            {
                if (i < centerList.Count)
                {
                   centerList[i].SetActive(true);
                }
                if (i>=start)
                {
                    int j = i - start;
                    if (j< friendRoot.childCount)
                    {
                        friendRoot.GetChild(j).gameObject.SetActive(isShow);
                    }

                    if (j < apkRoot.childCount)
                    {
                        apkRoot.GetChild(j).gameObject.SetActive(isShow);
                    }
                }

                yield return new WaitForSeconds(0.1f);
            }


        }


    }

}
