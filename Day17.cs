public class Day17
{
    private int[][] _map;

    public object CalcA()
    {
        _map = File
            .ReadAllLines("Day17.txt")
            .Select(line => line.Select(c => c - '0').ToArray())
            .ToArray();
        
        var goalX = _map[0].Length - 1;
        var goalY = _map.Length - 1;

        var visited = new Dictionary<(int x, int y, Direction), int>();
        var searchList = new PriorityQueue<SearchNode, int>();
        searchList.Enqueue(new SearchNode(0, 0, 0, Direction.Right, null), goalX + goalY);
        searchList.Enqueue(new SearchNode(0, 0, 0, Direction.Down, null), goalX + goalY);
        while (searchList.Count > 0)
        {
            var node = searchList.Dequeue();
            if (node.X == goalX && node.Y == goalY)
            {
                // Print(node);
                return node.Cost;
            }

            foreach (var move in GetNextMoves(node))
            {
                var hasVisited = visited.TryGetValue((move.X, move.Y, move.Dir), out var prevCost);
                if (hasVisited && prevCost <= move.Cost)
                    continue;
                if (hasVisited == false)
                    visited.Add((move.X, move.Y, move.Dir), move.Cost);
                searchList.Enqueue(move, move.Cost + goalX - move.X + goalY - move.Y);
            }
        }

        throw new Exception("I am lost!");
    }

    public object CalcB()
    {
        _map = File
            .ReadAllLines("Day17.txt")
            .Select(line => line.Select(c => c - '0').ToArray())
            .ToArray();
        
        var goalX = _map[0].Length - 1;
        var goalY = _map.Length - 1;

        var visited = new Dictionary<(int x, int y, Direction), int>();
        var searchList = new PriorityQueue<SearchNode, int>();
        searchList.Enqueue(new SearchNode(0, 0, 0, Direction.Right, null), goalX + goalY);
        searchList.Enqueue(new SearchNode(0, 0, 0, Direction.Down, null), goalX + goalY);
        visited.Add((0, 0, Direction.Right), goalX + goalY);
        visited.Add((0, 0, Direction.Down), goalX + goalY);
        
        while (searchList.Count > 0)
        {
            var node = searchList.Dequeue();
            if (node.X == goalX && node.Y == goalY)
            {
                 //Print(node); // 821 too high
                return node.Cost;
            }

            foreach (var move in GetNextMovesB(node))
            {
                var hasVisited = visited.TryGetValue((move.X, move.Y, move.Dir), out var prevCost);
                if (hasVisited && prevCost <= move.Cost)
                    continue;
                if (hasVisited == false)
                    visited.Add((move.X, move.Y, move.Dir), move.Cost);
                searchList.Enqueue(move, move.Cost + goalX - move.X + goalY - move.Y);
            }
        }

        throw new Exception("I am lost!");
    }

    
    private void Print(SearchNode goal)
    {
        var visited = new HashSet<(int x, int y)>();
        var node = goal;
        while (node != null)
        {
            visited.Add((node.X, node.Y));
            node = node.Prev;
        }
        
        for (int y = 0; y < _map.Length; y++)
        {
            for (int x = 0; x < _map[0].Length; x++)
            {
                if (visited.Contains((x, y)))
                    Console.Write("#");
                else
                    Console.Write(_map[y][x]);
            }
            Console.WriteLine();
        }
        Console.WriteLine();
        Console.WriteLine();
        
        for (int y = 0; y < _map.Length; y++)
        {
            for (int x = 0; x < _map[0].Length; x++)
            {
                if (visited.Contains((x, y)))
                    Console.Write(_map[y][x]);
                else
                    Console.Write(' ');
            }
            Console.WriteLine();
        }
        Console.WriteLine();
    }
    
    private IEnumerable<SearchNode> GetNextMoves(SearchNode node)
    {
        var leftDir = TurnLeft(node.Dir);
        var leftDelta = DirToDelta[leftDir];
        var rightDir = TurnRight(node.Dir);
        var rightDelta = DirToDelta[rightDir];
        var forwardDelta = DirToDelta[node.Dir];

        int x = node.X;
        int y = node.Y;
        int cost = node.Cost;
        for (int i = 0; i < 3; i++)
        {
            var lx = x + leftDelta.dx;
            var ly = y + leftDelta.dy;
            if (IsValid(lx, ly))
                yield return new SearchNode(lx, ly, cost + _map[ly][lx], leftDir, node);

            var rx = x + rightDelta.dx;
            var ry = y + rightDelta.dy;
            if (IsValid(rx, ry))
                yield return new SearchNode(rx, ry, cost + _map[ry][rx], rightDir, node);

            x += forwardDelta.dx;
            y += forwardDelta.dy;
            if (IsValid(x, y) == false)
                yield break;
            cost += _map[y][x];
            node = new SearchNode(x, y, cost, node.Dir, node);
        }
    }

    private IEnumerable<SearchNode> GetNextMovesB(SearchNode node)
    {
        var leftDir = TurnLeft(node.Dir);
        var rightDir = TurnRight(node.Dir);
        var forwardDelta = DirToDelta[node.Dir];

        int x = node.X;
        int y = node.Y;
        int cost = node.Cost;
        for (int i = 0; i < 4; i++)
        {
            x += forwardDelta.dx;
            y += forwardDelta.dy;
            if (IsValid(x, y) == false)
                yield break;
            cost += _map[y][x];
            node = new SearchNode(x, y, cost, node.Dir, node);
        }
        
        for (int i = 0; i < 7; i++)
        {
            yield return new SearchNode(x, y, cost, leftDir, node);
            yield return new SearchNode(x, y, cost, rightDir, node);

            x += forwardDelta.dx;
            y += forwardDelta.dy;
            if (IsValid(x, y) == false)
                yield break;
            cost += _map[y][x];
            node = new SearchNode(x, y, cost, node.Dir, node);
        }
    }

    private bool IsValid(int x, int y)
    {
        if (x < 0 || y < 0)
            return false;
        if (y >= _map.Length || x >= _map[y].Length)
            return false;
        return true;
    }
    
    private enum Direction
    {
        Left,
        Up,
        Right,
        Down
    }

    private static Direction TurnLeft(Direction dir)
    {
        if (dir == Direction.Left)
            return Direction.Down;
        return dir - 1;
    }
    private static Direction TurnRight(Direction dir)
    {
        if (dir == Direction.Down)
            return Direction.Left;
        return dir + 1;
    }

    private static readonly Dictionary<Direction, (int dx, int dy)> DirToDelta = new()
    {
        { Direction.Left, (-1, 0) },
        { Direction.Right, (1, 0) },
        { Direction.Up, (0, -1) },
        { Direction.Down, (0, 1) },
    };

    private class SearchNode(int x, int y, int cost, Direction dir, SearchNode? prev)
    {
        public readonly int X = x;
        public readonly int Y = y;
        public readonly int Cost = cost;
        public readonly Direction Dir = dir;
        public readonly SearchNode? Prev = prev;
    }

}

