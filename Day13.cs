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
}