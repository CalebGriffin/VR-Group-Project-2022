using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

// This class is used to create the boats and store their data
public class Boat
{
    // Used to store the boat's positions on the board and be accessible to other scripts
    private int[] positions;
    public int[] Positions{ get { return positions; } }

    // Used to store the boat's remaining positions and be accessible to other scripts
    private List<int> remainingPositions = new List<int>();
    public List<int> RemainingPositions { get { return remainingPositions; } }

    // The name of the boat, may be used to identify it
    private string name;
    public string Name{ get { return name; } }

    // The direction of the boat
    private string direction;
    public string Direction{ get { return direction; } }

    // The different constructors for the boat class, used to create the boats in different ways (with fixed positions or random positions)
    #region Constructors
    public Boat(int boatLength)
    {
        this.positions = new int[boatLength];
    }

    public Boat(string name, int boatLength)
    {
        this.positions = new int[boatLength];
        this.name = name;
    }

    public Boat(Board board, int boatLength)
    {
        this.positions = new int[boatLength];
        this.positions = InitializePositions(board, boatLength);
        InitializeRemainingPositions(this.positions);
    }

    public Boat(Board board, string name, int boatLength)
    {
        this.positions = new int[boatLength];
        this.name = name;
        this.positions = InitializePositions(board, boatLength);
        InitializeRemainingPositions(this.positions);
    }

    public Boat(Board board, int boatLength, int[] positions)
    {
        this.positions = InitializeFixedPositions(board, boatLength, positions);
        InitializeRemainingPositions(this.positions);
    }

    public Boat(Board board, string name, int boatLength, int[] positions)
    {
        this.name = name;
        this.positions = InitializeFixedPositions(board, boatLength, positions);
        InitializeRemainingPositions(this.positions);
    }
    #endregion

    // Initialize the positions of the boat with random positions on the board (not overlapping with other boats)
    private int[] InitializePositions(Board board, int boatLength)
    {
        int[,] matrix = board.Matrix;
        int[] boatPositions = new int[boatLength];

        // Will continue to loop until the boat's positions are not overlapping with other boats
        bool emptyPositionFound = false;
        while (emptyPositionFound == false)
        {
            boatPositions = new int[boatLength];

            // The possible directions the boat could be placed from a random position
            List<string> possibleDirections = new List<string>();

            // Pick a random position on the board
            Random random = new Random();
            int row = random.Next(0, matrix.GetLength(1));
            int col = random.Next(0, matrix.GetLength(0));

            // From that position, check which directions the boat could be placed in and add them to the possibleDirections list
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

            // Choose a random direction from the possibleDirections list
            int directionIndex = random.Next(0, possibleDirections.Count);
            
            // Place the boat in the chosen direction
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

            // Assume the boat is valid and check if it is overlapping with other boats, if it is, the boat is not valid and the loop will continue
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

        // When a valid position is found, add the boat's positions to the currentBoatPositions list and the currentBoatPositionsWithBorders list
        board.currentBoatPositions.AddRange(boatPositions);
        board.currentBoatPositionsWithBorders.AddRange(boatPositions);

        // Add the positions around the boat to the currentBoatPositionsWithBorders list
        GetPointsAround(board, boatPositions);

        // Return the boat's positions
        return boatPositions;
    }

    // Initialize the positions of the boat with fixed positions on the board with the given parameters
    private int[] InitializeFixedPositions(Board board, int boatLength, int[] positions)
    {
        int[,] matrix = board.Matrix;
        this.positions = new int[boatLength];

        // Add the given positions to the currentBoatPositions list and the currentBoatPositionsWithBorders list
        board.currentBoatPositions.AddRange(positions);
        board.currentBoatPositionsWithBorders.AddRange(positions);

        // Add the positions around the boat to the currentBoatPositionsWithBorders list
        GetPointsAround(board, positions);

        // Return the boat's positions
        return positions;
    }

    // Adds all of the boat's positions to a list can that can be dynamically changed to track which positions haven't been hit yet
    private void InitializeRemainingPositions(int[] positions)
    {
        this.remainingPositions.AddRange(positions);
    }

    // Adds all of the positions around the boat's positions to the list so that other boats aren't placed at those positions
    private void GetPointsAround(Board board, int[] points)
    {
        int[,] matrix = board.Matrix;

        // Repeat the process for each point in the boat's positions
        for (int i = 0; i < points.Length; i++)
        {
            // Get the row and column on the matrix of the current point
            int row = (points[i] - 1) / matrix.GetLength(0);
            int col = (points[i] - 1) % matrix.GetLength(0);

            // Add the points around the current point to the currentBoatPositionsWithBorders list (including diagonals)
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

    // Adds the given point to the list of positions that boats can't be placed on
    private void AddPointsAround(Board board, int point)
    {
        // If the point is valid and hasn't been added to the list yet, add it
        if (point != 0 && !board.currentBoatPositionsWithBorders.Contains(point))
        {
            board.currentBoatPositionsWithBorders.Add(point);
        }
    }

    // Returns a bool of whether the given position is a part of the boat
    public bool HitCheck(int point)
    {
        if (this.remainingPositions.Contains(point))
        {
            Hit(point);
            return true;
        }
        else
        {
            return false;
        }
    }

    // Removes the given point from the list of remaining positions
    public void Hit(int point)
    {
        this.remainingPositions.Remove(point);
    }

    // Returns a bool of whether the boat is sunk
    public bool SunkCheck()
    {
        return this.remainingPositions.Count == 0;
    }

    // Returns all of the points around the boat's positions when it is sunk so that misses can be placed automatically in those positions
    public int[] Sunk(Board board)
    {
        int[,] matrix = board.Matrix;

        // Create an array to store the positions around the boat's positions with a max length of the maximum number of positions that can be around the boat
        List<int> returnPositions = new List<int>();

        // Repeat the process for each point in the boat's positions
        for (int i = 0; i < this.positions.Length; i++)
        {
            // Get the row and column on the matrix of the current point
            int row = (this.positions[i] - 1) / matrix.GetLength(0);
            int col = (this.positions[i] - 1) % matrix.GetLength(0);

            // Add the points around the current point to the returnPositions list (including diagonals) only if they aren't already in the returnPositions list or the boat's positions
            for (int j = 0; j < board.Directions.Length; j++)
            {
                switch (board.Directions[j])
                {
                    case "upLeft":
                        if (row - 1 >= 0 && col - 1 >= 0)
                        {
                            if (!this.positions.Contains(matrix[row - 1, col - 1]) && !returnPositions.Contains(matrix[row - 1, col - 1]))
                            {
                                returnPositions.Add(matrix[row - 1, col - 1]);
                            }
                        }
                        break;
                    case "up":
                        if (row - 1 >= 0)
                        {
                            if (!this.positions.Contains(matrix[row - 1, col]) && !returnPositions.Contains(matrix[row - 1, col]))
                            {
                                returnPositions.Add(matrix[row - 1, col]);
                            }
                        }
                        break;
                    case "upRight":
                        if (row - 1 >= 0 && col + 1 < matrix.GetLength(0))
                        {
                            if (!this.positions.Contains(matrix[row - 1, col + 1]) && !returnPositions.Contains(matrix[row - 1, col + 1]))
                            {
                                returnPositions.Add(matrix[row - 1, col + 1]);
                            }
                        }
                        break;
                    case "downLeft":
                        if (row + 1 < matrix.GetLength(0) && col - 1 >= 0)
                        {
                            if (!this.positions.Contains(matrix[row + 1, col - 1]) && !returnPositions.Contains(matrix[row + 1, col - 1]))
                            {
                                returnPositions.Add(matrix[row + 1, col - 1]);
                            }
                        }
                        break;
                    case "down":
                        if (row + 1 < matrix.GetLength(0))
                        {
                            if (!this.positions.Contains(matrix[row + 1, col]) && !returnPositions.Contains(matrix[row + 1, col]))
                            {
                                returnPositions.Add(matrix[row + 1, col]);
                            }
                        }
                        break;
                    case "downRight":
                        if (row + 1 < matrix.GetLength(0) && col + 1 < matrix.GetLength(0))
                        {
                            if (!this.positions.Contains(matrix[row + 1, col + 1]) && !returnPositions.Contains(matrix[row + 1, col + 1]))
                            {
                                returnPositions.Add(matrix[row + 1, col + 1]);
                            }
                        }
                        break;
                    case "left":
                        if (col - 1 >= 0)
                        {
                            if (!this.positions.Contains(matrix[row, col - 1]) && !returnPositions.Contains(matrix[row, col - 1]))
                            {
                                returnPositions.Add(matrix[row, col - 1]);
                            }
                        }
                        break;
                    case "right":
                        if (col + 1 < matrix.GetLength(0))
                        {
                            if (!this.positions.Contains(matrix[row, col + 1]) && !returnPositions.Contains(matrix[row, col + 1]))
                            {
                                returnPositions.Add(matrix[row, col + 1]);
                            }
                        }
                        break;
                    default:
                        break;
                }
            }
        }

        // Return the positions around the boat's positions
        return returnPositions.ToArray();
    }
}