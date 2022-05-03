using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem.Sample;

public class ModelBoat : MonoBehaviour
{
    [SerializeField] private int length;
    public int Length { get { return length; } }

    [SerializeField] private string direction;
    public string Direction { get { return direction; } }

    [SerializeField] private bool placed;
    public bool Placed { get { return placed; } set { placed = value; } }

    [SerializeField] private string boatName;
    public string Name { get { return boatName; } }

    [SerializeField] public int[] positions;
    [SerializeField] public int[] positionsAround;

    [SerializeField] public Player player;

    [SerializeField] private Transform originalPosition;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public void SetLockPoint(Transform previewBoatPosition)
    {
        this.GetComponent<LockToPoint>().snapTo = previewBoatPosition;
    }

    public void ResetLockPoint()
    {
        this.GetComponent<LockToPoint>().snapTo = originalPosition;
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

    public void OnPickUp()
    {
        placed = false;

        player.RemoveShip(this);
    }

    public void OnLetGo()
    {
        Debug.Log("Let go");

        placed = true;

        // Call a method on the player to create a boat with the length and direction
        player.AddShip(this);
    }
}
