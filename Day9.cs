public class Day9
{
    public string CalcA()
    {
        return File.ReadLines("Day9.txt")
            .Select(row => row.Split(' ').Select(int.Parse).Append(0).ToArray())
            .Select(CalcRow)
            .Sum()
            .ToString();
    }
    
    public string CalcB()
    {
        return File.ReadLines("Day9.txt")
            .Select(row => row.Split(' ').Select(int.Parse).Reverse().Append(0).ToArray())
            .Select(CalcRow)
            .Sum()
            .ToString();
    }
    
    private int CalcRow(int[] num)
    {
        var rows = new List<int[]> { num };

        while (num.All(n => n == 0) == false)
        {
            var diff = new int[num.Length - 1];
            for (int i = 1; i < num.Length - 1; i++)
            {
                diff[i - 1] = num[i] - num[i-1];
            }

            rows.Add(diff);
            num = diff;
        }
        
        // Extrapolate
        for (int i = rows.Count - 2; i >= 0; i--)
            rows[i][rows[i].Length - 1] = rows[i][rows[i].Length - 2] + rows[i + 1][rows[i + 1].Length - 1];

        return rows[0][^1];
    }
}