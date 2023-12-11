namespace AdventOfCode;

public sealed partial class Day10 : BaseDay
{
    private readonly string[] _input;

    public Day10()
    {
        _input = File.ReadAllLines(InputFilePath);
    }

    private enum PipeType
    {
        Vertical,
        Horizontal,
        NorthEast,
        NorthWest,
        SouthEast,
        SouthWest,
        Ground
    }

    private enum HorizontalDirection
    {
        Left,
        Right,
        Unknown
    }

    private enum VerticalDirection
    {
        Up,
        Down,
        Unknown
    }

    public override ValueTask<string> Solve_1()
    {
        var (map, startingCoordinates) = ParseMap();
        var adjacentElements =
            AdjacentElements(map, startingCoordinates.Item1, startingCoordinates.Item2)
                .Where(i => map[startingCoordinates.Item1, startingCoordinates.Item2] != '.')
                .ToList();

        var numberOfStepsList = new List<int>();

        foreach (var adjacentElement in adjacentElements)
        {
            var numberOfSteps = 1;
            var visited = new HashSet<(int, int)>(); // Initialize visited set

            // Try catch to block calculation of invalid adjacent elements (too lazy to check)
            try
            {
                var (horizontalDirection, verticalDirection) =
                    GetDirectionsBasedOnTwoCoordinates(startingCoordinates, adjacentElement.Item2);

                TravelMap(map, adjacentElement.Item2, horizontalDirection, verticalDirection, ref numberOfSteps, visited);
            }
            catch (Exception)
            {
                continue;
            }

            numberOfStepsList.Add(numberOfSteps);
        }

        var min = numberOfStepsList.Max();
        var result = (min / 2) + (min % 2);

        return new ValueTask<string>(result.ToString());
    }

    public override ValueTask<string> Solve_2()
    {
        return new ValueTask<string>("Too much for my brain to handle.");
    }

    private void TravelMap(char[,] map, (int x, int y) currentPosition, HorizontalDirection horizontalDirection,
        VerticalDirection verticalDirection, ref int numberOfSteps, HashSet<(int, int)> visited)
    {
        // Check if the position has been visited and we are in an infinite loop
        if (visited.Contains(currentPosition))
            return;

        visited.Add(currentPosition);

        // Get the current point based on its coordinates
        var point = map[currentPosition.x, currentPosition.y];

        // Calculate the next position based on the current point
        var nextPosition = CalculateNextPosition(map, currentPosition, point, horizontalDirection, verticalDirection);
        var nextPoint = map[nextPosition.Item1, nextPosition.Item2];

        if (nextPoint != 'S')
        {
            numberOfSteps++;
            // Calculate directions based on the current point and the next point
            var (newHorizontalDirection, newVerticalDirection) =
                GetDirectionsBasedOnTwoCoordinates(currentPosition, nextPosition);
            TravelMap(map, nextPosition, newHorizontalDirection, newVerticalDirection, ref numberOfSteps, visited);
        }
    }

    private (int x, int y) CalculateNextPosition(char[,] map, (int x, int y) currentPosition, char currentPoint,
        HorizontalDirection horizontalDirection, VerticalDirection verticalDirection)
    {
        var (x, y) = currentPosition;
        var type = GetPipeType(currentPoint);

        var (nextX, nextY) = GetNextPositionsCoordinates(type, horizontalDirection, verticalDirection);
        var nextPosition = (x + nextX, y + nextY);
        return nextPosition;
    }

    private (int row, int col) GetNextPositionsCoordinates(PipeType type, HorizontalDirection horizontalDirection,
        VerticalDirection verticalDirection)
    {
        return type switch
        {
            PipeType.Vertical => verticalDirection switch
            {
                VerticalDirection.Up => (-1, 0),
                VerticalDirection.Down => (1, 0),
                _ => throw new ArgumentOutOfRangeException(nameof(verticalDirection), verticalDirection, null)
            },
            PipeType.Horizontal => horizontalDirection switch
            {
                HorizontalDirection.Left => (0, -1),
                HorizontalDirection.Right => (0, 1),
                _ => throw new ArgumentOutOfRangeException(nameof(horizontalDirection), horizontalDirection, null)
            },
            PipeType.NorthEast => horizontalDirection == HorizontalDirection.Left ? (-1, 0) : (0, 1),
            PipeType.NorthWest => horizontalDirection == HorizontalDirection.Right ? (-1, 0) : (0, -1),
            PipeType.SouthEast => horizontalDirection == HorizontalDirection.Left ? (1, 0) : (0, 1),
            PipeType.SouthWest => horizontalDirection == HorizontalDirection.Right ? (1, 0) : (0, -1),
            PipeType.Ground => (0, 0),
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
        };
    }


    private (HorizontalDirection, VerticalDirection) GetDirectionsBasedOnTwoCoordinates((int row1, int col1) first,
        (int row2, int col2) second)
    {
        var (row1, col1) = first;
        var (row2, col2) = second;

        var horizontalDirection = col1 == col2 ? HorizontalDirection.Unknown :
            col1 > col2 ? HorizontalDirection.Left : HorizontalDirection.Right;
        var verticalDirection = row1 == row2 ? VerticalDirection.Unknown :
            row1 > row2 ? VerticalDirection.Up : VerticalDirection.Down;

        return (horizontalDirection, verticalDirection);
    }

    private (char[,], (int, int)) ParseMap()
    {
        var map = new char[_input.Length, _input[0].Length];
        var startingCoordinates = (0, 0);

        for (var i = 0; i < _input.Length; i++)
        {
            for (var j = 0; j < _input[0].Length; j++)
            {
                map[i, j] = _input[i][j];
                if (_input[i][j] == 'S')
                {
                    startingCoordinates = (i, j);
                }
            }
        }

        return (map, startingCoordinates);
    }

    private PipeType GetPipeType(char pipe)
    {
        return pipe switch
        {
            '|' => PipeType.Vertical,
            '-' => PipeType.Horizontal,
            'L' => PipeType.NorthEast,
            'J' => PipeType.NorthWest,
            '7' => PipeType.SouthWest,
            'F' => PipeType.SouthEast,
            '.' => PipeType.Ground,
            _ => throw new ArgumentOutOfRangeException(nameof(pipe), pipe, null)
        };
    }

    private static IEnumerable<(T, (int row, int column))> AdjacentElements<T>(T[,] arr, int row, int column)
    {
        var rows = arr.GetLength(0);
        var columns = arr.GetLength(1);

        for (var j = row - 1; j <= row + 1; j++)
        {
            for (var i = column - 1; i <= column + 1; i++)
            {
                // Check if the element is within bounds and not the center element
                if (i >= 0 && j >= 0 && i < columns && j < rows && !(j == row && i == column))
                {
                    // Additional check to exclude diagonals
                    if (i == column || j == row)
                    {
                        yield return (arr[j, i], (j, i));
                    }
                }
            }
        }
    }
}