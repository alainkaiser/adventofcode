using System.Text.RegularExpressions;

namespace AdventOfCode;

public sealed partial class Day03 : BaseDay
{
    private readonly string[] _input;
    private readonly string[,] _matrix;

    [GeneratedRegex(@"\d+")]
    private static partial Regex DigitRegex();

    [GeneratedRegex(@"[^a-zA-Z0-9\.]")]
    private static partial Regex SymbolRegex();

    [GeneratedRegex(@"\*")]
    private static partial Regex GearRegex();

    private enum Range
    {
        Above,
        Below
    }

    public Day03()
    {
        _input = File.ReadAllLines(InputFilePath);
        _matrix = ParseInputToMatrix(_input);
    }

    public override ValueTask<string> Solve_1()
    {
        var sum = _input
            .Select(t => DigitRegex()
                .Matches(t))
            .Select((matches, i) =>
                (from match in matches
                    let isAdjacent = IsAdjacentToSymbol(i, match, _matrix)
                    where isAdjacent
                    select int.Parse(match.Value)).Sum())
            .Sum();

        return new ValueTask<string>(sum.ToString());
    }

    public override ValueTask<string> Solve_2()
    {
        var sum = 0;

        for (var i = 0; i < _input.Length; i++)
        {
            foreach (Match match in GearRegex().Matches(_input[i]))
            {
                var numberIndexesRowAbove = new List<Tuple<int, int, int>>();
                var numberIndexesRowBelow = new List<Tuple<int, int, int>>();
                var numberIndexesSameRow = new List<Tuple<int, int, int>>();
                

                if (i > 0)
                {
                    numberIndexesRowAbove = DigitRegex().Matches(_input[i - 1])
                        .Select(m => new Tuple<int, int, int>(m.Index, m.Index + m.Length - 1, int.Parse(m.Value)))
                        .ToList();
                }

                if (i < _input.Length - 1)
                {
                    numberIndexesRowBelow = DigitRegex().Matches(_input[i + 1])
                        .Select(m => new Tuple<int, int, int>(m.Index, m.Index + m.Length - 1, int.Parse(m.Value)))
                        .ToList();
                }

                numberIndexesSameRow = DigitRegex().Matches(_input[i])
                    .Select(m => new Tuple<int, int, int>(m.Index, m.Index + m.Length - 1, int.Parse(m.Value)))
                    .ToList();

                // Now check if the gear connects two adjacent numbers
                var numberIndexes = numberIndexesRowAbove
                    .Concat(numberIndexesRowBelow)
                    .Concat(numberIndexesSameRow)
                    .Where(t => match.Index >= t.Item1 -1 && match.Index <= t.Item2 + 1)
                    .ToList();

                if (numberIndexes.Count == 2)
                {
                    sum += numberIndexes.First().Item3 * numberIndexes.Last().Item3;
                }
            }
        }


        return new ValueTask<string>(sum.ToString());
    }

    private string[,] ParseInputToMatrix(IReadOnlyList<string> input)
    {
        var matrix = new string[input.Count, input[0].Length];

        for (var i = 0; i < input.Count; i++)
        {
            for (var j = 0; j < input[i].Length; j++)
            {
                matrix[i, j] = input[i][j].ToString();
            }
        }

        return matrix;
    }

    private bool IsAdjacentToSymbol(int lineIndex, Match match, string[,] matrix)
    {
        var startIndex = match.Index;
        var numberLength = match.Index + match.Length;

        // Check for adjacent symbols on the same line (left and right)
        var adjacentLeft = startIndex - 1 >= 0 && IsSymbol(matrix[lineIndex, startIndex - 1]);
        var adjacentRight = numberLength < matrix.GetLength(1) && IsSymbol(matrix[lineIndex, numberLength]);

        // Check for adjacent symbols above and below
        var adjacentAbove = CheckRangeAboveOrBelow(Range.Above, lineIndex, startIndex, matrix, numberLength);
        var adjacentBelow = CheckRangeAboveOrBelow(Range.Below, lineIndex, startIndex, matrix, numberLength);

        return adjacentLeft || adjacentRight || adjacentAbove || adjacentBelow;
    }

    private bool CheckRangeAboveOrBelow(Range range, int lineIndex, int startIndex, string[,] matrix, int numberLength)
    {
        var isAdjacent = false;
        var lineDelta = range == Range.Above ? -1 : 1;

        var doesFullfullBounds = range == Range.Above
            ? lineIndex + lineDelta >= 0
            : lineIndex + lineDelta < matrix.GetLength(0);

        if (doesFullfullBounds)
        {
            for (var i = startIndex - 1; i < numberLength + 1; i++)
            {
                if (i >= 0 && i < matrix.GetLength(1) && IsSymbol(matrix[lineIndex + lineDelta, i]))
                {
                    isAdjacent = true;
                }
            }
        }

        return isAdjacent;
    }

    private bool IsSymbol(string character) => SymbolRegex().IsMatch(character);
}