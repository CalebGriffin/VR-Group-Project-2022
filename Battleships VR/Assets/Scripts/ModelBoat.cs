using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModelBoat : MonoBehaviour
{
    [SerializeField] private int length;
    public int Length { get { return length; } }

    [SerializeField] private string direction;
    public string Direction { get { return direction; } }

    [SerializeField] private bool placed;
    public bool Placed { get { return placed; } set { placed = value; } }

    [SerializeField] public int[] positionsAround;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetDirection(string direction)
    {
        this.direction = direction;
    }

    public void Rotate()
    {
        switch(direction)
        {
            case "up":
                direction = "right";
                break;
            
            case "down":
                direction = "left";
                break;
            
            case "left":
                direction = "up";
                break;
            
            case "right":
                direction = "down";
                break;
            
            default:
                break;
        }
    }

    public void OnLetGo()
    {
        placed = true;

        // Call a method on the player to create a boat with the length and direction

    }
}
