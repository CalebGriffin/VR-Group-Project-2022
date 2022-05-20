using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModelBoatAnimator : MonoBehaviour
{
    public Material dissolveMat;
    public bool isAnimatingIn = false;
    public bool isAnimatingOut = false;

    public float animatingTime = 1f;
    private float elapsedTime = 0f;

    // Start is called before the first frame update
    void Start()
    {
        AnimateIn();
    }

    // Update is called once per frame
    void Update()
    {
        elapsedTime += Time.deltaTime;

        if (isAnimatingIn)
        {
            dissolveMat.SetFloat("_Cutoff", Mathf.Lerp(1, 0, elapsedTime/animatingTime));
            if (elapsedTime >= animatingTime)
            {
                elapsedTime = 0f;
                isAnimatingIn = false;
            }
        }

        if (isAnimatingOut)
        {
            dissolveMat.SetFloat("_Cutoff", Mathf.Lerp(0, 1, elapsedTime/animatingTime));
            if (elapsedTime >= animatingTime)
            {
                elapsedTime = 0f;
                isAnimatingOut = false;
            }
        }

    }

    private void AnimateIn()
    {
        isAnimatingIn = true;
    }

    private void AnimateOut()
    {
        isAnimatingOut = true;
    }
}
