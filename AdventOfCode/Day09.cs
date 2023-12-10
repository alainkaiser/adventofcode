namespace AdventOfCode;

public sealed class Day09 : BaseDay
{
    private readonly string[] _input;
    
    public Day09()
    {
        _input = File.ReadAllLines(InputFilePath);
    }

    private record History()
    {
        public List<int> Items { get; init; }
    }
    
    public override ValueTask<string> Solve_1()
    {
        var nextValues = GenerateNextValues((differenceSequence, extrapolatedItems, i) => differenceSequence[i].Last() + extrapolatedItems.Last());
        var sum = nextValues.Sum();
        return new ValueTask<string>(sum.ToString());
    }

    public override ValueTask<string> Solve_2()
    {
        var nextValues = GenerateNextValues((differenceSequence, extrapolatedItems, i) => differenceSequence[i].First() - extrapolatedItems.Last());
        var sum = nextValues.Sum();
        return new ValueTask<string>(sum.ToString());
    }
    
    private IEnumerable<int> GenerateNextValues(Func<List<IEnumerable<int>>, List<int>, int, int> calculateExtrapolatedItem)
    {
        var histories = ParseInput();
        var nextValues = new List<int>();

        foreach (var history in histories)
        {
            var differenceSequence = GenerateDifferences(history.Items, [history.Items]).ToList();
            var extrapolatedItems = new List<int>();

            for (var i = differenceSequence.Count -1 ; i >= 0; i--)
            {
                extrapolatedItems.Add(i == differenceSequence.Count - 1
                    ? 0
                    : calculateExtrapolatedItem(differenceSequence, extrapolatedItems, i));
            }

            nextValues.Add(extrapolatedItems.Last());
        }

        return nextValues;
    }

    private IEnumerable<IEnumerable<int>> GenerateDifferences(IReadOnlyList<int> items, ICollection<List<int>> differences)
    {
        var currentDifferences = new List<int>();
        for (var i = 0; i < items.Count - 1; i++)
        {
            currentDifferences.Add(items[i + 1] - items[i]);
        }
        
        differences.Add(currentDifferences);

        if (currentDifferences.Any(d => d != 0))
        {
            GenerateDifferences(currentDifferences, differences);
        }
        
        return differences;
    }
    
    private List<History> ParseInput()
    {
        return _input
            .Select(line => line.Split(" ").Select(int.Parse).ToList())
            .Select(numbers => new History { Items = numbers })
            .ToList();
    }
}