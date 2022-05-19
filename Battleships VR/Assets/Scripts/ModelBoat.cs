using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;
using Valve.VR.InteractionSystem.Sample;

public class ModelBoat : MonoBehaviour
{
    [SerializeField] private int length;
    public int Length { get { return length; } }

    [SerializeField] public string direction;

    [SerializeField] public bool placed;

    [SerializeField] private string boatName;
    public string BoatName { get { return boatName; } }

    [SerializeField] public int[] positions = null;
    [SerializeField] public int[] positionsAround = null;

    [SerializeField] private float handRotationY;

    [SerializeField] public Player player;

    [SerializeField] public Transform originalPosition;
    [SerializeField] public Transform dynamicPosition;

    public GameObject boardParent;
    public GameObject modelBoatParent;

    private RaycastHit hit;
    private float raycastDistance = 1f;
    public bool hoveringOverTheBoard = false;

    // Start is called before the first frame update
    void Start()
    {
        // This is just a comment to force the project to compile
        
    }

    // Update is called once per frame
    void Update()
    {
    }

    void FixedUpdate()
    {
        if (!placed)
        {
            SetDirection();
            FireRaycast();
        }
    }

    public void FireRaycast()
    {
        Debug.DrawRay(transform.position, Vector3.down * raycastDistance, Color.red);
        if (Physics.Raycast(transform.position, Vector3.down, out hit, raycastDistance) && hit.collider.gameObject.tag == "Player Board")
        {
            if (hoveringOverTheBoard == false)
            {
                hoveringOverTheBoard = true;
                boardParent.GetComponent<PlayerBoardInput>().OnBoatHoverEnter(this.gameObject);
            }
            else
            {
                boardParent.GetComponent<PlayerBoardInput>().OnBoatHoverStay(this.gameObject);
            }
        }
        else if (hoveringOverTheBoard == true)
        {
            hoveringOverTheBoard = false;
            boardParent.GetComponent<PlayerBoardInput>().OnBoatHoverExit(this.gameObject);
        }
    }
    
    public void SetLockPoint(Transform previewBoatTransform)
    {
        dynamicPosition.position = previewBoatTransform.position;
        dynamicPosition.rotation = previewBoatTransform.rotation;
        this.GetComponent<LockToPoint>().snapTo = dynamicPosition;
    }

    public void ResetLockPoint()
    {
        this.GetComponent<LockToPoint>().snapTo = originalPosition;
    }

    private void SetDirection()
    {
        switch (transform.localEulerAngles.y)
        {
            case float x when x > -45 && x < 45:
                direction = "up";
                break;
            
            case float x when x > 45 && x < 135:
                direction = "right";
                break;
            
            case float x when x > 135 && x < 225:
                direction = "down";
                break;
            
            case float x when x > 225 && x < 315:
                direction = "left";
                break;
            
            default:
                break;
        }

        boardParent.SendMessage("AdjustBounds", this.gameObject);
    }

    public void RefreshBoardPosition()
    {
        player.RemoveShip(this);
        player.AddShip(this);
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

        if (positions != null)
        {
            player.RemoveShip(this);
        }

        modelBoatParent.GetComponent<ModelBoatParent>().RefreshBoatsOnBoard();

        positions = null;
        positionsAround = null;
    }

    public void OnLetGo()
    {
        if (!hoveringOverTheBoard)
        {
            return;
        }

        if (GetComponent<LockToPoint>().snapTo != dynamicPosition)
        {
            return;
        }

        Debug.Log("Let go");

        placed = true;
        hoveringOverTheBoard = false;

        boardParent.SendMessage("ResetBounds");

        // Set the positions of the boat based on the location of the model boat gameobject
        int startingPosition = player.Board.Matrix[(int)dynamicPosition.localPosition.x, (int)dynamicPosition.localPosition.z];
        positions = new int[length];
        positions[0] = startingPosition;
        switch(direction)
        {
            case "up":
                for (int i = 1; i < length; i++)
                {
                    int newPosition = startingPosition - (i * 10);
                    positions[i] = newPosition;
                }
                break;
            
            case "down":
                for (int i = 1; i < length; i++)
                {
                    int newPosition = startingPosition + (i * 10);
                    positions[i] = newPosition;
                }
                break;
            
            case "left":
                for (int i = 1; i < length; i++)
                {
                    int newPosition = startingPosition - i;
                    positions[i] = newPosition;
                }
                break;
            
            case "right":
                for (int i = 1; i < length; i++)
                {
                    int newPosition = startingPosition + i;
                    positions[i] = newPosition;
                }
                break;
            
            default:
                break;
        }

        // Call a method on the player to create a boat with the length and direction
        player.AddShip(this);
    }
}
