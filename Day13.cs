using MoreLinq;

public class Day13
{
    public string CalcA()
    {
        var lines = File.ReadAllLines("Day13.txt");
        var boards = lines.Split("").Select(b => b.ToArray()).ToArray();

        int result = 0;
        for (int i = 0; i < boards.Length; i++)
        {
            var rowVal = GetRowMatchIx(boards[i]);
            if (rowVal > 0)
            {
                result += rowVal * 100;
                continue;
            }

            var colVal = GetRowMatchIx(Transpose(boards[i]));
            result += colVal;
        }
        
        return result.ToString();
    }

    public object CalcB()
    {
        var lines = File.ReadAllLines("Day13.txt");
        var boards = lines.Split("").Select(b => b.ToArray()).ToArray();

        int result = 0;
        foreach (var board in boards)
        {
            var dirtyRow = GetRowMatchIx(board);
            var dirtyCol = GetRowMatchIx(Transpose(board));
            bool found = false;

            for(int smudgeY = 0; smudgeY < board.Length && !found; smudgeY++)
            for(int smudgeX = 0; smudgeX < board[smudgeY].Length; smudgeX++)
            {
                var b2 = (string[])board.Clone();

                var toInsert = b2[smudgeY][smudgeX] == '#' ? "." : "#";
                b2[smudgeY] = b2[smudgeY].Remove(smudgeX, 1).Insert(smudgeX, toInsert);
                
                var rowVal = GetRowMatchIx(b2, dirtyRow);
                if (rowVal > 0)
                {
                    result += rowVal * 100;
                    found = true;
                    break;
                }
                
                var colVal = GetRowMatchIx(Transpose(b2), dirtyCol);
                if (colVal > 0)
                {
                    result += colVal;
                    found = true;
                    break;
                }
            }
        }
        
        return result.ToString();
    }

    private static IEnumerable<string> Transpose(IEnumerable<string> inp)
    {
        return inp.Transpose().Select(s => new string(s.ToArray()));
    }
    private static int GetRowMatchIx(IEnumerable<string> rows)
    {
        var rows2 = rows.ToArray();

        for (int i = 0; i < rows2.Length - 1; i++)
        {
            int diff = 0;
            while (true)
            {
                var ix1 = i - diff;
                var ix2 = i + 1 + diff;
                if (ix1 < 0 || ix2 >= rows2.Length)
                    return i + 1;
                if (rows2[ix1] != rows2[ix2])
                    break;
                diff++;
            }
        }

        return -1;
    }
    
    private static int GetRowMatchIx(IEnumerable<string> rows, int skipRow)
    {
        var rows2 = rows.ToArray();

        for (int i = 0; i < rows2.Length - 1; i++)
        {
            if (i + 1 == skipRow)
                continue;
            
            int diff = 0;
            while (true)
            {
                var ix1 = i - diff;
                var ix2 = i + 1 + diff;
                if (ix1 < 0 || ix2 >= rows2.Length)
                    return i + 1;
                if (rows2[ix1] != rows2[ix2])
                    break;
                diff++;
            }
        }

        return -1;
    }
}