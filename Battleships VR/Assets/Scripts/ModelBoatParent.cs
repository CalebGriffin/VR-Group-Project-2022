using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
}
