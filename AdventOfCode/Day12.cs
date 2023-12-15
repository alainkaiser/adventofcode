namespace AdventOfCode;

public sealed class Day12 : BaseDay
{
    private readonly string[] _input;

    public Day12()
    {
        _input = File.ReadAllLines(InputFilePath);
    }

    private enum SpringState
    {
        Operational,
        Damaged,
        Unknown
    }

    private record ConditionRecord(List<Spring> Springs, List<int> ContiguousGroupSizes);

    private record Spring(char Value, SpringState State, int Index);

    public override ValueTask<string> Solve_1()
    {
        var records = ParseInput();
        var validVariants = 0;

        foreach (var record in records)
        {
            GeneratePossibleValidVariants(record, "", 0, ref validVariants);
        }

        return new ValueTask<string>(validVariants.ToString());
    }

    public override ValueTask<string> Solve_2()
    {
        return new ValueTask<string>("I don't touch second parts anymore");
    }

    private static void GeneratePossibleValidVariants(ConditionRecord record, string currentVariant, int index,
        ref int validVariants)
    {
        if (index == record.Springs.Count)
        {
            if (IsVariantValid(record, currentVariant))
            {
                validVariants++;
            }

            return;
        }

        var spring = record.Springs[index];
        if (spring.State != SpringState.Unknown)
        {
            GeneratePossibleValidVariants(record, currentVariant + spring.Value, index + 1, ref validVariants);
        }
        else
        {
            GeneratePossibleValidVariants(record, currentVariant + '.', index + 1, ref validVariants);
            GeneratePossibleValidVariants(record, currentVariant + '#', index + 1, ref validVariants);
        }
    }

    private static bool IsVariantValid(ConditionRecord record, string variant)
    {
        var groups = new List<int>();
        var currentGroupSize = 0;

        foreach (var c in variant)
        {
            if (c == '#')
            {
                currentGroupSize++;
            }
            else if (currentGroupSize > 0)
            {
                groups.Add(currentGroupSize);
                currentGroupSize = 0;
            }
        }

        if (currentGroupSize > 0)
        {
            groups.Add(currentGroupSize);
        }

        return groups.SequenceEqual(record.ContiguousGroupSizes);
    }

    private List<ConditionRecord> ParseInput()
    {
        return (from line in _input
            select line.Split(" ")
            into split
            let springs = split[0]
                .ToCharArray()
                .Select((c, i) => new Spring(c, GetSpringState(c), i))
                .ToList()
            let contiguousGroupSizes = split[1]
                .Split(",")
                .Select(int.Parse)
                .ToList()
            select new ConditionRecord(springs, contiguousGroupSizes)).ToList();
    }

    private SpringState GetSpringState(char springValue)
    {
        return springValue switch
        {
            '.' => SpringState.Operational,
            '#' => SpringState.Damaged,
            '?' => SpringState.Unknown,
            _ => throw new ArgumentOutOfRangeException(nameof(springValue), springValue, null)
        };
    }
}