using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOver : MonoBehaviour
{
    #region Singleton
    public static GameOver instance;
    void Awake()
    {
        if (instance == null)
            instance = this;
    }
    #endregion

    public FireworkSpawner fireworkSpawner;
    public DeskAnimator deskAnimator;

    public void Winner(string winner)
    {
        bool playerWon = (winner == "Player");
        if (playerWon)
            fireworkSpawner.StartSpawning();
        deskAnimator.TransitionToEnd(playerWon);
    }
}
