using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PinHolder : MonoBehaviour
{
    public GameObject pinObj;
    public Transform originalPosition;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SpawnPin()
    {
        //Instantiate(pinPrefab, originalPosition.position, Quaternion.identity, this.transform);
        pinObj.SetActive(false);
        pinObj.transform.localPosition = originalPosition.localPosition;
        pinObj.GetComponent<Pin>().Reset();
        pinObj.SetActive(true);
    }
}
