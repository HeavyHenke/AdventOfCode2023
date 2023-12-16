public class Day15
{
    public object CalcA()
    {
        var q = File.ReadAllText("Day15.txt")
            .Split(",")
            .Select(GetHash)
            .Sum();
        return q;
    }
    
    public object CalcB()
    {
        var q = File.ReadAllText("Day15.txt")
            .Split(",");

        var boxes = new List<(string label, int focal)>[256];
        for (int i = 0; i < boxes.Length; i++)
            boxes[i] = new List<(string label, int focal)>();
        
        foreach (var operation in q)
        {
            var lensParts = operation.Split('-', '=');
            var hash = GetHash(lensParts[0]);
            var op = operation.Contains('-') ? '-' : '=';

            if (op == '-')
            {
                var ix = boxes[hash].FindIndex(l => l.label == lensParts[0]);
                if (ix >= 0) 
                    boxes[hash].RemoveAt(ix);
            }
            else if (op == '=')
            {
                int focalLength = int.Parse(lensParts[1]);
                var ix = boxes[hash].FindIndex(l => l.label == lensParts[0]);
                if (ix >= 0)
                {
                    boxes[hash][ix] = (lensParts[0], focalLength);
                }
                else
                {
                    boxes[hash].Add((lensParts[0], focalLength));
                }
            }
        }

        long totalFocal = 0;
        for (int i = 0; i < boxes.Length; i++)
        {

            for (int j = 0; j < boxes[i].Count; j++)
                totalFocal += (i + 1) * (j + 1) * boxes[i][j].focal;
        }
        
        return totalFocal;
    }

    private static int GetHash(string str)
    {
        var hash = 0;
        foreach (var chr in str)
        {
            hash += chr;
            hash *= 17;
            hash &= 0xFF;
        }
        return hash;
    }
}