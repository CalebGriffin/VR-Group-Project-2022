using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static BattleshipAI.AIHelper;
using static BattleshipAI.AIDebug;

public class AI : MonoBehaviour
{
    #region Variable Declarations

    // Custom enum for AI difficulty, can be changed in the Inspector
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
    public bool targetMode = false;

    // An integer to store how far below the optimal move the AI could potentially shoot at
    private int deviation = 0;
    public int Deviation { get { return deviation; } }

    // An integer to remember the original deviation value so that it can be dynamically changed at runtime
    private int startingDeviation = 0;
    public int StartingDeviation { get { return startingDeviation; } }

    // A boolean that stops functions from running if the AI is clearing all of the information at the end of a game
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
    // Returns a ValueTuple containing the position that was shot and whether it was a hit or a miss and whether it sunk a boat
    public (int, bool, bool) ShotFired(int position)
    {
        // A guard clause to make sure that the AI is not resetting
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

                    // Call the method to remove the points around the boat from the player's list of unchecked positions
                    // TESTING // CHANGE
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
            // Enable the object on that position with the 'miss' object
            int row = (position - 1) / board.Matrix.GetLength(0);
            int col = (position- 1) % board.Matrix.GetLength(0);
            // TESTING // CHANGE
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
        // Removes the first element from the list of previous guesses if it sunk a ship to fix a bug that didn't correctly reset the list
        if (previousGuesses.Count > 0 && previousGuesses[0].Sunk)
        {
            previousGuesses.RemoveAt(0);
        }

        // Only run the decision if the AI is not resetting
        if (!resetting)
        {
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
            // Else if the AI is not in target mode then calculate the heat map for the board and call the Hunt method
            else
            {
                CalculateHeatMap();
                Hunt();
            }
        }
    }

    // Choose the first position in the target stack and fire at it, if the shot is a hit and sunk then clear the target stack
    private void Target()
    {
        if (targetStack.Count > 0)
        {
            // Get the last two positions in the previous guesses list where it was a hit
            int[] LastTwoHits = GetLastTwoHits();

            // Get the absolute value of the difference between the last two positions that were hit
            int diff = Mathf.Abs(LastTwoHits[0] - LastTwoHits[1]);
            int row1 = (LastTwoHits[0] - 1) / board.Matrix.GetLength(0);
            int row2 = (LastTwoHits[1] - 1) / board.Matrix.GetLength(0);

            // If the difference is 1 and they are in the same row, then removes any positions in the target stack that are not in the same row as the last two positions that were hit
            if (diff == 1 && row1 == row2)
            {
                for (int i = targetStack.Count - 1; i >= 0; i--)
                {
                    if (targetStack[i].Direction == "North" || targetStack[i].Direction == "South")
                    {
                        targetStack.RemoveAt(i);
                    }
                }
            }
            // If the difference is 10 then removes any positions in the target stack that are not in the same column as the last two positions that were hit
            else if (diff == board.Matrix.GetLength(0))
            {
                for (int i = targetStack.Count - 1; i >= 0; i--)
                {
                    if (targetStack[i].Direction == "East" || targetStack[i].Direction == "West")
                    {
                        targetStack.RemoveAt(i);
                    }
                }
            }

            // Get the first position in the target stack and fire at it by calling the ShotFired method on the player's script only if the position hasn't already been checked
            int target = targetStack[0].Position;
            if (UncheckedPositionsContains(target))
            {
                targetStack.RemoveAt(0);
                uncheckedPositions.Remove(uncheckedPositions.First(x => x.Position == target));
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
                        uncheckedPositions.Remove(uncheckedPositions.First(x => x.Position == i.Position));
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
        // If the unchecked positions list is empty then return without doing anything
        if (uncheckedPositions.Count == 0)
        {
            return;
        }

        // Create a temporary integer to store the chosen position that will be fired at
        int position = 0;

        switch(aiDifficulty)
        {
            // If the AI is normal difficulty then choose a random position from the unchecked positions list
            case Difficulty.Normal:
                position = uncheckedPositions[UnityEngine.Random.Range(0, uncheckedPositions.Count)].Position;
                break;
            
            // If the AI is hard difficulty
            case Difficulty.Hard:
                // Get the first position in the list which will have the highest weight
                int currentWeight = uncheckedPositions[0].Weight;
                // Find the first element in the list that has a lower weight than the current highest weight
                int indexOfFirstLowerWeight = uncheckedPositions.FindIndex(x => x.Weight < currentWeight);

                // Clamp the deviation value to the number of different weights in the unchecked positions list
                // This prevents the AI firing at a position that has a weight that doesn't exist
                deviation = Mathf.Clamp(DifferentWeights() - 2, 0, startingDeviation);

                // If the deviation is 0 then this loop will be ignored
                // Otherwise it will repeat the process above of finding the first element in the list that has a lower weight than the current highest weight the number of times specified by the deviation
                for (int i = 0; i < deviation; i++)
                {
                    currentWeight = uncheckedPositions[indexOfFirstLowerWeight].Weight;
                    indexOfFirstLowerWeight = uncheckedPositions.FindIndex(x => x.Weight < currentWeight);
                }

                // The position will then be set as the position of a random element in the list that has a weight greater than or equal to the current weight
                position = uncheckedPositions[UnityEngine.Random.Range(0, indexOfFirstLowerWeight)].Position;
                break;
            
            default:
                break;
        }

        // Verify that the position hasn't already been checked
        if (UncheckedPositionsContains(position))
        {
            // Remove the position from the unchecked positions list
            // Fire at it by calling the ShotFired method on the player's script and add the result to the previous guesses list
            uncheckedPositions.Remove(uncheckedPositions.FirstOrDefault(x => x.Position == position));
            // TESTING // CHANGE
            previousGuesses.Add(otherAI.ShotFired(position));

            // If the shot was a hit, then set the target mode to true
            if (previousGuesses.Last().Hit)
            {
                targetMode = true;
            }

            // If the shot sunk a boat, then an error will be thrown because a ship can't be sunk by the AI in hunt mode
            if (previousGuesses.Last().Sunk)
            {
                Debug.Log("AI2 returned Sunk when it shouldn't have");
                previousGuesses.Remove(previousGuesses.Last());
                targetMode = false;
            }
        }
        // Throw an error if the position has already been checked
        else
        {
            Debug.LogError("Position " + position + " is not in the unchecked positions list");
        }
    }

    // Assigns weights to all of the unchecked positions based on how likely it is that a ship could be placed there
    private void CalculateHeatMap()
    {
        // Reset the weight of all positions to 0
        ClearUncheckedPositionWeight();

        // TESTING // CHANGE
        foreach (Boat boat in otherAI.Boats)
        {
            if (boat.RemainingPositions.Count > 0)
            {
                //Debug.Log("Boat: " + boat.Name + " Length: " + boat.Positions.Length);
                // For each of the boats that haven't been sunk, calculate the weight for each unchecked position if this boat could be placed there
                ProbabilityDensity(boat.Positions.Length, "Horizontal");
            }
        }

        // Order the list by weight from highest to lowest so that the AI can choose from the highest weighted positions
        uncheckedPositions = uncheckedPositions.OrderByDescending(x => x.Weight).ToList();

        // TESTING // REMOVE
        //DisplayPositionWeights();
    }

    // Runs through all of the positions in the unchecked positions list and attempts to fit the boat in that position and direction and adds the weight to the positions it can fit
    private void ProbabilityDensity(int boatLength, string directionToCheck)
    {
        // Create a temporary variable and set it's value based on the direction that is being checked
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

        // For each of the positions in the unchecked positions list
        for (int i = 0; i < uncheckedPositions.Count; i++)
        {
            // Create a temporary array to store the positions that the boat will fill if placed in the current position
            int[] boatPositionsToCheck = new int[boatLength];

            // Starting from the current position, add all of the positions to the array that the boat would take up if placed in the current position
            int loopNumber = uncheckedPositions[i].Position;
            for (int j = uncheckedPositions[i].Position; j < uncheckedPositions[i].Position + (boatLength * incrementValue); j += incrementValue)
            {
                boatPositionsToCheck[loopNumber - uncheckedPositions[i].Position] = j;
                loopNumber++;
            }

            bool valid = true;

            // Verifies that all the positions of the boat are on the same row, if not, then the boat is invalid
            if (directionToCheck == "Horizontal")
            {
                int startRow = (boatPositionsToCheck[0] - 1)/ board.Matrix.GetLength(0);
                int endRow = (boatPositionsToCheck[boatLength - 1] - 1) / board.Matrix.GetLength(0);
                if (startRow != endRow)
                {
                    valid = false;
                }
            }

            // If the previous validation passed, then check that none of the positions have already been fired at by the AI
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

            // If both validations passed, then add 1 to the weight of each position that the boat would fill if it were fit in that position
            if (valid)
            {
                foreach (int l in boatPositionsToCheck)
                {
                    int index = uncheckedPositions.FindIndex(x => x.Position == l);
                    uncheckedPositions[index] = (l, uncheckedPositions[index].Weight + 1);
                }
            }
        }

        // If it has checked the horizontal direction for this boat then it will call the function again with the vertical direction
        if (directionToCheck == "Horizontal")
        {
            ProbabilityDensity(boatLength, "Vertical");
        }
    }
}