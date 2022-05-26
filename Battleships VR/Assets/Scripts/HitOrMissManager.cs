using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitOrMissManager : MonoBehaviour
{
    #region Singleton
    public static HitOrMissManager instance;
    private void Awake()
    {
        if (instance == null)
            instance = this;
    }
    #endregion

    public void ResultOfAttack(string name, int position, bool hit)
    {
        //Find the opposite of the board passed in
        switch (name)
        {
            case "Player":
                StartCoroutine(Wait(name, position, hit));
                //UpdateResult("AIBoardInSea", position, hit);
                break;
            case "AI":
                //UpdateResult("PlayerBoarInCC", position, hit);
                UpdateResult("PlayerBoardInSea", position, hit);
                gVar.playerTurn = true;
                break;
        }
        
    }

    private IEnumerator Wait(string name, int position, bool hit)
    {
        while (gVar.playerTurnOver == false)
        {
            yield return null;
        }

        yield return new WaitForSeconds(0.1f);

        UpdateResult("AIBoardInCC", position, hit);
        AI.instance.Decision();
    }
    

    public void SpawnFireOnBoats(int positionFromCC)
    {
        //Assign a number to the fire which is the same as the grid position
        //Get the position of the AI's attack
        //Translate that into the players board on sea position
        //Activate the fire particles in that position


    }


    private void UpdateResult(string name, int position, bool hit)
    {
        GameObject.Find(name).transform.Find(position.ToString()).GetChild(hit ? 1 : 0).gameObject.SetActive(true);
    }
}
