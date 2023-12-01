using System.Text.RegularExpressions;

namespace AdventOfCode;

public sealed class Day01 : BaseDay
{
    private readonly string[] _input;
    
    public Day01()
    {
        _input = File.ReadAllLines(InputFilePath);
    }
    
    public override ValueTask<string> Solve_1()
    {
        // Store all the calibration values in a list
        var calibrationValues = new List<int>();
        
        // Loop trough all the lines in the input file
        foreach (var line in _input)
        {
            // Get all the digits in the line
            var digits = line
                .ToCharArray()
                .Where(char.IsDigit)
                .ToList();

            // Get the first and last digit
            var first = digits.First();
            var last = digits.Last();
            
            // Combine the first and last digit to a string
            var calibrationValue = int.Parse(first + last.ToString());
            calibrationValues.Add(calibrationValue);
        }
        
        // Get sum of all the calibration values
        var sum = calibrationValues.Sum();
        
        return new ValueTask<string>(sum.ToString());
    }

    public override ValueTask<string> Solve_2()
    {
        var result = PartTwo(File.ReadAllText(InputFilePath));
        return new ValueTask<string>(result.ToString());
    }
    
    public object PartTwo(string input) => 
        Solve(input, @"\d|one|two|three|four|five|six|seven|eight|nine");

    int Solve(string input, string rx) => (
        from line in input.Split("\n")
        let first = Regex.Match(line, rx)
        let last = Regex.Match(line, rx, RegexOptions.RightToLeft)
        select ParseMatch(first.Value) * 10 + ParseMatch(last.Value)
    ).Sum();

    int ParseMatch(string st) => st switch {
        "one" => 1,
        "two" => 2,
        "three" => 3,
        "four" => 4,
        "five" => 5,
        "six" => 6,
        "seven" => 7,
        "eight" => 8,
        "nine" => 9,
        var d => int.Parse(d)
    };
}