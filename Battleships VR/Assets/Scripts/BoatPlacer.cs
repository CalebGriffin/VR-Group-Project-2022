using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoatPlacer : MonoBehaviour
{
    public void PlaceBoat(string name, Transform modelBoatTransform)
    {
        transform.Find(name).transform.localRotation = modelBoatTransform.localRotation;
        transform.Find(name).transform.localPosition = new Vector3(modelBoatTransform.localPosition.x * 60, 0, modelBoatTransform.localPosition.z * 60);
    }
}
