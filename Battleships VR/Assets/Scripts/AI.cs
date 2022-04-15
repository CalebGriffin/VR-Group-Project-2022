using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI : MonoBehaviour
{
    public GameObject shipCube;
    public GameObject borderCube;

    private Board board = new Board(10);

    private List<Boat> boats = new List<Boat>();

    private List<int> positionGuesses = new List<int>();
    
    // Start is called before the first frame update
    [ContextMenu("Start")]
    void Start()
    {
        string positionGuessesString = "";
        foreach(int i in board.Matrix)
        {
            positionGuesses.Add(i);
            positionGuessesString += i + ", ";
        }
        Debug.Log(positionGuessesString);

        board.currentBoatPositions.Clear();
        board.currentBoatPositionsWithBorders.Clear();
        
        boats.Clear();
        boats.Add(new Boat(board, "Carrier", 5));
        boats.Add(new Boat(board, "Battleship", 4));
        boats.Add(new Boat(board, "Cruiser", 3));
        boats.Add(new Boat(board, "Submarine", 3));
        boats.Add(new Boat(board, "Destroyer", 2));

        string boatPositions = "";
        foreach (Boat boat in boats)
        {
            Debug.Log(boat.Name + ":");
            foreach (int position in boat.Positions)
            {
                boatPositions += position + ", ";
                int row = (position - 1) / board.Matrix.GetLength(0);
                int col = (position - 1) % board.Matrix.GetLength(0);
                GameObject.Instantiate(shipCube, new Vector3(row, 0, col), Quaternion.identity);
            }
            Debug.Log(boatPositions);
            boatPositions = "";
        }

        string boatPositionsWithBorders = "";
        foreach (int position in board.currentBoatPositionsWithBorders)
        {
            boatPositionsWithBorders += position + ", ";
            if (!board.currentBoatPositions.Contains(position))
            {
                int row = (position - 1) / board.Matrix.GetLength(0);
                int col = (position - 1) % board.Matrix.GetLength(0);
                GameObject.Instantiate(borderCube, new Vector3(row, 0, col), Quaternion.identity);
            }
        }
        Debug.Log(boatPositionsWithBorders);
    }

    private void ShotFired(int position)
    {
        bool hit = false;

        foreach (Boat boat in boats)
        {
            if (boat.HitCheck(position))
            {
                hit = true;
                // Enable the object on that position with the 'hit' object
                Debug.Log("Hit!");
                if (boat.SunkCheck())
                {
                    Debug.Log("Sunk!");
                    int[] positionsAround = boat.Sunk(this.board);

                    foreach (int positionAround in positionsAround)
                    {
                        if (positionAround != 0)
                        {
                            // Enable the object on that position with the 'miss' object
                        }
                    }
                    WinCheck();
                }
                break;
            }
        }

        if (!hit)
        {
            // Enable the object on that position with the 'miss' object
            Debug.Log("Miss!");
        }
    }

    private void WinCheck()
    {
        bool win = true;

        foreach (Boat boat in boats)
        {
            if (!boat.SunkCheck())
            {
                win = false;
                break;
            }
        }

        if (win)
        {
            // Call a GameOver method to end the game because the player has won
            Debug.Log("You win!");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
