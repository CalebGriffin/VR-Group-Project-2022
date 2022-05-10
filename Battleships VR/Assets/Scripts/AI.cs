using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static BattleshipAI.AIHelper;
using static BattleshipAI.AIDebug;
// TESTING // REMOVE
using TMPro;

public class AI : MonoBehaviour
{
    #region Variable Declarations

    public enum Difficulty
    {
        Normal,
        Hard
    }
    [SerializeField] private Difficulty aiDifficulty;

    // TESTING // REMOVE
    public AI2 otherAI; // Reference to the other AI script
    public TestRunner testRunner; // Calls the Decision method on each of the AI scripts to run the simulation

    // Creates a new instance of the Board class and stores its data
    private Board board = new Board(10);
    public Board Board { get { return board; } }

    // A list to store all of the boats
    private List<Boat> boats = new List<Boat>();
    public List<Boat> Boats { get { return boats; } }

    // A list to store all of the guesses that have not been made yet
    public List<(int Position, int Weight)> uncheckedPositions = new List<(int Position, int Weight)>();

    // A list to store all of the previous guesses and whether they were hits or misses and whether they sunk a boat
    // Uses ValueTuples to store the data which allow each entry to have a name and a value
    public List<(int Position, bool Hit, bool Sunk)> previousGuesses = new List<(int, bool, bool)>();

    // A list of positions that is used in target mode to pick points around the currently targeted boat
    public List<(int Position, string Direction)> targetStack = new List<(int Position, string Direction)>();

    // A boolean to determine whether the AI is in target mode or not
    [SerializeField] private bool targetMode = false;
    public bool TargetMode { get { return targetMode; } set { targetMode = value; } }

    // An integer to store how far below the optimal move the AI could potentially shoot at
    private int deviation = 0;
    public int Deviation { get { return deviation; } }
    private int startingDeviation = 0;
    public int StartingDeviation { get { return startingDeviation; } }

    public bool resetting = false;

    #endregion

    // Clears the boats list and fills it with the relevant boats
    [ContextMenu(nameof(CreateBoats))]
    public void CreateBoats()
    {
        boats.Clear();
        boats.Add(new Boat(board, "Carrier", 5));
        boats.Add(new Boat(board, "Battleship", 4));
        boats.Add(new Boat(board, "Cruiser", 3));
        boats.Add(new Boat(board, "Submarine", 3));
        boats.Add(new Boat(board, "Destroyer", 2));
    }

    // This method will be called by the Player when they choose a position on the board to shoot
    // Returns a Tuple containing the position that was shot and whether it was a hit or a miss
    public (int, bool, bool) ShotFired(int position)
    {
        if (otherAI.resetting || resetting)
        {
            return (0, false, false);
        }

        // These are the return values
        bool hit = false;
        bool sunk = false;

        // Foreach of the boats in the boat list, if they have been hit then set the bool to true and check if they have been sunk then set the bool to true
        foreach (Boat boat in boats)
        {
            // Calls the method on the Boat class that returns a bool of whether the position is within it's remaining positions
            if (boat.HitCheck(position))
            {
                hit = true;

                // TESTING // CHANGE
                // Enable the object on that position with the 'hit' object
                int row = (position - 1) / board.Matrix.GetLength(0);
                int col = (position - 1) % board.Matrix.GetLength(0);
                //GameObject.Instantiate(hitCube, new Vector3(row, 1, col), Quaternion.identity);
                //Debug.Log("Hit!");

                // Calls the method on the Boat class that returns a bool of whether the boat has any remaining positions left
                if (boat.SunkCheck())
                {
                    sunk = true;
                    //Debug.Log("Sunk!");

                    // Get all of the points around the boat and place miss objects around them because the boats can't be there
                    int[] positionsAround = boat.Sunk(this.board);
                    foreach (int positionAround in positionsAround)
                    {
                        if (positionAround != 0 && otherAI.positionGuesses.Contains(positionAround))
                        {
                            // TESTING // CHANGE
                            // Enable the object on that position with the 'miss' object
                            row = (positionAround - 1) / board.Matrix.GetLength(0);
                            col = (positionAround - 1) % board.Matrix.GetLength(0);
                            //GameObject.Instantiate(missCube, new Vector3(row, 1, col), Quaternion.identity);
                        }
                    }

                    // TESTING // REMOVE
                    // Call the method to remove the points around the boat from the other AI's position guesses
                    otherAI.RemoveSunkPoints(positionsAround);

                    // Call the method to check if the player has won
                    WinCheck();
                }
                break;
            }
        }

        // If the shot was a miss
        if (!hit)
        {
            // TESTING // CHANGE
            // Enable the object on that position with the 'miss' object
            int row = (position - 1) / board.Matrix.GetLength(0);
            int col = (position- 1) % board.Matrix.GetLength(0);
            //GameObject.Instantiate(missCube, new Vector3(row, 1, col), Quaternion.identity);
            //Debug.Log("Miss!");
        }

        // Return the ValueTuple with the hit and sunk booleans
        return ValueTuple.Create(position, hit, sunk);
    }

    // This method will be called by the player to remove the points around a sunk boat from the position guesses list
    public void RemoveSunkPoints(int[] pointsAround)
    {
        foreach (int i in pointsAround)
        {
            if (UncheckedPositionsContains(i))
            {
                uncheckedPositions.Remove(uncheckedPositions.Find(x => x.Position == i));
            }   
        }
    }

    // TESTING
    [ContextMenu(nameof(Decision))]
    // This method is the decision of the AI and will be called by the Player after they have played their turn
    public void Decision()
    {
        if (previousGuesses.Count > 0 && previousGuesses[0].Sunk)
        {
            previousGuesses.RemoveAt(0);
        }
        if (!resetting)
        {
            // TESTING // REMOVE
            //CalculateHeatMap();

            if (targetMode)
            {
                // If the AI is in target mode and the previous shot was a hit then get all the positions around that point and add them to the target stack
                if (previousGuesses.Last().Hit)
                {
                    // Get the positions around the previous guess
                    List<(int Position, string Direction)> cardinalPositionsAround = GetCardinalPositionsAround(previousGuesses.Last().Position);

                    // Validate all the returned positions to make sure that they haven't already been added to the target stack or have already been checked or are out of bounds
                    for (int i = cardinalPositionsAround.Count - 1; i > -1; i--)
                    {
                        if (TargetStackContains(cardinalPositionsAround[i].Position) || !UncheckedPositionsContains(cardinalPositionsAround[i].Position) || cardinalPositionsAround[i].Position == 0)
                        {
                            cardinalPositionsAround.RemoveAt(i);
                        }
                    }
                    string cardinalPositionsAroundString = "";
                    foreach ((int Position, string Direction) cardinalPosition in cardinalPositionsAround)
                    {
                        cardinalPositionsAroundString += cardinalPosition.Position + ", ";
                    }
                    //Debug.Log("Cardinal Positions Around: " + cardinalPositionsAroundString);

                    // For each of the positions around the previous guess, add them to the front of the target stack
                    foreach ((int Position, string Direction) cardinalPosition in cardinalPositionsAround)
                    {
                        targetStack.Insert(0, cardinalPosition);
                    }
                    // Call the Target method
                    Target();
                }
                // If the AI is in target mode and the previous shot was a miss then call the Target method without updating the target stack
                else if (!previousGuesses.Last().Hit)
                {
                    Target();
                }
            }
            // Else if the AI is not in target mode then call the Hunt method
            else
            {
                CalculateHeatMap();
                Hunt();
            }

            // TESTING // REMOVE
            //CalculateHeatMap();
        }
    }

    // Choose the first position in the target stack and fire at it, if the shot is a hit and sunk then clear the target stack
    private void Target()
    {
        if (targetStack.Count > 0)
        {
            // Get the last two positions in the previous guesses list where it was a hit
            int[] LastTwoHits = GetLastTwoHits();
            //Debug.Log("LastTwoHits: " + LastTwoHits[0] + ", " + LastTwoHits[1]);

            // Get the absolute value of the difference between the last two positions that were hit
            int diff = Mathf.Abs(LastTwoHits[0] - LastTwoHits[1]);
            int row1 = (LastTwoHits[0] - 1) / board.Matrix.GetLength(0);
            int row2 = (LastTwoHits[1] - 1) / board.Matrix.GetLength(0);
            //Debug.Log("Diff: " + diff);

            // If the difference is 1 then removes any positions in the target stack that are not in the same column as the last two positions that were hit
            if (diff == 1 && row1 == row2)
            {
                // Loop through the target stack backwards, removing any elements where the Direction is "North" or "South"
                for (int i = targetStack.Count - 1; i >= 0; i--)
                {
                    if (targetStack[i].Direction == "North" || targetStack[i].Direction == "South")
                    {
                        targetStack.RemoveAt(i);
                    }
                }
            }
            // If the difference is 10 then removes any positions in the target stack that are not in the same row as the last two positions that were hit
            else if (diff == board.Matrix.GetLength(0))
            {
                // Loop through the target stack backwards, removing any elements where the Direction is "East" or "West"
                for (int i = targetStack.Count - 1; i >= 0; i--)
                {
                    if (targetStack[i].Direction == "East" || targetStack[i].Direction == "West")
                    {
                        targetStack.RemoveAt(i);
                    }
                }
            }

            // Get the first position in the target stack and fire at it by calling the ShotFired method on the player's script
            int target = targetStack[0].Position;
            if (UncheckedPositionsContains(target))
            {
                targetStack.RemoveAt(0);
                uncheckedPositions.Remove(uncheckedPositions.FirstOrDefault(x => x.Position == target));
            }
            // TESTING // CHANGE
            previousGuesses.Add(otherAI.ShotFired(target));

            // If the shot was a hit and sunk then clear the target stack and remove the elements from the position guesses list and set the target mode to false
            if (previousGuesses.Last().Hit && previousGuesses.Last().Sunk)
            {
                foreach ((int Position, string Direction) i in targetStack)
                {
                    if (UncheckedPositionsContains(i.Position))
                    {
                        uncheckedPositions.Remove(uncheckedPositions.FirstOrDefault(x => x.Position == i.Position));
                    }
                }
                targetStack.Clear();

                targetMode = false;
            }
        }
    }

    // Choose a random position from the position guesses list and fire at it, if the shot is a hit then set the AI to target mode
    private void Hunt()
    {
        // Make sure that the position guesses list isn't empty
        if (uncheckedPositions.Count > 0)
        {
            int position = 0;
            switch(aiDifficulty)
            {
                case Difficulty.Normal:
                    position = uncheckedPositions[UnityEngine.Random.Range(0, uncheckedPositions.Count)].Position;
                    break;
                
                case Difficulty.Hard:
                    int currentHighestWeight = uncheckedPositions[0].Weight;
                    int lastElementWithHighestWeight = uncheckedPositions.FindIndex(x => x.Weight < currentHighestWeight);
                    if (DifferentWeights() < startingDeviation + 2)
                    {
                        deviation = Mathf.Clamp(DifferentWeights() - 2, 0, startingDeviation);
                    }
                    else
                    {
                        deviation = startingDeviation;
                    }
                    for (int i = 0; i < deviation; i++)
                    {
                        currentHighestWeight = uncheckedPositions[lastElementWithHighestWeight].Weight;
                        lastElementWithHighestWeight = uncheckedPositions.FindIndex(x => x.Weight < currentHighestWeight);
                    }
                    position = uncheckedPositions[UnityEngine.Random.Range(0, lastElementWithHighestWeight)].Position;
                    break;
                
                default:
                    break;
            }

            if (UncheckedPositionsContains(position))
            {
                // Pick a random position, remove it from the positions guesses list and fire at it by calling the ShotFired method on the player's script
                uncheckedPositions.Remove(uncheckedPositions.FirstOrDefault(x => x.Position == position));
                // TESTING // CHANGE
                previousGuesses.Add(otherAI.ShotFired(position));

                // If the shot was a hit, then set the target mode to true
                if (previousGuesses.Last().Hit)
                {
                    targetMode = true;
                }
                if (previousGuesses.Last().Sunk)
                {
                    Debug.Log("AI2 returned Sunk when it shouldn't have");
                    previousGuesses.Remove(previousGuesses.Last());
                    targetMode = false;
                }
            }
            else
            {
                Debug.LogError("Position " + position + " is not in the unchecked positions list");
            }
        }
    }

    private void CalculateHeatMap()
    {
        ClearUncheckedPositionWeight();

        foreach (Boat boat in otherAI.Boats)
        {
            if (boat.RemainingPositions.Count > 0)
            {
                //Debug.Log("Boat: " + boat.Name + " Length: " + boat.Positions.Length);
                ProbabilityDensity(boat.Positions.Length, "Horizontal");
            }
        }

        // REMOVE
        // Fixes a strange bug where some positions had a weight of 0 but weren't being removed
        //if (uncheckedPositions.Any(x => x.Weight == 0))
        //{
            //for (int i = uncheckedPositions.Count - 1; i >= 0; i--)
            //{
                //if (uncheckedPositions[i].Weight == 0)
                //{
                    //uncheckedPositions.RemoveAt(i);
                //}
            //}
        //}

        uncheckedPositions = uncheckedPositions.OrderByDescending(x => x.Weight).ToList();

        //DisplayPositionWeights();
    }

    private void ProbabilityDensity(int boatLength, string directionToCheck)
    {
        int incrementValue = 0;

        switch (directionToCheck)
        {
            case "Horizontal":
                incrementValue = 1;
                break;
            
            case "Vertical":
                incrementValue = board.Matrix.GetLength(0);
                break;
            
            default:
                break;
        }

        for (int i = 0; i < uncheckedPositions.Count; i++)
        {
            int[] boatPositionsToCheck = new int[boatLength];

            int loopNumber = uncheckedPositions[i].Position;
            for (int j = uncheckedPositions[i].Position; j < uncheckedPositions[i].Position + (boatLength * incrementValue); j += incrementValue)
            {
                boatPositionsToCheck[loopNumber - uncheckedPositions[i].Position] = j;
                loopNumber++;
            }
            string boatPositionsToCheckString = "";
            foreach (int j in boatPositionsToCheck)
            {
                boatPositionsToCheckString += j + ", ";
            }

            bool valid = true;
            // A strange edge case where the boat isn't in a straight line
            if (directionToCheck == "Horizontal")
            {
                int startRow = (boatPositionsToCheck[0] - 1)/ board.Matrix.GetLength(0);
                int endRow = (boatPositionsToCheck[boatLength - 1] - 1) / board.Matrix.GetLength(0);
                if (startRow != endRow)
                {
                    valid = false;
                }
            }

            if (valid)
            {
                foreach (int k in boatPositionsToCheck)
                {
                    if (!UncheckedPositionsContains(k))
                    {
                        valid = false;
                        break;
                    }
                }
            }

            //Debug.Log("BoatPositionsToCheck: " + boatPositionsToCheckString + " Valid: " + valid);

            if (valid)
            {
                foreach (int l in boatPositionsToCheck)
                {
                    //uncheckedPositions[l] = (l, uncheckedPositions[l].Weight + 1);
                    int index = uncheckedPositions.FindIndex(x => x.Position == l);
                    uncheckedPositions[index] = (l, uncheckedPositions[index].Weight + 1);
                }
            }
        }

        if (directionToCheck == "Horizontal")
        {
            ProbabilityDensity(boatLength, "Vertical");
        }
    }
}