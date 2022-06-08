using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyDuplicate : MonoBehaviour
{
    //[SerializeField] private GameObject lightPrefab;
    public static DestroyDuplicate instance;

    // Start is called before the first frame update
    void Start()
    {
        if (instance == null)
        {
            instance = this;
            //Instantiate(lightPrefab, Vector3.zero, Quaternion.identity, this.gameObject.transform);
        }
        else
            Destroy(this.gameObject);
    }
}
