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
        var calibrationValues = new List<int>();
        
        var wordDigits = new Dictionary<string, int>()
        {
            {"one", 1},
            {"two", 2},
            {"three", 3},
            {"four", 4},
            {"five", 5},
            {"six", 6},
            {"seven", 7},
            {"eight", 8},
            {"nine", 9}
        };
        
        // Loop trough all the lines in the input file
        foreach (var line in _input)
        {
            var wordDigitsOccurrence = new Dictionary<int, string>();
            
            // Gather some infos about written digits in the line first
            foreach (var wordDigit in wordDigits.Keys)
            {
                if (line.Contains(wordDigit, StringComparison.CurrentCultureIgnoreCase))
                {
                    // Get all starting indexes of the word digit
                    var indexes = Regex.Matches(line, wordDigit, RegexOptions.IgnoreCase)
                        .Select(match => match.Index);
                    
                    // Add all the starting indexes to the dictionary
                    foreach (var index in indexes)
                    {
                        wordDigitsOccurrence.Add(index, wordDigit);
                    }
                }
            }
            
            string firstNumberChar = null;
            string lastNumberChar = null;
            
            // Loop trough all the chars in one line
            for (var i = 0; i < line.Length; i++)
            {
                // Check if value is a digit
                if (char.IsDigit(line[i]))
                {
                    if (string.IsNullOrEmpty(firstNumberChar))
                    {
                        firstNumberChar = line[i].ToString();
                    }
                    
                    lastNumberChar = line[i].ToString();
                }
                else
                {
                    // Check if we have any written digit starting at index i. If so, use it as our first number and last number
                    if(wordDigitsOccurrence.TryGetValue(i, out var foundWrittenDigit))
                    {
                        if (string.IsNullOrEmpty(firstNumberChar))
                        {
                            firstNumberChar = foundWrittenDigit;   
                        }
                        
                        lastNumberChar = foundWrittenDigit;
                    }
                }
            }
            
            // Check if first and/or last number is a written digit and extract the number from the dictionary
            if(wordDigits.TryGetValue(firstNumberChar, out var firstNumber))
            {
                firstNumberChar = firstNumber.ToString();
            }
            if(wordDigits.TryGetValue(lastNumberChar, out var lastNumber))
            {
                lastNumberChar = lastNumber.ToString();
            }
            
            // Add the calibration value to the list
            var combinedNumberString = firstNumberChar + lastNumberChar;
            calibrationValues.Add(int.Parse(combinedNumberString));
        }
        
        // Get sum of all the calibration values
        var sum = calibrationValues.Sum();
        return new ValueTask<string>(sum.ToString());
    }
}