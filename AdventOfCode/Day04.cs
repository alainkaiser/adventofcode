using System.Text.RegularExpressions;

namespace AdventOfCode;

public sealed partial class Day04 : BaseDay
{
    private readonly string[] _input;

    public Day04()
    {
        _input = File.ReadAllLines(InputFilePath);
    }

    public override ValueTask<string> Solve_1()
    {
        var sum = 0;

        foreach (var game in _input)
        {
            var gameSum = 0;

            var splitOne = game.Split(":");
            var numbersSplit = splitOne[1].Split("|");
            var winningNumbers = Regex.Split(numbersSplit[0].Trim(), @"\D+");
            var numbersIHave = Regex.Split(numbersSplit[1].Trim(), @"\D+");

            var numbersFound = 0;

            foreach (var numberIHave in numbersIHave)
            {
                if (winningNumbers.Contains(numberIHave))
                {
                    numbersFound++;
                    gameSum = numbersFound == 1 ? 1 : (gameSum * 2);
                }
            }

            sum += gameSum;
        }

        return new ValueTask<string>(sum.ToString());
    }

    public override ValueTask<string> Solve_2()
    {
        var sum = 0;

        foreach (var game in _input)
        {
            var gameSum = 0;

            var splitOne = game.Split(":");
            var numbersSplit = splitOne[1].Split("|");
            var winningNumbers = Regex.Split(numbersSplit[0].Trim(), @"\D+");
            var numbersIHave = Regex.Split(numbersSplit[1].Trim(), @"\D+");

            var numbersFound = 0;

            foreach (var numberIHave in numbersIHave)
            {
                if (winningNumbers.Contains(numberIHave))
                {
                    numbersFound++;
                    gameSum = numbersFound == 1 ? 1 : (gameSum * 2);
                }
            }

            Console.WriteLine(numbersFound);
        }

        return new ValueTask<string>(sum.ToString());
    }
}