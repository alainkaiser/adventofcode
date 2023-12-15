namespace AdventOfCode;

public sealed class Day11 : BaseDay
{
    private readonly string[] _input;

    public Day11()
    {
        _input = File.ReadAllLines(InputFilePath);
    }

    private record Galaxy(int Row, int Column);

    public override ValueTask<string> Solve_1()
    {
        return Solve();
    }

    public override ValueTask<string> Solve_2()
    {
        return Solve(isPartTwo: true);
    }

    private ValueTask<string> Solve(bool isPartTwo = false)
    {
        var galaxies = GetGalaxies();
        var pairsCalculated = new HashSet<(Galaxy, Galaxy)>();
        var distances = new List<long>();

        var emptyRowIndexes = new List<int>();
        var emptyColumnIndexes = new List<int>();

        // Get all rows that contains no galaxies
        for (var row = 0; row < _input.Length; row++)
        {
            if (_input[row].All(c => c == '.'))
                emptyRowIndexes.Add(row);
        }

        // Get all columns that contains no galaxies
        for (var column = 0; column < _input[0].Length; column++)
        {
            if (_input.All(row => row[column] == '.'))
                emptyColumnIndexes.Add(column);
        }

        foreach (var galaxy in galaxies)
        {
            foreach (var otherGalaxy in galaxies)
            {
                // Skip if the galaxy is the same or if the pair has already been calculated
                if (galaxy == otherGalaxy || pairsCalculated.Contains((galaxy, otherGalaxy)) ||
                    pairsCalculated.Contains((otherGalaxy, galaxy)))
                {
                    continue;
                }

                // Calculate the distance between the two galaxies via the Manhattan distance
                var distance = Math.Abs(galaxy.Row - otherGalaxy.Row) + Math.Abs(galaxy.Column - otherGalaxy.Column);
                distance += emptyRowIndexes.Count(r => r > Math.Min(galaxy.Row, otherGalaxy.Row) &&
                                                       r < Math.Max(galaxy.Row, otherGalaxy.Row)) *
                            (isPartTwo ? 999_999 : 1);
                distance += emptyColumnIndexes.Count(c => c > Math.Min(galaxy.Column, otherGalaxy.Column) &&
                                                          c < Math.Max(galaxy.Column, otherGalaxy.Column)) *
                            (isPartTwo ? 999_999 : 1);

                // Add the distance to the distances list
                distances.Add(distance);

                // Add the pair to the calculated pairs
                pairsCalculated.Add((galaxy, otherGalaxy));
            }
        }

        var sum = distances.Sum();
        return new ValueTask<string>(sum.ToString());
    }

    private List<Galaxy> GetGalaxies()
    {
        var galaxies = new List<Galaxy>();

        for (var row = 0; row < _input.Length; row++)
        {
            for (var column = 0; column < _input[row].Length; column++)
            {
                if (_input[row][column] == '#')
                {
                    galaxies.Add(new Galaxy(row, column));
                }
            }
        }

        return galaxies;
    }
}