using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private TextAsset menuFile;
    [SerializeField] private TextAsset winnerFile;
    [SerializeField] private TextAsset loserFile;

    #region Singleton
    public static UIManager instance;
    void Awake()
    {
        if (instance == null)
            instance = this;
    }
    #endregion

    private void Start()
    {
        text.text = "";
        StartCoroutine(RevealLetters(menuFile));
    }

    public void ResetMenuText()
    {
        text.text = "";
    }

    public void DisplayEndText(bool playerWon)
    {
        RevealLetters(playerWon ? winnerFile : loserFile);
    }

    private IEnumerator RevealLetters(TextAsset textFile)
    {
        string stringTextFile = textFile.ToString();
        for(int i = 0; i < stringTextFile.Length; i++)
        {
            text.text += stringTextFile[i];
            yield return new WaitForSeconds(0.08f);
            //if (stringTextFile[i] == '_')
            //Alpha(stringTextFile[i]);
        }
    }
    private void Alpha(char c)
    {
        //UNDER CONSTRUCTION
        //Debug.Log("Reached end");
        
    }
}
