namespace AdventOfCode2023;

public class Day1
{
    public string CalcA()
    {
        return File.ReadLines("Day1.txt")
            .Select(l => l.Where(char.IsNumber))
            .Select(l => l.First() + "" + l.Last())
            .Select(int.Parse)
            .Sum()
            .ToString();
    }

    public string CalcB()
    {
        return File.ReadLines("Day1.txt")
            .Select(l => GetNumbers(l).MinBy(l => l.ix).num + "" + GetNumbers(l).MaxBy(l => l.ix).num)
            .Select(int.Parse)
            .Sum()
            .ToString();
    }

    private static IEnumerable<(int num, int ix)> GetNumbers(string inp)
    {
        var toSearch = new[] { "one", "two", "three", "four", "five", "six", "seven", "eight", "nine" }
            .Concat(Enumerable.Range(1, 9).Select(i => i.ToString()))
            .ToArray();

        for (var si = 0; si < toSearch.Length; si++)
        {
            int ix = 0;
            while (true)
            {
                ix = inp.IndexOf(toSearch[si], ix, StringComparison.Ordinal);
                if (ix >= 0)
                {
                    if (si < 9)
                        yield return (si + 1, ix);
                    else
                        yield return (si - 8, ix);
                    ix++;
                }
                else
                {
                    break;
                }
            }
        }
    }
}