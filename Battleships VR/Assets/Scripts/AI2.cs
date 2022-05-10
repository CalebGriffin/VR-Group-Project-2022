using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;

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

    public bool resetting = false;

    public GameObject textParent;
    
    // Start is called before the first frame update
    [ContextMenu("Start")]
    public void Start()
    {

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

    public IEnumerator Reset()
    {
        resetting = true;
        positionGuesses.Clear();
        positionGuessesWithParity.Clear();
        targetMode = false;
        huntMode = true;
        targetStack.Clear();
        previousGuesses.Clear();

        board.currentBoatPositions.Clear();
        board.currentBoatPositionsWithBorders.Clear();

        yield return new WaitForSeconds(0.001f);

        foreach (int i in board.Matrix)
        {
            positionGuesses.Add(i);
        }
        SetUpParity();
        //DisplayParity();

        CreateBoats();
        yield return new WaitForSeconds(0.001f);
        DisplayBoats();
        if (previousGuesses.Count > 0 && previousGuesses[0].Item3)
        {
            //Debug.Log("Found the error 2");
            positionGuesses.RemoveAt(0);
        }

        if (positionGuesses.Count < 100)
        {
            //Debug.LogError("Not enough positions");
            positionGuesses.Clear();
            foreach (int i in board.Matrix)
            {
                positionGuesses.Add(i);
            }
        }
        resetting = false;
    }

    public void SetUpParity()
    {
        int smallestBoatLength = 10;
        foreach (Boat boat in AI.Boats)
        {
            if (boat.Positions.Length < smallestBoatLength && boat.RemainingPositions.Count > 0)
            {
                smallestBoatLength = boat.Positions.Length;
            }
        }
        positionGuessesWithParity.Clear();
        foreach (int i in positionGuesses)
        {
            int row = (i - 1) / board.Matrix.GetLength(0);
            int col = (i - 1) % board.Matrix.GetLength(0);

            if ((row % smallestBoatLength) + (col % smallestBoatLength) == (smallestBoatLength-1))
            {
                positionGuessesWithParity.Add(i);
            }

            // Dan's old parity algorithm
            //if ((row % 3 == 1 && col % 3 == 0) || (row % 3 == 0 && col % 3 == 1))
            //{
                //positionGuessesWithParity.Add(i);
            //}   
        }
    }

    public void DisplayParity()
    {
        foreach (int i in board.Matrix)
        {
            textParent.transform.Find(i.ToString()).GetChild(0).GetComponent<TextMeshProUGUI>().color = Color.black;

            if (positionGuessesWithParity.Contains(i))
            {
                textParent.transform.Find(i.ToString()).GetChild(0).GetComponent<TextMeshProUGUI>().color = Color.green;
            }
        }
    }

    public void DisplayBoats()
    {
        string boatPositions = "";
        foreach (Boat boat in boats)
        {
            //Debug.Log(boat.Name + ":");
            foreach (int position in boat.Positions)
            {
                boatPositions += position + ", ";
                int row = (position - 1) / board.Matrix.GetLength(0);
                int col = (position - 1) % board.Matrix.GetLength(0);
                //GameObject.Instantiate(shipCube, new Vector3(row, 0, col + 11), Quaternion.identity);
            }
            //Debug.Log(boatPositions);
            boatPositions = "";
        }
    }

    void Update()
    {
        var duplicates = previousGuesses
            .GroupBy(x => x.Position)
            .Where(g => g.Count() > 1)
            .Select(g => g.Key);
        if (duplicates.Count() > 0)
        {
            string duplicatesString = "";
            foreach (int position in duplicates)
            {
                duplicatesString += position + ", ";
            }
            Debug.LogError("AI2 Duplicate positions found: " + duplicatesString);
        }
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

    [ContextMenu(nameof(PrintPreviousGuesses))]
    private void PrintPreviousGuesses()
    {
        string previousGuessesString = "";
        for (int i = 0; i < previousGuesses.Count; i++)
        {
            previousGuessesString = i + ", " + previousGuesses[i].Position + ", " + previousGuesses[i].Hit + ", " + previousGuesses[i].Sunk;
            Debug.Log(previousGuessesString);
        }
    }

    [ContextMenu(nameof(PrintTargetStack))]
    private void PrintTargetStack()
    {
        string targetStackString = "";
        foreach ((int Position, string Direction) target in targetStack)
        {
            targetStackString = target.Position + ", " + target.Direction;
            Debug.Log(targetStackString);
        }
    }

    [ContextMenu(nameof(PrintUncheckedPositions))]
    private void PrintUncheckedPositions()
    {
        string positionGuessesString = "";
        foreach (int i in positionGuesses)
        {
            positionGuessesString += i + ", ";
        }
        Debug.Log(positionGuessesString);
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
                        if (positionAround != 0 && BattleshipAI.AIHelper.UncheckedPositionsContains(positionAround))
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
            if (positionGuessesWithParity.Contains(i))
            {
                positionGuessesWithParity.Remove(i);
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
        int row1 = (LastTwoHits[0] - 1) / board.Matrix.GetLength(0);
        int row2 = (LastTwoHits[1] - 1) / board.Matrix.GetLength(0);

        if (diff == 1 && row1 == row2)
        {
            directions[0] = "East";
            directions[1] = "West";
        }
        else if (diff == board.Matrix.GetLength(0))
        {
            directions[0] = "North";
            directions[1] = "South";
        }
        else if (diff < board.Matrix.GetLength(0) && row1 == row2)
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
        if (previousGuesses.Count > 0 && previousGuesses[0].Sunk)
        {
            previousGuesses.RemoveAt(0);
        }
        if (!resetting)
        {
            if (targetMode)
            {
                if (previousGuesses.Last().Item2)
                {
                    List<(int Position, string Direction)> cardinalPositionsAround = GetCardinalPositionsAround(previousGuesses.Last().Item1);

                    for (int i = cardinalPositionsAround.Count - 1; i >= 0; i--)
                    {
                        if (TargetStackContains(cardinalPositionsAround[i].Position) || !positionGuesses.Contains(cardinalPositionsAround[i].Position))
                        {
                            cardinalPositionsAround.RemoveAt(i);
                        }
                    }

                    foreach ((int Position, string Direction) cardinalPosition in cardinalPositionsAround)
                    {
                        targetStack.Insert(0, cardinalPosition);
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
    }

    private void Target()
    {
        if (targetStack.Count > 0)
        {
            int[] LastTwoHits = GetLastTwoHits();

            int diff = Mathf.Abs(LastTwoHits[0] - LastTwoHits[1]);
            int row1 = (LastTwoHits[0] - 1) / board.Matrix.GetLength(0);
            int row2 = (LastTwoHits[1] - 1) / board.Matrix.GetLength(0);

            if (diff == 1 && row1 == row2)
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
            if (positionGuesses.Contains(target))
            {
                targetStack.RemoveAt(0);
                positionGuesses.Remove(target);
                if (positionGuessesWithParity.Contains(target))
                {
                    positionGuessesWithParity.Remove(target);
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
                    SetUpParity();
                    //DisplayParity();
                    targetStack.Clear();

                    targetMode = false;
                    huntMode = true;
                }
            }
            else
            {
                Debug.LogError("(AI2) Target stack contains a position that is not in the position guesses list");
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
            if (previousGuesses.Last().Item3)
            {
                Debug.Log("AI returned Sunk when it shouldn't have");
                previousGuesses.Remove(previousGuesses.Last());
                targetMode = false;
            }
        }
    }
}
