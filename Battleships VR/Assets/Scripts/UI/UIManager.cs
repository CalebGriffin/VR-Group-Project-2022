using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private TextAsset textFile;
    private void Start()
    {
        text.text = "";
        StartCoroutine(RevealLetters());
    }
    private IEnumerator RevealLetters()
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
