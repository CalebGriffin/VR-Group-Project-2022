using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreviewBoat : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }

    public void Hide()
    {

        foreach(Transform child in transform)
        {
            Debug.Log("Hiding preview boat");
            Debug.Assert(child.gameObject.name == null, child.gameObject.name);
            child.gameObject.SetActive(false);
        }
    }

    public void ChangePosition(Vector3 position, string direction, string name)
    {
        transform.localPosition = position;

        transform.Find(name).gameObject.SetActive(true);
        Debug.Log(name);
        Debug.Log(transform.Find(name).gameObject.name);

        switch(direction)
        {
            case "up":
                transform.localRotation = Quaternion.Euler(0, 0, 0);
                break;
            
            case "down":
                transform.localRotation = Quaternion.Euler(0, 180, 0);
                break;
            
            case "left":
                transform.localRotation = Quaternion.Euler(0, -90, 0);
                break;
            
            case "right":
                transform.localRotation = Quaternion.Euler(0, 90, 0);
                break;
            
            default:
                break;
        }
    }
}
