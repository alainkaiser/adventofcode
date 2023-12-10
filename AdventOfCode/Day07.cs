namespace AdventOfCode;

public sealed class Day07 : BaseDay
{
    private readonly string[] _input;

    public Day07()
    {
        _input = File.ReadAllLines(InputFilePath);
    }

    private record Hand(int Strength, List<Card> Cards, HandType Type, int BidAmount);

    private enum HandType
    {
        Unknown,
        HighCard,
        OnePair,
        TwoPair,
        ThreeOfAKind,
        FullHouse,
        FourOfAKind,
        FiveOfAKind
    }

    private record Card(int Strength, char Value);

    public override ValueTask<string> Solve_1()
    {
        var total = ParseHands()
            .OrderBy(h => h.Strength)
            .ThenBy(h => h.Cards, new CardComparer())
            .Select((hand, i) => (hand, i))
            .Aggregate(0, (total, handWithIndex) => total + handWithIndex.hand.BidAmount * (handWithIndex.i + 1));

        return new ValueTask<string>(total.ToString());
    }

    public override ValueTask<string> Solve_2()
    {
        var total = ParseHands(true)
            .OrderBy(h => h.Strength)
            .ThenBy(h => h.Cards, new CardComparer())
            .Select((hand, i) => (hand, i))
            .Aggregate(0, (total, handWithIndex) => total + handWithIndex.hand.BidAmount * (handWithIndex.i + 1));

        return new ValueTask<string>(total.ToString());
    }

    private IEnumerable<Hand> ParseHands(bool isPartTwo = false)
    {
        return (from line in _input
            let cardsRaw = line.Split(" ")[0].ToCharArray().ToList()
            let cards = cardsRaw.Select(c => new Card(GetStrengthOfCard(c), c)).ToList()
            let handType = GetTypeOfHand(cardsRaw, isPartTwo)
            let strength = GetStrengthOfHand(handType)
            let bidAmount = int.Parse(line.Split(" ")[1])
            select new Hand(strength, cards, handType, bidAmount)).ToList();
    }

    private HandType GetTypeOfHand(IReadOnlyCollection<char> cards, bool isPartTwo = false)
    {
        // Replace Jokers with the most common card for part 2
        var replacedCards = isPartTwo ? ReplaceJokers(cards) : cards;

        var groupedCards = replacedCards.GroupBy(c => c).Select(g => g.Count()).ToList();

        return groupedCards.Count switch
        {
            1 => HandType.FiveOfAKind,
            2 when groupedCards.Contains(4) => HandType.FourOfAKind,
            2 when groupedCards.Contains(3) && groupedCards.Contains(2) => HandType.FullHouse,
            3 when groupedCards.Contains(3) => HandType.ThreeOfAKind,
            3 when groupedCards.Count(g => g == 2) == 2 => HandType.TwoPair,
            4 when groupedCards.Contains(2) => HandType.OnePair,
            _ => replacedCards.Distinct().Count() == replacedCards.Count ? HandType.HighCard : HandType.Unknown
        };
    }

    private int GetStrengthOfHand(HandType type)
    {
        return type switch
        {
            HandType.FiveOfAKind => 7,
            HandType.FourOfAKind => 6,
            HandType.FullHouse => 5,
            HandType.ThreeOfAKind => 4,
            HandType.TwoPair => 3,
            HandType.OnePair => 2,
            HandType.HighCard => 1,
            _ => 0
        };
    }

    private int GetStrengthOfCard(char cardChar)
    {
        return cardChar switch
        {
            'A' => 13,
            'K' => 12,
            'Q' => 11,
            'T' => 10,
            'J' => 1,
            _ => cardChar - '0'
        };
    }

    private IReadOnlyCollection<char> ReplaceJokers(IReadOnlyCollection<char> cards)
    {
        var cardsList = cards.ToList();
        var numberOfJokers = cardsList.Count(c => c == 'J');

        if (numberOfJokers == 0)
        {
            return cardsList;
        }

        var mostCommon = cardsList
            .Where(c => c != 'J')
            .GroupBy(c => c)
            .OrderByDescending(g => g.Count())
            .ThenBy(g => g.Key)
            .FirstOrDefault()?.Key;

        if (mostCommon.HasValue)
        {
            return cardsList.Select(c => c == 'J' ? mostCommon.Value : c).ToList();
        }

        return cardsList;
    }

    private class CardComparer : IComparer<List<Card>>
    {
        public int Compare(List<Card> cardsL, List<Card> cardsR)
        {
            for (var i = 0; i < cardsL.Count; i++)
            {
                var comparison = cardsL[i].Strength.CompareTo(cardsR[i].Strength);
                if (comparison != 0)
                {
                    return comparison;
                }
            }

            return 0;
        }
    }
}