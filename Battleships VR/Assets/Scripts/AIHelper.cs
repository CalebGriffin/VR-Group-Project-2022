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

        public static int[] GetLastTwoHits()
        {
            var temp = new List<(int Position, bool Hit, bool Sunk)>(ai.previousGuesses);

            var lastPosition = temp.LastOrDefault(x => x.Hit == true);

            temp.Remove(lastPosition);

            var lastPosition2 = temp.LastOrDefault(x => x.Hit == true);

            return new int[] { lastPosition.Position, lastPosition2.Position };
        }

        [ContextMenu(nameof(DifferentWeights))]
        public static int DifferentWeights()
        {
            List<int> weights = new List<int>();

            foreach ((int Position, int Weight) i in ai.uncheckedPositions)
            {
                if (!weights.Contains(i.Weight))
                {
                    weights.Add(i.Weight);
                }
            }

            //Debug.Log("Different Weights: " + weights.Count);
            return weights.Count;
        }

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

        public IEnumerator Reset()
        {
            ai.resetting = true;
            // Reset from the previous time the game was played
            ai.uncheckedPositions.Clear();
            ai.TargetMode = false;
            ai.targetStack.Clear();
            ai.previousGuesses.Clear();
            ai.Board.currentBoatPositions.Clear();
            ai.Board.currentBoatPositionsWithBorders.Clear();

            // Initialize the position guesses list with the values from the board
            foreach (int i in ai.Board.Matrix)
            {
                ai.uncheckedPositions.Add((i, 0));
            }
            //DisplayPositionWeights();

            ai.CreateBoats();
            yield return new WaitForSeconds(0.001f);
            DisplayBoats();
            if (ai.previousGuesses.Count > 0 && ai.previousGuesses[0].Sunk)
            {
                // TESTING // REMOVE
                //Debug.Log("Found the error");
                ai.previousGuesses.RemoveAt(0);
            }
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
            else if (diff < ai.Board.Matrix.GetLength(0) && row1 == row2)
            {
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
            else if (diff % ai.Board.Matrix.GetLength(0) == 0)
            {
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

            // The positions are added to the target stack in the reverse order that they are checked
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

            return returnValues;
        }
    }
}