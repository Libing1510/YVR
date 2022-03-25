using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Box : MonoBehaviour
{
    // Start is called before the first frame update
    public Bounds bounds;
    void Start()
    {
        BoxCollider boxCollider = GetComponent<BoxCollider>();
        bounds = boxCollider.bounds;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
