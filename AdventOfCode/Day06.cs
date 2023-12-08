using System.Text.RegularExpressions;

namespace AdventOfCode;

public sealed partial class Day06 : BaseDay
{
    private readonly string[] _input;

    [GeneratedRegex(@"\d+")]
    private static partial Regex DigitRegex();

    private record Race(long Duration, long RecordDistance);

    public Day06()
    {
        _input = File.ReadAllLines(InputFilePath);
    }

    public override ValueTask<string> Solve_1()
    {
        var races = ParseRaces(_input);
        var winningWaysPerRace = races.Values
            .Select(GetWinningWays)
            .ToList();

        var result = winningWaysPerRace.Aggregate((total, next) => total * next);

        return new ValueTask<string>(result.ToString());
    }

    public override ValueTask<string> Solve_2()
    {
        var races = ParseRaces(_input);

        var realRaceTime = string.Empty;
        var realRaceDistance = string.Empty;

        foreach (var race in races.Values)
        {
            realRaceTime += race.Duration;
            realRaceDistance += race.RecordDistance;
        }

        var winningWays = GetWinningWays(new Race(long.Parse(realRaceTime), long.Parse(realRaceDistance)));

        return new ValueTask<string>(winningWays.ToString());
    }

    private int GetWinningWays(Race race)
    {
        var winningWays = 0;

        for (var startTime = 0; startTime <= race.Duration; startTime++)
        {
            var playTime = race.Duration - startTime;
            var doneDistance = startTime * playTime;

            if (doneDistance > race.RecordDistance)
            {
                winningWays++;
            }
        }

        return winningWays;
    }

    private Dictionary<int, Race> ParseRaces(string[] input)
    {
        var racesDic = new Dictionary<int, Race>();

        var times = DigitRegex().Matches(input[0])
            .Select(m => int.Parse(m.Value))
            .ToArray();
        var distances = DigitRegex().Matches(input[1])
            .Select(m => int.Parse(m.Value))
            .ToArray();

        for (var i = 0; i < times.Length; i++)
        {
            racesDic.Add(i, new Race(times[i], distances[i]));
        }

        return racesDic;
    }
}