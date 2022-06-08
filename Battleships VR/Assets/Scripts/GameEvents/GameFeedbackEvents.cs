using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameFeedbackEvents : MonoBehaviour
{
    #region Singleton
    public static GameFeedbackEvents instance;
    private void Awake()
    {
        if (instance == null)
            instance = this;
    }
    #endregion

    public Action<int, Vector3, int> fireGuns;
    public Action<Vector3, int> switchViewToShip;
    public Action switchToBirdsEye;
    public Action<int> shipHasSunk;
    public Action<Transform> switchToWaterView;
    public Action<Transform> switchToPlaneView;
    public void FireGuns(int id, Vector3 target, int amount = 1)
    {
        //IMPORTANT - id represents the index of the ship model in the drone camera list
        switchViewToShip.Invoke(target, id);

        if (amount == 0)
        {
            gVar.playerTurnOver = true;
            return;
        }

        fireGuns.Invoke(id, target, amount);
    }
    public void SwitchToBirdsEye()
    {
        switchToBirdsEye.Invoke();
    }

    public void ShipHasSunk(int id)
    {
        shipHasSunk.Invoke(id);
    }

    public void SwitchToWaterView(Transform target)
    {
        switchToWaterView.Invoke(target);
    }
    public void SwitchToPlaneView(Transform target)
    {
        switchToPlaneView.Invoke(target);
    }
}
