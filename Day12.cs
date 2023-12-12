using MoreLinq.Extensions;

public class Day12
{
    public string CalcA()
    {
        var input = File
            .ReadAllLines("Day12.txt")
            .Select(s => s.Split(" "))
            .Select(s => (str: s[0], rle: s[1].Split(",").Select(int.Parse).ToArray()));
        
        var validRows = input
            .SelectMany(s => GetAllVariants(s.str).Select(v => (str: v, rle: s.rle)))
            .Count(s => Validate(s.str, s.rle));

        return validRows.ToString();
    }

    private IEnumerable<IEnumerable<char>> GetAllVariants(IEnumerable<char> input)
    {
        var numUnknown = input.Count(c => c == '?');
        var maxVal = (int)Math.Pow(2, numUnknown);

        for (int i = 0; i < maxVal; i++)
            yield return SetAccordingToInt(input, i);
    }

    private static IEnumerable<char> SetAccordingToInt(IEnumerable<char> chr, int i)
    {
        foreach (var c in chr)
        {
            if (c != '?')
                yield return c;
            else
            {
                yield return ((i & 1) == 0) ? '.' : '#';
                i >>= 1;
            }
        }
    }

    private static bool Validate(IEnumerable<char> str, int[] rle)
    {
        return str.RunLengthEncode()
            .Where(c => c.Key == '#')
            .Select(c => c.Value)
            .SequenceEqual(rle);
    }
}