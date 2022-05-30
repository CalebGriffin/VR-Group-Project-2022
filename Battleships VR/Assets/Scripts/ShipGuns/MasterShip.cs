using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MasterShip : MonoBehaviour
{
    [SerializeField] private GameObject[] ships;

    private void Start()
    {
        GameFeedbackEvents.instance.fireGuns += DetermineWhichShip;
    }

    private void DetermineWhichShip(int id, Vector3 target, bool isGun, int amount)
    {
        switch (id)
        {
            case 3:
            case 2:
            case 0:
                ShipController controller1 = ships[id].GetComponent<ShipController>();
                if (id == controller1.Id)
                    controller1.FireGuns(id, target, isGun, amount);

                break;
            case 4:
                break;
            case 1:
                ships[id].GetComponent<Submarine>().Submerge();
                break;
        }
    }
}
