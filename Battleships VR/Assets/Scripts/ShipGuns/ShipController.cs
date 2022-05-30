using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipController : MonoBehaviour
{
    [SerializeField] private GameObject[] guns;
    List<Tuple<GameObject, int>> fireParticleList = new List<Tuple<GameObject, int>>();

    [SerializeField] private int id;
    public int Id { get { return id; } }
    private int gunCount;
    // Start is called before the first frame update
    void Start()
    {
        //GameFeedbackEvents.instance.fireGuns += DetermineWhichShip;
    }

    private void DetermineWhichShip(int id, Vector3 target, bool isGun, int amount)
    {
        switch (id)
        {
            case 3:
            case 2:
            case 0:
                FireGuns(id, target, isGun, amount);
                break;
            case 4:
                break;
            case 1:
                break;
        }
    }

    public void FindParticle(string shipName, int position)
    {
        //Need to check 
    }

    private void PlayAttackAnimation(int id, Vector3 target)
    {

    }

    //This method is called when the event "fireGuns" is executed
    public void FireGuns(int id, Vector3 target, bool isGun, int amount)
    {
        gunCount = 0;
        foreach (GameObject gun in guns)
        {
            gun.GetComponent<GunBehaviour>().Fire(target, amount);
        }


        //Check the id passed in so only the correct ship guns are called
        if (id == this.id)
        {
            if (isGun)
            {

            }
            else
            {
                //Play other animations
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
