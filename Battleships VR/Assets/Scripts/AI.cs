using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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
    // Cube prefabs to display different objects on the board
    public GameObject shipCube;
    public GameObject borderCube;
    public GameObject hitCube;
    public GameObject missCube;
    public AI2 otherAI; // Reference to the other AI script
    public TestRunner testRunner; // Calls the Decision method on each of the AI scripts to run the simulation

    // Creates a new instance of the Board class and stores its data
    private Board board = new Board(10);

    // A list to store all of the boats
    private List<Boat> boats = new List<Boat>();

    // A list to store all of the guesses that have not been made yet
    public List<int> positionGuesses = new List<int>();

    // A list to store all of the previous guesses and whether they were hits or misses and whether they sunk a boat
    // Uses ValueTuples to store the data which allow each entry to have a name and a value
    private List<(int Position, bool Hit, bool Sunk)> previousGuesses = new List<(int, bool, bool)>();

    // A list of positions that is used in target mode to pick points around the currently targeted boat
    private List<(int Position, string Direction)> targetStack = new List<(int Position, string Direction)>();

    // A boolean to determine whether the AI is in target mode or not
    private bool targetMode = false;

    #endregion
    
    // ContextMenu allows this method to be run from within the Inspector
    [ContextMenu("Start")]
    // Start is called before the first frame update
    void Start()
    {
        // TESTING // REMOVE
        CreateBoats();

        // Initialize the position guesses list with the values from the board
        foreach (int i in board.Matrix)
        {
            positionGuesses.Add(i);
        }

        // TESTING // REMOVE
        // Reset from the previous time the game was played
        targetMode = false;
        targetStack.Clear();
        previousGuesses.Clear();
        board.currentBoatPositions.Clear();
        board.currentBoatPositionsWithBorders.Clear();
        
        // TESTING // REMOVE
        // Prints out the boats and their positions
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

        // TESTING // REMOVE
        // Prints out the borders
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
    // TESTING // MOVE
    // Clears the boats list and fills it with the relevant boats
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

    // TESTING // REMOVE
    // Prints out the un-hit positions for each boat
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
        foreach ((int Position, bool Hit, bool Sunk) previousGuess in previousGuesses)
        {
            previousGuessesString = previousGuess.Position + ", " + previousGuess.Hit + ", " + previousGuess.Sunk;
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
    #endregion

    // This method will be called by the Player when they choose a position on the board to shoot
    // Returns a Tuple containing the position that was shot and whether it was a hit or a miss
    public (int, bool, bool) ShotFired(int position)
    {
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
                GameObject.Instantiate(hitCube, new Vector3(row, 1, col), Quaternion.identity);
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
                            GameObject.Instantiate(missCube, new Vector3(row, 1, col), Quaternion.identity);
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
            GameObject.Instantiate(missCube, new Vector3(row, 1, col), Quaternion.identity);
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
            if (positionGuesses.Contains(i))
            {
                positionGuesses.Remove(i);
            }   
        }
    }

    // This method will check each of the boats and if none have any remaining positions then the player has won
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
            // TESTING // REMOVE
            testRunner.Button();
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

    // This method takes in a position and returns a list of all the positions around it in the 4 cardinal directions
    public List<(int, string)> GetCardinalPositionsAround(int position)
    {
        string[] directions = new string[4];

        List<(int Position, string Direction)> returnValues = new List<(int, string)>();

        int row = (position - 1) / board.Matrix.GetLength(0);
        int col = (position - 1) % board.Matrix.GetLength(0);

        // Get the last two positions in the previous guesses list where it was a hit
        int[] LastTwoHits = GetLastTwoHits();
        Debug.Log("LastTwoHits: " + LastTwoHits[0] + ", " + LastTwoHits[1]);

        // Get the absolute value of the difference between the last two positions that were hit
        int diff = Mathf.Abs(LastTwoHits[0] - LastTwoHits[1]);
        Debug.Log("Diff: " + diff);

        // If the difference is 1 then only check in left and right directions
        if (diff == 1)
        {
            directions[0] = "East";
            directions[1] = "West";
        }
        // If the difference is 10 then only check in up and down directions
        else if (diff == board.Matrix.GetLength(0))
        {
            directions[0] = "North";
            directions[1] = "South";
        }
        else if (diff < board.Matrix.GetLength(0))
        {
            bool sameBoat = true;
            int smallerNumber = 0;
            int largerNumber = 0;

            if (LastTwoHits[0] < LastTwoHits[1])
            {
                smallerNumber = LastTwoHits[0];
                largerNumber = LastTwoHits[1];
            }
            else
            {
                smallerNumber = LastTwoHits[1];
                largerNumber = LastTwoHits[0];
            }

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
                directions[1] = "South";
                directions[2] = "East";
                directions[3] = "West";
            }
        }
        else if (diff % board.Matrix.GetLength(0) == 0)
        {
            bool sameBoat = true;
            int smallerNumber = 0;
            int largerNumber = 0;


            if (LastTwoHits[0] < LastTwoHits[1])
            {
                smallerNumber = LastTwoHits[0];
                largerNumber = LastTwoHits[1];
            }
            else
            {
                smallerNumber = LastTwoHits[1];
                largerNumber = LastTwoHits[0];
            }

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
                directions[1] = "South";
                directions[2] = "East";
                directions[3] = "West";
            }
        }
        // Else, check in all directions
        else
        {
            directions[0] = "North";
            directions[1] = "South";
            directions[2] = "East";
            directions[3] = "West";
        }

        // The positions are added to the target stack in the reverse order that they are checked
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

    private bool TargetStackContains(int point)
    {
        foreach ((int Position, string Direction) in targetStack)
        {
            if (point == Position)
            {
                return true;
            }
        }

        return false;
    }

    // TESTING
    [ContextMenu(nameof(Decision))]
    // This method is the decision of the AI and will be called by the Player after they have played their turn
    public void Decision()
    {
        if (targetMode)
        {
            // If the AI is in target mode and the previous shot was a hit then get all the positions around that point and add them to the target stack
            if (previousGuesses.Last().Hit)
            {
                // Get the positions around the previous guess
                List<(int Position, string Direction)> cardinalPositionsAround = GetCardinalPositionsAround(previousGuesses.Last().Position);
                // For each of the positions around the previous guess, if they haven't already been guessed and they are not already in the target stack then add them to the target stack at the front
                foreach ((int Position, string Direction) cardinalPosition in cardinalPositionsAround)
                {
                    if (positionGuesses.Contains(cardinalPosition.Position) && !TargetStackContains(cardinalPosition.Position) && cardinalPosition.Position != 0)
                    {
                        targetStack.Insert(0, cardinalPosition);
                    }
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
            Hunt();
        }

    }

    // Choose the first position in the target stack and fire at it, if the shot is a hit and sunk then clear the target stack
    private void Target()
    {
        if (targetStack.Count > 0)
        {
            // Get the last two positions in the previous guesses list where it was a hit
            int[] LastTwoHits = GetLastTwoHits();
            Debug.Log("LastTwoHits: " + LastTwoHits[0] + ", " + LastTwoHits[1]);

            // Get the absolute value of the difference between the last two positions that were hit
            int diff = Mathf.Abs(LastTwoHits[0] - LastTwoHits[1]);
            Debug.Log("Diff: " + diff);

            // If the difference is 1 then removes any positions in the target stack that are not in the same column as the last two positions that were hit
            if (diff == 1)
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
            targetStack.RemoveAt(0);
            if (positionGuesses.Contains(target))
            {
                positionGuesses.Remove(target);
            }
            // TESTING // CHANGE
            previousGuesses.Add(otherAI.ShotFired(target));

            // If the shot was a hit and sunk then clear the target stack and remove the elements from the position guesses list and set the target mode to false
            if (previousGuesses.Last().Hit && previousGuesses.Last().Sunk)
            {
                foreach ((int Position, string Direction) i in targetStack)
                {
                    if (positionGuesses.Contains(i.Position))
                    {
                        positionGuesses.Remove(i.Position);
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
        if (positionGuesses.Count > 0)
        {
            // Pick a random position, remove it from the positions guesses list and fire at it by calling the ShotFired method on the player's script
            int position = positionGuesses[UnityEngine.Random.Range(0, positionGuesses.Count)];
            positionGuesses.Remove(position);
            // TESTING // CHANGE
            previousGuesses.Add(otherAI.ShotFired(position));

            // If the shot was a hit, then set the target mode to true
            if (previousGuesses.Last().Hit)
            {
                targetMode = true;
            }
        }
    }
}