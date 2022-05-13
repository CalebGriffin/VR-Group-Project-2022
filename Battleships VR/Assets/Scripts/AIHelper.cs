using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static BattleshipAI.AIDebug;

namespace BattleshipAI
{
    public class AIHelper : MonoBehaviour
    {
        // Reference to the AI script so that is can access the AI's variables and methods
        public static AI ai;

        // Returns a bool of whether or not the position is in the unchecked positions list
        public static bool UncheckedPositionsContains(int point)
        {
            return ai.uncheckedPositions.Any(x => x.Position == point);
        }

        // Returns a bool of whether or not the position is in the target stack
        public static bool TargetStackContains(int point)
        {
            return ai.targetStack.Any(x => x.Position == point);
        }

        // Returns an array of integers of the last two positions that were hit by the AI
        public static int[] GetLastTwoHits()
        {
            // Create a copy of the previous guesses list
            var temp = new List<(int Position, bool Hit, bool Sunk)>(ai.previousGuesses);

            // Get the last position that was hit
            var lastPosition = temp.LastOrDefault(x => x.Hit == true);

            // Remove it from the temporary list
            temp.Remove(lastPosition);

            // Get the last position in the updated list
            var lastPosition2 = temp.LastOrDefault(x => x.Hit == true);

            // Return the positions
            return new int[] { lastPosition.Position, lastPosition2.Position };
        }

        // TESTING // REMOVE
        [ContextMenu(nameof(DifferentWeights))]
        // Returns an integer of the number of different waits in the unchecked positions list
        public static int DifferentWeights()
        {
            // Create a temporary list to store the different weights
            List<int> weights = new List<int>();

            // Foreach of the positions in the unchecked positions list, if the weight is not already in the temporary list, add it to the list
            foreach ((int Position, int Weight) i in ai.uncheckedPositions)
            {
                if (!weights.Contains(i.Weight))
                {
                    weights.Add(i.Weight);
                }
            }

            // Return the number of elements in the temporary list
            return weights.Count;
        }

        // Resets the weight of all positions in the unchecked positions list to 0
        public static void ClearUncheckedPositionWeight()
        {
            for (int i = 0; i < ai.uncheckedPositions.Count; i++)
            {
                ai.uncheckedPositions[i] = (ai.uncheckedPositions[i].Position, 0);
            }
        }

        // This method will check each of the boats and if none have any remaining positions then the player has won
        public static void WinCheck()
        {
            bool win = true;

            foreach (Boat boat in ai.Boats)
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
                // TESTING // REMOVE
                if (ai.testRunner.playing)
                {
                    ai.testRunner.Button("AI2");
                }
            }
        }

        // Reset all of the AI's variables after a game has ended
        public void Reset()
        {
            // A boolean so that other methods can check if the AI is resetting
            ai.resetting = true;

            // Clear the unchecked positions list
            ai.uncheckedPositions.Clear();

            // Set the AI target mode to false
            ai.targetMode = false;

            // Clear the target stack
            ai.targetStack.Clear();

            // Clear the previous guesses list
            ai.previousGuesses.Clear();

            // Clear the boat positions list and the boat positions with borders list
            ai.Board.currentBoatPositions.Clear();
            ai.Board.currentBoatPositionsWithBorders.Clear();

            // Initialize the position guesses list with the values from the board
            foreach (int i in ai.Board.Matrix)
            {
                ai.uncheckedPositions.Add((i, 0));
            }

            // Call the method to create the AI's boats
            ai.CreateBoats();

            // Call the method to display the AI's boats on the board and on the sea
            DisplayBoats();

            // Fixes a bug where the last shot of the previous game is added to the previous guesses list
            if (ai.previousGuesses.Count > 0 && ai.previousGuesses[0].Sunk)
            {
                ai.previousGuesses.RemoveAt(0);
            }

            // Set the boolean to false
            ai.resetting = false;
        }

        // This method takes in a position and returns a list of all the positions around it in the 4 cardinal directions
        public static List<(int, string)> GetCardinalPositionsAround(int position)
        {
            string[] directions = new string[4];

            List<(int Position, string Direction)> returnValues = new List<(int, string)>();

            int row = (position - 1) / ai.Board.Matrix.GetLength(0);
            int col = (position - 1) % ai.Board.Matrix.GetLength(0);

            // Get the last two positions in the previous guesses list where it was a hit
            int[] LastTwoHits = GetLastTwoHits();
            //Debug.Log("LastTwoHits: " + LastTwoHits[0] + ", " + LastTwoHits[1]);

            // Get the absolute value of the difference between the last two positions that were hit
            int diff = Mathf.Abs(LastTwoHits[0] - LastTwoHits[1]);
            int row1 = (LastTwoHits[0] - 1) / ai.Board.Matrix.GetLength(0);
            int row2 = (LastTwoHits[1] - 1) / ai.Board.Matrix.GetLength(0);
            //Debug.Log("Diff: " + diff);

            // If the difference is 1 then only check in left and right directions
            if (diff == 1 && row1 == row2)
            {
                directions[0] = "East";
                directions[1] = "West";
            }
            // If the difference is 10 then only check in up and down directions
            else if (diff == ai.Board.Matrix.GetLength(0))
            {
                directions[0] = "North";
                directions[1] = "South";
            }
            // If the difference is less than 10 and the positions are in the same row
            else if (diff < ai.Board.Matrix.GetLength(0) && row1 == row2)
            {
                // Verifies if the last two hits were in the same boat by checking each of the positions between them and if any of them weren't a hit then it's not in the same boat
                bool sameBoat = true;
                int smallerNumber = Mathf.Min(LastTwoHits[0], LastTwoHits[1]);
                int largerNumber = Mathf.Max(LastTwoHits[0], LastTwoHits[1]);

                for (int i = smallerNumber; i < largerNumber; i++)
                {
                    if (!ai.previousGuesses.Contains((i, true, false)))
                    {
                        sameBoat = false;
                        break;
                    }
                }

                // If the last two hits were on the same boat then only check in left and right directions, else, check in all directions
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
            // If the last two hits were in the same column
            else if (diff % ai.Board.Matrix.GetLength(0) == 0)
            {
                // Verifies if the last two hits were in the same boat by checking each of the positions between them and if any of them weren't a hit then it's not in the same boat
                bool sameBoat = true;
                int smallerNumber = Mathf.Min(LastTwoHits[0], LastTwoHits[1]);
                int largerNumber = Mathf.Max(LastTwoHits[0], LastTwoHits[1]);

                for (int i = smallerNumber; i < largerNumber; i += ai.Board.Matrix.GetLength(0))
                {
                    if (!ai.previousGuesses.Contains((i, true, false)))
                    {
                        sameBoat = false;
                        break;
                    }
                }

                // If the last two hits were on the same boat then only check in up and down directions, else, check in all directions
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
                directions[1] = "East";
                directions[2] = "South";
                directions[3] = "West";
            }

            // Loop through the array of directions and add the positions to the return list
            foreach (string direction in directions)
            {
                switch (direction)
                {
                    case "North":
                        if (row - 1 >= 0)
                        {
                            returnValues.Add(ValueTuple.Create(ai.Board.Matrix[row - 1, col], direction));
                        }
                        break;
                    case "East":
                        if (col + 1 < ai.Board.Matrix.GetLength(0))
                        {
                            returnValues.Add(ValueTuple.Create(ai.Board.Matrix[row, col + 1], direction));
                        }
                        break;
                    case "South":
                        if (row + 1 < ai.Board.Matrix.GetLength(0))
                        {
                            returnValues.Add(ValueTuple.Create(ai.Board.Matrix[row + 1, col], direction));
                        }
                        break;
                    case "West":
                        if (col - 1 >= 0)
                        {
                            returnValues.Add(ValueTuple.Create(ai.Board.Matrix[row, col - 1], direction));
                        }
                        break;
                }
            }

            // Return the list of positions and the directions to the calling method
            return returnValues;
        }
    }
}