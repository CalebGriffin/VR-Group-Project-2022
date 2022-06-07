using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CCAnimator : MonoBehaviour
{
    [SerializeField] private Transform hourHand, minuteHand;
    private const float degreesInHours = 360/12, degreesInMinutes = 360/60;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        UpdateClock();
    }

    void UpdateClock()
    {
        TimeSpan time = DateTime.Now.TimeOfDay;
        hourHand.localRotation = Quaternion.Euler(0, 0, (degreesInHours * time.Hours)+90);
        minuteHand.localRotation = Quaternion.Euler(0, 0, (degreesInMinutes * time.Minutes)+90);
    }
}
