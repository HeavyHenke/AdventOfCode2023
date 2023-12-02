using System.Text.RegularExpressions;
using static System.Math; 

namespace AdventOfCode2023;

public partial class Day2
{
    public string CalcA()
    {
        var q = File.ReadAllLines("Day2.txt").Select(ParseGameRow);
        return q
            .Where(w => IsValidGameA(w.info))
            .Sum(w => w.id)
            .ToString();
    }

    [GeneratedRegex(@"(?<red>\d+ red)|(?<green>\d+ green)|(?<blue>\d+ blue)")]
    private static partial Regex ParseHand();
    
    private static (int id, ICollection<(int red, int green, int blue)> info) ParseGameRow(string row)
    {
        var split1 = row.Split(':');
        var gameId = int.Parse(split1[0].Split(" ")[1]);

        var regEx = ParseHand();
        
        var infoRows = split1[1].Split(';');
        var infoList = new List<(int red, int green, int blue)>(infoRows.Length);
        foreach (var r in infoRows)
        {
            var matches = regEx.Matches(r);
            int red = 0;
            int green = 0;
            int blue = 0;
            foreach (Match m in matches)
            {
                var value = int.Parse(m.Value.Split(" ")[0]);
                if (m.Value.Contains("red"))
                    red += value;
                else if (m.Value.Contains("green"))
                    green += value;
                else if (m.Value.Contains("blue"))
                    blue += value;
                else
                    throw new Exception("Unknown color: " + m.Value);
            }
            infoList.Add((red, green, blue));
        }

        return (gameId, infoList);
    }

    private static bool IsValidGameA(IEnumerable<(int red, int green, int blue)> info)
    {
        foreach (var row in info)
        {
            if (row.red > 12)
                return false;
            if (row.green > 13)
                return false;
            if (row.blue > 14)
                return false;
        }

        return true;
    }
    
    public string CalcB()
    {
        var q = File.ReadAllLines("Day2.txt").Select(ParseGameRow);
        return q
            .Sum(w => GetGamePower(w.info))
            .ToString();
    }

    private static int GetGamePower(IEnumerable<(int red, int green, int blue)> info)
    {
        int minRed = 0;
        int minGreen = 0;
        int minBlue = 0;
        foreach (var row in info)
        {
            minRed = Max(minRed, row.red);
            minGreen = Max(minGreen, row.green);
            minBlue = Max(minBlue, row.blue);
        }

        return minRed * minGreen * minBlue;
    }
    
}