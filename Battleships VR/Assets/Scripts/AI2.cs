using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AI2 : MonoBehaviour
{
    public GameObject shipCube;
    public GameObject borderCube;
    public GameObject hitCube;
    public GameObject missCube;

    public AI AI;

    public TestRunner testRunner;

    private Board board = new Board(10);

    private List<Boat> boats = new List<Boat>();

    public List<int> positionGuesses = new List<int>();

    private List<Tuple<int, bool, bool>> previousGuesses = new List<Tuple<int, bool, bool>>();

    private List<int> targetStack = new List<int>();

    private bool targetMode = false;
    private bool huntMode = true;
    
    // Start is called before the first frame update
    [ContextMenu("Start")]
    void Start()
    {
        CreateBoats();

        foreach (int i in board.Matrix)
        {
            positionGuesses.Add(i);
        }

        targetMode = false;
        huntMode = true;
        targetStack.Clear();
        previousGuesses.Clear();

        board.currentBoatPositions.Clear();
        board.currentBoatPositionsWithBorders.Clear();
        

        string boatPositions = "";
        foreach (Boat boat in boats)
        {
            Debug.Log(boat.Name + ":");
            foreach (int position in boat.Positions)
            {
                boatPositions += position + ", ";
                int row = (position - 1) / board.Matrix.GetLength(0);
                int col = (position - 1) % board.Matrix.GetLength(0);
                GameObject.Instantiate(shipCube, new Vector3(row, 0, col + 11), Quaternion.identity);
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
                //GameObject.Instantiate(borderCube, new Vector3(row, 0, col), Quaternion.identity);
            }
        }
        //Debug.Log(boatPositionsWithBorders);
    }

    #region Testing Methods
    [ContextMenu(nameof(CreateBoats))]
    private void CreateBoats()
    {
        boats.Clear();
        boats.Add(new Boat(board, "Carrier", 5));
        boats.Add(new Boat(board, "Battleship", 4));
        boats.Add(new Boat(board, "Cruiser", 3));
        boats.Add(new Boat(board, "Submarine", 3));
        boats.Add(new Boat(board, "Destroyer", 2));
    }

    [ContextMenu(nameof(PrintRemainingPositions))]
    private void PrintRemainingPositions()
    {
        foreach (Boat boat in boats)
        {
            string remainingPositions = "";
            Debug.Log(boat.Name + ":");
            foreach (int position in boat.RemainingPositions)
            {
                remainingPositions += position + ", ";
            }
            Debug.Log(remainingPositions);
        }
    }
    #endregion

    public Tuple<int, bool, bool> ShotFired(int position)
    {
        bool hit = false;
        bool sunk = false;

        foreach (Boat boat in boats)
        {
            if (boat.HitCheck(position))
            {
                hit = true;

                // Enable the object on that position with the 'hit' object
                int row = (position - 1) / board.Matrix.GetLength(0);
                int col = (position - 1) % board.Matrix.GetLength(0);
                GameObject.Instantiate(hitCube, new Vector3(row, 1, col + 11), Quaternion.identity);
                Debug.Log("Hit!");
                if (boat.SunkCheck())
                {
                    sunk = true;
                    Debug.Log("Sunk!");
                    int[] positionsAround = boat.Sunk(this.board);

                    foreach (int positionAround in positionsAround)
                    {
                        if (positionAround != 0 && AI.positionGuesses.Contains(positionAround))
                        {
                            // Enable the object on that position with the 'miss' object
                            row = (positionAround - 1) / board.Matrix.GetLength(0);
                            col = (positionAround - 1) % board.Matrix.GetLength(0);
                            GameObject.Instantiate(missCube, new Vector3(row, 1, col + 11), Quaternion.identity);
                        }
                    }

                    AI.RemoveSunkPoints(positionsAround);

                    WinCheck();
                }
                break;
            }
        }

        if (!hit)
        {
            // Enable the object on that position with the 'miss' object
            int row = (position - 1) / board.Matrix.GetLength(0);
            int col = (position - 1) % board.Matrix.GetLength(0);
            GameObject.Instantiate(missCube, new Vector3(row, 1, col + 11), Quaternion.identity);
            Debug.Log("Miss!");
        }

        return Tuple.Create(position, hit, sunk);
    }

    public void RemoveSunkPoints(int[] pointsAround)
    {
        foreach (int i in pointsAround)
        {
            if (positionGuesses.Contains(i))
            {
                positionGuesses.Remove(i);
            }   
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
            testRunner.Button();
        }
    }

    public int[] GetCardinalPositionsAround(int position)
    {
        int[] returnValues = new int[4];

        int row = (position - 1) / board.Matrix.GetLength(0);
        int col = (position - 1) % board.Matrix.GetLength(0);

        foreach (string direction in new string[] { "North", "East", "South", "West" })
        {
            switch (direction)
            {
                case "North":
                    if (row - 1 >= 0)
                    {
                        returnValues[0] = board.Matrix[row - 1, col];
                    }
                    break;
                case "East":
                    if (col + 1 < board.Matrix.GetLength(0))
                    {
                        returnValues[1] = board.Matrix[row, col + 1];
                    }
                    break;
                case "South":
                    if (row + 1 < board.Matrix.GetLength(0))
                    {
                        returnValues[2] = board.Matrix[row + 1, col];
                    }
                    break;
                case "West":
                    if (col - 1 >= 0)
                    {
                        returnValues[3] = board.Matrix[row, col - 1];
                    }
                    break;
            }
        }

        return returnValues;
    }

    [ContextMenu(nameof(Decision))]
    public void Decision()
    {
        if (targetMode)
        {
            if (previousGuesses.Last().Item2)
            {
                int[] cardinalPositionsAround = GetCardinalPositionsAround(previousGuesses.Last().Item1);
                foreach (int cardinalPosition in cardinalPositionsAround)
                {
                    if (positionGuesses.Contains(cardinalPosition) && !targetStack.Contains(cardinalPosition))
                    {
                        targetStack.Insert(0, cardinalPosition);
                    }
                }
                Target();
            }
            else if (!previousGuesses.Last().Item2)
            {
                Target();
            }
        }
        else
        {
            Hunt();
        }

    }

    private void Target()
    {
        if (targetStack.Count > 0)
        {
            int target = targetStack[0];
            targetStack.RemoveAt(0);
            if (positionGuesses.Contains(target))
            {
                positionGuesses.Remove(target);
            }
            previousGuesses.Add(AI.ShotFired(target));
            if (previousGuesses.Last().Item2 && previousGuesses.Last().Item3)
            {
                foreach (int i in targetStack)
                {
                    if (positionGuesses.Contains(i))
                    {
                        positionGuesses.Remove(i);
                    }
                }
                targetStack.Clear();

                targetMode = false;
                huntMode = true;
            }
        }
    }

    private void Hunt()
    {
        if (positionGuesses.Count > 0)
        {
            int position = positionGuesses[UnityEngine.Random.Range(0, positionGuesses.Count)];
            positionGuesses.Remove(position);
            // Call the ShotFired method with the hunt position on the Player script
            previousGuesses.Add(AI.ShotFired(position));
            if (previousGuesses.Last().Item2)
            {
                huntMode = false;
                targetMode = true;
            }
        }
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
