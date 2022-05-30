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

    public Action<int, Vector3, bool, int> fireGuns;
    public Action<Vector3, int> switchViewToShip;
    public Action switchToBirdsEye;
    public void FireGuns(int id, Vector3 target, bool isGun, int amount = 1)
    {
        //IMPORTANT - id represents the index of the ship model in the drone camera list
        switchViewToShip.Invoke(target, id);

        if (amount == 0)
        {
            gVar.playerTurnOver = true;
            return;
        }

        fireGuns.Invoke(id, target, isGun, amount);
    }
    public void SwitchToBirdsEye()
    {
        switchToBirdsEye.Invoke();
    }
}
