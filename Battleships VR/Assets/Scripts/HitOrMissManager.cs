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
                UpdateResult("AIBoardInCC", position, hit);
                //UpdateResult("AIBoardInSea", position, hit);
                break;
            case "AI":
                //UpdateResult("PlayerBoarInCC", position, hit);
                UpdateResult("PlayerBoardInSea", position, hit);
                break;
        }
        
    }

    private void UpdateResult(string name, int position, bool hit)
    {
        GameObject.Find(name).transform.Find(position.ToString()).GetChild(hit ? 1 : 0).gameObject.SetActive(true);
    }
}
