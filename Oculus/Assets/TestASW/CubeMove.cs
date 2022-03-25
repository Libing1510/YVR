using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeMove : MonoBehaviour
{

    public Vector3 start;
    public Vector3 stop;
    public float speed = 1.0f;


    private Vector3 _dir;
    private float _distance;
    private void Start()
    {
        _dir = (stop - start).normalized;
        transform.position = start;
        _distance = Vector3.Distance(start, stop)+0.2f;
    }


    private void Update()
    {
        if (Vector3.Distance(transform.position,start)+Vector3.Distance(transform.position,stop)> _distance)
        {
            _dir *= -1;
        }
        transform.Translate(_dir * Time.deltaTime * speed);
    }





}
