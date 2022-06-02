using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunTest : MonoBehaviour
{
    float countdown = 5f;
    [SerializeField] private Transform testTarget;
    [SerializeField] private HitOrMissManager manager;
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

            //manager.SpawnFireOnBoats(47);
            GameFeedbackEvents.instance.FireGuns(1, testTarget.position, 2);
            //GameFeedbackEvents.instance.SwitchToBirdsEye();
            countdown = 15f;
        }
    }
}
