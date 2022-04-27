using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardInput : MonoBehaviour
{
    private int lowerX, lowerZ, upperX, upperZ;

    public Player player;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void ResetBounds()
    {
        lowerX = 0;
        lowerZ = 0;
        upperX = 9;
        upperZ = 9;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "ModelBoat")
        {
            int boatLength = other.gameObject.GetComponent<ModelBoat>().Length;

            string direction = other.gameObject.GetComponent<ModelBoat>().Direction;

            ResetBounds();

            switch(direction)
            {
                case "up":
                    lowerX = 0 + boatLength - 1;
                    break;
                
                case "down":
                    upperX = 9 - boatLength + 1;
                    break;
                
                case "left":
                    lowerZ = 0 + boatLength - 1;
                    break;
                
                case "right":
                    upperZ = 9 - boatLength + 1;
                    break;
            }
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "ModelBoat" && other.gameObject.GetComponent<ModelBoat>().Placed == false)
        {
            Vector3 previewPosition = FindRoundedPosition(other.transform.position, other.gameObject.GetComponent<ModelBoat>().Direction, other.gameObject.GetComponent<ModelBoat>().Length);
            if (previewPosition.x >= lowerX && previewPosition.x <= upperX && previewPosition.z >= lowerZ && previewPosition.z <= upperZ && previewPosition != new Vector3(100, 100, 100))
            {
                // Set the position of the preview boat to the preview position
                // Set the lock to point of the model boat to the position of the preview boat
            }
            
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "ModelBoat")
        {
            ResetBounds();
            other.gameObject.GetComponent<ModelBoat>().Placed = false;
            // Set the lock to point of the model boat to the original position
            // Hide the preview boat
        }
    }

    private Vector3 FindRoundedPosition(Vector3 position, string direction, int length)
    {
        int newX = RoundFloat(position.x);
        int newZ = RoundFloat(position.z);

        int boardPosition = int.Parse(newX.ToString() + (newZ + 1).ToString());
        int[] boatPositions = new int[length];
        boatPositions[0] = boardPosition;

        switch(direction)
        {
            case "up":
                for (int i = 1; i < length; i++)
                {
                    int newPosition = boardPosition - (i * 10);
                    boatPositions[i] = newPosition;
                }
                break;
            
            case "down":
                for (int i = 1; i < length; i++)
                {
                    int newPosition = boardPosition + (i * 10);
                    boatPositions[i] = newPosition;
                }
                break;
            
            case "left":
                for (int i = 1; i < length; i++)
                {
                    int newPosition = boardPosition - i;
                    boatPositions[i] = newPosition;
                }
                break;
            
            case "right":
                for (int i = 1; i < length; i++)
                {
                    int newPosition = boardPosition + i;
                    boatPositions[i] = newPosition;
                }
                break;
            
            default:
                break;
        }

        bool valid = true;
        foreach (int potentialPosition in boatPositions)
        {
            if (player.Board.currentBoatPositionsWithBorders.Contains(potentialPosition))
            {
                valid = false;
                break;
            }
        }

        if (valid)
        {
            return new Vector3(newX, 0, newZ);
        }
        else
        {
            return new Vector3(100, 100, 100);
        }
    }

    private int RoundFloat(float number)
    {
        int diff = (int)number % player.Board.Matrix.GetLength(0);

        number -= diff;

        if (diff > player.Board.Matrix.GetLength(0) / 2)
            number += player.Board.Matrix.GetLength(0);

        return (int)number;
    }
}
