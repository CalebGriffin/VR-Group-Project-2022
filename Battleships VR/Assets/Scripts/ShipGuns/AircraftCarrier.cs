using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AircraftCarrier : MonoBehaviour
{
    [SerializeField] private GameObject plane;

    public void BeginPlane(Vector3 target)
    {
        plane.GetComponent<Plane>().StartPlaneMovement(target);
    }
    
}
