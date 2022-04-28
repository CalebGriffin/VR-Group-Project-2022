using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardInput : MonoBehaviour
{
    private int lowerX, lowerZ, upperX, upperZ;

    public Player player;

    public PreviewBoat previewBoat;
    
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
        Debug.Log("Collision with " + other.gameObject.name);

        GameObject modelBoat;
        if (other.gameObject.transform.parent != null)
        {
            modelBoat = other.gameObject.transform.parent.gameObject;
        }
        else
        {
            modelBoat = other.gameObject;
        }

        if (modelBoat.tag == "ModelBoat")
        {
            int boatLength = modelBoat.GetComponent<ModelBoat>().Length;

            string direction = modelBoat.GetComponent<ModelBoat>().Direction;

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
        Debug.Log("OnTriggerStay with " + other.gameObject.name);

        GameObject modelBoat;
        if (other.gameObject.transform.parent != null)
        {
            modelBoat = other.gameObject.transform.parent.gameObject;
        }
        else
        {
            modelBoat = other.gameObject;
        }

        if (modelBoat.tag == "ModelBoat" && modelBoat.GetComponent<ModelBoat>().Placed == false)
        {
            Vector3 previewPosition = FindRoundedPosition(this.gameObject.transform.InverseTransformPoint(modelBoat.transform.position), modelBoat.GetComponent<ModelBoat>().Direction, modelBoat.GetComponent<ModelBoat>().Length);
            Debug.Log("Preview position: " + previewPosition);
            if (previewPosition.x >= lowerX && previewPosition.x <= upperX && previewPosition.z >= lowerZ && previewPosition.z <= upperZ && previewPosition != new Vector3(100, 100, 100))
            {
                // Set the position of the preview boat to the preview position
                previewBoat.Show();
                previewBoat.ChangePosition(previewPosition, modelBoat.GetComponent<ModelBoat>().Direction, modelBoat.GetComponent<ModelBoat>().Name);

                // Set the lock to point of the model boat to the position of the preview boat
                modelBoat.GetComponent<ModelBoat>().SetLockPoint(previewBoat.transform);
            }
            
        }
    }

    void OnTriggerExit(Collider other)
    {
        GameObject modelBoat;
        if (other.gameObject.transform.parent != null)
        {
            modelBoat = other.gameObject.transform.parent.gameObject;
        }
        else
        {
            modelBoat = other.gameObject;
        }

        if (modelBoat.tag == "ModelBoat")
        {
            ResetBounds();
            modelBoat.GetComponent<ModelBoat>().Placed = false;

            // Set the lock to point of the model boat to the original position
            modelBoat.GetComponent<ModelBoat>().ResetLockPoint();

            // Hide the preview boat
            previewBoat.Hide();
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
            return new Vector3(newX, 1, newZ);
        }
        else
        {
            return new Vector3(100, 100, 100);
        }
    }

    private int RoundFloat(float number)
    {
        int diff = (int)number % 1;

        number -= diff;

        if (diff > 1 / 2)
            number += 1;

        return (int)number;
    }
}
