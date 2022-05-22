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
    public void FireGuns(int id, Vector3 target, int amount = 1)
    {
        fireGuns.Invoke(id, target, amount);
    }

}
