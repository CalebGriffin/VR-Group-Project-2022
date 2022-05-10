using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private Board board = new Board(10);
    public Board Board { get { return board; } }

    private List<Boat> boats = new List<Boat>();
    public List<Boat> Boats { get { return boats; } }

    private List<(int Position, bool Hit, bool Sunk)> previousGuesses = new List<(int Position, bool Hit, bool Sunk)>();

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void AddShip(ModelBoat boat)
    {
        boats.Add(new Boat(this.board, boat.Name, boat.Length, boat.positions));
    }

    public void RemoveShip(ModelBoat boat)
    {
        boats.Remove(boats.Find(x => x.Name == boat.Name));
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

                // Enable the hit object at the position in the sea and on the mini board

                if (boat.SunkCheck())
                {
                    sunk = true;

                    // Get all of the points around the boat and place miss objects around them because the boats can't be there
                    int[] positionsAround = boat.Sunk(this.board);
                    foreach (int positionAround in positionsAround)
                    {
                        if (positionAround != 0)
                        {
                            // Enable the miss object at the position in the sea and on the mini board
                        }
                    }

                    WinCheck();
                }

                break;
            }
        }

        if (!hit)
        {
            // Enable the miss object at the position in the sea and on the mini board
        }

        return ValueTuple.Create(position, hit, sunk);
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
        // Call the ShotFired method on the AI and get the return values and add them to the previous guesses list
    }
}