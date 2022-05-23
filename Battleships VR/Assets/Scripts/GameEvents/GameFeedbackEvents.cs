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
    public void FireGuns(int id, Vector3 target, int amount = 1)
    {
        //IMPORTANT - id represents the index of the ship model in the drone camera list
        switchViewToShip.Invoke(target, id);
        fireGuns.Invoke(id, target, amount);
    }

}
