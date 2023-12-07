using System.Collections.Frozen;

public class Day7
{
    public string CalcA()
    {
        var totalWinning = File.ReadLines("Day7.txt")
            .Select(line => new Hand(line))
            .Order()
            .Select((h, i) => (double) (i+1) * h.Bid)
            .Sum();

        return totalWinning.ToString();
    }
    
    public string CalcB()
    {
        var totalWinning = File.ReadLines("Day7.txt")
            .Select(line => new Hand2(line))
            .Order()
            .Select((h, i) => (double) (i+1) * h.Bid)
            .Sum();
        
        return totalWinning.ToString();
    }
}

file class Hand : IComparable<Hand>
{
    private readonly (char Key, int Count)[] _handGroups;
    public char[] HandStr { get; }

    public int Bid { get; }

    private static readonly FrozenDictionary<char, char> HandTranslation;

    static Hand()
    {
        HandTranslation = Enumerable.Range(2, 8).Select(r => ((char)(r+'0'), (char)(r+'0')))
            .Concat(new[]
            {
                ('T', 'A'),
                ('J', 'B'),
                ('Q', 'C'),
                ('K', 'D'),
                ('A', 'E')
            })
            .ToFrozenDictionary(key => key.Item1, val => val.Item2);
    }
    
    public Hand(string handString)
    {
        HandStr = handString
            .Take(5)
            .Select(c => HandTranslation[c])
            .ToArray();

        _handGroups = HandStr.GroupBy(c => c)
            .Select(g => (g.Key, count: g.Count()))
            .ToArray();

        Bid = int.Parse(handString.Split(' ').Last());
    }

    public int CompareTo(Hand? other)
    {
        if (other == null)
            return -1;

        var handDiff = GetHandType() - other.GetHandType();
        if (handDiff != 0)
            return handDiff;

        for (int i = 0; i <= 4; i++)
        {
            if (HandStr[i] < other.HandStr[i])
                return -1;
            if (HandStr[i] > other.HandStr[i])
                return 1;
        }

        return 0;
    }

    private int GetHandType()
    {
        if (_handGroups.Any(h => h.Count == 5))
            return 7;
        if (_handGroups.Any(h => h.Count == 4))
            return 6;
        if (_handGroups.Any(h => h.Count == 3) && _handGroups.Any(h => h.Count == 2))
            return 5;
        if (_handGroups.Any(h => h.Count == 3))
            return 4;
        if (_handGroups.Count(h => h.Count == 2) == 2)
            return 3;
        if (_handGroups.Any(h => h.Count == 2))
            return 2;

        return 1;
    }
} 

file class Hand2 : IComparable<Hand2>
{
    private readonly (char Key, int Count)[] _handGroups;
    public char[] HandStr { get; }

    public int Bid { get; }

    private static readonly FrozenDictionary<char, char> HandTranslation;

    static Hand2()
    {
        HandTranslation = Enumerable.Range(2, 8).Select(r => ((char)(r+'0'), (char)(r+'0')))
            .Concat(new[]
            {
                ('T', 'A'),
                ('J', '0'),
                ('Q', 'C'),
                ('K', 'D'),
                ('A', 'E')
            })
            .ToFrozenDictionary(key => key.Item1, val => val.Item2);
    }
    
    public Hand2(string handString)
    {
        HandStr = handString
            .Take(5)
            .Select(c => HandTranslation[c])
            .ToArray();

        _handGroups = HandStr
            .Where(c => c != '0')
            .GroupBy(c => c)
            .Select(g => (g.Key, count: g.Count()))
            .OrderByDescending(g => g.count)
            .ToArray();

        int numJokers = handString.Count(c => c == 'J');
        if (_handGroups.Length == 0)
            _handGroups = new[] { ('E', numJokers) };   // All jokers
        else if (numJokers > 0) 
            _handGroups[0].Count += numJokers;

        Bid = int.Parse(handString.Split(' ').Last());
    }

    public int CompareTo(Hand2? other)
    {
        if (other == null)
            return -1;

        var handDiff = GetHandType() - other.GetHandType();
        if (handDiff != 0)
            return handDiff;

        for (int i = 0; i <= 4; i++)
        {
            if (HandStr[i] < other.HandStr[i])
                return -1;
            if (HandStr[i] > other.HandStr[i])
                return 1;
        }

        return 0;
    }

    private int GetHandType()
    {
        if (_handGroups.Any(h => h.Count == 5))
            return 7;
        if (_handGroups.Any(h => h.Count == 4))
            return 6;
        if (_handGroups.Any(h => h.Count == 3) && _handGroups.Any(h => h.Count == 2))
            return 5;
        if (_handGroups.Any(h => h.Count == 3))
            return 4;
        if (_handGroups.Count(h => h.Count == 2) == 2)
            return 3;
        if (_handGroups.Any(h => h.Count == 2))
            return 2;

        return 1;
    }
} 