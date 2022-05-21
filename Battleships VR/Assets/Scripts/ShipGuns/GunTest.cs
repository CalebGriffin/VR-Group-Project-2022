using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunTest : MonoBehaviour
{
    float countdown = 5f;
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
            GameFeedbackEvents.instance.FireGuns(4, testTarget.position);
            countdown = 1000f;
        }
    }
}
