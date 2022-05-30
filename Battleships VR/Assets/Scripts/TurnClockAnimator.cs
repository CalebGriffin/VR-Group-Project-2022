using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnClockAnimator : MonoBehaviour
{
    #region Singleton
    public static TurnClockAnimator instance;
    void Awake()
    {
        if (instance == null)
            instance = this;
    }
    #endregion

    public Animator clockAnimator;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AnimateTo(string name)
    {
        string animation = "";

        switch (name)
        {
            case "Player":
                animation = "Their-Your";
                break;
            case "AI":
                animation = "Your-Their";
                break;
        }

        clockAnimator.Play(animation);
    }
}