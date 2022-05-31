using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChessAnimator : MonoBehaviour
{
    private float moveTime = 1f;

    List<(string ignore, string whiteMove, string blackMove)> moves = new List<(string, string, string)>();

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private IEnumerator PlayTheGame()
    {
        foreach ((string ignore, string whiteMove, string blackMove) move in moves)
        {
            yield return new WaitForSeconds(moveTime);

            if (move.whiteMove == "0-0")
            {
                // Call the edge case for castle kings side
            }
            else if (move.whiteMove == "0-0-0")
            {
                // Call the edge case for castle queens side
            }
        }
    }

    private void MovePiece()
    {

    }

    private void CapturePiece()
    {

    }

    private void MoveImporter()
    {
        // Open the file
        // For each line in the file, split the line by spaces and assign each part to a new tuple in the list

    }
}
