using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipController : MonoBehaviour
{
    [SerializeField] private GameObject[] guns;
    [SerializeField] private int id;
    // Start is called before the first frame update
    void Start()
    {
        GameFeedbackEvents.instance.fireGuns += FireGuns;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void FireGuns(int id, Vector3 target)
    {
        if(id == this.id)
        {
            foreach(GameObject gun in guns)
            {
                gun.GetComponent<GunBehaviour>().Fire(target);
            }
        }
    }
}
