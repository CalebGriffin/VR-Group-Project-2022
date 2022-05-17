using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;
using Valve.VR.InteractionSystem.Sample;

public class ModelBoatParent : MonoBehaviour
{
    public void RefreshBoatsOnBoard()
    {
        foreach (Transform child in transform)
        {
            if (child.GetComponent<ModelBoat>().Placed == true)
                child.GetComponent<ModelBoat>().RefreshBoardPosition();
        }
    }

    public void PlaceBoat(string name, int position, int rotation, Boat boat, Player player)
    {
        Transform modelBoatTransform = transform.Find($"Model Boat ({name})");
        GameObject modelBoat = modelBoatTransform.gameObject;
        Debug.Log($"Model Boat ({name})");

        modelBoat.GetComponent<LockToPoint>().enabled = false;
        modelBoat.GetComponent<Interactable>().enabled = false;
        modelBoat.GetComponent<Throwable>().enabled = false;
        modelBoat.GetComponent<ModelBoat>().enabled = false;

        int row = (position - 1) / 10;
        int column = (position - 1) % 10;

        modelBoat.GetComponent<ModelBoat>().dynamicPosition.localPosition = new Vector3(row, 1, column);
        modelBoat.GetComponent<ModelBoat>().dynamicPosition.localRotation = Quaternion.Euler(0, rotation, 0);
        modelBoat.GetComponent<LockToPoint>().snapTo = modelBoat.GetComponent<ModelBoat>().dynamicPosition;
        
        modelBoatTransform.localPosition = new Vector3(row, 1, column);
        modelBoatTransform.localRotation = Quaternion.Euler(0, rotation, 0);

        modelBoat.GetComponent<ModelBoat>().Placed = true;
        modelBoat.GetComponent<ModelBoat>().hoveringOverTheBoard = false;

        string direction = null;
        switch (rotation)
        {
            case 0:
                direction = "up";
                break;

            case 90:
                direction = "right";
                break;
            
            case 180:
                direction = "down";
                break;
            
            case -90:
                direction = "left";
                break;
            
            default:
                break;
        }
        modelBoat.GetComponent<ModelBoat>().direction = direction;

        modelBoat.GetComponent<ModelBoat>().positions = boat.Positions;
        modelBoat.GetComponent<ModelBoat>().positionsAround = boat.Sunk(player.Board);

        modelBoat.GetComponent<LockToPoint>().enabled = true;
        modelBoat.GetComponent<Interactable>().enabled = true;
        modelBoat.GetComponent<Throwable>().enabled = true;
        modelBoat.GetComponent<ModelBoat>().enabled = true;
    }

    public void ResetAllBoatPositions()
    {
        foreach (Transform child in transform)
        {
            GameObject modelBoat = child.gameObject;

            modelBoat.GetComponent<LockToPoint>().enabled = false;
            modelBoat.GetComponent<Interactable>().enabled = false;
            modelBoat.GetComponent<Throwable>().enabled = false;
            modelBoat.GetComponent<ModelBoat>().enabled = false;

            modelBoat.GetComponent<ModelBoat>().dynamicPosition.localPosition = new Vector3(0, 1, 0);
            modelBoat.GetComponent<ModelBoat>().dynamicPosition.localRotation = Quaternion.Euler(0, 0, 0);
            child.localPosition = modelBoat.GetComponent<ModelBoat>().originalPosition.localPosition;
            child.localRotation = modelBoat.GetComponent<ModelBoat>().originalPosition.localRotation;
            modelBoat.GetComponent<ModelBoat>().Placed = false;
            modelBoat.GetComponent<ModelBoat>().hoveringOverTheBoard = false;
            modelBoat.GetComponent<ModelBoat>().direction = "up";
            modelBoat.GetComponent<ModelBoat>().positions = null;
            modelBoat.GetComponent<ModelBoat>().positionsAround = null;
            modelBoat.GetComponent<LockToPoint>().snapTo = modelBoat.GetComponent<ModelBoat>().originalPosition;

            modelBoat.GetComponent<LockToPoint>().enabled = true;
            modelBoat.GetComponent<Interactable>().enabled = true;
            modelBoat.GetComponent<Throwable>().enabled = true;
            modelBoat.GetComponent<ModelBoat>().enabled = true;
        }
    }
}
