using System.Collections.Concurrent;
using System.Text.RegularExpressions;

namespace AdventOfCode;

public sealed partial class Day05 : BaseDay
{
    private readonly string[] _input;

    public Day05()
    {
        _input = File.ReadAllLines(InputFilePath);
    }

    [GeneratedRegex(@"\d+")]
    private static partial Regex DigitRegex();

    private record MapPart(Int128 DestinationRangeStart, Int128 SourceRangeStart, Int128 RangeLength);

    public override ValueTask<string> Solve_1()
    {
        var seeds = GetSeeds(_input[0]);
        var maps = GetMaps();

        var locations = new List<Int128>();

        foreach (var seed in seeds)
        {
            var location = seed;
            foreach (var map in maps)
            {
                var mapParts = map.OrderBy(m => m.SourceRangeStart).ToList();

                foreach (var mapPart in mapParts)
                {
                    // Check if location is within range
                    if (location >= mapPart.SourceRangeStart &&
                        location <= mapPart.SourceRangeStart + mapPart.RangeLength)
                    {
                        // Get the offset from the start of the range
                        var offset = location - mapPart.SourceRangeStart;

                        // Calculate the value from the destination
                        location = mapPart.DestinationRangeStart + offset;

                        // We found the correct location, so we can break out of the loop
                        break;
                    }
                }
            }

            locations.Add(location);
        }

        return new ValueTask<string>(locations.Min().ToString());
    }

    public override ValueTask<string> Solve_2()
    {
        var seedRanges = GetSeedRanges(_input[0]);
        var maps = GetMaps().ToList();
        var minLocations = new ConcurrentQueue<Int128>();

        Parallel.ForEach(seedRanges, new ParallelOptions
        {
            MaxDegreeOfParallelism = Environment.ProcessorCount
        }, (range) =>
        {
            var localMin = Int128.MaxValue;

            for (var seed = range.start; seed < range.start + range.length; seed++)
            {
                var location = seed;
                foreach (var map in maps)
                {
                    foreach (var mapPart in map.Where(mapPart =>
                                 location >= mapPart.SourceRangeStart &&
                                 location < mapPart.SourceRangeStart + mapPart.RangeLength))
                    {
                        var offset = location - mapPart.SourceRangeStart;
                        location = mapPart.DestinationRangeStart + offset;

                        break;
                    }
                }

                if (location < localMin)
                {
                    localMin = location;
                }
            }

            minLocations.Enqueue(localMin);
        });

        return new ValueTask<string>(minLocations.Min().ToString());
    }

    private IEnumerable<(Int128 start, Int128 length)> GetSeedRanges(string firstLineInput)
    {
        var seedPairs = firstLineInput
            .Split(": ")
            .Skip(1)
            .First()
            .Split(" ")
            .Select(Int128.Parse)
            .ToArray();

        for (var i = 0; i < seedPairs.Length; i += 2)
        {
            yield return (seedPairs[i], seedPairs[i + 1]);
        }
    }

    private IEnumerable<Int128> GetSeeds(string firstLineInput)
    {
        var seeds = firstLineInput
            .Split(": ")
            .Skip(1)
            .First()
            .Split(" ")
            .Select(Int128.Parse);

        return seeds;
    }

    private IEnumerable<List<MapPart>> GetMaps()
    {
        var mapLines = File.ReadAllText(InputFilePath).Split("\n\n", StringSplitOptions.RemoveEmptyEntries)
            .Skip(1);

        var maps = mapLines
            .Select(l => l.Split('\n').Skip(1).ToList())
            .Select(l => l.Select(i => i.Split(" ").Select(Int128.Parse).ToList()).ToList())
            .Select(l => l.Select(i => new MapPart(i[0], i[1], i[2])).ToList())
            .ToList();

        return maps;
    }
}