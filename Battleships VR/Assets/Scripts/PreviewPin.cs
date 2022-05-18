using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreviewPin : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Hide()
    {
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(false);
        }
    }

    public void ChangePosition(Vector3 position)
    {
        transform.localPosition = position;
    }
}
