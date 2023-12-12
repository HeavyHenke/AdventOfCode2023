public class Day11
{
    public string CalcA()
    {
        var map = File.ReadAllLines("Day11.txt").ToList();
        for (int row = 0; row < map.Count; row++)
        {
            if (map[row].All(c => c == '.'))
            {
                map.Insert(row, map[row]);
                row++;
            }
        }

        for (int col = 0; col < map[0].Length; col++)
        {
            if (map.All(r => r[col] == '.'))
            {
                for (int row = 0; row < map.Count; row++) 
                    map[row] = map[row].Insert(col, ".");
                col++;
            }
        }

        var coord = new List<(int row, int col)>();
        for (int row = 0; row < map.Count; row++)
        for (int col = 0; col < map[0].Length; col++)
            if(map[row][col] == '#')
                coord.Add((row, col));

        int minDist = 0;
        for(int i = 0; i < coord.Count-1;i++)
        for (int j = i + 1; j < coord.Count; j++)
        {
            minDist += Math.Abs(coord[i].row - coord[j].row);
            minDist += Math.Abs(coord[i].col - coord[j].col);
        }
        
        return minDist.ToString();
    }
    
    public string CalcB()
    {
        var map = File.ReadAllLines("Day11.txt").ToList();
        var rowAdd = new long[map.Count];
        int emptyRowsFound = 0;
        for (int row = 0; row < map.Count; row++)
        {
            if (map[row].All(c => c == '.'))
                emptyRowsFound++;
            rowAdd[row] = emptyRowsFound * (1_000_000 - 1);
        }

        var colAdd = new long[map[0].Length];
        int emptyColsFound = 0;
        for (int col = 0; col < map[0].Length; col++)
        {
            if (map.All(r => r[col] == '.'))
                emptyColsFound++;
            colAdd[col] = emptyColsFound * (1_000_000 - 1);
        }

        var coord = new List<(long row, long col)>();
        for (int row = 0; row < map.Count; row++)
        for (int col = 0; col < map[0].Length; col++)
            if (map[row][col] == '#')
                coord.Add((rowAdd[row] + row, colAdd[col] + col));

        long minDist = 0;
        for (int i = 0; i < coord.Count-1;i++)
        for (int j = i + 1; j < coord.Count; j++)
        {
            var numRows = Math.Abs(coord[i].row - coord[j].row);
            var numCols = Math.Abs(coord[i].col - coord[j].col);
            minDist += numRows + numCols;
        }
        
        return minDist.ToString();
    }

}