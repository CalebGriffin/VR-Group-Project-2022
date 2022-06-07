using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClockTimeCopy : MonoBehaviour
{
    [SerializeField]
    private Transform originalHourHand, originalMinuteHand, hourHand, minuteHand;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        hourHand.localRotation = originalHourHand.localRotation;
        minuteHand.localRotation = originalMinuteHand.localRotation;
    }
}
