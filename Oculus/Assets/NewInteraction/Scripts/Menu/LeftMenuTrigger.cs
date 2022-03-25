using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class LeftMenuTrigger : MonoBehaviour
{
    public LeftMenu menu;
    public ColliderTrigger enterTrigger;
    public ColliderTrigger exitTrigger;
    public Transform controller;
    public Transform head;

    private void Start()
    {
        enterTrigger.onTriggerEnterCallback += (trigger, colldier) => { if (colldier.name == "MenuPointer") menu.Show(true); /*Debug.Log(colldier.name);*/ };

        exitTrigger.onTriggerExitCallback += (trigger, colldier) => { if (colldier.name == "MenuPointer") menu.Show(false); };
    }

    // controller:(-0.020, -0.198, 0.337),(2.615, 61.702, 33.765),head:(0.027, -0.020, -0.042),(350.367, 7.676, 5.631)
    public bool simlation;

    private void Update()
    {
        if (simlation)
        {
            controller.position = new Vector3(-0.02f, -0.198f, 0.337f);
            controller.rotation = Quaternion.Euler(2.615f, 61.702f, 33.765f);

            head.position = new Vector3(0.027f, -0.020f, -0.042f);
            head.rotation = Quaternion.Euler(350.367f, 7.676f, 5.631f);
        }

        transform.position = controller.position;
        transform.rotation = Quaternion.Euler(0, head.eulerAngles.y, 0);

    }




}
