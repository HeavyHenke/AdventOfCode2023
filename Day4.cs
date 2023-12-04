
public class Day4
{
    public string CalcA()
    {
        var points = File.ReadLines("Day4.txt")
            .Select(ParseRow)
            .Select(p => p.winning.Intersect(p.ownNumbers).Count())
            .Where(p => p > 0)
            .Select(p => Math.Pow(2, p-1))
            .Sum();
        return points.ToString();
    }
    
    private (ICollection<int> winning, ICollection<int> ownNumbers) ParseRow(string row)
    {
        var split1 = row.Split(": ");
        var split2 = split1[1].Split(" | ");

        var winnings = split2[0].Split(" ", StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToList();
        var ownNumbers = split2[1].Split(" ", StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToList();

        return (winnings, ownNumbers);
    }

    public string CalcB()
    {
        var cards = File.ReadLines("Day4.txt")
            .Select(ParseRow)
            .ToArray();

        var totalCards = Enumerable.Range(1, cards.Length)
            .Select(i => ScoreCard(i, cards))
            .Sum();

        return totalCards.ToString();
    }

    private static int ScoreCard(int cardId, (ICollection<int> winning, ICollection<int> ownNumbers)[] cards)
    {
        if (cardId > cards.Length)
            return 0;
        
        var winnings = cards[cardId-1].winning.Intersect(cards[cardId-1].ownNumbers).Count();
        var ret = 1;
        for (int i = 0; i < winnings; i++)
            ret += ScoreCard(cardId + i + 1, cards);
        return ret;
    }
}