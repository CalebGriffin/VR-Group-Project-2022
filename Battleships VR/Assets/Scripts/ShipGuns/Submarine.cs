using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Submarine : MonoBehaviour
{
    [SerializeField] private AnimationCurve curve;
    [SerializeField] private float submergeDistance = -10f;
    private Vector3 startingPos;
    public void Submerge()
    {
        startingPos = transform.position;
        LeanTween.moveLocalY(gameObject, submergeDistance, 5f).setEase(curve).
            setOnComplete(Reimerge);
    }

    private void Reimerge()
    {
        Debug.Log("Coming up");
        LeanTween.moveLocalY(gameObject, 0, 5f).setOnComplete(() =>
        {
            gVar.playerTurnOver = true;
            GameFeedbackEvents.instance.SwitchToBirdsEye();
        });
    }
}
