﻿namespace AdventOfCode;

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
        var wordDigits = new Dictionary<string, int>()
        {
            { "one", 1 }, { "two", 2 }, { "three", 3 }, { "four", 4 },
            { "five", 5 }, { "six", 6 }, { "seven", 7 }, { "eight", 8 }, { "nine", 9 }
        };

        int sum = 0;

        foreach (var line in _input)
        {
            string firstNumber = null;
            string lastNumber = null;

            for (var i = 0; i < line.Length; i++)
            {
                foreach (var wordDigit in wordDigits)
                {
                    if (line.Length - i >= wordDigit.Key.Length &&
                        line.Substring(i, wordDigit.Key.Length) == wordDigit.Key)
                    {
                        if (string.IsNullOrEmpty(firstNumber))
                        {
                            firstNumber = wordDigit.Value.ToString();
                        }

                        lastNumber = wordDigit.Value.ToString();
                    }
                }

                if (string.IsNullOrEmpty(firstNumber) && char.IsDigit(line[i]))
                {
                    firstNumber = line[i].ToString();
                }

                if (char.IsDigit(line[i]))
                {
                    lastNumber = line[i].ToString();
                }
            }

            sum += int.Parse(firstNumber + lastNumber);
        }

        return new ValueTask<string>(sum.ToString());
    }
}