using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;
using static BattleshipAI.AIHelper;

namespace BattleshipAI
{
    public class AIDebug : MonoBehaviour
    {
        #region Variable Declarations
        public static AI ai;

        public static GameObject textParent;

        #region Debug Objects
        // Cube prefabs to display different objects on the board
        public GameObject shipCube;
        public GameObject borderCube;
        public GameObject hitCube;
        public GameObject missCube;
        #endregion
        #endregion

        void Update()
        {
            var duplicates = ai.previousGuesses
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
                Debug.LogError("AI Duplicate positions found: " + duplicatesString);
            }
        }

        public static void DisplayBoats()
        {
            // TESTING // REMOVE
            // Prints out the boats and their positions
            string boatPositions = "";
            foreach (Boat boat in ai.Boats)
            {
                //Debug.Log(boat.Name + ":");
                foreach (int position in boat.Positions)
                {
                    boatPositions += position + ", ";
                    int row = (position - 1) / ai.Board.Matrix.GetLength(0);
                    int col = (position - 1) % ai.Board.Matrix.GetLength(0);
                    //GameObject.Instantiate(shipCube, new Vector3(row, 0, col), Quaternion.identity);
                }
                //Debug.Log(boatPositions);
                boatPositions = "";
            }
        }

        public static void DisplayBorders()
        {
            // Prints out the borders
            string boatPositionsWithBorders = "";
            foreach (int position in ai.Board.currentBoatPositionsWithBorders)
            {
                boatPositionsWithBorders += position + ", ";
                if (!ai.Board.currentBoatPositions.Contains(position))
                {
                    int row = (position - 1) / ai.Board.Matrix.GetLength(0);
                    int col = (position - 1) % ai.Board.Matrix.GetLength(0);
                    //GameObject.Instantiate(borderCube, new Vector3(row, 0, col), Quaternion.identity);
                }
            }
            Debug.Log(boatPositionsWithBorders);
        }


        // TESTING // REMOVE
        // Prints out the un-hit positions for each boat
        [ContextMenu(nameof(PrintRemainingPositions))]
        public static void PrintRemainingPositions()
        {
            foreach (Boat boat in ai.Boats)
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
        public static void PrintPreviousGuesses()
        {
            string previousGuessesString = "";
            for (int i = 0; i < ai.previousGuesses.Count; i++)
            {
                previousGuessesString = i + ", " + ai.previousGuesses[i].Position + ", " + ai.previousGuesses[i].Hit + ", " + ai.previousGuesses[i].Sunk;
                Debug.Log(previousGuessesString);
            }
        }

        [ContextMenu(nameof(PrintTargetStack))]
        public static void PrintTargetStack()
        {
            string targetStackString = "";
            foreach ((int Position, string Direction) target in ai.targetStack)
            {
                targetStackString = target.Position + ", " + target.Direction;
                Debug.Log(targetStackString);
            }
        }

        [ContextMenu(nameof(PrintUncheckedPositions))]
        public static void PrintUncheckedPositions()
        {
            string uncheckedPositionsString = "";
            foreach ((int Position, int Weight) uncheckedPosition in ai.uncheckedPositions)
            {
                uncheckedPositionsString = uncheckedPosition.Position + ", " + uncheckedPosition.Weight;
                Debug.Log(uncheckedPositionsString);
            }
        }

        [ContextMenu(nameof(DisplayPositionWeights))]
        public static void DisplayPositionWeights()
        {
            int currentHighestWeight = ai.uncheckedPositions.Max(x => x.Weight);

            foreach (int i in ai.Board.Matrix)
            {
                if (UncheckedPositionsContains(i))
                {
                    textParent.transform.Find(i.ToString()).GetChild(0).GetComponent<TextMeshProUGUI>().text = ai.uncheckedPositions.Find(x => x.Position == i).Weight.ToString();
                    textParent.transform.Find(i.ToString()).GetChild(0).GetComponent<TextMeshProUGUI>().color = Color.black;

                    if (ai.uncheckedPositions.Find(x => x.Position == i).Weight == currentHighestWeight && currentHighestWeight != 0)
                    {
                        textParent.transform.Find(i.ToString()).GetChild(0).GetComponent<TextMeshProUGUI>().color = Color.green;
                    }
                }
                else
                {
                    textParent.transform.Find(i.ToString()).GetChild(0).GetComponent<TextMeshProUGUI>().text = "0";
                    textParent.transform.Find(i.ToString()).GetChild(0).GetComponent<TextMeshProUGUI>().color = Color.black;
                }
            }
        }

        [ContextMenu(nameof(PrintDeviation))]
        public static void PrintDeviation()
        {
            Debug.Log("Deviation: " + ai.Deviation);
            Debug.Log("Starting Deviation: " + ai.StartingDeviation);
        }
    }
}
