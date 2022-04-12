using System;
using System.Collections;
using System.Collections.Generic;

public class Boat
{
    private int[] positions;

    private string name;

    public Boat(int boatLength)
    {
        positions = new int[boatLength];
    }

    public Boat(string name, int boatLength)
    {
        positions = new int[boatLength];
        this.name = name;
    }

    public Boat(Board board, int boatLength)
    {
        positions = new int[boatLength];
        InitializePositions(board, boatLength);
    }

    public Boat(Board board, string name, int boatLength)
    {
        positions = new int[boatLength];
        this.name = name;
        InitializePositions(board, boatLength);
    }

    private int[] InitializePositions (Board board, int boatLength)
    {
        int[,] matrix = board.Matrix;
        int[] boatPositions = new int[boatLength];
        bool emptyPositionFound = false;
        while (emptyPositionFound == false)
        {
            boatPositions = new int[boatLength];
            List<string> possibleDirections = new List<string>();

            Random random = new Random();

            int row = random.Next(0, matrix.GetLength(1));
            int col = random.Next(0, matrix.GetLength(0));

            if (row + boatLength <= matrix.GetLength(1))
            {
                possibleDirections.Add("right");
            }
            if (row - boatLength >= 0)
            {
                possibleDirections.Add("left");
            }
            
            if (col + boatLength <= matrix.GetLength(0))
            {
                possibleDirections.Add("down");
            }

            if (col - boatLength >= 0)
            {
                possibleDirections.Add("up");
            }

            int directionIndex = random.Next(0, possibleDirections.Count);
            
            switch (possibleDirections[directionIndex])
            {
                case "up":
                    for (int i = 0; i < boatLength; i++)
                    {
                        boatPositions[i] = matrix[col - i, row];
                    }
                    break;
                
                case "down":
                    for (int i = 0; i < boatLength; i++)
                    {
                        boatPositions[i] = matrix[col + i, row];
                    }
                    break;
                
                case "left":
                    for (int i = 0; i < boatLength; i++)
                    {
                        boatPositions[i] = matrix[col, row - i];
                    }
                    break;
                
                case "right":
                    for (int i = 0; i < boatLength; i++)
                    {
                        boatPositions[i] = matrix[col, row + i];
                    }
                    break;
                
                default:
                    break;
            }

            emptyPositionFound = true;

            foreach (int position in boatPositions)
            {
                if (board.currentBoatPositionsWithBorders.Contains(position))
                {
                    emptyPositionFound = false;
                    break;
                }
            }
        }

        board.currentBoatPositions.AddRange(boatPositions);
        board.currentBoatPositionsWithBorders.AddRange(boatPositions);
        GetPointsAround(board, boatPositions);
        return boatPositions;
    }

    private void GetPointsAround(Board board, int[] points)
    {
        int[,] matrix = board.Matrix;

        for (int i = 0; i < points.Length; i++)
        {
            int row = (points[i] - 1) / matrix.GetLength(0);
            int col = (points[i] - 1) % matrix.GetLength(0);

            for (int j = 0; j < board.Directions.Length; j++)
            {
                switch (board.Directions[j])
                {
                    case "upLeft":
                        if (row - 1 >= 0 && col - 1 >= 0)
                        {
                            AddPointsAround(board, matrix[row - 1, col - 1]);
                        }
                        break;

                    case "up":
                        if (row - 1 >= 0)
                        {
                            AddPointsAround(board, matrix[row - 1, col]);
                        }
                        break;

                    case "upRight":
                        if (row - 1 >= 0 && col + 1 < matrix.GetLength(0))
                        {
                            AddPointsAround(board, matrix[row - 1, col + 1]);
                        }
                        break;

                    case "downLeft":
                        if (row + 1 < matrix.GetLength(0) && col - 1 >= 0)
                        {
                            AddPointsAround(board, matrix[row + 1, col - 1]);
                        }
                        break;

                    case "down":
                        if (row + 1 < matrix.GetLength(0))
                        {
                            AddPointsAround(board, matrix[row + 1, col]);
                        }
                        break;
                    
                    case "downRight":
                        if (row + 1 < matrix.GetLength(0) && col + 1 < matrix.GetLength(0))
                        {
                            AddPointsAround(board, matrix[row + 1, col + 1]);
                        }
                        break;

                    case "left":
                        if (col - 1 >= 0)
                        {
                            AddPointsAround(board, matrix[row, col - 1]);
                        }
                        break;

                    case "right":
                        if (col + 1 < matrix.GetLength(0))
                        {
                            AddPointsAround(board, matrix[row, col + 1]);
                        }
                        break;

                    default:
                        break;
                }
            }
        }
    }

    private void AddPointsAround(Board board, int point)
    {
        if (point != 0 && !board.currentBoatPositionsWithBorders.Contains(point))
        {
            board.currentBoatPositionsWithBorders.Add(point);
        }
    }
}