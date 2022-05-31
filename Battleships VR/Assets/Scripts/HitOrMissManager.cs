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
    [SerializeField] private Player playerBoard;
    [SerializeField] private GameObject emptyMarker;
    private GameObject[] firePart;

    private void Start()
    {
        firePart = GameObject.FindGameObjectsWithTag("Fire");
        foreach (GameObject fire in firePart)
            fire.SetActive(false);
    }

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
                if(hit == true)
                    SpawnFireOnBoats(position);
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
            //SpawnFireOnBoats(position);
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
        Debug.Log("Trying to spawn fire " + positionFromCC);
        emptyMarker.transform.localPosition = new Vector3(row * 60, 0, col * 60);
        FindNearestFire().SetActive(true);
        //GameObject temp = Instantiate(fireParticles, new Vector3(0,0,0), Quaternion.identity, GameObject.Find("Full Board Parent").transform);
        //temp.transform.localPosition = new Vector3(row * 60, 10, col * 60);
        // Find the nearest object with tag / on layer for fire and enable it


    }

    private GameObject FindNearestFire()
    {
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
