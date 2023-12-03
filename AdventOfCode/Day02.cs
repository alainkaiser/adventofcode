using System.Text.RegularExpressions;

namespace AdventOfCode;

public sealed class Day02 : BaseDay
{
    private readonly string[] _input;

    private const int RedConstraint = 12;
    private const int GreenConstraint = 13;
    private const int BlueConstraint = 14;

    private readonly Dictionary<string, List<(string, int)>> _games = new();

    public Day02()
    {
        _input = File.ReadAllLines(InputFilePath);
        _games = GetParsedGameInput(_input);
    }

    public override ValueTask<string> Solve_1()
    {
        var sum = (from game in _games
                let validGame = game.Value.All(DoesBlockFullfillConstraints)
                where validGame
                select int.Parse(game.Key))
            .Sum();

        return new ValueTask<string>(sum.ToString());
    }

    public override ValueTask<string> Solve_2()
    {
        var sum = (from game in _games
                let maxBlue = game.Value.Where(b => b.Item1 == "blue").Max(b => b.Item2)
                let maxRed = game.Value.Where(b => b.Item1 == "red").Max(b => b.Item2)
                let maxGreen = game.Value.Where(b => b.Item1 == "green").Max(b => b.Item2)
                let result = maxBlue * maxRed * maxGreen
                select result
            ).Sum();

        return new ValueTask<string>(sum.ToString());
    }

    private Dictionary<string, List<(string, int)>> GetParsedGameInput(string[] input)
    {
        var gameDict = new Dictionary<string, List<(string, int)>>();
        const string pattern = @"Game\s(\d+):\s(([^;]+);?\s?)+";

        foreach (var line in input)
        {
            var match = Regex.Match(line, pattern);
            var gameId = match.Groups[1].Value;

            var sets = match.Groups[2].Captures;
            foreach (var set in sets)
            {
                var setString = set.ToString()?.Replace(";", "").Trim();
                var setBlocks = setString?.Split(",").ToList();

                if (setBlocks != null)
                    foreach (var block in setBlocks)
                    {
                        var blockParts = block.Trim().Split(" ");
                        var color = blockParts[1];
                        var count = blockParts[0];

                        // Add to the dictionary
                        if (gameDict.ContainsKey(gameId))
                        {
                            gameDict[gameId].Add((color, int.Parse(count)));
                        }
                        else
                        {
                            gameDict.Add(gameId, new List<(string, int)> { (color, int.Parse(count)) });
                        }
                    }
            }
        }

        return gameDict;
    }

    private bool DoesBlockFullfillConstraints((string, int) block)
    {
        var color = block.Item1;
        var count = block.Item2;

        return color switch
        {
            "red" => count <= RedConstraint,
            "green" => count <= GreenConstraint,
            "blue" => count <= BlueConstraint,
            _ => false
        };
    }
}