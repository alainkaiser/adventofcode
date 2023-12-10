using System.Text.RegularExpressions;

namespace AdventOfCode;

public sealed class Day08 : BaseDay
{
    private readonly string[] _input;
    private const string StartKey = "AAA";
    private const string EndKey = "ZZZ";

    public Day08()
    {
        _input = File.ReadAllLines(InputFilePath);
    }
    
    private record Node(NodeElement Key, NodeElement Left, NodeElement Right);
    
    private record NodeElement(string Value, bool IsStartingNode, bool IsEndingNode);
    
    private enum Direction
    {
        Left,
        Right
    }

    public override ValueTask<string> Solve_1()
    {
        var (map, path) = ProcessMap();

        var currentNode = StartKey;
        var stepsNeeded = 0;
        
        while (currentNode != EndKey)
        {
            var direction = path[stepsNeeded % path.Length];
            currentNode = direction == Direction.Left ? map[currentNode].Left.Value : map[currentNode].Right.Value;
            stepsNeeded++;
        }
        
        return new ValueTask<string>(stepsNeeded.ToString());
    }
    
    public override ValueTask<string> Solve_2()
    {
        var (map, path) = ProcessMap();
        
        var startingNodes = map.Values
            .Where(n => n.Key.IsStartingNode)
            .Select(n => n.Key.Value);

        long lcmOfAllPaths = 1;
        
        foreach (var startNode in startingNodes)
        {
            var cycleLength = CalculateCycleLength(map, path, startNode);
            lcmOfAllPaths = Lcm(lcmOfAllPaths, cycleLength);
        }

        return new ValueTask<string>(lcmOfAllPaths.ToString());
    }
    
    private long CalculateCycleLength(Dictionary<string, Node> map, Direction[] path, string startNode)
    {
        var currentNode = startNode;
        long steps = 0;
        var pathIndex = 0;

        while (!currentNode.EndsWith('Z'))
        {
            var direction = path[pathIndex % path.Length];
            currentNode = direction == Direction.Left ? map[currentNode].Left.Value : map[currentNode].Right.Value;
            pathIndex++;
            steps++;

            // If we have looped back to the start node, it means there's no path to a 'Z' ending node
            if (currentNode == startNode)
            {
                throw new InvalidOperationException("No path to a 'Z' ending node from " + startNode);
            }
        }

        return steps;
    }

    
    private long Lcm(long a, long b)
    {
        return (a / Gcd(a, b)) * b;
    }

    private static long Gcd(long a, long b)
    {
        while (b != 0)
        {
            long temp = b;
            b = a % b;
            a = temp;
        }
        return a;
    }

    private (Dictionary<string, Node>, Direction[]) ProcessMap()
    {
        var path = _input[0].ToCharArray()
            .Select(c => c switch
            {
                'R' => Direction.Right,
                'L' => Direction.Left,
                _ => throw new Exception("Invalid direction")
            }).ToArray();

        var network = (from line in _input.Skip(2)
            let keyMatch = Regex.Match(line, @"^\w{3}")
            let key = keyMatch.Value
            let keyIsStartingNode = key.Last() == 'A'
            let keyIsEndingNode = key.Last() == 'Z'
            let pairMatch = Regex.Match(line, @"\((\w{3}), (\w{3})\)")
            let left = pairMatch.Groups[1].Value
            let leftIsStartingNode = key.Last() == 'A'
            let leftIsEndingNode = key.Last() == 'Z'
            let right = pairMatch.Groups[2].Value
            let rightIsStartingNode = key.Last() == 'A'
            let rightIsEndingNode = key.Last() == 'Z'
            select new Node(new NodeElement(key, keyIsStartingNode, keyIsEndingNode), new NodeElement(left, leftIsStartingNode, leftIsEndingNode),
                new NodeElement(right, rightIsStartingNode, rightIsEndingNode)))
            .ToDictionary(n => n.Key.Value);

        return (network, path);
    }
}