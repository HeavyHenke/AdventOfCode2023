using MoreLinq;

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
    
    public string CalcB()
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
        foreach (var seed in seeds.Batch(2))
        {
            IEnumerable<(long start, long length)> val = new[]{ (seed[0] , seed[1])};
            foreach (var m in maps) 
                val = val.SelectMany(v => m.MapRange(v));
            lowestLocation = Math.Min(lowestLocation, val.Min(s => s.start));
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

        _mapList = _mapList.OrderBy(l => l.sourceStart).ToList();
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

    public IEnumerable<(long start, long length)> MapRange((long start, long length) val)
    {
        if (val.start < _mapList[0].sourceStart)
        {
            yield return (val.start, _mapList[0].sourceStart - val.start);
            val.start = _mapList[0].sourceStart;
            val.length -= val.start - _mapList[0].sourceStart;
        }

        while (true)
        {
            if(val.length == 0)
                yield break;

            var mapIx = _mapList.FindIndex(m => val.start >= m.sourceStart && val.start < m.sourceStart + m.range);
            if (mapIx == -1)
            {
                yield return val;
                yield break;
            }

            var start = val.start + _mapList[mapIx].destDelta;
            var range = Math.Min(val.length, _mapList[mapIx].range - (val.start - _mapList[mapIx].sourceStart));
            yield return (start, range);

            val.start += range;
            val.length -= range;
        }
    }
}
