using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunTest : MonoBehaviour
{
    float countdown = 10f;
    [SerializeField] private Transform testTarget;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        countdown -= Time.deltaTime;

        if(countdown <= 0f)
        {
            Debug.Log("Running down");
            GameFeedbackEvents.instance.FireGuns(1, testTarget.position, false, 2);
            //GameFeedbackEvents.instance.SwitchToBirdsEye();
            countdown = 20f;
        }
    }
}
