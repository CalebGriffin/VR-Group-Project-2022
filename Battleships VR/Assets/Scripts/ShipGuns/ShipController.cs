using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipController : MonoBehaviour
{
    [SerializeField] private GameObject[] guns;
    [SerializeField] private int id;
    private int gunCount;
    // Start is called before the first frame update
    void Start()
    {
        GameFeedbackEvents.instance.fireGuns += FireGuns;
    }

    private void Update()
    {
        float y = Mathf.Sin(Time.time) * 3f;
        //transform.position = new Vector3(transform.position.x, transform.position.y + y, transform.position.z);
    }

    //This method is called when the event "fireGuns" is executed
    private void FireGuns(int id, Vector3 target, int amount)
    {
        //Check the id passed in so only the correct ship guns are called
        if(id == this.id)
        {
            gunCount = 0;
            foreach (GameObject gun in guns)
            {
                gun.GetComponent<GunBehaviour>().Fire(target, amount);
            }

        }
    }
    public void FinishedFiring()
    {
        gunCount++;
        if(gunCount == guns.Length)
        {
            gVar.playerTurnOver = true;
            GameFeedbackEvents.instance.SwitchToBirdsEye();
        }
    }


}
