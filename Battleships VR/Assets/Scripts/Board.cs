using System.Collections;
using System.Collections.Generic;

public class Board
{
    private int[,] matrix;
    public int[,] Matrix { get { return matrix; } }
    
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

    public List<int> currentBoatPositions = new List<int>();
    public List<int> currentBoatPositionsWithBorders = new List<int>();

    public Board(int size)
    {
        matrix = new int[size, size];
        InitializeMatrix(size);
    }

    private void InitializeMatrix(int size)
    {
        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                matrix[i, j] = i * size + j + 1;
            }
        }
    }
}
