using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoatSunk : MonoBehaviour
{
    [SerializeField] private AnimationCurve curve;
    [SerializeField] private float submergeDistance = -500f;
    public void SinkShip()
    {
        LeanTween.moveLocalY(gameObject, submergeDistance, 10f).setEase(curve);
    }
}
