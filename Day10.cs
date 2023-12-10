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
}