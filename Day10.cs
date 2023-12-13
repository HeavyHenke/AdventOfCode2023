using MoreLinq;

public class Day10
{
    private string[] _map;

    private static Dictionary<char, (int dx1, int dy1, int dx2, int dy2)> Symbols = new Dictionary<char, (int dx1, int dy1, int dx2, int dy2)>
    {
        { '|', (0, -1, 0, 1) },
        { '-', (-1, 0, 1, 0) },
        { 'L', (0, -1, 1, 0) },
        { 'J', (0, -1, -1, 0) },
        { '7', (-1, 0, 0, 1) },
        { 'F', (1, 0, 0, 1) }
    };
    
    public string CalcA()
    {
        _map = File.ReadAllLines("Day10.txt");
        int sy = -1;
        int sx = -1;
        for (int y = 0; y < _map.Length; y++)
        {
            var ix = _map[y].IndexOf('S');
            if (ix >= 0)
            {
                sy = y;
                sx = ix;
                break;
            }
        }

        int posX, posY;
        if (GetNeighbours(sx + 1, sy).Contains((sx, sy)))
        {
            posX = sx + 1;
            posY = sy;
        }
        else if (GetNeighbours(sx, sy+1).Contains((sx, sy)))
        {
            posX = sx;
            posY = sy+1;
            
        }
        else if (GetNeighbours(sx - 1, sy).Contains((sx, sy)))
        {
            posX = sx - 1;
            posY = sy;
        }
        else
            throw new Exception("Knas!");

        int numSteps = 1;
        int lastX = sx, lastY = sy;
        while (posX != sx || posY != sy)
        {
            var next = GetNeighbours(posX, posY).Single(n => n.x != lastX || n.y != lastY);
            lastX = posX;
            lastY = posY;
            posX = next.x;
            posY = next.y;
            numSteps++;
        }

        return (numSteps / 2).ToString();
    }

    private IEnumerable<(int x, int y)> GetNeighbours(int x, int y)
    {
        if (IsValid(x, y) == false)
            yield break;
        
        if(Symbols.TryGetValue(_map[y][x], out var delta) == false)
            yield break;

        yield return (x + delta.dx1, y + delta.dy1);
        yield return (x + delta.dx2, y + delta.dy2);
    }

    private bool IsValid(int x, int y)
    {
        if(y < 0 || y >= _map.Length)
            return false;
        var row = _map[y];
        if(x < 0 || x >= row.Length)
            return false;
        return true;
    }
    
    
    public string CalcB()
    {
        _map = File.ReadAllLines("Day10.txt");
        int sy = -1;
        int sx = -1;
        for (int y = 0; y < _map.Length; y++)
        {
            var ix = _map[y].IndexOf('S');
            if (ix >= 0)
            {
                sy = y;
                sx = ix;
                break;
            }
        }

        int posX, posY;
        char startChar;
        if (GetNeighbours(sx + 1, sy).Contains((sx, sy)))
        {
            posX = sx + 1;
            posY = sy;
            startChar = 'F';
        }
        else if (GetNeighbours(sx, sy+1).Contains((sx, sy)))
        {
            posX = sx;
            posY = sy+1;
            startChar = '7';
        }
        else if (GetNeighbours(sx - 1, sy).Contains((sx, sy)))
        {
            posX = sx - 1;
            posY = sy;
            startChar = 'C';
        }
        else
            throw new Exception("Knas!");

        // Determine where the used pipes are
        var pipes = new HashSet<(int x, int y)>();
        AddOccupied(pipes, sx, sy, startChar);
        
        int numSteps = 1;
        int lastX = sx, lastY = sy;
        while (posX != sx || posY != sy)
        {
            var next = GetNeighbours(posX, posY).Single(n => n.x != lastX || n.y != lastY);
            AddOccupied(pipes, posX, posY, _map[posY][posX]);
            
            lastX = posX;
            lastY = posY;
            posX = next.x;
            posY = next.y;
            numSteps++;
        }

        // Flood fill
        var maxY = _map.Length * 3 + 1;
        var maxX = _map[0].Length * 3 + 1;

        var water = new HashSet<(int x, int y)>();
        var searchStack = new Stack<(int x, int y)>();
        searchStack.Push((-1,-1));
        while (searchStack.Any())
        {
            var coord = searchStack.Pop();
            foreach (var c in GetNeighboursWaterCoord(coord, maxX, maxY))
            {
                if(pipes.Contains(c))
                    continue;
                if (water.Add(c))
                    searchStack.Push(c);
            }
        }

        // Draw the enlarged version
        // for (int y = -1; y < maxY; y++)
        // {
        //     for (int x = -1; x < maxX; x++)
        //     {
        //         if (pipes.Contains((x, y)))
        //             Console.Write("#");
        //         else if (water.Contains((x,y)))
        //             Console.Write(".");
        //         else
        //             Console.Write("I");
        //     }
        //     Console.WriteLine();
        // }
        // Console.WriteLine();
        
        // Find 3*3 with no water and no pipes
        var numInner = 0;
        for (int y = 0; y < _map.Length; y++)
        for (int x = 0; x < _map[0].Length; x++)
        {
            var waterOrPipes = Enumerable.Range(y * 3, 3).Cartesian(Enumerable.Range(x * 3, 3), (x, y) =>
            {
                if (pipes.Contains((x, y)))
                    return 1;
                if (water.Contains((x, y)))
                    return 1;
                return 0;
            }).Sum();
            if (waterOrPipes == 0)
                numInner++;
        }
        
        return numInner.ToString();
    }

    private static IEnumerable<(int x, int y)> GetNeighboursWaterCoord((int x, int y) coord, int maxX, int maxY)
    {
        return new (int dx, int dy)[] { (-1, 0), (1, 0), (0, -1), (0, 1) }
            .Select(d => (x: coord.x + d.dx, y: coord.y + d.dy))
            .Where(c => c is { x: >= -1, y: >= -1 })
            .Where(c => c.x < maxX && c.y < maxY);
    }
    
    private static void AddOccupied(HashSet<(int x, int y)> occupied, int x, int y, char c)
    {
        foreach (var o in GetOccupiedParts(x, y, c))
            occupied.Add(o);
    }
    
    // Turn into 3*3
    private static IEnumerable<(int x, int y)> GetOccupiedParts(int x, int y, char c)
    {
        if (c == 'F')
        {
            yield return (x * 3+2, y * 3+1);
            yield return (x * 3+1, y * 3+1);
            yield return (x * 3+1, y * 3+2);
        }
        else if (c == '7')
        {
            yield return (x * 3, y * 3+1);
            yield return (x * 3+1, y * 3+1);
            yield return (x * 3+1, y * 3+2);
        }
        else if (c == 'J')
        {
            yield return (x * 3+1, y * 3);
            yield return (x * 3+1, y * 3+1);
            yield return (x * 3, y * 3+1);
        }
        else if (c == '|')
        {
            yield return (x * 3 + 1, y * 3);
            yield return (x * 3 + 1, y * 3+1);
            yield return (x * 3 + 1, y * 3+2);
        }
        else if (c == '-')
        {
            yield return (x * 3, y * 3 + 1);
            yield return (x * 3 + 1, y * 3 + 1);
            yield return (x * 3 + 2, y * 3 + 1);
        }
        else if (c == 'L')
        {
            yield return (x * 3+1, y * 3);
            yield return (x * 3+1, y * 3+1);
            yield return (x * 3+2, y * 3+1);
        }
        else
        {
            throw new Exception("Knas: " + c);
        }

    }
}