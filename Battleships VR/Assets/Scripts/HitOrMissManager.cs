using System.Linq;
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
    [SerializeField] private GameObject waterSplash;
    [SerializeField] private Player playerBoard;
    [SerializeField] private GameObject emptyMarker;
    [SerializeField] private GameObject seaBoard;

    private GameObject[] firePart;

    private void Start()
    {
        //Find all the fire particles at the beginning of the game so they can be found after they have all been set inactive 
        firePart = GameObject.FindGameObjectsWithTag("Fire");
        foreach (GameObject fire in firePart)
            fire.SetActive(false);
    }

    public void ResultOfAttack(string name, int position, bool hit)
    {
        //Call the correct part of this script depending on who is using this script to return the value of their shot
        switch (name)
        {
            case "Player":
                StartCoroutine(Wait(name, position, hit, false));
                break;
            case "AI":
                UpdateResult("PlayerBoardInSea", position, hit);

                if (hit == true)
                    SpawnFireOnBoats(position);
                else
                    SpawnSplash(position);

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

        if (!sunk)
        {
            TurnClockAnimator.instance.AnimateTo("AI");
            AI.instance.StartCoroutine("WaitToDecide");
        }
    }
    

    public void SpawnFireOnBoats(int positionFromCC)
    {
        //Get the board position from the players board so we can correctly get the board positions on the sea
        int row = (positionFromCC - 1) / playerBoard.Board.Matrix.GetLength(0); 
        int col = (positionFromCC - 1) % playerBoard.Board.Matrix.GetLength(0);

        //Moving an empty object to the position where the AI fired on the players board because it is easier to debug than just using numbers and calculations
        emptyMarker.transform.localPosition = new Vector3(row * 60, 0, col * 60);
        FindNearestFire().SetActive(true);
    }

    private GameObject FindNearestFire()
    {
        //The starting number is set really high to ensure the check will always run against each fire particle in the array
        float closest = 10000f;
        GameObject nearestFire = null;

        foreach(GameObject fire in firePart)
        {
            float distance = (fire.transform.position - emptyMarker.transform.position).magnitude;

            if(distance < closest)
            {
                closest = distance;
                nearestFire = fire;
            }
        }
        return nearestFire;
    }

    private void SpawnSplash(int positionFromCC)
    {
        int row = (positionFromCC - 1) / playerBoard.Board.Matrix.GetLength(0);
        int col = (positionFromCC - 1) % playerBoard.Board.Matrix.GetLength(0);
        GameObject temp = Instantiate(waterSplash, new Vector3(0, 0, 0), Quaternion.identity, seaBoard.transform);
        temp.transform.localPosition = new Vector3(row * 60, 0, col * 60);
        Debug.Log("Spawned some water");
        Destroy(temp, 2f);
    }

    private void UpdateResult(string name, int position, bool hit)
    {
        try
        {
            //Find the gameobject with the correct parent name and child name (chilren are named after the position they ocupy - 1, 2 etc.)
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
