using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TextSetUP : MonoBehaviour
{
    public AI ai;
    public GameObject textPrefab;

    public GameObject textParent;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    [ContextMenu(nameof(CreateText))]
    public void CreateText()
    {
        foreach (int i in ai.Board.Matrix)
        {
            int row = (i - 1) / ai.Board.Matrix.GetLength(0);
            int col = (i - 1) % ai.Board.Matrix.GetLength(0);
            GameObject text = Instantiate(textPrefab, textParent.transform);
            text.name = i.ToString();
            text.transform.localPosition = new Vector3(row, 2, col + 11);
            text.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = i.ToString();
        }
    }
}
