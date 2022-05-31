using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Submarine : MonoBehaviour
{
    [SerializeField] private AnimationCurve curve;
    [SerializeField] private float submergeDistance = -10f;
    public void Submerge()
    {
        //The custom curve is used to make the ship start submerging slowly and then get faster
        //This way the time it takes for the animation can stay the same without having the ship submerge too fast
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
