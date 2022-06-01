using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Submarine : MonoBehaviour
{
    [SerializeField] private GameObject[] fireParticles;
    [SerializeField] private AnimationCurve curve;
    [SerializeField] private float submergeDistance = -10f;
    private List<GameObject> revealedFire = new List<GameObject>();
    public void Submerge()
    {
        foreach (GameObject fire in fireParticles)
            if (fire.gameObject.activeSelf == true)
            {
                revealedFire.Add(fire);
                fire.SetActive(false);
            }

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
            foreach (GameObject fire in revealedFire)
                fire.SetActive(true);
            revealedFire.Clear();
            gVar.playerTurnOver = true;
            GameFeedbackEvents.instance.SwitchToBirdsEye();
        });
    }
}
