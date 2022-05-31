using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MasterShip : MonoBehaviour
{
    [SerializeField] private GameObject[] ships;

    private void Start()
    {
        GameFeedbackEvents.instance.fireGuns += DetermineWhichShip;
        GameFeedbackEvents.instance.shipHasSunk += ShipHasSunk;
    }

    private void DetermineWhichShip(int id, Vector3 target, int amount)
    {
        Debug.Log("Calling ship to hit" + id.ToString());
        switch (id)
        {
            case 3:
            case 2:
            case 0:
                ShipController controller1 = ships[id].GetComponent<ShipController>();
                if (id == controller1.Id)
                    controller1.FireGuns(id, target, amount);

                break;
            case 4:
                ships[id].GetComponent<AircraftCarrier>().BeginPlane(target);
                break;
            case 1:
                ships[id].GetComponent<Submarine>().Submerge();
                break;
        }
    }

    private void ShipHasSunk(int id)
    {
        ships[id].GetComponent<BoatSunk>().SinkShip();
    }
}
