using System.Text.RegularExpressions;

namespace AdventOfCode;

public sealed class Day04 : BaseDay
{
    private readonly string[] _input;

    public Day04()
    {
        _input = File.ReadAllLines(InputFilePath);
    }

    private record Card(int CardId, int Matches, int Points = 0);

    public override ValueTask<string> Solve_1()
    {
        var sumOfPoints = GetCards().Sum(x => x.Points);

        return new ValueTask<string>(sumOfPoints.ToString());
    }

    public override ValueTask<string> Solve_2()
    {
        var sum = 0;

        var cards = GetCards();
        foreach (var card in cards)
        {
            sum++;

            // If the card has no matches, skip it
            if (card.Matches == 0) continue;
            ProcessMatchingCard(card);
        }

        return new ValueTask<string>(sum.ToString());

        void ProcessMatchingCard(Card card)
        {
            // Get the won copies from the original cards list
            var copies = cards
                .Skip(card.CardId)
                .Take(card.Matches);

            // Iterate through the won copies and add them to the wonCards dictionary
            foreach (var copy in copies)
            {
                sum++;

                // Recursive call for copies with matches
                if (copy.Matches > 0)
                {
                    ProcessMatchingCard(copy);
                }
            }
        }
    }

    private List<Card> GetCards()
    {
        var cards = new List<Card>();

        for (var i = 0; i < _input.Length; i++)
        {
            var game = _input[i];

            var splitOne = game.Split(":");
            var numbersSplit = splitOne[1].Split("|");

            var winningNumbers = Regex.Split(numbersSplit[0].Trim(), @"\D+");
            var numbersIHave = Regex.Split(numbersSplit[1].Trim(), @"\D+");

            var points = numbersIHave
                .Where(numberIHave => winningNumbers.Contains(numberIHave))
                .Select((number, index) => new { Number = number, Index = index })
                .Aggregate(0, (total, next) => next.Index == 0 ? 1 : total * 2);

            var matches = numbersIHave
                .Where(numberIHave => winningNumbers.Contains(numberIHave))
                .Select(numberIHave => numberIHave)
                .Count();

            cards.Add(new Card(i + 1, matches, points));
        }

        return cards;
    }
}