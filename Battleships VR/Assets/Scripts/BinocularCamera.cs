using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BinocularCamera : MonoBehaviour
{
    private bool binocularsOn = true;

    public GameObject vrCameraObj;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = vrCameraObj.transform.position;
        transform.rotation = vrCameraObj.transform.rotation;
        
    }
}
