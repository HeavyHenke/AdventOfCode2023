namespace AdventOfCode2023;

public class Day3
{
    public string CalcA()
    {
        var map = File.ReadAllLines("Day3.txt");
        var numbers = Enumerable.Range(0, 10).Select(n => (char)('0' + n)).ToArray();
        int ret = 0;

        for (int line = 0; line < map.Length; line++)
        {
            for (int x = 0; x < map[line].Length; x++)
            {
                var nextNum = map[line].IndexOfAny(numbers, x);
                if (nextNum < 0)
                    break;
                
                var num = map[line].Skip(nextNum).TakeWhile(char.IsNumber).ToArray();
                bool neighbourIsSymbol = Enumerable.Range(nextNum, num.Length)
                    .Select(n => new Coordinate(line, n))
                    .SelectMany(c => c.GetNeighbours())
                    .Where(n => n.IsValidForMap(map))
                    .Select(n => n.GetCharInMap(map))
                    .Any(c => !char.IsNumber(c) && c != '.');

                if (neighbourIsSymbol)
                    ret += int.Parse(num);

                x = nextNum + num.Length - 1;
            }
        }

        return ret.ToString();
    }

    public string CalcB()
    {
        var map = File.ReadAllLines("Day3.txt");
        var numbers = Enumerable.Range(0, 10).Select(n => (char)('0' + n)).ToArray();
        var gears = new Dictionary<Coordinate, List<int>>();

        for (int line = 0; line < map.Length; line++)
        {
            for (int x = 0; x < map[line].Length; x++)
            {
                var nextNum = map[line].IndexOfAny(numbers, x);
                if (nextNum < 0)
                    break;
                
                var num = map[line].Skip(nextNum).TakeWhile(char.IsNumber).ToArray();
                var gearCoord = Enumerable
                    .Range(nextNum, num.Length)
                    .Select(n => new Coordinate(line, n))
                    .SelectMany(c => c.GetNeighbours())
                    .Where(n => n.IsValidForMap(map))
                    .FirstOrDefault(n => n.GetCharInMap(map) == '*');

                if (gearCoord != null)
                {
                    if (gears.TryGetValue(gearCoord, out var list) == false)
                    {
                        list = new List<int>();
                        gears.Add(gearCoord, list);
                    }
                    list.Add(int.Parse(num));
                }
                
                x = nextNum + num.Length - 1;
            }
        }

        var ret = 0;
        foreach (var g in gears.Values)
        {
            if (g.Count == 2)
                ret += g[0] * g[1];
        }

        return ret.ToString();
    }

}

file class Coordinate(int line, int col)
{
    private readonly int _line = line;
    private readonly int _col = col;

    public IEnumerable<Coordinate> GetNeighbours()
    {
        yield return new Coordinate(_line - 1, _col - 1);
        yield return new Coordinate(_line - 1, _col);
        yield return new Coordinate(_line - 1, _col + 1);
        yield return new Coordinate(_line, _col - 1);
        yield return new Coordinate(_line, _col + 1);
        yield return new Coordinate(_line + 1, _col - 1);
        yield return new Coordinate(_line + 1, _col);
        yield return new Coordinate(_line + 1, _col + 1);
    }

    public bool IsValidForMap(string[] map)
    {
        if (_line < 0 || _col < 0)
            return false;
        if (_line >= map.Length)
            return false;
        if (_col >= map[_line].Length)
            return false;
        return true;
    }

    public char GetCharInMap(string[] map)
    {
        return map[_line][_col];
    }

    private bool Equals(Coordinate other)
    {
        return _line == other._line && _col == other._col;
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != GetType()) return false;
        return Equals((Coordinate)obj);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(_line, _col);
    }
} 