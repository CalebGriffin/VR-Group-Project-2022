using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipController : MonoBehaviour
{
    [SerializeField] private GameObject[] guns;

    [SerializeField] private int id;
    public int Id { get { return id; } }
    private int gunCount;

    //This method is called when the event "fireGuns" is executed
    public void FireGuns(Vector3 target, int amount)
    {
        gunCount = 0;
        foreach (GameObject gun in guns)
        {
            gun.GetComponent<GunBehaviour>().Fire(target, amount);
        }


    }
    public void FinishedFiring()
    {
        gunCount++;
        if(gunCount == guns.Length)
        {
            gVar.playerTurnOver = true;
            GameFeedbackEvents.instance.SwitchToBirdsEye();

            foreach(GameObject gun in guns)
            {
                float x = gun.transform.localEulerAngles.x;
                float z = gun.transform.localEulerAngles.z;
                gun.transform.localEulerAngles = new Vector3(x, 0, z);
            }
        }
    }


}
