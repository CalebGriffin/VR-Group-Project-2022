using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static BattleshipAI.AIHelper;

public class Player : MonoBehaviour
{
    private Board board = new Board(10);
    public Board Board { get { return board; } }

    private List<Boat> boats = new List<Boat>();
    public List<Boat> Boats { get { return boats; } }

    public GameObject modelBoatParent;

    public List<int> uncheckedPositions = new List<int>();

    private List<(int Position, bool Hit, bool Sunk)> previousGuesses = new List<(int Position, bool Hit, bool Sunk)>();

    public GameObject[] modelBoats;
    public BoatPlacer boatPlacer;

    public AI ai;

    // Start is called before the first frame update
    void Start()
    {
        foreach (int i in board.Matrix)
        {
            uncheckedPositions.Add(i);
        }
    }

    [ContextMenu(nameof(PrintShipPositionsAndPositionsAround))]
    private void PrintShipPositionsAndPositionsAround()
    {
        foreach (Boat boat in boats)
        {
            string boatPositionsString = "";
            foreach (int position in boat.Positions)
            {
                boatPositionsString += position + ", ";
            }
            Debug.Log(boat.Name + ": " + boatPositionsString);

            string boatPositionsAroundString = "";
            foreach (int position in board.currentBoatPositionsWithBorders)
            {
                if (!board.currentBoatPositions.Contains(position))
                    boatPositionsAroundString += position + ", ";
            }
            Debug.Log(boat.Name + ": " + boatPositionsAroundString);
        }
    }

    public void AddShip(ModelBoat boat)
    {
        boats.Add(new Boat(this.board, boat.BoatName, boat.Length, boat.positions));
        boat.positionsAround = boats[boats.Count - 1].Sunk(board);
    }

    public void RemoveShip(ModelBoat boat)
    {
        boats.Remove(boats.Find(x => x.Name == boat.BoatName));
        foreach(int position in boat.positions)
        {
            board.currentBoatPositions.Remove(position);
            board.currentBoatPositionsWithBorders.Remove(position);
        }
        foreach(int position in boat.positionsAround)
        {
            board.currentBoatPositionsWithBorders.Remove(position);
        }
    }

    [ContextMenu(nameof(RandomizeButton))]
    public void RandomizeButton()
    {
        // Clear the list of boats and populate it with random boats (the same way that the AI creates the boats)
        board.currentBoatPositions.Clear();
        board.currentBoatPositionsWithBorders.Clear();
        boats.Clear();
        boats.Add(new Boat(board, "Carrier", 5));
        boats.Add(new Boat(board, "Battleship", 4));
        boats.Add(new Boat(board, "Cruiser", 3));
        boats.Add(new Boat(board, "Submarine", 3));
        boats.Add(new Boat(board, "Destroyer", 2));

        // Move the model boats to their positions
        foreach (Boat boat in boats)
        {
            int x = UnityEngine.Random.Range(1, 3);
            int rotation = 0;
            int position = 0;

            if (boat.Positions.Max() - boat.Positions.Min() < board.Matrix.GetLength(0))
            {
                if (x == 1)
                {
                    position = boat.Positions.Min();
                    rotation = 90;
                }
                else
                {
                    position = boat.Positions.Max();
                    rotation = -90;
                }
            }
            else
            {
                if (x == 1)
                {
                    position = boat.Positions.Min();
                    rotation = 180;
                }
                else
                {
                    position = boat.Positions.Max();
                    rotation = 0;
                }
            }

            modelBoatParent.GetComponent<ModelBoatParent>().PlaceBoat(boat.Name, position, rotation, boat, this);
        }
    }

    [ContextMenu(nameof(ClearButton))]
    public void ClearButton()
    {
        board.currentBoatPositions.Clear();
        board.currentBoatPositionsWithBorders.Clear();
        boats.Clear();

        modelBoatParent.GetComponent<ModelBoatParent>().ResetAllBoatPositions();
    }
    
    [ContextMenu(nameof(ConfirmButton))]
    public void ConfirmButton()
    {
        if (boats.Count < 5)
        {
            return;
        }

        foreach (GameObject modelBoat in modelBoats)
        {
            boatPlacer.PlaceBoat(modelBoat.GetComponent<ModelBoat>().BoatName, modelBoat.transform);
        }
        gVar.playerTurn = true;
    }


    public (int, bool, bool) ShotFired(int position)
    {
        // Theses are the return values
        bool hit = false;
        bool sunk = false;

        // Foreach of the boats in the boat list, if they have been hit then set the bool to true and check if they have been sunk then set the bool to true
        foreach (Boat boat in boats)
        {
            if (boat.HitCheck(position))
            {
                hit = true;

                HitOrMissManager.instance.ResultOfAttack("AI", position, true);

                // Enable the hit object at the position in the sea and on the mini board

                if (boat.SunkCheck())
                {
                    sunk = true;

                    // Get all of the points around the boat and place miss objects around them because the boats can't be there
                    int[] positionsAround = boat.Sunk(this.board);
                    foreach (int positionAround in positionsAround)
                    {
                        if (positionAround != 0 && UncheckedPositionsContains(positionAround))
                        {
                            // Enable the miss object at the position in the sea and on the mini board
                            HitOrMissManager.instance.ResultOfAttack("AI", positionAround, false, true);
                        }
                    }

                    ai.RemoveSunkPoints(positionsAround);

                    WinCheck();
                }

                break;
            }
        }

        if (!hit)
        {
            // Enable the miss object at the position in the sea and on the mini board
            HitOrMissManager.instance.ResultOfAttack("AI", position, false);
        }

        return ValueTuple.Create(position, hit, sunk);
    }

    public void RemoveSunkPoints(int[] pointsAround)
    {
        foreach (int i in pointsAround)
        {
            if (uncheckedPositions.Contains(i))
            {
                uncheckedPositions.Remove(i);
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
            // Call a GameOver method to end the game because the AI has won
        }
    }

    public void Decision(int position)
    {
        Debug.Log("Player shot at " + position.ToString());
        // Call the ShotFired method on the AI and get the return values and add them to the previous guesses list
        previousGuesses.Add(ai.ShotFired(position));
        Debug.Log("AI Returned " + previousGuesses.Last().Hit + " at position " + previousGuesses.Last().Position);
        uncheckedPositions.Remove(position);

        // Calculate where to fire the guns and call the event to fire them
        int row = (position - 1) / board.Matrix.GetLength(0);
        int col = (position - 1) % board.Matrix.GetLength(0);
        int boatID = -1;
        int amount = 0;

        if (boats.Find(x => x.Name == "Battleship").RemainingPositions.Count > 0)
        {
            boatID = 3;
        }
        else if (boats.Find(x => x.Name == "Cruiser").RemainingPositions.Count > 0)
        {
            boatID = 2;
        }
        else if (boats.Find(x => x.Name == "Destroyer").RemainingPositions.Count > 0)
        {
            boatID = 0;
        }
        else if (boats.Find(x => x.Name == "Carrier").RemainingPositions.Count > 0)
        {
            boatID = 4;
        }
        else
        {
            boatID = 1;
        }

        switch (boatID)
        {
            case 2:
            case 3:
                amount = 2;
                break;

            case 0:
                amount = 1;
                break;
        }
        
        GameFeedbackEvents.instance.FireGuns(boatID, new Vector3((row * 60) - 270, 0, (col * 60) + 1550), amount);

        gVar.playerTurn = false;
    }
}