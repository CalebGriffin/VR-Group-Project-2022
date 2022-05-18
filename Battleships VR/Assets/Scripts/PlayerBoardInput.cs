using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBoardInput : MonoBehaviour
{
    [SerializeField] private int lowerX, lowerZ, upperX, upperZ;

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

    private void AdjustBounds(GameObject modelBoat)
    {
        int boatLength = modelBoat.GetComponent<ModelBoat>().Length;

        string direction = modelBoat.GetComponent<ModelBoat>().direction;

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

    public void OnBoatHoverStay(GameObject modelBoat)
    {
        if (modelBoat.GetComponent<ModelBoat>().placed == false && modelBoat.GetComponent<ModelBoat>().hoveringOverTheBoard == true)
        {
            ResetBounds();
            AdjustBounds(modelBoat);

            Vector3 previewPosition = FindRoundedPosition(modelBoat.transform.localPosition, modelBoat.GetComponent<ModelBoat>().direction, modelBoat.GetComponent<ModelBoat>().Length);

            if (previewPosition.x >= lowerX && previewPosition.x <= upperX && previewPosition.z >= lowerZ && previewPosition.z <= upperZ && previewPosition != new Vector3(100, 100, 100))
            {
                // Set the position of the preview boat to the preview position
                previewBoat.ChangePosition(previewPosition, modelBoat.GetComponent<ModelBoat>().direction, modelBoat.GetComponent<ModelBoat>().BoatName);

                // Set the lock to point of the model boat to the position of the preview boat
                modelBoat.GetComponent<ModelBoat>().SetLockPoint(previewBoat.transform);
            }
            else
            {
                previewBoat.HideChildren();
                modelBoat.GetComponent<ModelBoat>().ResetLockPoint();
            }
        }
        else
        {
            previewBoat.HideChildren();
        }
    }

    public void OnBoatHoverEnter(GameObject modelBoat)
    {
        //Debug.Log("OnBoatHoverEnter with " + modelBoat.name);

        ResetBounds();
        AdjustBounds(modelBoat);
    }

    public void OnBoatHoverExit(GameObject modelBoat)
    {
        ResetBounds();
        modelBoat.GetComponent<ModelBoat>().placed = false;

        // Set the lock to point of the model boat to the original position
        modelBoat.GetComponent<ModelBoat>().ResetLockPoint();

        // Hide the preview boat
        previewBoat.HideChildren();
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
