using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoatSunk : MonoBehaviour
{
    [SerializeField] private AnimationCurve curve;
    [SerializeField] private float submergeDistance = -500f;
    [SerializeField] private GameObject[] shipParts;
    public void SinkShip()
    {
        foreach(GameObject obj in shipParts)
        {
            //Move the all the ship parts along with the model when it is sinking
            obj.transform.parent = gameObject.transform;
            StartCoroutine(TurnOffFire(obj));
        }
        LeanTween.moveLocalY(gameObject, submergeDistance, 10f).setEase(curve);
    }
    private IEnumerator TurnOffFire(GameObject obj)
    {
        //Come on, fire can't burn underwater :D
        yield return new WaitForSeconds(0.5f);
        obj.SetActive(false);
    }
}
