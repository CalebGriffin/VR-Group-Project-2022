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
    [SerializeField] private GameObject fireParticles;
    [SerializeField] private Player playerBoard; 

    public void ResultOfAttack(string name, int position, bool hit)
    {
        //Find the opposite of the board passed in
        switch (name)
        {
            case "Player":
                StartCoroutine(Wait(name, position, hit, false));
                //UpdateResult("AIBoardInSea", position, hit);
                break;
            case "AI":
                //UpdateResult("PlayerBoarInCC", position, hit);
                UpdateResult("PlayerBoardInSea", position, hit);
                TurnClockAnimator.instance.AnimateTo("Player");
                gVar.playerTurn = true;
                break;
        }
        
    }

    public void ResultOfAttack(string name, int position, bool hit, bool sunk)
    {
        switch (name)
        {
            case "Player":
                StartCoroutine(Wait(name, position, hit, true));
                break;
            case "AI":
                UpdateResult("PlayerBoardInSea", position, hit);
                break;
        }
    }

    private IEnumerator Wait(string name, int position, bool hit, bool sunk)
    {
        while (gVar.playerTurnOver == false)
        {
            yield return null;
        }

        yield return new WaitForSeconds(0.1f);

        UpdateResult("AIBoardInCC", position, hit);
        if(hit == true)
        {
            SpawnFireOnBoats(position);
        }

        if (!sunk)
        {
            TurnClockAnimator.instance.AnimateTo("AI");
            AI.instance.StartCoroutine("WaitToDecide");
        }
    }
    

    public void SpawnFireOnBoats(int positionFromCC)
    {
        //Also needs the name of the ship as an overload up in the result of attack method
        //Convert the position passed from the position on the player board to the sea board
        int row = (positionFromCC - 1) / playerBoard.Board.Matrix.GetLength(0); 
        int col = (positionFromCC - 1) % playerBoard.Board.Matrix.GetLength(0);
        Vector3 positionToSpawn = new Vector3(row * 60, -50, col * 60);
        Instantiate(fireParticles, positionToSpawn, Quaternion.identity);

    }


    private void UpdateResult(string name, int position, bool hit)
    {
        try
        {
            GameObject.Find(name).transform.Find(position.ToString()).GetChild(hit ? 1 : 0).gameObject.SetActive(true);
        }
        catch (NullReferenceException)
        {
            Debug.LogError("The position wasn't found on the board!");
        }
        //if(name == "PlayerBoardInSea")
        //{
            //GameObject.Find(name).transform.Find(position.ToString()).GetChild(0).gameObject.SetActive(true);
        //}
    }
}
