public class Day12
{
    public string CalcA()
    {
        var input = File
            .ReadAllLines("Day12.txt")
            .Select(s => s.Split(" "))
            .Select(s => (str: s[0], rle: s[1].Split(",").Select(int.Parse).ToArray()));
           
        var validRows = input.Select(s => (str: s.str, rle: string.Join(",", s.rle)))
            .Select(s => GetNumberOfVariants(s.str, s.rle))
            .Sum();

        return validRows.ToString();
    }

    public string CalcB()
    {
        var input = File
            .ReadAllLines("Day12.txt")
            .Select(s => s.Split(" "))
            .Select(s => (str: s[0], rle: s[1]))
            .Select(s => (str: string.Join("?", Enumerable.Repeat(s.str, 5)), rle: string.Join(",", Enumerable.Repeat(s.rle, 5))));

        var result = input
            .Select(i => GetNumberOfVariants(i.str, i.rle))
            .Sum();

        return result.ToString();
    }

    private static long GetNumberOfVariants(string str, string rle)
    {
        if (str[0] == '.')
            str = str.TrimStart('.');
        var totalLeft = rle.Split(',').Select(int.Parse).Sum();
        return GetNumberOfVariantsImpl(new[] { (str, 1L) }, rle, totalLeft);
    }
    
    private static long GetNumberOfVariantsImpl(IEnumerable<(string str, long num)> strs, string rle, int totalLeft)
    {
        if (rle.Length == 0)
            return strs.Sum(s => s.str.All(c => c is '?' or '.') ? s.num : 0);
        
        var comma = rle.IndexOf(',');
        int firstNum;
        string restRle;
        if (comma > 0)
        {
            firstNum = int.Parse(rle[..comma]);
            restRle = rle[(comma + 1)..];
        }
        else
        {
            firstNum = int.Parse(rle);
            restRle = "";
        }

        totalLeft -= firstNum;

        var next = strs.Select(str => (str: GetRestPossibilities(str.str, totalLeft, firstNum), num: str.num))
            .SelectMany(s => s.str.Select(q => (str: q, num: s.num)));

        var groups = next.GroupBy(key => key.str, val => val.num, (s, enumerable) => (str: s, num: enumerable.Sum()));
        var nextGroups = groups.Select(g => (g.str, g.num))
            .ToArray();
        return GetNumberOfVariantsImpl(nextGroups, restRle, totalLeft);
    }

    private static IEnumerable<string> GetRestPossibilities(string str, int totalLeft, int firstNum)
    {
        if (str.Length == 0)
            yield break;
        if (str.Length < totalLeft)
            yield break;
        
        while (str.Length >= (totalLeft - firstNum) && str.Length > 0)
        {
            var str2 = str;
            if (str2[0] == '#')
            {
                if (IsFirstPossibleWhenStartingWithHash(ref str2, firstNum) == false)
                    break;
                str = str2[firstNum..].TrimStart('.');
                yield return str;
                break;
            }

            if (str[0] != '?')
                throw new Exception("Knas!");

            str2 = '#' + str2[1..];
            if (IsFirstPossibleWhenStartingWithHash(ref str2, firstNum))
            {
                str2 = str2[firstNum..].TrimStart('.');
                yield return str2;
            }

            str = str[1..];
            str = str.TrimStart('.');
        }
    }

    private static bool IsFirstPossibleWhenStartingWithHash(ref string str, int firstNum)
    {
        if (str.Length < firstNum)
            return false;
        
        for (int i = 0; i < firstNum; i++)
        {
            if (str[i] == '.')
                return false;

            if (str[i] == '?')
                str = str.Remove(i, 1).Insert(i, "#");
        }

        if (str.Length == firstNum)
            return true;

        if (str[firstNum] == '#')
            return false;

        if (str[firstNum] == '?')
            str = str.Remove(firstNum, 1).Insert(firstNum, ".");
        return true;
    }
}