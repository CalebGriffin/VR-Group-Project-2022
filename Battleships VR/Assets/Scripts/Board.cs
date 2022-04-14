using System.Collections;
using System.Collections.Generic;

// This class is used to create a board and store its data
public class Board
{
    // A 2D array that stores the board's data
    private int[,] matrix;
    public int[,] Matrix { get { return matrix; } }
    
    // All the possible directions for the boats to get the points around them
    private string[] directions = new string[]
    {
        "up",
        "upLeft",
        "upRight",
        "down",
        "downLeft",
        "downRight",
        "left",
        "right"
    };
    public string[] Directions { get { return directions; } }

    // Lists storing the positions of all the boats and the points around them on the board
    public List<int> currentBoatPositions = new List<int>();
    public List<int> currentBoatPositionsWithBorders = new List<int>();

    // Constructor to create the board
    public Board(int size)
    {
        this.matrix = InitializeMatrix(size);
    }

    // Create the board's matrix and fills it with integers from 1 to size^2 (probably 100)
    private int[,] InitializeMatrix(int size)
    {
        matrix = new int[size, size];

        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                matrix[i, j] = i * size + j + 1;
            }
        }

        return matrix;
    }
}
