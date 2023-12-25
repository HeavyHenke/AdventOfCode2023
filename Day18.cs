using System.Globalization;
using MoreLinq;

public class Day18
{
    public object CalcA()
    {
        var commands = File.ReadLines("Day18.txt")
            .Select(r => r.Split(' '))
            .Select(r => (dir: ChrToDir[r[0][0]], meters: int.Parse(r[1])));

        var x = 0;
        var y = 0;
        var digout = new HashSet<(int x, int y)>();
        digout.Add((x, y));
        foreach (var cmd in commands)
        {
            var d = DirToDelta[cmd.dir];
            for (int i = 0; i < cmd.meters; i++)
            {
                x += d.dx;
                y += d.dy;
                digout.Add((x, y));
            }
        }

        FloodFill(digout);

        // int minY = digout.Min(c => c.y);
        // int maxY = digout.Max(c => c.y);
        // int minX = digout.Min(c => c.x);
        // int maxX = digout.Max(c => c.x);
        // for (y = minY; y <= maxY; y++)
        // {
        //     for (x = minX; x <= maxX; x++)
        //     {
        //         if(digout.Contains((x,y)))
        //             Console.Write('#');
        //         else
        //             Console.Write('.');
        //     }
        //     Console.WriteLine();
        // }
        // Console.WriteLine();
        
        return digout.Count;
    }

    private void FloodFill(HashSet<(int x, int y)> digout)
    {
        int minX = digout.Min(c => c.x)-1;
        int maxX = digout.Max(c => c.x)+1;
        int minY = digout.Min(c => c.y)-1;
        int maxY = digout.Max(c => c.y)+1;

        var q = new Queue<(int x, int y)>();
        q.Enqueue((minX, minY));
        var visited = new HashSet<(int x, int y)>();

        while (q.Count > 0)
        {
            var c = q.Dequeue();
            foreach (var n in GetNeighbours(c, minX, maxX, minY, maxY))
            {
                if (digout.Contains(n))
                    continue;
                if(visited.Add(n))
                    q.Enqueue(n);
            }
        }
        
        for (int y = minY; y <= maxY; y++)
        for (int x = minX; x <= maxX; x++)
        {
            if (visited.Contains((x, y)) == false)
                digout.Add((x, y));
        }
    }

    private static IEnumerable<(int x, int y)> GetNeighbours((int x, int y) cord, int minX, int maxX, int minY, int maxY)
    {
        if (IsValid(cord.x + 1, cord.y))
            yield return (cord.x + 1, cord.y);
        if (IsValid(cord.x - 1, cord.y))
            yield return (cord.x - 1, cord.y);
        if (IsValid(cord.x, cord.y + 1))
            yield return (cord.x, cord.y + 1);
        if (IsValid(cord.x, cord.y - 1))
            yield return (cord.x, cord.y - 1);
        yield break;

        bool IsValid(int x, int y)
        {
            return x >= minX && x <= maxX && y >= minY && y <= maxY;
        }
    }

    private static readonly Dictionary<char, Direction> ChrToDir = new()
    {
        { 'L', Direction.Left },
        { 'U', Direction.Up },
        { 'R', Direction.Right },
        { 'D', Direction.Down }
    };
    private static readonly Dictionary<char, Direction> ChrToDir2 = new()
    {
        { '2', Direction.Left },
        { '3', Direction.Up },
        { '0', Direction.Right },
        { '1', Direction.Down }
    };
    
    private enum Direction
    {
        Left,
        Up,
        Right,
        Down
    }
    private static readonly Dictionary<Direction, (int dx, int dy)> DirToDelta = new()
    {
        { Direction.Left, (-1, 0) },
        { Direction.Right, (1, 0) },
        { Direction.Up, (0, -1) },
        { Direction.Down, (0, 1) },
    };

    public object CalcB()
    {
        var commands = File.ReadLines("Day18.txt")
            .Select(r => r.Substring(r.IndexOf('#') + 1, r.Length - r.IndexOf('#') - 2))
            .Select(r => (dir: ChrToDir2[r.Last()],
                dist: long.Parse(new string(r.Take(5).ToArray()), NumberStyles.HexNumber)))
            .ToArray();

        List<(long x, long y)> coords = new();
        long x = 0;
        long y = 0;
        foreach (var cmd in commands)
        {
            var d = DirToDelta[cmd.dir];
            var x2 = x + d.dx * cmd.dist;
            var y2 = y + d.dy * cmd.dist;
            coords.Add((x2, y2));

            x = x2;
            y = y2;
        }


        var xValues = coords.Select(c => c.x).Distinct().Order().ToList();
        var yValues = coords.Select(c => c.y).Distinct().Order().ToList();

        var pixelSizeX = new Dictionary<int, long>();
        var pixelSizeY = new Dictionary<int, long>();
        
        var digOutInNewCordSystem = new HashSet<(int x, int y)>();
        digOutInNewCordSystem.Add((0, 0));
        x = y = 0;  // old cord system
        int x1 = xValues.IndexOf(0); // New cord system
        int y1 = yValues.IndexOf(0);
        foreach (var cmd in commands)
        {
            var d = DirToDelta[cmd.dir];
            var distLeft = cmd.dist;
            while (distLeft > 0)
            {
                if (d.dx > 0)
                {
                    long moveDist;
                    if (xValues.Contains(x))
                        moveDist = 1;
                    else
                        moveDist = xValues.First(q => q > x) - x;
                    pixelSizeX[x1] = moveDist;
                    
                    distLeft -= moveDist; 
                    x += moveDist;
                    x1++;
                }
                else if (d.dx < 0)
                {
                    long moveDist;
                    if (xValues.Contains(x))
                        moveDist = 1;
                    else
                        moveDist = x - xValues.Last(q => q < x);
                    pixelSizeX[x1] = moveDist;
                    
                    distLeft -= moveDist; 
                    x -= moveDist;
                    x1--;
                }
                else if (d.dy > 0)
                {
                    long moveDist;
                    if (yValues.Contains(y))
                        moveDist = 1;
                    else
                        moveDist = yValues.First(q => q > y) - y;
                    pixelSizeY[y1] = moveDist;

                    distLeft -= moveDist; 
                    y += moveDist;
                    y1++;
                }
                else if (d.dy < 0)
                {
                    long moveDist;
                    if (yValues.Contains(y))
                        moveDist = 1;
                    else
                        moveDist = y - yValues.Last(q => q < y);
                    pixelSizeY[y1] = moveDist;

                    distLeft -= moveDist; 
                    y -= moveDist;
                    y1--;
                }
                digOutInNewCordSystem.Add((x1, y1));
            }

            if (distLeft != 0)
                ;

        }
        
        FloodFill(digOutInNewCordSystem);
        
        int minY = digOutInNewCordSystem.Min(c => c.y);
        int maxY = digOutInNewCordSystem.Max(c => c.y);
        int minX = digOutInNewCordSystem.Min(c => c.x);
        int maxX = digOutInNewCordSystem.Max(c => c.x);
        long volume = 0;
        for (y1 = minY; y1 <= maxY; y1++)
        {
            for (x1 = minX; x1 <= maxX; x1++)
            {
                if (digOutInNewCordSystem.Contains((x1, y1)))
                {
                    volume += pixelSizeX[x1] * pixelSizeY[y1]; 
                    //Console.Write('#');
                }
                // else
                //     Console.Write('.');
            }
            //Console.WriteLine();
        }
        //Console.WriteLine();
        
        return volume;
    }


}