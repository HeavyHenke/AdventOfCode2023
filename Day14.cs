using System.Text;

public class Day14
{
    public object CalcA()
    {
        var board = File.ReadAllLines("Day14.txt")
            .Select(s => s.ToArray())
            .ToArray();

        for (int y = 1; y < board.Length; y++)
        {
            for (int x = 0; x < board[0].Length; x++)
            {
                if (board[y][x] == 'O')
                    Roll(board, x, y);
            }
        }

        var weight = 0;
        for (int y = 0; y < board.Length; y++)
        {
            for (int x = 0; x < board[0].Length; x++)
            {
                Console.Write(board[y][x]);
                if (board[y][x] == 'O')
                    weight += board.Length - y;
            }
            Console.WriteLine();
        }
        Console.WriteLine();

        return weight;
    }

    public object CalcB()
    {
        var board = File.ReadAllLines("Day14.txt")
            .Select(s => s.ToArray())
            .ToArray();

        long counter = 0;
        var dict = new Dictionary<string, long>();
        while (counter < 1_000_000_000)
        {
            RollOneTurn(board);

            var str = ToStr(board);
            if (dict.TryAdd(str, counter) == false)
            {
                var diff = counter - dict[str];
                var iterationsLeft = 1_000_000_000 - counter;
                var addTimes = iterationsLeft / diff;
                counter += diff * addTimes;
                dict[str] = counter;
            }
            
            counter++;
        }

        var weight = 0;
        for (int y = 0; y < board.Length; y++)
        {
            for (int x = 0; x < board[0].Length; x++)
            {
                Console.Write(board[y][x]);
                if (board[y][x] == 'O')
                    weight += board.Length - y;
            }
            Console.WriteLine();
        }
        Console.WriteLine();

        return weight;
    }

    private static string ToStr(char[][] board)
    {
        var sb = new StringBuilder(board.Length * board[0].Length);
        foreach (var line in board)
            sb.Append(line);
        return sb.ToString();
    }
    
    private static void Roll(char[][] board, int x, int y)
    {
        Roll2(board, x, y, 0, -1);
    }

    private static void RollOneTurn(char[][] board)
    {
        // North
        for (int y = 1; y < board.Length; y++)
        {
            for (int x = 0; x < board[0].Length; x++)
            {
                if (board[y][x] == 'O')
                    Roll2(board, x, y, 0, -1);
            }
        }
        
        // West
        for (int y = 0; y < board.Length; y++)
        {
            for (int x = 1; x < board[0].Length; x++)
            {
                if (board[y][x] == 'O')
                    Roll2(board, x, y, -1, 0);
            }
        }
        
        // South
        for (int y = board.Length - 2; y >= 0; y--)
        {
            for (int x = 0; x < board[0].Length; x++)
            {
                if (board[y][x] == 'O')
                    Roll2(board, x, y, 0, 1);
            }
        }
        
        // East
        for (int y = board.Length - 1; y >= 0; y--)
        {
            for (int x = board[0].Length - 2; x >= 0; x--)
            {
                if (board[y][x] == 'O')
                    Roll2(board, x, y, 1, 0);
            }
        }

    }
    
    private static void Roll2(char[][] board, int x, int y, int dx, int dy)
    {
        while (true)
        {
            if (y == 0 && dy < 0)
                return;
            if (x == 0 && dx < 0)
                return;
            if (y == board.Length - 1 && dy > 0)
                return;
            if (x == board[0].Length - 1 && dx > 0)
                return;

            if (board[y + dy][x + dx] != '.')
                return;
            board[y][x] = '.';
            board[y + dy][x + dx] = 'O';
            y += dy;
            x += dx;
        }
    }

}