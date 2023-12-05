namespace AdventOfCode2023;

public class Day5
{
    public string CalcA()
    {
        var lines = File.ReadAllLines("Day5.txt");
        int lineIx = 0;

        var seeds = lines[lineIx++].Replace("seeds: ", "").Split(" ").Select(long.Parse).ToArray();
        lineIx++;

        var maps = new Map[7];
        for (int mapi = 0; mapi < maps.Length; mapi++)
        {
            maps[mapi] = new Map(lines.Skip(lineIx).TakeWhile(l => l != ""));
            lineIx += maps[mapi].UsedLineNumbers + 1;
        }

        long lowestLocation = long.MaxValue;
        foreach (var seed in seeds)
        {
            var val = seed;
            foreach (var m in maps)
                val = m.MapValue(val);
            lowestLocation = Math.Min(lowestLocation, val);
        }
        
        return lowestLocation.ToString();
    }

}

file class Map
{
    public readonly string MapName = "";
    private readonly List<(long destDelta, long sourceStart, long range)> _mapList = new ();

    public readonly int UsedLineNumbers;
    
    public Map(IEnumerable<string> def)
    {
        UsedLineNumbers = 0;
        foreach (var line in def)
        {
            UsedLineNumbers++;
            if (char.IsNumber(line[0]) == false)
                MapName = line;
            else
            {
                var parts = line.Split(" ").Select(long.Parse).ToArray();
                _mapList.Add((parts[0] - parts[1], parts[1], parts[2]));
            }
        }
    }

    public long MapValue(long source)
    {
        foreach (var l in _mapList)
        {
            if (source >= l.sourceStart && source <= l.sourceStart + l.range)
            {
                return source + l.destDelta;
            }
        }

        return source;
    }
}
