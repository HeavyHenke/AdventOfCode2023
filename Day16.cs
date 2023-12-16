
public class Day16
{
    public object CalcA()
    {
        var map = File.ReadAllLines("Day16.txt");

        var energizedTiles = GetEnergizedTiles(map, (-1, 0, Direction.Right));
        
        return energizedTiles;
    }

    private static int GetEnergizedTiles(string[] map, (int, int, Direction Right) startingDir)
    {
        var visited = new Dictionary<(int x, int y), HashSet<Direction>>();
        var searchQueue = new Queue<(int x, int y, Direction dir)>();
        searchQueue.Enqueue(startingDir);
        while (searchQueue.Count > 0)
        {
            var item = searchQueue.Dequeue();
            var delta = DirToDelta[item.dir];

            item.x += delta.dx;
            item.y += delta.dy;
            if (item.x < 0 || item.y < 0 || item.x >= map[0].Length || item.y >= map.Length)
                continue;

            if (visited.TryGetValue((item.x, item.y), out var directions) == false)
            {
                directions = new HashSet<Direction>();
                visited.Add((item.x, item.y), directions);
            }

            if (directions.Add(item.dir) == false)
                continue;

            foreach (var dir in GetNewDirection(map[item.y][item.x], item.dir))
                searchQueue.Enqueue((item.x, item.y, dir));
        }
        
        return visited.Count;
    }

    public object CalcB()
    {
        var map = File.ReadAllLines("Day16.txt");

        int best = 0;
        
        // From top
        for (int x = 0; x < map[0].Length; x++)
        {
            var v = GetEnergizedTiles(map, (x, -1, Direction.Down));
            best = Math.Max(best, v);
        }
        // From right
        for (int y = 0; y < map.Length; y++)
        {
            var v = GetEnergizedTiles(map, (map[0].Length, y, Direction.Left));
            best = Math.Max(best, v);
        }
        // From bottom
        for (int x = 0; x < map[0].Length; x++)
        {
            var v = GetEnergizedTiles(map, (x, map.Length, Direction.Up));
            best = Math.Max(best, v);
        }
        // From left
        for (int y = 0; y < map.Length; y++)
        {
            var v = GetEnergizedTiles(map, (-1, y, Direction.Right));
            best = Math.Max(best, v);
        }

        return best;
    }

    private enum Direction
    {
        Left,
        Right,
        Up,
        Down
    }

    private static readonly Dictionary<Direction, (int dx, int dy)> DirToDelta = new()
    {
        { Direction.Left, (-1, 0) },
        { Direction.Right, (1, 0) },
        { Direction.Up, (0, -1) },
        { Direction.Down, (0, 1) },
    };

    private static Direction[] GetNewDirection(char chr, Direction inDir)
    {
        if (chr == '.')
            return new[] { inDir };
        if (chr == '/' && inDir == Direction.Right)
            return new[] { Direction.Up };
        if (chr == '/' && inDir == Direction.Down)
            return new[] { Direction.Left };
        if (chr == '/' && inDir == Direction.Left)
            return new[] { Direction.Down };
        if (chr == '/' && inDir == Direction.Up)
            return new[] { Direction.Right };
        
        if (chr == '\\' && inDir == Direction.Right)
            return new[] { Direction.Down };
        if (chr == '\\' && inDir == Direction.Down)
            return new[] { Direction.Right };
        if (chr == '\\' && inDir == Direction.Left)
            return new[] { Direction.Up };
        if (chr == '\\' && inDir == Direction.Up)
            return new[] { Direction.Left };

        if (chr == '|' && inDir is Direction.Up or Direction.Down)
            return new[] { inDir };
        if (chr == '-' && inDir is Direction.Left or Direction.Right)
            return new[] { inDir };

        if (chr == '|')
            return new[] { Direction.Down, Direction.Up };
        if (chr == '-')
            return new[] { Direction.Left, Direction.Right };

        throw new Exception($"Knas {chr}, {inDir}");
    }

}