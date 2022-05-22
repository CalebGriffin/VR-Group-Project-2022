using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipController : MonoBehaviour
{
    public GameObject[] guns;
    [SerializeField] private int id;
    // Start is called before the first frame update
    void Start()
    {
        GameFeedbackEvents.instance.fireGuns += FireGuns;
    }

    //This method is called when the event "fireGuns" is executed
    public void FireGuns(int id, Vector3 target, int amount)
    {
        //Check the id passed in so only the correct ship guns are called
        if(id == this.id)
        {
            foreach (GameObject gun in guns)
            {
                gun.GetComponent<GunBehaviour>().Fire(target, amount);
            }

        }
    }


}
