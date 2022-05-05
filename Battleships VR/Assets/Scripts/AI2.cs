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
    public List<Boat> Boats { get { return boats; } }

    public List<int> positionGuesses = new List<int>();
    public List<int> positionGuessesWithParity = new List<int>();

    private List<(int Position, bool Hit, bool Sunk)> previousGuesses = new List<(int, bool, bool)>();

    private List<(int Position, string Direction)> targetStack = new List<(int, string)>();

    private bool targetMode = false;
    private bool huntMode = true;
    
    // Start is called before the first frame update
    [ContextMenu("Start")]
    public void Start()
    {
        CreateBoats();

        positionGuesses.Clear();
        positionGuessesWithParity.Clear();

        foreach (int i in board.Matrix)
        {
            positionGuesses.Add(i);
        }
        foreach (int i in positionGuesses)
        {
            int row = (i - 1) / board.Matrix.GetLength(0);
            int col = (i - 1) % board.Matrix.GetLength(0);

            if ((row % 2 == 1 && col % 2 == 0) || (row % 2 == 0 && col % 2 == 1))
            {
                positionGuessesWithParity.Add(i);
            }   
        }

        targetMode = false;
        huntMode = true;
        targetStack.Clear();
        previousGuesses.Clear();

        board.currentBoatPositions.Clear();
        board.currentBoatPositionsWithBorders.Clear();
        

        //string boatPositions = "";
        //foreach (Boat boat in boats)
        //{
            //Debug.Log(boat.Name + ":");
            //foreach (int position in boat.Positions)
            //{
                //boatPositions += position + ", ";
                //int row = (position - 1) / board.Matrix.GetLength(0);
                //int col = (position - 1) % board.Matrix.GetLength(0);
                //GameObject.Instantiate(shipCube, new Vector3(row, 0, col + 11), Quaternion.identity);
            //}
            //Debug.Log(boatPositions);
            //boatPositions = "";
        //}

        //string boatPositionsWithBorders = "";
        //foreach (int position in board.currentBoatPositionsWithBorders)
        //{
            //boatPositionsWithBorders += position + ", ";
            //if (!board.currentBoatPositions.Contains(position))
            //{
                //int row = (position - 1) / board.Matrix.GetLength(0);
                //int col = (position - 1) % board.Matrix.GetLength(0);
                //GameObject.Instantiate(borderCube, new Vector3(row, 0, col), Quaternion.identity);
            //}
        //}
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

    public (int, bool, bool) ShotFired(int position)
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
                //GameObject.Instantiate(hitCube, new Vector3(row, 1, col + 11), Quaternion.identity);
                //Debug.Log("Hit!");
                if (boat.SunkCheck())
                {
                    sunk = true;
                    //Debug.Log("Sunk!");
                    int[] positionsAround = boat.Sunk(this.board);

                    foreach (int positionAround in positionsAround)
                    {
                        if (positionAround != 0 && AI.UncheckedPositionsContains(positionAround))
                        {
                            // Enable the object on that position with the 'miss' object
                            row = (positionAround - 1) / board.Matrix.GetLength(0);
                            col = (positionAround - 1) % board.Matrix.GetLength(0);
                            //GameObject.Instantiate(missCube, new Vector3(row, 1, col + 11), Quaternion.identity);
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
            //GameObject.Instantiate(missCube, new Vector3(row, 1, col + 11), Quaternion.identity);
            //Debug.Log("Miss!");
        }

        return ValueTuple.Create(position, hit, sunk);
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
            if (testRunner.playing)
            {
                testRunner.Button("AI");
            }
        }
    }

    public int[] GetLastTwoHits()
    {
        var temp = new List<(int Position, bool Hit, bool Sunk)>(previousGuesses);

        var lastPosition = temp.LastOrDefault(x => x.Hit == true);

        temp.Remove(lastPosition);

        var lastPosition2 = temp.LastOrDefault(x => x.Hit == true);

        return new int[] { lastPosition.Position, lastPosition2.Position };
    }
    public List<(int, string)> GetCardinalPositionsAround(int position)
    {
        string[] directions = new string[4];

        List<(int Position, string Direction)> returnValues = new List<(int Position, string Direction)>();

        int row = (position - 1) / board.Matrix.GetLength(0);
        int col = (position - 1) % board.Matrix.GetLength(0);

        int[] LastTwoHits = GetLastTwoHits();

        int diff = Mathf.Abs(LastTwoHits[0] - LastTwoHits[1]);

        if (diff == 1)
        {
            directions[0] = "East";
            directions[1] = "West";
        }
        else if (diff == board.Matrix.GetLength(0))
        {
            directions[0] = "North";
            directions[1] = "South";
        }
        else if (diff < board.Matrix.GetLength(0))
        {
            bool sameBoat = true;
            int smallerNumber = Mathf.Min(LastTwoHits[0], LastTwoHits[1]);
            int largerNumber = Mathf.Max(LastTwoHits[0], LastTwoHits[1]);

            for (int i = smallerNumber; i < largerNumber; i++)
            {
                if (!previousGuesses.Contains((i, true, false)))
                {
                    sameBoat = false;
                    break;
                }
            }

            if (sameBoat)
            {
                directions[0] = "East";
                directions[1] = "West";
            }
            else
            {
                directions[0] = "North";
                directions[1] = "East";
                directions[2] = "South";
                directions[3] = "West";
            }
        }
        else if (diff % board.Matrix.GetLength(0) == 0)
        {
            bool sameBoat = true;
            int smallerNumber = Mathf.Min(LastTwoHits[0], LastTwoHits[1]);
            int largerNumber = Mathf.Max(LastTwoHits[0], LastTwoHits[1]);

            for (int i = smallerNumber; i < largerNumber; i += board.Matrix.GetLength(0))
            {
                if (!previousGuesses.Contains((i, true, false)))
                {
                    sameBoat = false;
                    break;
                }
            }

            if (sameBoat)
            {
                directions[0] = "North";
                directions[1] = "South";
            }
            else
            {
                directions[0] = "North";
                directions[1] = "East";
                directions[2] = "South";
                directions[3] = "West";
            }
        }
        else
        {
            directions[0] = "North";
            directions[1] = "East";
            directions[2] = "South";
            directions[3] = "West";
        }

        foreach (string direction in directions)
        {
            switch (direction)
            {
                case "North":
                    if (row - 1 >= 0)
                    {
                        returnValues.Add(ValueTuple.Create(board.Matrix[row - 1, col], direction));
                    }
                    break;
                case "East":
                    if (col + 1 < board.Matrix.GetLength(0))
                    {
                        returnValues.Add(ValueTuple.Create(board.Matrix[row, col + 1], direction));
                    }
                    break;
                case "South":
                    if (row + 1 < board.Matrix.GetLength(0))
                    {
                        returnValues.Add(ValueTuple.Create(board.Matrix[row + 1, col], direction));
                    }
                    break;
                case "West":
                    if (col - 1 >= 0)
                    {
                        returnValues.Add(ValueTuple.Create(board.Matrix[row, col - 1], direction));
                    }
                    break;
            }
        }

        return returnValues;
    }

    private bool TargetStackContains(int position)
    {
        return targetStack.Any(x => x.Position == position);
    }

    [ContextMenu(nameof(Decision))]
    public void Decision()
    {
        if (targetMode)
        {
            if (previousGuesses.Last().Item2)
            {
                List<(int Position, string Direction)> cardinalPositionsAround = GetCardinalPositionsAround(previousGuesses.Last().Item1);

                for (int i = cardinalPositionsAround.Count - 1; i > -1; i--)
                {
                    if (positionGuesses.Contains(cardinalPositionsAround[i].Position) && !TargetStackContains(cardinalPositionsAround[i].Position) && cardinalPositionsAround[i].Position != 0)
                    {
                        targetStack.Insert(0, cardinalPositionsAround[i]);
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
            int[] LastTwoHits = GetLastTwoHits();

            int diff = Mathf.Abs(LastTwoHits[0] - LastTwoHits[1]);

            if (diff == 1)
            {
                for (int i = targetStack.Count - 1; i > -1; i--)
                {
                    if (targetStack[i].Direction == "North" || targetStack[i].Direction == "South")
                    {
                        targetStack.RemoveAt(i);
                    }
                }
            }
            else if (diff == board.Matrix.GetLength(0))
            {
                for (int i = targetStack.Count - 1; i > -1; i--)
                {
                    if (targetStack[i].Direction == "East" || targetStack[i].Direction == "West")
                    {
                        targetStack.RemoveAt(i);
                    }
                }
            }

            int target = targetStack[0].Position;
            targetStack.RemoveAt(0);
            if (positionGuesses.Contains(target))
            {
                positionGuesses.Remove(target);
                if (positionGuessesWithParity.Contains(target))
                {
                    positionGuessesWithParity.Remove(target);
                }
            }
            previousGuesses.Add(AI.ShotFired(target));
            if (previousGuesses.Last().Item2 && previousGuesses.Last().Item3)
            {
                foreach ((int Position, string Direction) i in targetStack)
                {
                    if (positionGuesses.Contains(i.Position))
                    {
                        positionGuesses.Remove(i.Position);
                        if (positionGuessesWithParity.Contains(i.Position))
                        {
                            positionGuessesWithParity.Remove(i.Position);
                        }
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
        if (positionGuessesWithParity.Count > 0)
        {
            int position = positionGuessesWithParity[UnityEngine.Random.Range(0, positionGuessesWithParity.Count)];
            positionGuesses.Remove(position);
            positionGuessesWithParity.Remove(position);
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
